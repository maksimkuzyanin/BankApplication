using System;
using System.Collections.Generic;
using System.Linq;
using Bank.Common.Abstraction;
using Bank.Common.Dao;
using Bank.Common.DTO;
using Bank.Common.Model.Account;
using Bank.Common.Security;
using Bank.Infrastructure.Common.Components;
using Bank.Infrastructure.Common.Implementation.Context;
using Bank.Infrastructure.Exceptions;

namespace Bank.Services
{
    public class AccountsService
    {
        private readonly AccountDao _accountDao;
        private readonly IExecutionContext _executionContext;
        private readonly PersonDao _personDao;

        private readonly Dictionary<AccountType, IReplenisher<Account>> _replenishers =
            new Dictionary<AccountType, IReplenisher<Account>>
            {
                {AccountType.Deposit, new DepositAccountReplenisher()},
                {AccountType.Undeposit, new AccountReplenisher()}
            };

        public AccountsService()
        {
            _executionContext = ComponentManager.Instance.Resolve<IExecutionContext>();
            _accountDao = new AccountDao();
            _personDao = new PersonDao();
        }

        public TransactionDto[] GetTransactions(int? accountId)
        {
            var query = accountId == null
                ? _accountDao.GetAll().SelectMany(x => x.Transactions)
                : _accountDao.GetById(accountId.Value).Transactions;

            var transactions = query.Select(x => new TransactionDto
            {
                Author = x.Author,
                Holder = x.Holder,
                Amount = x.Amount,
                Date = x.Date,
                Notes = x.Notes
            }).ToArray();

            return transactions;
        }

        /// <summary>
        ///     Открывает счет
        /// </summary>
        /// <param name="sum"></param>
        /// <param name="personId"></param>
        /// <param name="isDeposit"></param>
        public void Open(decimal sum, int personId, bool isDeposit = false)
        {
            var account = CreateAccount(personId, sum, isDeposit);
            var transaction = BuildTransaction(personId, sum, account.OpenDate, $@"Открытие счета +{sum}");

            account.Transactions.Add(transaction);
            _accountDao.Save(account);
        }

        private Account CreateAccount(int personId, decimal sum, bool isDeposit)
        {
            if (isDeposit) return new DepositAccount {PersonId = personId, Balance = sum, OpenDate = DateTime.Now};
            return new Account {PersonId = personId, Balance = sum, OpenDate = DateTime.Now};
        }

        private Transaction BuildTransaction(int personId, decimal balance, DateTime date, string amount)
        {
            var person = _personDao.GetById(personId);
            if (person == null) throw new AppException("Несуществующий пользователь");

            var author = _executionContext.Principal is Manager ? "Менеджер" : "Консультант";
            var holder = person.LastName + " " + person.FirstName.First() + "." + person.MiddleName.First() + ".";

            return new Transaction(author, holder, balance, date, amount);
        }

        /// <summary>
        ///     Пополняет счет
        /// </summary>
        /// <param name="sum"></param>
        /// <param name="id"></param>
        /// <param name="isDeposit"></param>
        public void Replenish(decimal sum, int id, bool isDeposit)
        {
            var account = _accountDao.GetById(id);
            var accountType = isDeposit ? AccountType.Deposit : AccountType.Undeposit;

            CheckAccount(account);

            if (account.IsDeposit != isDeposit) throw new AppException("account type does not match");

            var replenisher = _replenishers[accountType];

            var replenishedAccount = replenisher.Replenish(sum, id);
            var replenishTransaction =
                BuildTransaction(replenishedAccount.PersonId, sum, DateTime.Now, $"Пополнение счета +{sum}");

            replenishedAccount.Transactions.Add(replenishTransaction);

            _accountDao.Save(replenishedAccount);
        }

        /// <summary>
        /// </summary>
        /// <param name="account"></param>
        /// <exception cref="AppException"></exception>
        private void CheckAccount(Account account)
        {
            if (account == null) throw new AppException("invalid id");
            if (account.Closed) throw new AppException("account closed");
        }

