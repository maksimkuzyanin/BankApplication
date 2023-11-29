using System.IO;

namespace Bank.Infrastructure.Common.Configuration
{
    /// <summary>
    ///     Конфигурация системы
    /// </summary>
    public class Configuration
    {
        private static Configuration _instance;

        public static Configuration Instance => _instance ?? (_instance = new Configuration());

        /// <summary>
        ///     Путь к папке приложения
        /// </summary>
        public string InstallationPath { get; private set; }

        /// <summary>
        ///     Путь к логам приложения
        /// </summary>
        public string LogPath { get; set; }

        /// <summary>
        ///     Инициализирует конфигурацию приложения
        /// </summary>
        /// <param name="path"></param>
        public void Init(string path)
        {
            InstallationPath = path;
            LogPath = Path.Combine(path, "logs");
        }
    }
}