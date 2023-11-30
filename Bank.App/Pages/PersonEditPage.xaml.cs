using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Bank.Abstraction;
using Bank.App.Dialogs;
using Bank.App.Helpers;
using Bank.Common.DTO;
using Bank.Common.Model;
using Bank.Common.Security;
using Bank.Infrastructure.Common.Components;
using Bank.Infrastructure.Common.Implementation.Context;
using Bank.Services;

namespace Bank.App.Pages
{
	/// <summary>
	///     Логика взаимодействия для PersonEditPage.xaml
	/// </summary>
	public partial class PersonEditPage : Page
    {
        private const string PERSONS_LIST_PAGE_URI = "Pages/PersonsListPage.xaml";
        private readonly bool _editMode;

        private readonly IExecutionContext _executionContext;
        private readonly INavigationNotificationsService _notificationsService;

        private readonly Person _person;
        private readonly PersonsService _personsService;

        public PersonEditPage(Person person)
        {
            _executionContext = ComponentManager.Instance.Resolve<IExecutionContext>();
            _notificationsService = ComponentManager.Instance.Resolve<INavigationNotificationsService>();
            _personsService = new PersonsService();

            _person = person;
            _editMode = _person.Id != 0;

            Init();
        }

        private PersonEditPageData Data { get; set; }

        /// <summary>
        ///     Загружает данные
        /// </summary>
        private void Load()
        {
            var service = new DepartmentsService();

            var departments = service.GetDepartments();
            var departmentId = service.GetDepartmentId(_person.Id);
            var department = departments.FirstOrDefault(x => x.Id == departmentId);

            Data.Departments = departments;
            Data.Department = department;
        }

        /// <summary>
        ///     Инициализирует страницу (хелперы, компоненты разметки, сервисы) и выполняет загрузку данных
        /// </summary>
        private void Init()
        {
            WindowTitle = _editMode ? "Редактировать персону" : "Добавить персону";
            InitializeComponent();

            Data = new PersonEditPageData();
            Load();

            DEPARTMENTS.ItemsSource = Data.Departments;
            DEPARTMENTS.SelectedItem = Data.Department;

            FIRSTNAME.Text = _person.FirstName;
            LASTNAME.Text = _person.LastName;
            MIDDLENAME.Text = _person.MiddleName;
            PHONE.Text = _person.Phone;
            SERIES.Password = _person.Passport.Series;
            NUMBER.Password = _person.Passport.Number;
            SERIESUNMSK.Text = _person.Passport.Series;
            NUMBERUNMSK.Text = _person.Passport.Number;

            var principal = _executionContext.Principal;

            FIRSTNAME.IsEnabled = principal.CheckSetFioAccess();
            LASTNAME.IsEnabled = principal.CheckSetFioAccess();
            MIDDLENAME.IsEnabled = principal.CheckSetFioAccess();
            DEPARTMENTS.IsEnabled = principal.CheckSetDepartmentAccess();
            SERIES.Visibility = principal.CheckSetPassportAccess() ? Visibility.Hidden : Visibility.Visible;
            NUMBER.Visibility = principal.CheckSetPassportAccess() ? Visibility.Hidden : Visibility.Visible;
            SERIESUNMSK.Visibility = principal.CheckSetPassportAccess() ? Visibility.Visible : Visibility.Hidden;
            NUMBERUNMSK.Visibility = principal.CheckSetPassportAccess() ? Visibility.Visible : Visibility.Hidden;
        }

