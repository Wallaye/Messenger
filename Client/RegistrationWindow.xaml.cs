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
using System.IO;
using System.Threading;

namespace Client
{
    /// <summary>
    /// Interaction logic for RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }

        private void lblExit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Environment.Exit(0);
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (txtLogin.Text.Contains(" "))
            {
                MessageBox.Show("Логин не может содержать пробелы");
                return;
            }
            if (pswPass1.Password.Contains(" "))
            {
                MessageBox.Show("Пароль не может содержать пробелы");
                return;
            }
            if (txtLogin.Text.Length < 4 || txtLogin.Text.Length > 16)
            {
                MessageBox.Show("Логин должен содержать от 4 до 16 символов");
                return;
            }
            if (pswPass1.Password.Length < 4 || pswPass1.Password.Length > 16)
            {
                MessageBox.Show("Пароль должен содержать от 4 до 16 символов");
                return;
            }
            if (pswPass1.Password != pswPass2.Password)
            {
                MessageBox.Show("Пароли должны совпадать");
                return;
            }
            string pass = LoginWindow.hashPass(pswPass1.Password);
            string str = "* " + txtLogin.Text + " " + pass;
            string reply = "";
            TcpClient tcp = new();
            try
            {
                tcp.Connect("192.168.0.104", 5500);
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
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            switch (reply)
            {
                case "Новый пользователь зарегистрирован":
                    MessageBox.Show("Зарегестрировано успешно");
                    new MainWindow(tcp, txtLogin.Text).Show();
                    this.Close();
                    return;
                case "Такой пользователь уже существует":
                    MessageBox.Show("Такой пользователь уже существует");
                    return;
            }
        }

        private void txtLogin_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsLetterOrDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void pswPass1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsLetterOrDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void pswPass2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsLetterOrDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
    }
}
