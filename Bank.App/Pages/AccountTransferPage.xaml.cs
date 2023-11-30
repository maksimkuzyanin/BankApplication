using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Bank.Abstraction;
using Bank.App.Helpers;
using Bank.Common.DTO;
using Bank.Common.Model.Account;
using Bank.Infrastructure.Common.Components;
using Bank.Services;

namespace Bank.App.Pages
{
	/// <summary>
	///     Логика взаимодействия для AccountTransferPage.xaml
	/// </summary>
	public partial class AccountTransferPage : Page
    {
        private readonly AccountDto _account;
        private readonly bool _closeAccount;
        private AccountsService _accountsService;
        private INavigationNotificationsService _notificationsService;
        private NamedEntityDto _person;
        private PersonsService _personsService;
        private EnhanceComboSearchService _searchService;

        public AccountTransferPage(AccountDto account, bool closeAccount = false)
        {
            _account = account;
            _closeAccount = closeAccount;

            Init();
        }

        /// <summary>
        ///     Инициализирует страницу (хелперы, компоненты разметки, сервисы) и выполняет загрузку данных
        /// </summary>
        private void Init()
        {
            InitializeComponent();

            _notificationsService = ComponentManager.Instance.Resolve<INavigationNotificationsService>();
            _accountsService = new AccountsService();
            _personsService = new PersonsService();
            _person = _personsService.GetPersonByAccountId(_account.Id);

            var comboCollection = MapAccounts(_accountsService.GetAccounts());

            _searchService = new EnhanceComboSearchService(comboCollection);

            accountsBox.ItemsSource = _searchService.SearchCollection;

            PERSON.Text = _person.Name;
            ACID.Content = _account.Id;
            BALANCE.Content = _account.Balance;

            if (_searchService.NoItems)
            {
                accountsBox.Visibility = Visibility.Collapsed;
                NoAccounts.Visibility = Visibility.Visible;
            }

            if (_closeAccount)
            {
                SUM.Text = _account.Balance.ToString();
                SUM.IsEnabled = false;
            }
        }

        /// <summary>
        ///     Обрабатывает событие загрузки страницы
        ///     Здесь по загрузке обрабатываются пришедшие с других экранов уведомления
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AccountTransferPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).CenterMessageBox();
            _notificationsService.ProcessNotifications();
        }

        /// <summary>
        ///     Мапит счета для перевода в дто'шку
        /// </summary>
        /// <param name="accounts"></param>
        /// <returns></returns>
        private TransferedAccountItem[] MapAccounts(Account[] accounts)
        {
            var filteredAccounts = accounts;
            if (_closeAccount) filteredAccounts = filteredAccounts.Where(x => x.PersonId == _person.Id).ToArray();

            var items = filteredAccounts
                .Where(x => x.Id != _account.Id)
                .Select(x => new TransferedAccountItem {Id = x.Id}).ToArray();
            var personsIds = filteredAccounts.Select(x => x.PersonId).Distinct().ToArray();
            var persons = _personsService.GetPersons(personsIds);

            Fill(items, filteredAccounts, persons);

            return items;
        }

        /// <summary>
        ///     Заполняет данные в счетах для перевода из других связанных сущностей
        /// </summary>
        /// <param name="items"></param>
        /// <param name="accounts"></param>
        /// <param name="persons"></param>
        private void Fill(TransferedAccountItem[] items, Account[] accounts, NamedEntityDto[] persons)
        {
            foreach (var item in items)
            {
                var account = accounts.FirstOrDefault(x => x.Id == item.Id);
                var person = persons.FirstOrDefault(x => x.Id == account.PersonId);

                item.AccountNum = item.Id.ToString().PadLeft(6, '0');
                item.Fio = person.Name;
                item.IsDeposit = account.IsDeposit;
                item.AccountType = account.IsDeposit ? "депозитный" : "недепозитный";
                item.Name = item.AccountNum + item.AccountType + item.Fio;
            }
        }

        /// <summary>
        ///     Возвращает к списку клиентов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoBack(object sender, RoutedEventArgs e)
        {
            GoToAccounts();
        }

        private void GoToAccounts()
        {
            NavigationService.Navigate(new AccountsPage());
        }

        /// <summary>
        ///     Валидирует форму
        /// </summary>
        /// <returns></returns>
        private bool Validation()
        {
            Window.GetWindow(this).CenterMessageBox();

            decimal.TryParse(SUM.Text, out var sum);
            if (sum == decimal.Zero || sum < decimal.Zero)
            {
                MessageBox.Show("Введите ненулевую положительную сумму", "Ошибка перевода", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }

            if (sum > _account.Balance)
            {
                MessageBox.Show("На счете недостаточно средств", "Ошибка перевода", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }

            if (accountsBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите счет зачисления", "Ошибка перевода", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Осуществляет перевод
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Transfer(object sender, RoutedEventArgs e)
        {
            if (!Validation()) return;

            var sum = decimal.Parse(SUM.Text);
            var accountTo = accountsBox.SelectedItem as TransferedAccountItem;

            _accountsService.Transfer(sum, _account.Id, accountTo.Id, _closeAccount);

            _notificationsService.AddNotification(_closeAccount ? "Счет успешно закрыт" : "Перевод успешно совершен");

            GoToAccounts();
        }

        // --------------- Реализуют функциональность выпадающего списка с поиском ---------------
        // поиск
        private void GotFocus_EnhanceComboSearch(object sender, RoutedEventArgs e)
        {
            _searchService.GotFocus(sender, e);
        }

        private void DropDownOpened_EnhanceComboSearch(object sender, EventArgs e)
        {
            _searchService.DropDownOpened(sender, e);
        }

        private void DropDownClosed_EnhanceComboSearch(object sender, EventArgs e)
        {
            _searchService.DropDownClosed(sender, e);
        }

        private void PreviewTextInput_EnhanceComboSearch(object sender, TextCompositionEventArgs e)
        {
            _searchService.PreviewTextInput(sender, e);
        }

        private void PreviewKeyUp_EnhanceComboSearch(object sender, KeyEventArgs e)
        {
            _searchService.PreviewKeyUp(sender, e);
        }

        private void Pasting_EnhanceComboSearch(object sender, DataObjectPastingEventArgs e)
        {
            _searchService.Pasting(sender, e);
        }

        /// <summary>
        ///     Открывает счет
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenAccount(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AccountOpenPage(_person.Id));
        }
    }

    internal class TransferedAccountItem : EnhanceComboBoxItem
    {
        public string AccountNum { get; set; }
        public string Fio { get; set; }
        public string AccountType { get; set; }
        public bool IsDeposit { get; set; }
    }
}