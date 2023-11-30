using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Bank.Abstraction;
using Bank.App.Helpers;
using Bank.Common.DTO;
using Bank.Common.Model.Account;
using Bank.Infrastructure.Common.Components;
using Bank.Services;

namespace Bank.App.Pages
{
	/// <summary>
	///     Логика взаимодействия для AccountOpenPage.xaml
	/// </summary>
	public partial class AccountOpenPage : Page
    {
        private readonly NamedEntityDto[] _accountTypes =
        {
            new NamedEntityDto {Id = (int) AccountType.Undeposit, Name = "Недепозитный"},
            new NamedEntityDto {Id = (int) AccountType.Deposit, Name = "Депозитный"}
        };

        private readonly int _personId;

        private INavigationNotificationsService _notificationsService;

        public AccountOpenPage(int personId)
        {
            _personId = personId;
            Init();
        }

        private void Init()
        {
            _notificationsService = ComponentManager.Instance.Resolve<INavigationNotificationsService>();

            var accountsTypes = new AccountsService().GeAccountsTypes(_personId);
            var accountTypes = _accountTypes.Where(x => !accountsTypes.Contains((AccountType) x.Id)).ToArray();
            var comboList = new ObservableCollection<NamedEntityDto>(accountTypes);

            var selected = comboList.FirstOrDefault();
            var readOnly = !comboList.Any();

            InitializeComponent();

            if (!readOnly)
            {
                ACCOUNTTYPE.ItemsSource = accountTypes;
                ACCOUNTTYPE.SelectedItem = selected;
                DataContext = this;
            }

            BALANCE.IsEnabled = !readOnly;
            ACCOUNTTYPE.IsEnabled = !readOnly;
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

            decimal.TryParse(BALANCE.Text, out var balance);
            if (balance == decimal.Zero || balance < decimal.Zero)
            {
                MessageBox.Show("Введите ненулевой положительный баланс", "Ошибка открытия вклада", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }

            if (ACCOUNTTYPE.SelectedItem == null)
            {
                MessageBox.Show("Выберите тип открываемого счета", "Ошибка открытия вклада", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void OpenAccount(object sender, RoutedEventArgs e)
        {
            if (!Validate()) return;

            var accountService = new AccountsService();

            // данные нового счета
            decimal.TryParse(BALANCE.Text, out var balance);
            var accountTypeDto = (NamedEntityDto) ACCOUNTTYPE.SelectedItem;

            accountService.Open(balance, _personId, accountTypeDto.Id == (int) AccountType.Deposit);

            _notificationsService.AddNotification("Счет успешно открыт");

            GoToAccounts();
        }
    }
}