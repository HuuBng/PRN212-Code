using Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;


namespace Data.BLL
{
    public class UserService
    {
        private readonly UserRepository _repo;
        private readonly string _adminEmail;
        private readonly string _adminPassword;

        public UserService(UserRepository repo, IConfiguration config)
        {
            _repo = repo;
            _adminEmail = config["DefaultAdmin:Email"];
            _adminPassword = config["DefaultAdmin:Password"];
        }

        public string Authenticate(string email, string password)
        {
            if (email.Equals(_adminEmail, StringComparison.OrdinalIgnoreCase) && password == _adminPassword)
            {
                return "Admin";
            }

            var customer = _repo.GetCustomerByEmailAndPassword(email, password);
            if (customer != null)
            {
                return "Customer";
            }
            else
            {
                return null;
            }

        }
    }

}
