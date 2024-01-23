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
        [SerializeField, Range(10, 100)] private int _maxLogAmount = 100;
        
        private readonly List<DevConsoleLogObject> _logs = new List<DevConsoleLogObject>();
        
        private void OnEnable()
        {
            Application.logMessageReceived += CreateLog;
            DevConsoleLogFilter.FilterToggled += OnFilterToggled;
            
            InvokeRepeating(nameof(CheckForClearLogs), 0, 60);
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= CreateLog;
            DevConsoleLogFilter.FilterToggled -= OnFilterToggled;
            
            CancelInvoke(nameof(CheckForClearLogs));
        }
        
        private void CreateLog(string text, string stackTrace, LogType type)
        {
            var timestamp = $"[{System.DateTime.Now:HH:mm:ss}]";
            
            var logInstance = Instantiate(_logObjectTemplate, _logsStorage); // TODO: use pooling instead
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

        private void CheckForClearLogs()
        {
            if (_logs.Count >= _maxLogAmount)
            {
                ClearLogs();
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
        public void TestLog()
        {
            Debug.Log("test");
        }
        
        [UsedImplicitly]
        [ContextMenu(nameof(TestLogWarning))]
        public void TestLogWarning()
        {
            Debug.LogWarning("test");
        }
        
        [UsedImplicitly]
        [ContextMenu(nameof(TestLogError))]
        public void TestLogError()
        {
            Debug.LogError("test");
        }
#endif
    }
}