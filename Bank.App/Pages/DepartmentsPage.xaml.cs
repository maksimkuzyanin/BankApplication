using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Bank.Common.Dao;
using Bank.Common.Model;

namespace Bank.App.Pages
{
	/// <summary>
	///     Логика взаимодействия для DepartmentsPage.xaml
	///     Список департаментов
	/// </summary>
	public partial class DepartmentsPage : Page
    {
        public DepartmentsPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        public List<Department> DepartmentsData => new DepartmentDao().GetAll().ToList();


        private void GoBack(object sender, RoutedEventArgs e)
        {
        }
    }
}