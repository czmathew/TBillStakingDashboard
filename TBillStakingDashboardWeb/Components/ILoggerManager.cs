using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationApp.Components
{
    public interface ILoggerManager
    {
        void LogInformation(string message);
        void LogError(string message);
        void LogException(string message, Exception ex);

        void LogDebug(string message);
    }
}
