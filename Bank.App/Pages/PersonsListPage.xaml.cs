using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Bank.Abstraction;
using Bank.App.Helpers;
using Bank.Common.Dao;
using Bank.Common.Model;
using Bank.Common.Security;
using Bank.Infrastructure.Common.Components;
using Bank.Infrastructure.Common.Implementation.Context;

namespace Bank.App.Pages
{
	/// <summary>
	///     Логика взаимодействия для PersonsListPage.xaml
	/// </summary>
	public partial class PersonsListPage : Page
    {
        private readonly IExecutionContext _executionContext;
        private readonly INavigationNotificationsService _notificationsService;
        private LongWorkService _longWork;
        private ListViewColumnSorter _personsSorter;

        public PersonsListPage()
        {
            _executionContext = ComponentManager.Instance.Resolve<IExecutionContext>();
            _notificationsService = ComponentManager.Instance.Resolve<INavigationNotificationsService>();
            Init();
        }

        /// <summary>
        ///     Обрабатывает событие загрузки страницы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PersonsListPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).CenterMessageBox();
            _notificationsService.ProcessNotifications();
        }

        /// <summary>
        ///     Инициализирует страницу (хелперы, компоненты разметки) и выполняет загрузку данных
        /// </summary>
        private void Init()
        {
            InitializeComponent();

            _personsSorter = new ListViewColumnSorter(PERSONS);
            _longWork = new LongWorkService(BusyIndicator);

            ADDPERSON.Visibility = _executionContext.Principal is Manager ? Visibility.Visible : Visibility.Hidden;

            OnLoad();
        }

        /// <summary>
        ///     Загружает клиентов и показывает процессинг
        /// </summary>
        private void OnLoad()
        {
            _longWork.Execute(OnLoadPersons())
                .ContinueWith(OnReady(), CancellationToken.None, TaskContinuationOptions.None,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        ///     Загружает клиентов
        /// </summary>
        /// <returns></returns>
        private Action OnLoadPersons()
        {
            return () =>
            {
                var dao = new PersonDao();
                var persons = dao.GetAll().ToArray();

                PERSONS.Visibility = Visibility.Hidden;
                PERSONS.ItemsSource = persons;
            };
        }

        /// <summary>
        ///     Выполняет действие после завершения задачи
        /// </summary>
        /// <returns></returns>
        private Action<Task> OnReady()
        {
            return task => PERSONS.Visibility = Visibility.Visible;
        }

        /// <summary>
        ///     Добавляет клиента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddPerson(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PersonEditPage(new Person()));
        }

        /// <summary>
        ///     Редактирует клиента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditPerson(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var person = button.CommandParameter as Person;

            NavigationService.Navigate(new PersonEditPage(person));
        }

        /// <summary>
        ///     Сортирует список клиентов по колонке
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SortPersons(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            if (headerClicked == null) return;
            var notSortable = string.IsNullOrEmpty(headerClicked.Column.Header as string);
            if (notSortable) return;
            _personsSorter.SortByColumn(headerClicked);
        }
    }
}