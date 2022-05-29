using System.Net.Sockets;
namespace Messenger.Server
{
    public class ConnectedUser
    {
        public int ID { get; }
        public string Name { get; }
        public TcpClient tcpClient;
        public StreamReader sr;
        public StreamWriter sw;
        public ConnectedUser(int id, string name, TcpClient tcp)
        {
            ID = id;
            Name = name;
            tcpClient = tcp;
            sr = new(tcpClient.GetStream());
            sw = new(tcpClient.GetStream());
            sw.AutoFlush = true;
        }
    }
}
