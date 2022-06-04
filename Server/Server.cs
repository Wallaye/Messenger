using Data;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using System.Text.Json;
using System.Text;

namespace Messenger.Server
{
    static class Server
    {
        private static readonly Thread _trdAccept = new Thread(acceptConnections);
        private static readonly IPAddress _defaultIPAdress = IPAddress.Any;//IPAddress.Parse("192.168.0.115");
        private static readonly List<ConnectedUser> _connectedUsers = new List<ConnectedUser>();
        private static readonly int _port = 5500;
        private static List<User> _users = new List<User>();
        private static List<Message> _messages = new List<Message>();
        private static List<GroupChat> _chats = new List<GroupChat>();
        private static DataContractJsonSerializer _usersSer = new(typeof(List<User>));
        private static DataContractJsonSerializer _msgSer = new(typeof(List<Message>));
        private static DataContractJsonSerializer _grpSer = new(typeof(List<GroupChat>));
        private static readonly TcpListener _tcpListener = new(_defaultIPAdress, _port);

        /// <summary>
        /// Запуск сервера.
        /// </summary>
        public static void start()
        {
            DeserializeUsers();
            DeserializeMessages();
            DeserializeGroups();
            _trdAccept.Start();
        }

        /// <summary>
        /// Остановка сервера.
        /// </summary>
        public static void stop()
        {
            SerializeUsers();
            SerializeMessages();
            SerializeGroups();
            Environment.Exit(0);
        }

        #region Сериализация
        public static void SerializeUsers()
        {
            using (var file = new FileStream(@"..\..\..\JSON\Users.json", FileMode.Create))
            {
                _usersSer.WriteObject(file, _users);
            }
        }
        public static void DeserializeUsers()
        {
            using (var file = new FileStream(@"..\..\..\JSON\Users.json", FileMode.Open))
            {
                _users = _usersSer.ReadObject(file) as List<User>;
            }
        }
        public static void SerializeMessages()
        {
            using (var file = new FileStream(@"..\..\..\JSON\Messages.json", FileMode.Create))
            {
                _msgSer.WriteObject(file, _messages);
            }
        }
        public static void DeserializeMessages()
        {
            using (var file = new FileStream(@"..\..\..\JSON\Messages.json", FileMode.Open))
            {
                _messages = _msgSer.ReadObject(file) as List<Message>;
            }
        }
        public static void SerializeGroups()
        {
            using (var file = new FileStream(@"..\..\..\JSON\GroupChats.json", FileMode.Create))
            {
                _grpSer.WriteObject(file, _chats);
            }
        }
        public static void DeserializeGroups()
        {
            using (var file = new FileStream(@"..\..\..\JSON\GroupChats.json", FileMode.Open))
            {
                _chats = _grpSer.ReadObject(file) as List<GroupChat>;
            }
        }
        #endregion Сериализация
        public static void recieveMessages(ConnectedUser client)
        {
            while (client.tcpClient.Connected)
            {
                try
                {
                    StringBuilder sb = new();
                    string str = client.sr.ReadLine();
                    sb.Append(str);
                    if (sb.Length == 0) sb.Append('q');
                    if (sb[0] == 'p')
                    {
                        sb.Remove(0, 1);
                        PrivateMessage msg = JsonSerializer.Deserialize(sb.ToString(), typeof(PrivateMessage)) as PrivateMessage;
                        if (msg != null)
                        {
                            Console.WriteLine(client.user.Name + " написал личное сообщение");
                            _messages.Add(msg);
                            sendMessage(msg);
                        }
                    }
                    else if (sb[0] == 'g')
                    {
                        sb.Remove(0, 1);
                        MessageGroup msg = JsonSerializer.Deserialize(sb.ToString(), typeof(MessageGroup)) as MessageGroup;
                        if (msg != null)
                        {
                            Console.WriteLine(client.user.Name + " написал групповое сообщение");
                            _messages.Add(msg);
                            sendMessage(msg);
                        }
                    }
                    else if (sb[0] == 'q')
                    {
                        client.tcpClient.Close();
                        int i = _connectedUsers.IndexOf(client);
                        Console.WriteLine(_connectedUsers[i].user.Name + " отключился");
                        _connectedUsers.Remove(client);
                    }
                    else if (sb[0] == 'c')
                    {
                        var strs = sb.ToString().Split(" ");
                        _chats.Add(new(_chats.Count, strs[1..^1], strs[^1]));
                        Console.WriteLine(client.user.Name + " создал групповой чат ");
                        foreach (var el in _connectedUsers)
                        {
                            el.sw.WriteLine("c" + JsonSerializer.Serialize(_chats[^1]));
                        }
                    }
                    Task.Delay(10).Wait();
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                }
            }
        }

