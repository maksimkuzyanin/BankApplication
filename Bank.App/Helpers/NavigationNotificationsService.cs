using Bank.Abstraction;
using Bank.App.Dialogs;
using Bank.Infrastructure.Helpers;

namespace Bank.App.Helpers
{
    /// <summary>
    ///     Сервис работает с уведомлениями в системе
    /// </summary>
    public class NavigationNotificationsService : INavigationNotificationsService
    {
        private readonly EventEmitter _notificationEventEmitter;

        public NavigationNotificationsService()
        {
            _notificationEventEmitter = new EventEmitter();
        }

        /// <summary>
        ///     Регистрирует уведомление
        /// </summary>
        /// <param name="notification"></param>
        public void AddNotification(string notification)
        {
            // при зажигании события пользователю будет показано уведомление в виде диалогового окна
            _notificationEventEmitter.On((sender, args) => { new NotifyDialog(notification).ShowDialog(); });
        }

        /// <summary>
        ///     Показывает накопленные уведомления
        /// </summary>
        public void ProcessNotifications()
        {
            // зажигает событие
            _notificationEventEmitter.Emit();
            // разрегистрирует
            _notificationEventEmitter.Off();
        }
    }
}