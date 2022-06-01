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
        public GroupChat(int ID, string[] list, string name)
        {
            ID_Chat = ID;
            Users = list;
            Name = name;
        }
    }
}
