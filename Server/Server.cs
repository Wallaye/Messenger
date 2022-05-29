using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace Messenger.Server
{
    static class Server
    {
        private static readonly Thread _trdAccept = new Thread(acceptConnections);
        private static readonly IPAddress _defaultIPAdress = IPAddress.Parse("192.168.0.115");
        private static readonly List<ConnectedUser> _connectedUsers = new List<ConnectedUser>();
        private static readonly int _port = 5500;

        /// <summary>
        /// Запуск сервера.
        /// </summary>
        public static void start()
        {
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

        public static void recieveMessages()
        {
            throw new NotImplementedException();
        }

        public static void sendMessage()
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
            
        }

        public static void Authorize(string login, string pass)
        {

        }

        public static void Register(string login, string pass)
        {

        }
    }
}
