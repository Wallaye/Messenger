using System.Runtime.Serialization;
namespace Data
{
    [DataContract]
    public class User
    {
        [DataMember]
        private int UserID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        private string _pass;
        public string Pass { get { return _pass; } set { _pass = value; } }
        public User(int id, string name, string pass)
        {
            UserID = id;
            Name = name;
            _pass = pass;
        }
    }
}
