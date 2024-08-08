using UnityEngine;

namespace DevConsole.Logging
{
    public class DevConsoleLogData
    {
        private readonly string _timestamp;
        private readonly string _message;

        public readonly LogType LogType;
        public readonly string StackTrace;
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
            string color = logType switch
            {
                LogType.Log => "white",
                LogType.Error => "red",
                LogType.Exception => "red",
                LogType.Assert => "red",
                LogType.Warning => "yellow",
                _ => "white"
            };

            return $"<color={color}>{_timestamp}: {_message}</color>";
        }

        public void SetSelected(bool value)
        {
            string text = GetFormattedMessage(LogType);
            FormattedMessage = value ? $"<mark=#ffffff26>{text}</mark>" : text;
        }
    }
}