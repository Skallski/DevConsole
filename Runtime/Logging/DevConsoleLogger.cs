using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace DevConsole.Logging
{
    public class DevConsoleLogger : MonoBehaviour
    {
        internal readonly List<DevConsoleLogData> Logs = new List<DevConsoleLogData>();
        internal static event Action<DevConsoleLogData> OnLogReceived;
        internal static event Action OnClear;

        private void OnEnable()
        {
            Application.logMessageReceived += OnLogMessageReceived;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= OnLogMessageReceived;
        }
        
        private void OnLogMessageReceived(string message, string stackTrace, LogType logType)
        {
            DevConsoleLogData devConsoleLogData = new DevConsoleLogData(message, stackTrace, logType);
            Logs.Add(devConsoleLogData);

            OnLogReceived?.Invoke(devConsoleLogData);
        }

        [UsedImplicitly]
        public void ClearLogs()
        {
            Logs.Clear();
            OnClear?.Invoke();
        }
    }
}