using Bank.Common.Abstraction;
using Bank.Common.Dao;
using Bank.Common.Model.Account;

namespace Bank.Services
{
    public class SimpleTransferer : ITransferer<Account>
    {
        public void Transfer(decimal sum, Account accountFrom, Account accountTo)
        {
            var dao = new AccountDao();

            accountFrom.Balance -= sum;
            accountTo.Balance += sum;

            dao.Save(accountFrom);
            dao.Save(accountTo);
        }
    }
}