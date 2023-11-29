using System;

namespace Bank.Common.DTO
{
    public class TransactionDto
    {
        public string Author { get; set; }

        public string Holder { get; set; }

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public string Notes { get; set; }
    }
}