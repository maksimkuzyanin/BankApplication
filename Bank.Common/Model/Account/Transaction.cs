using System;

namespace Bank.Common.Model.Account
{
    public class Transaction
    {
        public Transaction(string author, string holder, decimal amount, DateTime date, string note)
        {
            Author = author;
            Holder = holder;
            Amount = amount;
            Date = date;
            Notes = note;
        }

        public string Author { get; }

        public string Holder { get; }

        public decimal Amount { get; }
        public DateTime Date { get; }

        public string Notes { get; set; }
    }
}