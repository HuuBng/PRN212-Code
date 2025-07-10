using Objects;

namespace DataAccessObjects
{
    public class UserAccountDAO(ResearchDbContext dbContext)
    {
        public UserAccount? Login(string email, string password)
        {
            return dbContext.UserAccount.FirstOrDefault(u => u.Email == email && u.Password == password);
        }
    }
}
