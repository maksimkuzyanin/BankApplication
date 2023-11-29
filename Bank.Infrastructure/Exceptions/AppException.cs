using System;

namespace Bank.Infrastructure.Exceptions
{
    /// <summary>
    ///     Ошибка приложения
    /// </summary>
    public class AppException : Exception
    {
        public AppException(string message) : base(message)
        {
        }
    }
}