namespace Bank.Abstraction
{
    public interface INavigationNotificationsService
    {
        void AddNotification(string notification);
        void ProcessNotifications();
    }
}