using FluentScheduler;

namespace Bank.Services.Jobs
{
    public class PercentCalculationJob : IJob
    {
        private readonly AccountsService _accountsService;

        public PercentCalculationJob()
        {
            _accountsService = new AccountsService();
        }

        public void Execute()
        {
            _accountsService.CalculatePercent();
        }
    }
}