using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DevConsole.Logging
{
    public class DevConsoleLogLabel : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private DevConsoleLogger _logger;
        [SerializeField] private DevConsoleLogFilters _filters;
        [Space]
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private TextMeshProUGUI _label;
        
        internal static event Action<DevConsoleLogData> OnLogClick;

        private Coroutine _scrollCoroutine;
        private WaitForEndOfFrame _scrollDelay = new WaitForEndOfFrame();

        private void OnEnable()
        {
            PrintAllLogs();

            DevConsoleLogger.OnClear += ClearLabel;
            DevConsoleLogger.OnLogReceived += OnLogReceived;
            DevConsoleLogFilters.FilterToggled += PrintAllLogs;
        }

        private void OnDisable()
        {
            DevConsoleLogger.OnClear += ClearLabel;
            DevConsoleLogger.OnLogReceived -= OnLogReceived;
            DevConsoleLogFilters.FilterToggled -= PrintAllLogs;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            int lineIndex = TMP_TextUtilities.FindIntersectingLine(_label, Input.mousePosition, null);
            if (lineIndex < 0 || lineIndex >= _logger.Logs.Count)
            {
                return;
            }
            
            DevConsoleLogData logData = _logger.Logs[lineIndex];
            if (logData != null)
            {
                OnLogClick?.Invoke(logData);
                PrintAllLogs();
            }
        }
        
        private void OnLogReceived(DevConsoleLogData data)
        {
            PrintLog(data);
            ScrollToNewestLog();
        }

        private void PrintLog(DevConsoleLogData data)
        {
            if (data == null)
            {
                return;
            }
            
            _label.text += $"{data.FormattedMessage}\n";
        }
        
        private void PrintAllLogs()
        {
            ClearLabel();
            
            DevConsoleLogData[] logsFiltered = _filters.GetFilteredLogs(_logger.Logs);
            foreach (DevConsoleLogData logData in logsFiltered)
            {
                PrintLog(logData);
            }
            
            ScrollToNewestLog();
        }

        private void ScrollToNewestLog()
        {
            if (_scrollCoroutine != null)
            {
                StopCoroutine(_scrollCoroutine);
            }
            
            _scrollCoroutine = StartCoroutine(ScrollToNewestLog_Coroutine());
            
            IEnumerator ScrollToNewestLog_Coroutine()
            {
                yield return _scrollDelay;
                _scrollRect.verticalNormalizedPosition = 0;
            }
        }


        private void ClearLabel()
        {
            _label.SetText(string.Empty);
        }
    }
}