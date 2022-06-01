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
        
        public MainWindow(TcpClient tcp)
        {
            InitializeComponent();
            tcpClient = tcp;
            if (tcpClient.Connected == true)
            {
                sr = new(tcpClient.GetStream());
                sw = new(tcpClient.GetStream());
            }
            Task.Factory.StartNew(() =>
            {
                if (tcpClient.Connected == true)
                {
                    chatNames = sr.ReadLine().Split(" ").ToList();
                    string Messages = sr.ReadLine();
                }
            });
        }

        private void lblExit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tcpClient.Close();
            Environment.Exit(0);
        }
    }
}
