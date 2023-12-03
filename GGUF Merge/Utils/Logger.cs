using Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class Logger
    {
        private static readonly object _fileSynchronizationLock = new object();
        private static readonly object _consoleSynchronizationLock = new object();

        public static void Log(string message, bool isWriteInConsole = true)
        {
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {message}";

            LogInDirectory(message, Settings.LogsDirectory);

            if (isWriteInConsole)
            {
                lock (_consoleSynchronizationLock)
                {
                    Console.WriteLine(logEntry);
                }
            }   
        }

        private static void LogInDirectory(string message, string directory)
        {
            Task.Run(() =>
            {
                directory = Path.GetFullPath(directory);

                try
                {
                    lock(_fileSynchronizationLock)
                    {
                        if (Directory.Exists(directory) == false)
                            Directory.CreateDirectory(directory);

                        string logFileName = $"{directory}\\log_{DateTime.Now:yyyy-MM-dd}.txt";
                        string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {message}";

                        using (StreamWriter writer = File.AppendText(logFileName))
                        {
                            writer.WriteLine(logEntry);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Log error: {ex.Message}");
                }
            });
        }
    }
}
