using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using Data;
using System.Runtime.Serialization.Json;
using System.Text.Json;

namespace Messenger.Server
{
    static class Server
    {
        private static readonly Thread _trdAccept = new Thread(acceptConnections);
        private static readonly IPAddress _defaultIPAdress = IPAddress.Parse("192.168.0.115");
        private static readonly List<ConnectedUser> _connectedUsers = new List<ConnectedUser>();
        private static readonly int _port = 5500;
        private static List<User> _users;
        private static List<Message> _messages;
        private static List<GroupChat> _chats;
        private static DataContractJsonSerializer _usersSer = new(typeof(List<User>));
        private static DataContractJsonSerializer _msgSer = new(typeof(List<Message>));
        private static DataContractJsonSerializer _grpSer = new(typeof(List<GroupChat>));
        private static readonly TcpListener _tcpListener = new(_defaultIPAdress, _port);

        /// <summary>
        /// Запуск сервера.
        /// </summary>
        public static void start()
        {
            //using (var file = new FileStream(@"..\..\..\JSON\Users.json", FileMode.Open))
            //{
            //    _users = _usersSer.ReadObject(file) as List<User>;
            //}
            //using (var file = new FileStream(@"..\..\..\JSON\Messages.json", FileMode.Open))
            //{
            //    _messages = _msgSer.ReadObject(file) as List<Message>;
            //}
            //using (var file = new FileStream(@"..\..\..\JSON\GroupChats.json", FileMode.Open))
            //{
            //    _chats = _grpSer.ReadObject(file) as List<GroupChat>;
            //}
            _trdAccept.Start();
        }

        /// <summary>
        /// Остановка сервера.
        /// </summary>
        public static void stop()
        {
            
        }

       
        public static void recieveMessage()
        {
            throw new NotImplementedException();
        }

        public static void recieveMessages(ConnectedUser client)
        {
            while (client.tcpClient.Connected)
            {
                try
                {
                    string str = client.sr.ReadToEnd();
                    Message msg = JsonSerializer.Deserialize(str, typeof(Message)) as Message;
                    sendMessage(msg);
                }
                catch (Exception exc) 
                {
                    Console.WriteLine(exc.Message);
                }
            }
        }

        public static void sendMessage(Message msg)
        {
            throw new NotImplementedException();
        }

        public static void sendMessages()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Метод для подключения к серверу в первый раз.
        /// Вызывает метод acceptConnection который обрабатывает клиента
        /// </summary>
        public static void acceptConnections()
        {
            _tcpListener.Start();
            while (true)
            {
                var client = _tcpListener.AcceptTcpClient();
                acceptConnection(client);
            }
            //Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //socket.Bind(new IPEndPoint(_defaultIPAdress, _port));
            //socket.Listen(5);
            //while (true)
            //{
            //    Socket client = socket.Accept();
            //    Console.WriteLine(client.RemoteEndPoint);
            //    acceptConnection(socket, client);
            //    client.Shutdown(SocketShutdown.Both);
            //    client.Close();
            //}
        }
        /// <summary>
        /// Метод выдачи подключенному клиенту отдельного потока и сокета.
        /// </summary>
        //public static void acceptConnection(Socket server, Socket client)
        //{
        //    byte[] data = new byte[256];
        //    int bytesread = 0;
        //    StringBuilder sb = new();
        //    bytesread = client.Receive(data);
        //    sb.Append(Encoding.UTF8.GetString(data, 0, bytesread));
        //    Console.WriteLine(sb);
        //    var str = sb.ToString().Split(" ");
        //    if (str.Length == 2)
        //    {
        //        switch (ValidateUser.Authorize(str[0], str[1], _users))
        //        {
        //            case 0:
        //                client.Send(Encoding.UTF8.GetBytes("Вы авторизованы"));
        //                break;
        //            case 1:
        //                client.Send(Encoding.UTF8.GetBytes("Неверный пароль"));
        //                break;
        //            case 2:
        //                client.Send(Encoding.UTF8.GetBytes("Такого пользователя не существует"));
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        switch (ValidateUser.Register(str[1], str[2], _users))
        //        {
        //            case 0:
        //                client.Send(Encoding.UTF8.GetBytes("Новый пользователь зарегистрирован"));
        //                break;
        //            case 1:
        //                client.Send(Encoding.UTF8.GetBytes("Такой пользователь уже существует"));
        //                break;
        //        }
        //    }

        //}
        public static void acceptConnection(TcpClient client)
        {
            Task.Factory.StartNew(() =>
            {
                if (client.Connected)
                {
                    var sr = new StreamReader(client.GetStream());
                    var sw = new StreamWriter(client.GetStream());
                    sw.AutoFlush = true;
                    var words = sr.ReadLine().Split(" ");
                    if (words.Length == 2)
                    {
                        int k = ValidateUser.Authorize(words[0], words[1], _users);
                        switch (k)
                        {
                            case 1:
                                sw.WriteLine("Неверный пароль");
                                client.Client.Disconnect(false);
                                //client.Send(Encoding.UTF8.GetBytes("Неверный пароль"));
                                return;
                            case 2:
                                sw.WriteLine("Такого пользователя не существует");
                                client.Client.Disconnect(false);
                                return;
                                //client.Send(Encoding.UTF8.GetBytes("Такого пользователя не существует"));
                            default:
                                sw.WriteLine("Вы авторизованы");
                                _connectedUsers.Add(new ConnectedUser(_users[k], client));
                                //recieveMessages(_connectedUsers[^1]);
                                //client.Send(Encoding.UTF8.GetBytes("Вы авторизованы"));
                                break;
                        }
                    }
                    else if (words.Length == 3)
                    {
                        switch (ValidateUser.Register(words[1], words[2], _users))
                        {
                            case 0:
                                sw.WriteLine("Новый пользователь зарегистрирован");
                                _users.Add(new User(_users.Count, words[1], words[2]));
                                _connectedUsers.Add(new ConnectedUser(_users[^1], client));
                                break;
                            case 1:
                                sw.WriteLine("Такой пользователь уже существует");
                                client.Client.Disconnect(false);
                                return;
                        }
                    }
                }
            });
        }
    }
}
