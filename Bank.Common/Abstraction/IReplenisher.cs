using Bank.Common.Model.Account;

namespace Bank.Common.Abstraction
{
    public interface IReplenisher<out T>
        where T : Account
    {
        T Replenish(decimal sum, int id);
    }
}