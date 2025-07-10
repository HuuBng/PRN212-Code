using DataAccessObjects;
using Objects;

namespace Services
{
    public class UserAccountService
    {
        private readonly UserAccountDAO userAccountDAO;
        public UserAccountService()
        {
            userAccountDAO = new UserAccountDAO(new ResearchDbContext());
        }

        public UserAccount? Login(string email, string password)
        {
            return userAccountDAO.Login(email, password);
        }
    }
}
