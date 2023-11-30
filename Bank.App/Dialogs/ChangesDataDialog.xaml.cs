using System.Windows;
using Bank.Common.Model;

namespace Bank.App.Dialogs
{
	/// <summary>
	///     Логика взаимодействия для ChangesDataDialog.xaml
	/// </summary>
	public partial class ChangesDataDialog : Window
    {
        public ChangesDataDialog(PersonChanges personChanges)
        {
            InitializeComponent();
            COMMONCHANGES.ItemsSource = personChanges.CommonChanges;
        }
    }
}