        /// <summary>
        ///     Валидирует форму редактирования
        /// </summary>
        /// <returns></returns>
        private bool Validate()
        {
            var owner = Window.GetWindow(this);

            // валидация фио
            var firstName = FIRSTNAME.Text;
            var lastName = LASTNAME.Text;
            var middleName = MIDDLENAME.Text;

            owner.CenterMessageBox();

            if (string.IsNullOrEmpty(firstName))
            {
                MessageBox.Show("Имя не должно быть пустым", "Ошибка сохранения", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrEmpty(lastName))
            {
                MessageBox.Show("Фамилия не должна быть пустой", "Ошибка сохранения", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrEmpty(middleName))
            {
                MessageBox.Show("Отчество не должно быть пустым", "Ошибка сохранения", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }

            // валидация телефона
            var phone = PHONE.Text;
            if (string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Номер телефона не должен быть пустым", "Ошибка сохранения", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }

            var cleaned = phone.Replace("+", "").Replace(" ", "").Replace("-", "");

            if (cleaned.Length != 11)
            {
                MessageBox.Show("Длина номера телефона должна быть равна 11 символам", "Ошибка сохранения",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            foreach (var c in cleaned)
                if (!char.IsDigit(c))
                {
                    MessageBox.Show("Номер телефона должен состоять из цифр", "Ошибка сохранения", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return false;
                }

            // валидация паспорта
            var series = SERIESUNMSK.Text;
            var number = NUMBERUNMSK.Text;

            if (string.IsNullOrEmpty(series))
            {
                MessageBox.Show("Серия паспорта не должна быть пустой", "Ошибка сохранения", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }

            if (series.Length != 4)
            {
                MessageBox.Show("Длина серии паспорта должна быть равна 4 символам", "Ошибка сохранения",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            foreach (var c in series)
                if (!char.IsDigit(c))
                {
                    MessageBox.Show("Серия паспорта должна состоять из цифр", "Ошибка сохранения", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return false;
                }

            if (string.IsNullOrEmpty(number))
            {
                MessageBox.Show("Номер паспорта не должен быть пустым", "Ошибка сохранения", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }

            if (number.Length != 6)
            {
                MessageBox.Show("Длина номера паспортп должна быть равна 6 символам", "Ошибка сохранения",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            foreach (var c in number)
                if (!char.IsDigit(c))
                {
                    MessageBox.Show("Номер паспорта должен состоять из цифр", "Ошибка сохранения", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return false;
                }

            if (DEPARTMENTS.SelectedItem == null)
            {
                MessageBox.Show("Выберите отдел", "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Сохраняет данные о клиенте
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save(object sender, RoutedEventArgs e)
        {
            if (!Validate()) return;

            var dto = new PersonDto
            {
                Id = _person.Id,
                FirstName = FIRSTNAME.Text,
                LastName = LASTNAME.Text,
                MiddleName = MIDDLENAME.Text,
                Phone = PHONE.Text,
                Series = SERIESUNMSK.Text,
                Number = NUMBERUNMSK.Text,
                Department = (NamedEntityDto) DEPARTMENTS.SelectedItem
            };


            var author = _executionContext.Principal is Manager ? "Менеджер" : "Консультант";

            _personsService.SavePerson(author, dto);

            _notificationsService.AddNotification("Изменения клиента успешно сохранены");

            NavigationService.Navigate(new Uri(PERSONS_LIST_PAGE_URI, UriKind.Relative));
        }

        /// <summary>
        ///     Показывает историю изменений
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewChanges(object sender, RoutedEventArgs e)
        {
            var owner = Window.GetWindow(this);

            var existsChanges = _personsService.HasChanges(_person.Id);
            if (!existsChanges)
            {
                owner.CenterMessageBox();
                MessageBox.Show(owner, "Нет данных об изменениях", "Внимание!", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            var dialog = new ChangesHistoryDialog(_person.Id) {Owner = owner};
            dialog.Show();
        }

        /// <summary>
        ///     Возвращает на экран со списком клиентов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoBack(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(PERSONS_LIST_PAGE_URI, UriKind.Relative));
        }

        /// <summary>
        ///     Сбрасывает изменения на форме
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Reset(object sender, RoutedEventArgs e)
        {
            FIRSTNAME.Text = _person.FirstName;
            LASTNAME.Text = _person.LastName;
            MIDDLENAME.Text = _person.MiddleName;
            PHONE.Text = _person.Phone;
            SERIES.Password = _person.Passport.Series;
            NUMBER.Password = _person.Passport.Number;
            SERIESUNMSK.Text = _person.Passport.Series;
            NUMBERUNMSK.Text = _person.Passport.Number;
            DEPARTMENTS.SelectedItem = Data.Department;
        }
    }

	/// <summary>
	///     Вспомогательная дто
	/// </summary>
	public class PersonEditPageData
    {
        public NamedEntityDto Department { get; set; }
        public NamedEntityDto[] Departments { get; set; }
    }
}