        /// <summary>
        ///     Переводит сумму со счета на счет
        /// </summary>
        /// <param name="sum"></param>
        /// <param name="idFrom"></param>
        /// <param name="idTo"></param>
        /// <param name="closeAccount"></param>
        public void Transfer(decimal sum, int idFrom, int idTo, bool closeAccount = false)
        {
            var accountFrom = _accountDao.GetById(idFrom);
            var accountTo = _accountDao.GetById(idTo);

            CheckAccount(accountFrom);
            CheckAccount(accountTo);

            var personFrom = _personDao.GetById(accountFrom.PersonId);
            var personTo = _personDao.GetById(accountTo.PersonId);

            ITransferer<Account> simpleTransferer = new SimpleTransferer();
            ITransferer<DepositAccount> transferer = simpleTransferer;

            if (closeAccount)
            {
                accountFrom.Closed = true;
                accountFrom.CloseDate = DateTime.Now;
            }

            var date = DateTime.Now;
            var transactionFrom = BuildTransaction(personFrom.Id, accountFrom.Balance, date,
                $"Перевод клиенту {personTo.Fio} {sum}");
            var transactionTo = BuildTransaction(personTo.Id, accountTo.Balance, date,
                $"=>Входящий перевод от {personFrom.Fio} +{sum}");

            accountFrom.Transactions.Add(transactionFrom);
            accountTo.Transactions.Add(transactionTo);

            if (accountTo.IsDeposit)
                transferer.Transfer(sum, accountFrom, (DepositAccount) accountTo);
            else
                simpleTransferer.Transfer(sum, accountFrom, accountTo);
        }

        public Account[] GetAccounts(int? personId = null)
        {
            var spec = BuildSpec(personId);
            var accounts = _accountDao.Get(spec).ToArray();

            return accounts;
        }

        public AccountDto[] GetAccountsInfo(int? personId = null)
        {
            var spec = BuildSpec(personId);
            var proj = BuildProj();

            var accounts = _accountDao.Get(spec, proj).ToArray();

            return accounts;
        }

        private Func<Account, AccountDto> BuildProj()
        {
            return x => new AccountDto
            {
                Id = x.Id,
                Balance = x.Balance,
                OpenDate = x.OpenDate,
                CloseDate = x.CloseDate,
                DaysPeriod = x.Days == null ? "Не задан" : $"{x.Days} дн.",
                AccountType = x.IsDeposit ? "Депозитный" : "Недепозитный",
                IsDeposit = x.IsDeposit
            };
        }

        private Func<Account, bool> BuildSpec(int? personId)
        {
            Func<Account, bool> notClosedSpec = x => !x.Closed;
            Func<Account, bool> personSpec = x => x.PersonId == personId;

            return personId == null ? notClosedSpec : x => notClosedSpec(x) && personSpec(x);
        }

        private AccountType GeAccountType(Account account)
        {
            return account.IsDeposit ? AccountType.Deposit : AccountType.Undeposit;
        }

        public AccountType[] GeAccountsTypes(int personId)
        {
            return GetAccounts(personId).Select(GeAccountType).ToArray();
        }

        /// <summary>
        /// </summary>
        public void CalculatePercent()
        {
            var accounts = GetAccounts();

            foreach (var account in accounts)
            {
                if (account.Balance <= 0) continue;

                var days = 30;
                var totalDays = (int) (DateTime.Now - account.OpenDate).TotalDays + 1;

                if (totalDays / days == 0) continue;

                var defaultPercent = (decimal) 4.7;
                var amount = Calculate(account.Balance, days, defaultPercent);

                var person = _personDao.GetById(account.PersonId);
                if (person == null) continue;

                var author = "Банк";
                var holder = person.LastName + " " + person.FirstName.First() + "." + person.MiddleName.First() + ".";
                var percentTransaction = new Transaction(author, holder, amount, DateTime.Now,
                    $"Начисление процентов +{amount}");

                account.Balance += amount;
                account.Transactions.Add(percentTransaction);
            }

            var wrappedAccounts = new List<Account>(accounts);
            _accountDao.Save(wrappedAccounts);
        }

        /// <summary>
        ///     Рассчитывает проценты по формуле S=(P*I*t:K):100, где
        ///     S – начисленный профит,
        ///     P – баланс счета,
        ///     I – годовая ставка,
        ///     t – срок (к-во дней),
        ///     K – число дней в году (беру 365).
        /// </summary>
        /// <param name="balance"></param>
        /// <param name="days"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        private decimal Calculate(decimal balance, int days, decimal percent)
        {
            var P = balance;
            var I = percent;
            var t = days;
            var K = 365;

            var S = P * I * t / K / 100;

            return S;
        }
    }
}