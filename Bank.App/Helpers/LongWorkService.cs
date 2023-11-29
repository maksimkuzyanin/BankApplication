using System;
using System.Threading;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit;

namespace Bank.App.Helpers
{
    public class LongWorkService
    {
        private readonly BusyIndicator _busyIndicator;

        public LongWorkService(BusyIndicator busyIndicator)
        {
            _busyIndicator = busyIndicator;
        }

        public void Show()
        {
            _busyIndicator.IsBusy = true;
        }

        public void Close()
        {
            _busyIndicator.IsBusy = false;
        }

        public Task Execute(Action work)
        {
            // показывает процессинг
            Show();
            // выполняет работу
            work();
            // имитирует работу и по истечении времени скрывает процессинг
            return Task.Run(() => { Thread.Sleep(2000); }).ContinueWith(task => Close(), CancellationToken.None,
                TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}