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

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TcpClient tcpClient;
        StreamReader sr;
        StreamWriter sw;
        ObservableCollection<string> chatNames = new ObservableCollection<string>();
        List<GroupChat> usersGrpChats = new List<GroupChat>();
        ObservableCollection<string> userNames = new ObservableCollection<string>();
        List<Message> messages = new List<Message>();
        
        public MainWindow(TcpClient tcp)
        {
            InitializeComponent();
            tcpClient = tcp;
            lstPrivateChats.ItemsSource = userNames;
            lstChats.ItemsSource = chatNames;
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
                        string str = sr.ReadLine();
                        var _userNames = str.Split(" ").ToList();
                        foreach (var item in _userNames)
                        {
                            userNames.Add(item);
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
                var strs = str.Split(" ");
                GroupChat temp;
                foreach (var item in strs)
                {
                    temp = JsonSerializer.Deserialize(item, typeof(GroupChat)) as GroupChat;
                    usersGrpChats.Add(temp);
                }
                foreach (var item in usersGrpChats)
                {
                    chatNames.Add(item.Name);
                }
            }
        }

        private void processFirstMsgs(string str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                var strs = str.Split(" ");
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
            sw.WriteLine("q");
            tcpClient.Close();
            Environment.Exit(0);
        }

        private void ReadMessage()
        {
            string str = sr.ReadLine();
            if (!string.IsNullOrEmpty(str))
            {
                if (str[0] == 'g')
                {
                    messages.Add(JsonSerializer.Deserialize(str[1..], typeof(MessageGroup)) as MessageGroup);
                }
                else if (str[0] == 'p')
                {
                    messages.Add(JsonSerializer.Deserialize(str[1..], typeof(PrivateMessage)) as PrivateMessage);
                }
                else if (str[0] == 'c')
                {
                    usersGrpChats.Add(JsonSerializer.Deserialize(str[1..], typeof(GroupChat)) as GroupChat);
                    chatNames.Add(usersGrpChats[^1].Name);
                }
            }
        }

        private void btnCreateGroup_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => { (new CreateGroupChat(userNames, tcpClient)).Show(); });
        }
    }
}
