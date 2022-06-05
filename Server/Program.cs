namespace Messenger.Server
{
    internal class Program 
    { 
        public static void Main()
        {
            Thread trd = new Thread(Server.start);
            trd.Start();
            while (true)
                if (Console.ReadLine() == "exit")
                    Server.stop();
        }
    }
}