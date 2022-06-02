using System.Net;
using System.Text;

namespace Messenger.Server
{
    internal class Program 
    { 
        public static void Main()
        {
            Thread trd = new Thread(Server.start);
            trd.Start();
            if (Console.ReadLine() == "exit")
                Server.stop();
        }
    }
}