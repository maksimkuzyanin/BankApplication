using Bank.Common.Abstraction;
using Bank.Common.Dao;
using Bank.Common.Model.Account;

namespace Bank.Services
{
    public class DepositAccountReplenisher : IReplenisher<DepositAccount>
    {
        public DepositAccount Replenish(decimal sum, int id)
        {
            var dao = new AccountDao();

            var account = dao.GetById(id);
            account.Balance += sum;

            return (DepositAccount) account;
        }
    }
}