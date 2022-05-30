using System.Runtime.Serialization;
namespace Data
{
    [DataContract]
    public class User
    {
        [DataMember]
        private int UserID { get; }
        [DataMember]
        public string Name { get; }
        [DataMember]
        private string _pass;
        public string Pass { get { return _pass; } }
        public User(int id, string name, string pass)
        {
            UserID = id;
            Name = name;
            _pass = pass;
        }
    }
}
