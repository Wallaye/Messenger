using System.Runtime.Serialization;

namespace Data
{
    [DataContract]
    [KnownType(typeof(MessageGroup))]
    [KnownType(typeof(PrivateMessage))]
    public abstract class Message
    {
        [DataMember]
        public string Body { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
    }
    [DataContract]
    public class MessageGroup : Message
    {
        [DataMember]
        public int ID_Chat { get; set; }
        [DataMember]
        public bool IsGroupChat { get; set; }
        public MessageGroup(int id, bool isgc, string body, DateTime date)
        {
            ID_Chat = id;
            IsGroupChat = isgc;
            Body = body;
            Date = date;
        }
    }
    [DataContract]
    public class PrivateMessage : Message
    {
        [DataMember]
        public int ID_Sender;
        [DataMember]
        public int ID_Reciever;

        public PrivateMessage(int ids, int idr, string body, DateTime date)
        {
            ID_Sender = ids;
            ID_Reciever = idr;
            Body = body;
            Date = date;
        }
    }
}