        public static void sendMessage(Message msg)
        {
            if (msg is MessageGroup)
            {
                foreach (var el in _connectedUsers)
                {
                    if (((MessageGroup)msg).Names.Contains(el.user.Name))
                    {
                        el.sw.WriteLine("g"+JsonSerializer.Serialize(msg, typeof(MessageGroup)));
                    }
                }
            }
            else if (msg is PrivateMessage)
            {
                foreach (var el in _connectedUsers)
                {
                    if (el.user.Name == ((PrivateMessage)msg).Reciever || el.user.Name == ((PrivateMessage)msg).Sender)
                    {
                        el.sw.WriteLine("p"+JsonSerializer.Serialize(msg, typeof(PrivateMessage)));
                    }
                }
            }
        }
        
        public static void newUserConnected(string str)
        {
            foreach (var el in _connectedUsers)
            {
                if (el.user.Name != str)
                    el.sw.WriteLine("n " + str);
            }
        }

        public static void SendMessagesOnConnect(ConnectedUser user)
        {
            StringBuilder sb = new();
            foreach(var msg in _messages)
            {
                if (msg is MessageGroup)
                {
                    if (((MessageGroup)msg).Names.Contains(user.user.Name))
                    {
                        sb.Append("g" + JsonSerializer.Serialize(msg, typeof(MessageGroup)) + "*^&%");
                    }
                }
                else if (user.user.Name == ((PrivateMessage)msg).Reciever || user.user.Name == ((PrivateMessage)msg).Sender)
                {
                    sb.Append("p" + JsonSerializer.Serialize(msg, typeof(PrivateMessage)) + "*^&%");
                }
            }
            if (sb.Length > 4)
            {
                sb.Remove(sb.Length - 4, 4);
                user.sw.WriteLine(sb.ToString());
            }
            else
            {
                user.sw.WriteLine("r");
            }
        }

        #region Подключения
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
        }
        /// <summary>
        /// Асинхронный метод принятия пользователяя
        /// </summary>
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
                    ConnectedUser usr = null;
                    if (words.Length == 2)
                    {
                        int k = ValidateUser.Authorize(words[0], words[1], _users);
                        switch (k)
                        {
                            case -1:
                                sw.WriteLine("Неверный пароль");
                                client.Client.Disconnect(false);
                                return;
                            case -2:
                                sw.WriteLine("Такого пользователя не существует");
                                client.Client.Disconnect(false);
                                return;
                            default:
                                sw.WriteLine("Вы авторизованы");
                                usr = new(_users[k], client);
                                _connectedUsers.Add(usr);
                                Console.WriteLine("Пользователь подключен: " + _users[k].Name);
                                break;
                        }
                    }
                    else if (words.Length == 3)
                    {
                        switch (ValidateUser.Register(words[1], words[2], _users))
                        {
                            case 0:
                                sw.WriteLine("Новый пользователь зарегистрирован");
                                Console.WriteLine("Зарегестрирован пользователь: " + _users[^1].Name);
                                usr = new(_users[^1], client);
                                newUserConnected(usr.user.Name);
                                _connectedUsers.Add(usr);
                                break;
                            case 1:
                                sw.WriteLine("Такой пользователь уже существует");
                                client.Client.Disconnect(false);
                                return;
                        }
                    }
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in _users)
                    {
                        if (item.Name != usr.user.Name)
                            sb.Append(item.Name + " ");
                    }
                    sb.Remove(sb.Length - 1, 1);
                    sw.WriteLine(sb.ToString());//Отправляем имена пользователей
                    sb.Clear();
                    SendMessagesOnConnect(usr);//Отправляем сообщения подключённому пользователю
                    foreach (var item in _chats)
                    {
                        if (item.Users.Contains(usr.user.Name))
                        {
                            sb.Append(JsonSerializer.Serialize(item) + " ");
                        }
                    }
                    if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);
                    sw.WriteLine(sb.ToString());//Отправляем групповые чаты к которым принадлежит данный пользователь
                    sw.Flush();
                    recieveMessages(usr);
                }
            });
        }
        #endregion Подключения
    }
}
