using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace Messenger.Server
{
    static class Server
    {
        private static List<IPEndPoint>? _activeConnections;
        private static List<Thread>? _activeThreads;
        private static List<Socket>? _activeSockets;
        private static readonly IPAddress _defaultIPAdress = IPAddress.Parse("192.0.0.1");
        private static int _port = 5500;

        public static void start()
        {
            _activeThreads.Add(new Thread(acceptConnections));
        }

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

        public static void acceptConnection(Socket server, Socket client)
        {
            
        }

        public static void acceptMessage()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Метод для подключения к серверу в первый раз
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

    }
}
