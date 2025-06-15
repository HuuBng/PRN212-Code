
using BusinessObjects;
using DataAccessLayer;

namespace Repositories
{
    public class AccountRepository : IAccountRepository
    {
        public AccountMember GetAccountById(String accountId) => AccountDAO.GetAccountById(accountId);
    }
}
