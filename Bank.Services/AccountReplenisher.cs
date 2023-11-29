using Bank.Common.Abstraction;
using Bank.Common.Dao;
using Bank.Common.Model.Account;

namespace Bank.Services
{
    public class AccountReplenisher : IReplenisher<Account>
    {
        /// <summary>
        ///     Пополняет счет
        /// </summary>
        /// <param name="sum"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Account Replenish(decimal sum, int id)
        {
            var dao = new AccountDao();

            var account = dao.GetById(id);
            account.Balance += sum;

            return account;
        }
    }
}