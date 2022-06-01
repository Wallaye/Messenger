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
        List<string> chatNames;
        List<string> userNames;
        
        public MainWindow(TcpClient tcp)
        {
            InitializeComponent();
            tcpClient = tcp;
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
                        processFirstString(str);
                        lstPrivateChats.ItemsSource = userNames;
                        lstChats.ItemsSource = chatNames;
                        string Messages = sr.ReadLine();
                    }
                    while (tcpClient.Connected == true)
                    {
                        string str = sr.ReadLine();

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }

        private void processFirstString(string str)
        {
            var strs = str.Split("/");
            userNames = strs[0].Split(" ").ToList();
            chatNames = strs[1].Split(" ").ToList();
        }

        private void lblExit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            sw.WriteLine("q");
            tcpClient.Close();
            Environment.Exit(0);
        }

        private void ReadMessage(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {

            }
        }
    }
}
