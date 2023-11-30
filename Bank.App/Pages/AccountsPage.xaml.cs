using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Caching;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Bank.Abstraction;
using Bank.App.Contracts;
using Bank.App.Dialogs;
using Bank.App.Helpers;
using Bank.Common.DTO;
using Bank.Common.Security;
using Bank.Infrastructure.Common.Components;
using Bank.Infrastructure.Common.Implementation.Context;
using Bank.Infrastructure.Helpers;
using Bank.Services;

namespace Bank.App.Pages
{
	/// <summary>
	///     Логика взаимодействия для AccountsPage.xaml
	/// </summary>
	public partial class AccountsPage : Page
    {
        private AccountsService _accountsService;
        private Cache _cache;
        private bool _isConsultant;

        private LongWorkService _longWork;
        private INavigationNotificationsService _notificationsService;
        private EnhanceComboSearchService _searchService;

        public AccountsPage()
        {
            Init();
        }

        private void Init()
        {
            InitializeComponent();

            _cache = CacheService.GetCache();
            _notificationsService = ComponentManager.Instance.Resolve<INavigationNotificationsService>();

            var personsService = new PersonsService();

            var comboCollection = MapPersons(personsService.GetPersons());

            _longWork = new LongWorkService(BusyIndicator);
            _searchService = new EnhanceComboSearchService(comboCollection, true);
            _accountsService = new AccountsService();

            searchBox.ItemsSource = _searchService.SearchCollection;

            if (_cache.Get(States.PersonId) is int personId)
            {
                searchBox.SelectedItem = _searchService.SearchCollection.FirstOrDefault(x => x.Id == (int?) personId);
                Load();
            }
            else if (_searchService.WithAll && _searchService.CanAll)
            {
                searchBox.SelectedIndex = 0;
            }

            var executionContext = ComponentManager.Instance.Resolve<IExecutionContext>();
            _isConsultant = !(executionContext.Principal is Manager);

            if (_isConsultant)
            {
                HideBtn(OpenBtn);
                HideBtn(CloseBtn);
                HideBtn(ReplenishBtn);
                HideBtn(TransferBtn);
            }
            else
            {
                CloseBtn.IsEnabled = false;
                ReplenishBtn.IsEnabled = false;
                TransferBtn.IsEnabled = false;
            }
        }

