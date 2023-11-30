using System.Windows;

namespace Bank.App.Dialogs
{
    public partial class NotifyDialog : Window
    {
        public NotifyDialog(string notify)
        {
            InitializeComponent();
            Notify.Text = notify;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}