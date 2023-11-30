using System.Windows;
using Bank.Services;

namespace Bank.App.Dialogs
{
    public partial class TransactionsDialog : Window
    {
        private readonly int? _accountId;
        private readonly AccountsService _accountsService;

        public TransactionsDialog(int? accountId)
        {
            _accountId = accountId;
            _accountsService = new AccountsService();

            Init();
        }

        private void Init()
        {
            InitializeComponent();

            TRANSACTIONS.ItemsSource = _accountsService.GetTransactions(_accountId);
        }
    }
}