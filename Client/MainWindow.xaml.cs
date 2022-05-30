﻿using System;
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
        
        public MainWindow(TcpClient tcp)
        {
            InitializeComponent();
            tcpClient = tcp;
            if (tcp.Connected == true)
            {
                sr = new(tcpClient.GetStream());
                sw = new(tcpClient.GetStream());
            }
        }
    }
}