        private void AccountsPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).CenterMessageBox();
            _notificationsService.ProcessNotifications();
        }

        private void HideBtn(Button btn)
        {
            btn.Visibility = Visibility.Collapsed;
        }

        private EnhanceComboBoxItem[] MapPersons(NamedEntityDto[] persons)
        {
            var ret = persons.Select(x => new EnhanceComboBoxItem {Id = x.Id, Name = x.Name}).ToArray();
            return ret;
        }

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

        private void LoadClick_Button(object sender, EventArgs e)
        {
            if (TryGetSelectedPerson(out _)) Load();
        }

        private void Load()
        {
            Action work = () =>
            {
                var selected = searchBox.SelectedItem as NamedEntityDto;
                var personId = selected == null || selected.Id == -1 ? (int?) null : selected.Id;
                var accounts = _accountsService.GetAccountsInfo(personId);

                PersonAccountsList.Visibility = Visibility.Hidden;
                PersonAccountsList.ItemsSource = accounts;
            };

            _longWork.Execute(work)
                .ContinueWith(OnReady(), CancellationToken.None, TaskContinuationOptions.None,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }

        private Action<Task> OnReady()
        {
            return task => PersonAccountsList.Visibility = Visibility.Visible;
        }

        private void OpenAccount(object sender, RoutedEventArgs e)
        {
            if (TryGetSelectedPerson(out var person, true))
            {
                _cache[States.PersonId] = person.Id;
                NavigationService.Navigate(new AccountOpenPage(person.Id));
            }
        }

        private void CloseAccount(object sender, RoutedEventArgs e)
        {
            if (TryGetSelectedAccount(out var account))
            {
                var person = (NamedEntityDto) searchBox.SelectedItem;
                _cache[States.PersonId] = person.Id;

                Window.GetWindow(this).CenterMessageBox();
                var result = MessageBox.Show("Вы уверены, что хотите закрыть счет?", "Внимание!",
                    MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.OK)
                {
                    _notificationsService.AddNotification(
                        "Необходимо перевести накопленные средства на любой свой счет.");
                    NavigationService.Navigate(new AccountTransferPage(account, true));
                }
            }
        }

        /// <summary>
        ///     Пополняет счет
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Replenish(object sender, RoutedEventArgs e)
        {
            if (TryGetSelectedAccount(out var account))
            {
                var person = (NamedEntityDto) searchBox.SelectedItem;
                var request = new AccountReplenishPageRequest {Account = account, PersonId = person.Id};
                _cache[States.PersonId] = person.Id;

                NavigationService.Navigate(new AccountReplenishPage(request));
            }
        }

        private bool CanTransfer(AccountDto account)
        {
            if (!account.IsDeposit) return true;

            int.TryParse(account.DaysPeriod.Replace(" ", "").Replace("дн.", ""), out var days);

            if (days == 0) return false;

            var totalDays = (int) (DateTime.Now - account.OpenDate).TotalDays + 1;
            if (totalDays % days != 0) return false;

            return true;
        }

        /// <summary>
        ///     Переводит со счета на счет
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Transfer(object sender, RoutedEventArgs e)
        {
            if (TryGetSelectedAccount(out var account))
            {
                var person = (NamedEntityDto) searchBox.SelectedItem;
                _cache[States.PersonId] = person.Id;

                NavigationService.Navigate(new AccountTransferPage(account));
            }
        }

        /// <summary>
        ///     Отображает данные по совершенным операциям
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetOperationsHistory(object sender, RoutedEventArgs e)
        {
            var account = PersonAccountsList.SelectedItem as AccountDto;
            var dialog = new TransactionsDialog(account?.Id) {Owner = Window.GetWindow(this)};

            dialog.ShowDialog();
        }

        private bool TryGetSelectedAccount(out AccountDto account)
        {
            account = PersonAccountsList.SelectedItem as AccountDto;

            if (account == null)
            {
                Window.GetWindow(this).CenterMessageBox();
                MessageBox.Show("Выберите счет", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Information);

                return false;
            }

            return true;
        }

        private bool TryGetSelectedPerson(out NamedEntityDto person, bool notAll = false)
        {
            person = searchBox.SelectedItem as NamedEntityDto;

            if (person == null || notAll && person.Id == -1)
            {
                Window.GetWindow(this).CenterMessageBox();
                MessageBox.Show("Выберите клиента, которому необходимо открыть счет", "Внимание!", MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return false;
            }

            return true;
        }

        private void SelectionChanged_PersonAccountsList(object sender, SelectionChangedEventArgs e)
        {
            var account = PersonAccountsList.SelectedItem as AccountDto;
            var isSelect = account != null;

            //HistoryBtn.IsEnabled = isSelect;

            if (_isConsultant) return;

            CloseBtn.IsEnabled = isSelect;
            ReplenishBtn.IsEnabled = isSelect;
            TransferBtn.IsEnabled = isSelect && CanTransfer(account);

            if (isSelect)
            {
                var viewItem = (ListViewItem) PersonAccountsList.ItemContainerGenerator.ContainerFromItem(account);

                if (!TransferBtn.IsEnabled)
                {
                    if (viewItem.ToolTip == null)
                        viewItem.ToolTip = new ToolTip
                            {Content = "С депозитного счета нельзя перевести средства до истечения срока счета"};
                }
                else
                {
                    viewItem.ToolTip = null;
                }
            }
        }
    }

	/// <summary>
	///     Сервис реализует функциональность (поиск, выбор ...) для выпадающего списка с поиском
	/// </summary>
	public class EnhanceComboSearchService
    {
        private readonly EnhanceComboBoxItem _allItem = new EnhanceComboBoxItem
            {Id = -1, Name = "Все", IsHighlighted = true};

        public EnhanceComboSearchService(EnhanceComboBoxItem[] collection, bool withAll = false)
        {
            SearchCollection = new ObservableCollection<EnhanceComboBoxItem>(collection);

            NoItems = !SearchCollection.Any();
            WithAll = withAll;
            CanAll = SearchCollection.Count > 1;

            if (WithAll && CanAll)
            {
                SearchCollection.Insert(0, _allItem);
                SelectedItem = _allItem;
            }
        }

        public ObservableCollection<EnhanceComboBoxItem> SearchCollection { get; }
        public EnhanceComboBoxItem SelectedItem { get; set; }
        public bool WithAll { get; }
        public bool CanAll { get; }
        public bool NoItems { get; }

        /// <summary>
        ///     https://stackoverflow.com/questions/27963022/autocomplete-combobox-for-wpf
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="depObj"></param>
        /// <returns></returns>
        private T GetChildOfType<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) return null;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = child as T ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }

            return null;
        }

        /// <summary>
        ///     Ищет элементы списка
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        private List<EnhanceComboBoxItem> Search(string search)
        {
            return SearchCollection
                .Where(x => x.Name.IndexOf(search, StringComparison.InvariantCultureIgnoreCase) != -1)
                .ToList();
        }

        /// <summary>
        ///     Получает фокус и раскрывает выпадающий список
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GotFocus(object sender, RoutedEventArgs e)
        {
            var searchBox = (ComboBox) sender;

            searchBox.IsDropDownOpen = true;
        }

        /// <summary>
        ///     Сбрасывает выбранный элемент, если раскрыт список
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DropDownOpened(object sender, EventArgs e)
        {
            Unselect((ComboBox) sender);
        }

        /// <summary>
        ///     Запоминает выбор и устанавливает элементу подсветку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DropDownClosed(object sender, EventArgs e)
        {
            var searchBox = (ComboBox) sender;

            searchBox.ItemsSource = SearchCollection.ToList();

            Select(searchBox);
            Highlight();
        }

        /// <summary>
        ///     Фиксирует выбранный элемент
        /// </summary>
        /// <param name="searchBox"></param>
        private void Select(ComboBox searchBox)
        {
            if (searchBox.SelectedItem is EnhanceComboBoxItem selected) SelectedItem = selected;

            if (searchBox.SelectedItem == null && SelectedItem != null) searchBox.SelectedItem = SelectedItem;
        }

        /// <summary>
        ///     Подсвечивает выбранный элемент
        /// </summary>
        private void Highlight()
        {
            foreach (var item in SearchCollection) item.IsHighlighted = false;

            if (SelectedItem != null) SelectedItem.IsHighlighted = true;
        }

        /// <summary>
        ///     Сбрасывает выбранный элемент
        /// </summary>
        /// <param name="searchBox"></param>
        private void Unselect(ComboBox searchBox)
        {
            searchBox.SelectedItem = null;
        }

        // --------------- Обрабатывают ввод пользователя и выполняют поиск ---------------
        public void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CommonSearchHandler(sender, e);
        }

        public void PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete) CommonSearchHandler(sender, e);
        }

        public void Pasting(object sender, DataObjectPastingEventArgs e)
        {
            CommonSearchHandler(sender, e);
        }

        private void CommonSearchHandler(object sender, EventArgs e)
        {
            var searchBox = (ComboBox) sender;
            var textBox = GetChildOfType<TextBox>(searchBox);

            searchBox.IsDropDownOpen = true;

            var request = new SearchRequest
            {
                ComboText = searchBox.Text,
                SelectedText = textBox.SelectedText,
                CaretIndex = textBox.CaretIndex
            };

            if (e is TextCompositionEventArgs compositionArgs)
            {
                request.TextComposition = compositionArgs.Text;
                request.SearchEvent = SearchEvent.PreviewTextInput;
            }
            else if (e is DataObjectPastingEventArgs pastingArgs)
            {
                request.PastedText = (string) pastingArgs.DataObject.GetData(typeof(string));
                request.SearchEvent = SearchEvent.Pasting;
            }
            else
            {
                request.SearchEvent = SearchEvent.PreviewKeyUp;
            }

            searchBox.ItemsSource = GetSearchResponse(request);
        }

        private List<EnhanceComboBoxItem> GetSearchResponse(SearchRequest request)
        {
            var textComposition = request.TextComposition;
            var pastedText = request.PastedText;
            var selectedText = request.SelectedText;
            var caretIndex = request.CaretIndex;
            var searchEvent = request.SearchEvent;

            var searchText = request.ComboText;

            if (searchEvent == SearchEvent.PreviewTextInput)
            {
                searchText = string.IsNullOrEmpty(searchText)
                    ? textComposition
                    : searchText.Insert(caretIndex, textComposition);
            }
            else if (searchEvent == SearchEvent.Pasting)
            {
                if (string.IsNullOrEmpty(searchText))
                    searchText = pastedText;
                else if (string.IsNullOrEmpty(selectedText))
                    searchText = searchText.Insert(caretIndex, pastedText ?? "");
                else
                    searchText = searchText.Replace(selectedText, pastedText);
            }

            var result = Search(searchText);
            return result;
        }
    }

    public class EnhanceComboBoxItem : NamedEntityDto
    {
        public bool IsHighlighted { get; set; }
    }

    public class SearchRequest
    {
        public string ComboText { get; set; }
        public string TextComposition { get; set; }
        public string PastedText { get; set; }
        public string SelectedText { get; set; }
        public int CaretIndex { get; set; }
        public SearchEvent SearchEvent { get; set; }
    }

    public enum SearchEvent
    {
        PreviewTextInput,
        PreviewKeyUp,
        Pasting
    }
}