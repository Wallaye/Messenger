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
        public Message() { }
    }
    [DataContract]
    public class MessageGroup : Message
    {
        [DataMember]
        public int ID_Chat { get; set; }
        [DataMember]
        public int[] IDS { get; set; }
        public MessageGroup() { }
        public MessageGroup(int id, string body, int[] ids, DateTime date)
        {
            ID_Chat = id;
            Body = body;
            Date = date;
            IDS = ids;
        }
    }
    [DataContract]
    public class PrivateMessage : Message
    {
        [DataMember]
        public int ID_Sender { get; set; }
        [DataMember]
        public int ID_Reciever { get; set; }

        public PrivateMessage() { }
        public PrivateMessage(int ids, int idr, string body, DateTime date)
        {
            ID_Sender = ids;
            ID_Reciever = idr;
            Body = body;
            Date = date;
        }
    }
}
