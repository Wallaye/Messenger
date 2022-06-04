using System.Runtime.Serialization;
namespace Data
{
    [DataContract]
    public class GroupChat
    {
        [DataMember]
        public int ID_Chat { get; set; }
        [DataMember]
        public string[] Users { get; set; }
        [DataMember]
        public string Name { get; set; }
        public GroupChat(int id_chat, string[] users, string name)
        {
            ID_Chat = id_chat;
            Users = users;
            Name = name;
        }
        public GroupChat() { }
    }
}
