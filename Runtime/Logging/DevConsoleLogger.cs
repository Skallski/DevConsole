using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace DevConsole.Logging
{
    public class DevConsoleLogger : MonoBehaviour
    {
        [SerializeField] private DevConsoleLogLabel _devConsoleLogLabel;
        [SerializeField] private DevConsoleLogStackTraceDisplay _devConsoleLogStackTraceDisplay;
        
        internal event Action<DevConsoleLogData> OnLogReceived;
        internal List<DevConsoleLogData> Logs { get; } = new List<DevConsoleLogData>();

        private void OnEnable()
        {
            Application.logMessageReceived += OnLogMessageReceived;
            DevConsoleLogFilter.FilterToggled += OnFilterToggled;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= OnLogMessageReceived;
            DevConsoleLogFilter.FilterToggled -= OnFilterToggled;
        }
        
        private void OnLogMessageReceived(string message, string stackTrace, LogType logType)
        {
            DevConsoleLogData devConsoleLogData = new DevConsoleLogData(message, stackTrace, logType);
            Logs.Add(devConsoleLogData);

            OnLogReceived?.Invoke(devConsoleLogData);
        }

        private void OnFilterToggled(LogType logType, bool isOn)
        {
            // var logs = Logs.Where(logData => logData.LogType == logType).ToList();
            // foreach (var log in logs)
            // {
            //     //log.gameObject.SetActive(isOn);
            // }
        }

        [UsedImplicitly]
        public void ClearLogs()
        {
            _devConsoleLogLabel.ClearLabel();
            _devConsoleLogStackTraceDisplay.ClearStackTrace();
            Logs.Clear();
        }

#if UNITY_EDITOR
        [UsedImplicitly]
        [ContextMenu(nameof(TestLog))]
        private void TestLog()
        {
            Debug.Log("test");
        }
        
        [UsedImplicitly]
        [ContextMenu(nameof(TestLogWarning))]
        private void TestLogWarning()
        {
            Debug.LogWarning("test");
        }
        
        [UsedImplicitly]
        [ContextMenu(nameof(TestLogError))]
        private void TestLogError()
        {
            Debug.LogError("test");
        }
#endif
    }
}