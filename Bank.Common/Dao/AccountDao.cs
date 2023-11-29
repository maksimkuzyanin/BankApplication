using Bank.Common.Model.Account;

namespace Bank.Common.Dao
{
    public class AccountDao : EntityDao<Account>
    {
        protected override string Connection => "accounts.json";
    }
}