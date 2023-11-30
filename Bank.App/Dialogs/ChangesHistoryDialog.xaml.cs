using System.Windows;
using System.Windows.Controls;
using Bank.Common.Dao;
using Bank.Common.Model;

namespace Bank.App.Dialogs
{
	/// <summary>
	///     Логика взаимодействия для ChangesHistoryDialog.xaml
	/// </summary>
	public partial class ChangesHistoryDialog : Window
    {
        public ChangesHistoryDialog(int personId)
        {
            InitializeComponent();
            CHANGES.ItemsSource = new PersonDao()
                .GetChanges(personId);
        }

        private void ViewChanges(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var personChanges = button.CommandParameter as PersonChanges;
            var dialog = new ChangesDataDialog(personChanges);

            dialog.Owner = this;
            dialog.ShowDialog();
        }
    }
}