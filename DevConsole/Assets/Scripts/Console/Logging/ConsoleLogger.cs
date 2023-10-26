using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Console.Logging
{
    public class ConsoleLogger : MonoBehaviour
    {
        [SerializeField] private ConsoleLogs _consoleLogs;
        [SerializeField] private ConsoleLogObject _logObjectTemplate;
        [SerializeField] private Transform _logsStorage;
        
        private readonly List<ConsoleLogObject> _logs = new List<ConsoleLogObject>();

        private void OnEnable()
        {
            Application.logMessageReceived += CreateLog;
            ConsoleLogFilter.FilterToggled += OnFilterToggled;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= CreateLog;
            ConsoleLogFilter.FilterToggled -= OnFilterToggled;
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
    }
}