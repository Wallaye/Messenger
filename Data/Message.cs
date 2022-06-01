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
        public string[] Names { get; set; }
        public MessageGroup() { }
        public MessageGroup(int id, string body, string[] names, DateTime date)
        {
            ID_Chat = id;
            Body = body;
            Date = date;
            Names = names;
        }
    }
    [DataContract]
    public class PrivateMessage : Message
    {
        [DataMember]
        public string Sender { get; set; }
        [DataMember]
        public string Reciever { get; set; }

        public PrivateMessage() { }
        public PrivateMessage(string sender, string reciever, string body, DateTime date)
        {
            Sender = sender;
            Reciever = reciever;
            Body = body;
            Date = date;
        }
    }
}
