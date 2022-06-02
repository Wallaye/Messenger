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
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Collections.ObjectModel;
using System.IO;

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
        public CreateGroupChat(ObservableCollection<string> users, TcpClient tcpClient)
        {
            InitializeComponent();
            this.users = new(users);
            this.chosenUsers = new();
            this.tcpClient = tcpClient;
            lstChosen.ItemsSource = chosenUsers;
            lstUsers.ItemsSource = users;
            lstChosen.SelectionMode = SelectionMode.Multiple;
            lstUsers.SelectionMode = SelectionMode.Multiple;
        }

        private void btnMove_Click(object sender, RoutedEventArgs e)
        {
            List<string> chosen1 = (List<string>)lstChosen.SelectedItems;
            List<string> chosen2 = (List<string>)lstUsers.SelectedItems;
            foreach (var item in chosen2)
            {
                int deleteIndex = lstUsers.Items.IndexOf(item);
                users.RemoveAt(deleteIndex);
                chosenUsers.Add(item);
                //lstUsers.Items.RemoveAt(deleteIndex);
                //lstChosen.Items.Add(item);
            }
            foreach (var item in chosen1)
            {
                int deleteIndex = lstChosen.Items.IndexOf(item);
                chosenUsers.RemoveAt(deleteIndex);
                users.Add(item);
                //lstChosen.Items.RemoveAt(deleteIndex);
                //lstUsers.Items.Add(item);
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (chosenUsers.Count < 3)
            {
                MessageBox.Show("Невозможно создать чат меньше чем из 3 пользователей");
            }
            else
            {
                StreamWriter sw = new StreamWriter(tcpClient.GetStream());
                StringBuilder sb = new StringBuilder();
                sb.Append("c ");
                foreach(var item in chosenUsers)
                {
                    sb.Append(item + " ");
                }
                sb.Remove(sb.Length - 1, 1);
                sw.WriteLine(sb.ToString());
                this.Close();
            }
        }
    }
}
