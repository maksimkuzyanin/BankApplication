using System;
using System.Collections.Generic;
using Bank.Common.Abstraction;

namespace Bank.Common.Model.Account
{
    public class Account : IEntity
    {
        public Account()
        {
            Transactions = new List<Transaction>();
        }

        public int PersonId { get; set; }
        public decimal Balance { get; set; }

        public DateTime OpenDate { get; set; }
        public DateTime? CloseDate { get; set; }

        public int? Days => IsDeposit ? 30 : (int?) null;

        public bool Closed { get; set; }

        public bool IsDeposit => this is DepositAccount;

        public List<Transaction> Transactions { get; set; }

        public int Id { get; set; }
    }
}