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
using System.Net;
using System.Threading;
using System.Security.Cryptography;
using System.IO;
using System.Text.Json;
using MvvmHelpers.Commands;

namespace Client
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void lblExit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Environment.Exit(0);
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (txtLogin.Text.Contains(" "))
            {
                MessageBox.Show("Логин не может содержать пробелы");
                return;
            }
            if (txtPass.Password.Contains(" "))
            {
                MessageBox.Show("Пароль не может содержать пробелы");
                return;
            }
            if (txtLogin.Text.Length < 4 || txtLogin.Text.Length > 16)
            {
                MessageBox.Show("Логин должен содержать от 4 до 16 символов");
                return;
            }
            if (txtPass.Password.Length < 4 || txtPass.Password.Length > 16)
            {
                MessageBox.Show("Пароль должен содержать от 4 до 16 символов");
                return;
            }
            string pass = hashPass(txtPass.Password);
            string str = txtLogin.Text + " " + pass;
            string reply = "";
            TcpClient tcp = new();
            try
            {
                tcp.Connect("192.168.0.115", 5500);
                if (tcp.Connected)
                {
                    var sw = new StreamWriter(tcp.GetStream());
                    var sr = new StreamReader(tcp.GetStream());
                    sw.WriteLine(str);
                    sw.Flush();
                    while (true)
                    {
                        reply = sr.ReadLine();
                        if (reply?.Equals(String.Empty) == false)
                            break;
                        Thread.Sleep(30);
                    }
                }           
            }
            catch (SocketException exc)
            {
                MessageBox.Show("Возникла ошибка при подключении к серверу\n" + exc.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            switch (reply)
            {
                case "Неверный пароль":
                    MessageBox.Show("Неверный пароль");
                    return;
                case "Такого пользователя не существует":
                    MessageBox.Show("Такого пользователя не существует");
                    return;
                case "Вы авторизованы":
                    MessageBox.Show("Вы авторизованы");
                    new MainWindow(tcp, txtLogin.Text).Show();
                    this.Close();
                    break;
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            new RegistrationWindow().Show();
            this.Close();
        }
        private void txtLogin_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsLetterOrDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void txtPass_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsLetterOrDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        public static string hashPass(string pass)
        {
            using (MD5 md = MD5.Create())
            {
                byte[] input = Encoding.UTF8.GetBytes(pass);
                byte[] hash = md.ComputeHash(input);
                return Convert.ToHexString(hash);
            }
        }
    }
}
