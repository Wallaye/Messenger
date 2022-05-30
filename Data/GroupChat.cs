using System.Runtime.Serialization;
namespace Data
{
    [DataContract]
    public class GroupChat
    {
        [DataMember]
        public int ID_Chat { get; set; }
        [DataMember]
        public List<int> UsersID { get; set; }
        public GroupChat(int ID, List<int> list)
        {
            ID_Chat = ID;
            UsersID = list;
        }
    }
}
