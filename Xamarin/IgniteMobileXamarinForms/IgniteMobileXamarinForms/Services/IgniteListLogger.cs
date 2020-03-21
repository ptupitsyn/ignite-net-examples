using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Apache.Ignite.Core.Log;

namespace IgniteMobileXamarinForms.Services
{
    public class IgniteListLogger : ILogger
    {
        private readonly ConcurrentQueue<string> _logs = new ConcurrentQueue<string>();

        public void Log(LogLevel level, string message, object[] args, IFormatProvider formatProvider, string category,
            string nativeErrorInfo, Exception ex)
        {
            message = args == null ? message : string.Format(message, args);

            _logs.Enqueue($"{category}: {message}");
        }

        public bool IsEnabled(LogLevel level)
        {
            return true;
        }

        public ICollection<string> GetLogs()
        {
            return _logs.ToArray();
        }
    }
}
