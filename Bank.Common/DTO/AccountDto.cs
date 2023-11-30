using System;

namespace Bank.Common.DTO
{
    public class AccountDto
    {
        public int Id { get; set; }
        public decimal Balance { get; set; }
        public DateTime OpenDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public string DaysPeriod { get; set; }
        public string AccountType { get; set; }
        public bool IsDeposit { get; set; }
    }
}