using System.Windows;
using System.Windows.Controls;
using Bank.Abstraction;
using Bank.App.Contracts;
using Bank.App.Helpers;
using Bank.Common.DTO;
using Bank.Infrastructure.Common.Components;
using Bank.Services;

namespace Bank.App.Pages
{
	/// <summary>
	///     Логика взаимодействия для AccountReplenishPage.xaml
	/// </summary>
	public partial class AccountReplenishPage : Page
    {
        private readonly AccountDto _account;
        private INavigationNotificationsService _notificationsService;

        public AccountReplenishPage(AccountReplenishPageRequest request)
        {
            _account = request.Account;

            Init();
        }

        private void Init()
        {
            InitializeComponent();

            _notificationsService = ComponentManager.Instance.Resolve<INavigationNotificationsService>();

            BALANCE.Content = _account.Balance;
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            GoToAccounts();
        }

        private void GoToAccounts()
        {
            NavigationService.Navigate(new AccountsPage());
        }

        private bool Validate()
        {
            Window.GetWindow(this).CenterMessageBox();

            decimal.TryParse(SUM.Text, out var sum);
            if (sum == decimal.Zero || sum < decimal.Zero)
            {
                MessageBox.Show("Введите ненулевую положительную сумму, на которую нужно пополнить счет",
                    "Ошибка пополнения счета", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void Replenish(object sender, RoutedEventArgs e)
        {
            if (!Validate()) return;

            var accountService = new AccountsService();
            var sum = decimal.Parse(SUM.Text);

            accountService.Replenish(sum, _account.Id, _account.IsDeposit);

            _notificationsService.AddNotification("Счет успешно пополнен");

            GoToAccounts();
        }
    }
}