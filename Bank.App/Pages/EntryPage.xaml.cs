using System;
using System.Windows;
using System.Windows.Controls;
using Bank.Common.Security;
using Bank.Infrastructure.Common.Abstraction.Security.Principals;
using Bank.Infrastructure.Common.Components;
using Bank.Infrastructure.Common.Implementation.Context;

namespace Bank.App.Pages
{
	/// <summary>
	///     Логика взаимодействия для EntryPage.xaml
	/// </summary>
	public partial class EntryPage : Page
    {
        private const string BANKSTART_PAGE_URI = "Pages/BankStartPage.xaml";
        private readonly IExecutionContext _executionContext;

        public EntryPage()
        {
            _executionContext = ComponentManager.Instance.Resolve<IExecutionContext>();
            Init();
        }

        private void Init()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Логинится в приложении
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoLogin(object sender, RoutedEventArgs e)
        {
            // строит принципала - менеджер или консультант
            var bankPrincipal = BuildPrincipal();

            _executionContext.CleanPrincipal();
            _executionContext.SetPrincipal(bankPrincipal);

            // направляет на экран приветствия
            NavigationService.Navigate(new Uri(BANKSTART_PAGE_URI, UriKind.Relative));
        }

        /// <summary>
        ///     Строит принципала
        /// </summary>
        /// <returns></returns>
        private IBankPrincipal BuildPrincipal()
        {
            if (PRINCIPALS.SelectedIndex == 1) return new Manager {UserId = 2, NickName = "Менеджер"};

            return new Consultant {UserId = 1, NickName = "Консультант"};
        }
    }
}