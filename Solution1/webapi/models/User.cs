using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace webapi.models
{
    public class User
    {
        public string login { get; set; }
        public string password { get; set; }
        public string role { get; set; }

        //public string email { get; set; }
        //public string firstName { get; set; }
        //public string lastName { get; set; }

        public User(string login, string password, string role)
        {
            this.login = login;
            this.password = password;
            this.role = role;
        }

        public static (User user, string Error) Create(string login, string password, string role)
        {
            var error = string.Empty;

            if (string.IsNullOrEmpty(login)) error = "Login can`t be empty";
            if (string.IsNullOrEmpty(password)) error = "Password can`t be empty";
            if (string.IsNullOrEmpty(role)) error = "Role can`t be empty";

            var user = new User(login, password, role);

            return (user, error);
        }
    }
}
