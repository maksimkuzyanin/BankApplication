using Bank.Common.DTO;

namespace Bank.App.Contracts
{
    public class AccountReplenishPageRequest
    {
        public AccountDto Account { get; set; }
        public int PersonId { get; set; }
    }
}