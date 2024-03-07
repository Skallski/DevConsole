using UnityEngine;

namespace DevConsole.Logging
{
    public class DevConsoleLogData
    {
        private readonly string _timestamp;
        private readonly string _message;

        public LogType LogType { get; private set; }
        public string StackTrace { get; private set; }
        public string FormattedMessage { get; private set; }
        
        public DevConsoleLogData(string message, string stackTrace, LogType logType)
        {
            _timestamp = $"[{System.DateTime.Now:HH:mm:ss}]";
            _message = message;
            StackTrace = stackTrace;
            LogType = logType;
            
            FormattedMessage = GetFormattedMessage(logType);
        }

        private string GetFormattedMessage(LogType logType)
        {
            switch (logType)
            {
                default:
                case LogType.Log:
                    return $"<color=white>{_timestamp}: {_message}</color>";
                case LogType.Error:
                case LogType.Exception:
                    return $"<color=red>{_timestamp}: {_message}</color>";
                case LogType.Assert:
                    return $"<color=green>{_timestamp}: {_message}</color>";
                case LogType.Warning:
                    return $"<color=yellow>{_timestamp}: {_message}</color>";
            }
        }

        public void SetSelected(bool value)
        {
            string text = GetFormattedMessage(LogType);
            FormattedMessage = value ? $"<mark=#ffffff26>{text}</mark>" : text;
        }
    }
}