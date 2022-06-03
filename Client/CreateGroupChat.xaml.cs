using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    /// <summary>
    /// Interaction logic for CreateGroupChat.xaml
    /// </summary>
    public partial class CreateGroupChat : Window
    {
        ObservableCollection<string> users;
        ObservableCollection<string> chosenUsers;
        TcpClient tcpClient;
        StreamWriter sw;
        public CreateGroupChat(List<string> users, StreamWriter sw)
        {
            InitializeComponent();
            this.users = new(users);
            this.chosenUsers = new();
            //this.tcpClient = tcpClient;
            this.sw = sw;
            lstChosen.ItemsSource = chosenUsers;
            lstUsers.ItemsSource = this.users;
            lstChosen.SelectionMode = SelectionMode.Multiple;
            lstUsers.SelectionMode = SelectionMode.Multiple;
        }

        private void btnMove_Click(object sender, RoutedEventArgs e)
        {
            var chosen2 = lstChosen.SelectedItems;
            var chosen1 = lstUsers.SelectedItems;
            var _users = new ObservableCollection<string>(users);
            var _chosenusers = new ObservableCollection<string>(chosenUsers);
            foreach (var item in chosen1)
            {
                string curr = (string)item;
                _chosenusers.Add(curr);
                _users.Remove(curr);
            }
            foreach (var item in chosen2)
            {
                string curr = (string)item;
                _users.Add(curr);
                _chosenusers.Remove(curr);
            }

            //string str;
            //foreach (var item in users)
            //{
            //    int deleteIndex = _users.IndexOf(item);
            //    _chosenusers.Add(users[deleteIndex]);
            //    _users.RemoveAt(deleteIndex);
            //}
            //foreach (var item in chosenUsers)
            //{
            //    int deleteIndex = lstChosen.Items.IndexOf(item);
            //    _users.Add(chosenUsers[deleteIndex]);
            //    _chosenusers.RemoveAt(deleteIndex);
            //}
            users = _users;
            chosenUsers = _chosenusers;
            lstChosen.ItemsSource = chosenUsers;
            lstUsers.ItemsSource = users;

        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (chosenUsers.Count < 3)
            {
                MessageBox.Show("Невозможно создать чат меньше чем из 3 пользователей");
            }
            else if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название чата");
            }
            else
            {
                //StreamWriter sw = new StreamWriter(tcpClient.GetStream());
                StringBuilder sb = new StringBuilder();
                sb.Append("c ");
                foreach (var item in chosenUsers)
                {
                    sb.Append(item + " ");
                }
                sb.Append(txtName.Text);
                sw.WriteLine(sb.ToString());
                this.Close();
            }
        }
    }
}
