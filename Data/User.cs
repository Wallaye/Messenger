namespace Data
{
    public class User
    {
        public int UserID { get; set; }
        public string Name { get; set; }
        private string _pass;
        public string Pass
        {
            get { return _pass; }
            set { _pass = value; } 
        }
        public User(int id, string name, string pass)
        {
            UserID = id;
            Name = name;
            _pass = pass;
        }
    }
}
