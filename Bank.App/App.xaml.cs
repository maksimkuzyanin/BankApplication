using System.IO;
using System.Windows;
using System.Windows.Threading;
using Bank.Abstraction;
using Bank.App.Helpers;
using Bank.Infrastructure.Common.Components;
using Bank.Infrastructure.Common.Configuration;
using Bank.Infrastructure.Common.Implementation.Context;
using Bank.Infrastructure.Exceptions;
using Bank.Services.Jobs;
using FluentScheduler;

namespace Bank.App
{
	/// <summary>
	///     Логика взаимодействия для App.xaml
	/// </summary>
	public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            RegisterComponents();
            RunJobs();

            Configuration.Instance.Init(GetApplicationPath());

            base.OnStartup(e);
        }

        /// <summary>
        ///     Получает путь
        /// </summary>
        /// <returns></returns>
        private string GetApplicationPath()
        {
            var path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            return path;
        }

        /// <summary>
        ///     Регистрирует общеиспользуемые компоненты
        /// </summary>
        private void RegisterComponents()
        {
            if (!ComponentManager.Instance.HasComponent<IExecutionContext>())
                ComponentManager.Instance.Register<IExecutionContext, ExecutionContext>();

            if (!ComponentManager.Instance.HasComponent<INavigationNotificationsService>())
                ComponentManager.Instance.Register<INavigationNotificationsService, NavigationNotificationsService>();
        }

        /// <summary>
        ///     Запускает задачи: 1) Инициализация базы; 2) Расчет процентов по счету
        /// </summary>
        private void RunJobs()
        {
            var registry = new Registry();
            registry.Schedule<InitDbJob>().ToRunNow();
            registry.Schedule<PercentCalculationJob>().ToRunEvery(1).Months().OnTheLastDay().At(0, 0);
            JobManager.Initialize(registry);
        }

        /// <summary>
        ///     Обрабатывает необработанное исключение
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var errorHandler = new ErrorHandler();
            errorHandler.OnDispatcherUnhandledException(sender, e);
        }
    }
}