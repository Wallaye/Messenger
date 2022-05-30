using System.Net.Sockets;
using Data;

namespace Messenger.Server
{
    public class ConnectedUser
    {
        public User user;
        public TcpClient tcpClient;
        public StreamReader sr;
        public StreamWriter sw;
        public ConnectedUser(User user, TcpClient tcp)
        {
            this.user = user;
            tcpClient = tcp;
            sr = new(tcpClient.GetStream());
            sw = new(tcpClient.GetStream());
            sw.AutoFlush = true;
        }
    }
}
