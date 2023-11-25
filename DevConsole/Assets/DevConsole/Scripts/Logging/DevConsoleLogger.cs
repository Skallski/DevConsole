using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace DevConsole.Logging
{
    public class DevConsoleLogger : MonoBehaviour
    {
        [SerializeField] private DevConsoleLogs _consoleLogs;
        [SerializeField] private DevConsoleLogObject _logObjectTemplate;
        [SerializeField] private Transform _logsStorage;
        
        private readonly List<DevConsoleLogObject> _logs = new List<DevConsoleLogObject>();

        private void OnEnable()
        {
            Application.logMessageReceived += CreateLog;
            DevConsoleLogFilter.FilterToggled += OnFilterToggled;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= CreateLog;
            DevConsoleLogFilter.FilterToggled -= OnFilterToggled;
        }
        
        private void CreateLog(string text, string stackTrace, LogType type)
        {
            var timestamp = $"[{System.DateTime.Now:HH:mm:ss}]";
            
            var logInstance = Instantiate(_logObjectTemplate, _logsStorage);
            logInstance.gameObject.SetActive(true);
            logInstance.name = $"Log_{timestamp}";
            logInstance.Setup($"{timestamp}: {text}", stackTrace, type);
            
            _logs.Add(logInstance);
        }
        
        private void OnFilterToggled(LogType logType, bool isOn)
        {
            var logs = _logs.Where(log => log.LogType == logType).ToList();
            foreach (var log in logs)
            {
                log.gameObject.SetActive(isOn);
            }
        }

        [UsedImplicitly]
        public void ClearLogs()
        {
            foreach (var log in _logs)
            {
                Destroy(log.gameObject);
            }
            
            _logs.Clear();
            _consoleLogs.Clear();
        }

#if UNITY_EDITOR
        [UsedImplicitly]
        [ContextMenu(nameof(TestLog))]
        public void TestLog() => Debug.Log("test log");

        [UsedImplicitly]
        [ContextMenu(nameof(TestLogWarning))]
        public void TestLogWarning() => Debug.LogWarning("test warning");

        [UsedImplicitly]
        [ContextMenu(nameof(TestLogError))]
        public void TestLogError() => Debug.LogError("test error");
#endif
    }
}