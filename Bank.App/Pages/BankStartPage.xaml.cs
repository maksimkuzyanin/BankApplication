using System;
using System.Windows;
using System.Windows.Controls;

namespace Bank.App.Pages
{
	/// <summary>
	///     Логика взаимодействия для BankStartPage.xaml
	/// </summary>
	public partial class BankStartPage : Page
    {
        private const string ENTRY_PAGE_URI = "Pages/EntryPage.xaml";

        public BankStartPage()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Осуществляет выход пользователя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Logout(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(ENTRY_PAGE_URI, UriKind.Relative));
        }

        // --------------- Обрабатывают пользовательский клик по вкладке меню и осуществляют переход ---------------
        private void Departments_Click(object sender, RoutedEventArgs e)
        {
            pageFrame.Content = new DepartmentsPage();
        }

        private void Persons_Click(object sender, RoutedEventArgs e)
        {
            pageFrame.Content = new PersonsListPage();
        }

        private void Accounts_Click(object sender, RoutedEventArgs e)
        {
            pageFrame.Content = new AccountsPage();
        }
    }
}