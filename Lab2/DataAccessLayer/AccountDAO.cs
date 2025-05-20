using BusinessObjects;

namespace DataAccessLayer
{
    public class AccountDAO
    {
        public static AccountMember GetAccountById(string accountID)
        {
            using var db = new MyStoreContext();
            return db.AccountMembers.FirstOrDefault(x => x.MemberId.Equals(accountID));
        }
    }
}