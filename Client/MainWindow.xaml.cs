using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.IO;
using Data;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.Threading;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string curr_name;
        TcpClient tcpClient;
        StreamReader sr;
        StreamWriter sw;
        ObservableCollection<string> chatNames = new ObservableCollection<string>();
        List<GroupChat> usersGrpChats = new List<GroupChat>();
        ObservableCollection<string> userNames = new ObservableCollection<string>();
        List<Message> messages = new List<Message>();
        public enum choice{
            Nothing = -1,
            Private = 0,
            Group = 1
        }
        choice _choice = MainWindow.choice.Nothing;
        SynchronizationContext UIContext;
        
        public MainWindow(TcpClient tcp, string curr_name)
        {
            InitializeComponent();
            this.curr_name = curr_name;
            tcpClient = tcp;
            lstPrivateChats.ItemsSource = userNames;
            lstChats.ItemsSource = chatNames;
            UIContext = SynchronizationContext.Current;
            if (tcpClient.Connected == true)
            {
                sr = new(tcpClient.GetStream());
                sw = new(tcpClient.GetStream());
                sw.AutoFlush = true;
            }
            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (tcpClient.Connected == true)
                    {
                        Task.Delay(30).Wait();

                        string str = sr.ReadLine();
                        var _userNames = str.Split(" ").ToList();
                        if (UIContext != null)
                        {
                            foreach (var item in _userNames)
                            {
                                UIContext.Send(x => userNames.Add(item), null);
                            }
                        }
                        string Messages = sr.ReadLine();
                        processFirstMsgs(Messages);
                        str = sr.ReadLine();
                        getGroupChats(str);
                    }
                }
                catch (JsonException) { }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                try
                {
                    while (tcpClient.Connected == true)
                    {
                        ReadMessage();
                        Task.Delay(20).Wait();
                    }
                    //MessageBox.Show("Разрыв соединения с сервером");
                    btnCreateGroup.IsEnabled = false;
                    btnSend.IsEnabled = false;
                }
                catch (SocketException ex)
                {
                    MessageBox.Show("Разрыв соединения с сервером");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }

        private void getGroupChats(string str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                if (str[0] == 'r')
                {
                    return;
                }
                var strs = str.Split(" ");
                GroupChat temp;
                foreach (var item in strs)
                {
                    temp = JsonSerializer.Deserialize(item, typeof(GroupChat)) as GroupChat;
                    usersGrpChats.Add(temp);
                }
                foreach (var item in usersGrpChats)
                {
                    UIContext.Send(x =>chatNames.Add(item.Name), null);
                }
            }
        }

        private void processFirstMsgs(string str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                var strs = str.Split("*^&%");
                foreach (var item in strs)
                {
                    if (item[0] == 'g')
                    {
                        messages.Add(JsonSerializer.Deserialize(item[1..], typeof(MessageGroup)) as MessageGroup);
                    }
                    else if (item[0] == 'p')
                    {
                        messages.Add(JsonSerializer.Deserialize(item[1..], typeof(PrivateMessage)) as PrivateMessage);
                    }
                }
            }
        }

        private void lblExit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                sw.WriteLine("q");
                tcpClient.Close();
                Environment.Exit(0);
            }
            catch { };
        }

        private void ReadMessage()
        {
            string str = sr.ReadLine();
            if (!string.IsNullOrEmpty(str))
            {
                if (str[0] == 'g')
                {
                    MessageGroup msg = JsonSerializer.Deserialize(str[1..], typeof(MessageGroup)) as MessageGroup;
                    messages.Add(msg);
                    int selected = 0;
                    Dispatcher.Invoke(() => selected = lstChats.SelectedIndex);
                    if (selected >= 0)
                    {
                        if (msg.ID_Chat == usersGrpChats[selected].ID_Chat)
                        {
                            UIContext.Send(x => txtChat.Text += $"{msg.Sender}({msg.Date.ToLocalTime()}): {msg.Body}\n", null);
                        }
                    }
                }
                else if (str[0] == 'p')
                {
                    PrivateMessage msg = JsonSerializer.Deserialize(str[1..], typeof(PrivateMessage)) as PrivateMessage;
                    messages.Add(msg);
                    string selected = null;
                    Dispatcher.Invoke(() => selected = (string)lstPrivateChats.SelectedValue);
                    if (msg.Sender == selected || msg.Reciever == selected)
                    {
                        UIContext.Send(x => txtChat.Text += $"{msg.Sender}({msg.Date.ToLocalTime()}): {msg.Body}\n", null);
                    }
                }
                else if (str[0] == 'c')
                {
                    usersGrpChats.Add(JsonSerializer.Deserialize(str[1..], typeof(GroupChat)) as GroupChat);
                    UIContext.Send(x => chatNames.Add(usersGrpChats[^1].Name), null);
                }
                else if (str[0] == 'n')
                {
                    UIContext.Send(x => userNames.Add(str[2..]), null);
                }
            }
        }

        private void btnCreateGroup_Click(object sender, RoutedEventArgs e)
        {
            List<string> names = new List<string>(userNames);
            //names.Add(curr_name);
            (new CreateGroupChat(names, ref this.tcpClient, curr_name)).Show();
        }

        private void lstPrivateChats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lstChats.UnselectAll();
            txtChat.Text = "";
            _choice = choice.Private;
            string selected = (string)lstPrivateChats.SelectedValue;
            if (!String.IsNullOrEmpty(selected))
            {
                foreach (var item in messages)
                {
                    var msg = item as PrivateMessage;
                    if (msg is not null)
                    {
                        if (msg.Sender == selected || msg.Reciever == selected)
                        {
                            txtChat.Text += $"{msg.Sender}({msg.Date.ToLocalTime()}): {msg.Body}\n";
                        }
                    }
                }
            }
            btnInfo.Visibility = Visibility.Collapsed;
        }

        private void lstChats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lstPrivateChats.UnselectAll();
            txtChat.Text = "";
            _choice = choice.Group;
            int selected = lstChats.SelectedIndex;
            if (selected >= 0)
            {
                foreach (var item in messages)
                {
                    var msg = item as MessageGroup;
                    if (msg is not null)
                    {

                        if (msg.ID_Chat == usersGrpChats[selected].ID_Chat)
                        {
                            txtChat.Text += $"{msg.Sender}({msg.Date.ToLocalTime()}): {msg.Body}\n";
                        }
                    }
                }
            }
            btnInfo.Visibility = Visibility.Visible;
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Message msg;
                string msgBody = null;
                Dispatcher.Invoke(() => msgBody = txtMessage.Text);
                if (!string.IsNullOrWhiteSpace(msgBody))
                {
                    switch (_choice)
                    {
                        case choice.Nothing:
                            break;
                        case choice.Private:
                            string selected = null;
                            Dispatcher.Invoke(() => selected = (string)lstPrivateChats.SelectedValue);
                            msg = new PrivateMessage(curr_name, selected, msgBody, DateTime.Now);
                            sw.WriteLine("p" + JsonSerializer.Serialize(msg, typeof(PrivateMessage)));
                            //sw.Flush();
                            break;
                        case choice.Group:
                            int? index = null;
                            Dispatcher.Invoke(() => index = lstChats.SelectedIndex);
                            GroupChat temp = usersGrpChats[index.Value];
                            msg = new MessageGroup(temp.ID_Chat, msgBody, temp.Users, curr_name, DateTime.Now);
                            sw.WriteLine("g" + JsonSerializer.Serialize(msg, typeof(MessageGroup)));
                            //sw.Flush();
                            break;
                    }
                }
                else MessageBox.Show("Нельзя отправить пустое сообщение");
                UIContext.Send(x => txtMessage.Text = String.Empty, null);
            });
        }

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            int selected = lstChats.SelectedIndex;
            StringBuilder sb = new();
            if (selected >= 0)
            {
                sb.Append("Участники:\n");
                foreach (var item in usersGrpChats[selected].Users)
                {
                    sb.Append(item + ", ");
                }
                sb.Remove(sb.Length - 2, 2);
                MessageBox.Show(sb.ToString());
            }
        }
    }
}
