using System;
using System.IO;
using System.Windows.Threading;
using Bank.Infrastructure.Common.Components;
using Bank.Infrastructure.Common.Configuration;
using Bank.Infrastructure.Common.Implementation.Context;
using Serilog;
using Serilog.Core;

namespace Bank.Infrastructure.Exceptions
{
    /// <summary>
    ///     Обработчик ошибок в приложении.
    ///     Пишет стек ошибки в логи
    /// </summary>
    public class ErrorHandler
    {
        // логгер
        private readonly Logger _log = new LoggerConfiguration()
            .WriteTo.File(Path.Combine(Configuration.Instance.LogPath, "log.txt"), rollingInterval: RollingInterval.Day)
            .CreateLogger();

        public void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                var currCtx = ComponentManager.Instance.Resolve<IExecutionContext>();
                var principal = currCtx.Principal;

                if (e.Exception.GetType() == typeof(AppException))
                    _log.Error(e.Exception, $@"Произошла ошибка приложения в сеансе пользователя {principal.NickName}");
                else
                    _log.Error(e.Exception, "Ошибка");

                e.Handled = true;
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error get context");
            }
        }
    }
}