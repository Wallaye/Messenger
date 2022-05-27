using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace Messenger.Server
{
    static class Server
    {
        private static List<IPEndPoint>? _activeConnections = new List<IPEndPoint>();
        private static List<Thread>? _activeThreads = new List<Thread>();
        private static List<Socket>? _activeSockets = new List<Socket>();
        private static readonly IPAddress _defaultIPAdress = IPAddress.Parse("127.0.0.1");
        private static int _port = 5500;
        
        /// <summary>
        /// Запуск сервера.
        /// </summary>
        public static void start()
        {
            _activeThreads?.Add(new Thread(acceptConnections));
            _activeThreads?[0].Start();
        }

        /// <summary>
        /// Остановка сервера.
        /// </summary>
        public static void stop()
        {
            foreach (Socket sock in _activeSockets)
            {
                sock.Shutdown(SocketShutdown.Both);
                sock.Close();
            }
            _activeSockets?.Clear();
            _activeThreads?.Clear();
            _activeConnections?.Clear();
        }

       
        public static void acceptMessage()
        {
            throw new NotImplementedException();
        }

        public static void acceptMessages()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Метод для подключения к серверу в первый раз.
        /// Вызывает метод acceptConnection который обрабатывает клиента
        /// </summary>
        public static void acceptConnections()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(_defaultIPAdress, _port));
            socket.Listen(5);
            while (true)
            {
                Socket client = socket.Accept();
                acceptConnection(socket, client);
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
        }
        /// <summary>
        /// Метод выдачи подключенному клиенту отдельного потока и сокета.
        /// </summary>
        public static void acceptConnection(Socket server, Socket client)
        {
            _port++;
            var new_IPEndPoint = new IPEndPoint(_defaultIPAdress, _port);
            _activeSockets?.Add(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
            _activeSockets?[^1].Bind(new_IPEndPoint);
            //_activeThreads?.Add(new Thread(acceptMessages));
            //_activeThreads?[^1].Start();
            IPEndPoint? curr_ip = client.RemoteEndPoint as IPEndPoint;
            if (curr_ip != null)
                _activeConnections?.Add(curr_ip);
            server.Send(Encoding.UTF8.GetBytes(new_IPEndPoint.ToString())); //отправляем клиенту его новый сокет куда надо подключиться
        }

    }
}
