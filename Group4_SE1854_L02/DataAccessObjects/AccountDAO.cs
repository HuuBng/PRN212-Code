﻿using BusinessObjects;
using System.Linq;
namespace DataAccessLayer
{
    public class AccountDAO
    {

        public static AccountMember GetAccountById(string accountID)
        {
            using var db = new MyStoreContext();
            return db.AccountMembers.FirstOrDefault(c => c.MemberId.Equals(accountID));

        }

    }
}
