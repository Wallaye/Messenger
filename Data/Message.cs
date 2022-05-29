namespace Data
{
    public class Message
    {
        public int ID_sendfrom { get; set; }
        public int ID_sendto { get; set; }
        //public int ID_Chat { get; set; }
        public string body { get; set; }
        public DateTime date { get; set; }
    }
}
