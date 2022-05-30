using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;

namespace Messenger.Server
{
    internal static class ValidateUser
    {
        /// <summary>Метод авторизации</summary>
        /// <returns>positive - OK, 1 - bad pass, 2 - no user</returns>
        public static int Authorize(string login, string pass, List<User> users)
        {
            foreach (var el in users)
            {
                if (el.Name == login)
                {
                    if (el.Pass == pass)
                    {
                        return users.IndexOf(el);
                    }
                    else return -1;
                }
            }
            return -2;
        }
        /// <summary>Метод регистрации</summary>
        /// <returns>0 - OK, 1 - already existing</returns>
        public static int Register(string login, string pass, List<User> users)
        {
            foreach (var el in users)
            {
                if (el.Name.Equals(login)) return 1;
            }
            users.Add(new User(users.Count, login, pass));
            return 0;
        }
    }
}
