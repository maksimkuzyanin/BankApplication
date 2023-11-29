using Bank.Common.Model.Account;

namespace Bank.Common.Abstraction
{
    public interface ITransferer<in T>
        where T : Account
    {
        void Transfer(decimal sum, Account accountFrom, T accountTo);
    }
}