using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DevConsole.Logging
{
    public class DevConsoleLogLabel : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private DevConsoleLogger _logger;
        [SerializeField] private TextMeshProUGUI _label;
        
        internal event Action<DevConsoleLogData> OnLogClick;

        private Coroutine _scrollCoroutine;
        private WaitForEndOfFrame _scrollDelay = new WaitForEndOfFrame();

        private void OnEnable()
        {
            PrintAllLogs();

            _logger.OnLogReceived += OnLogReceived;
        }

        private void OnDisable()
        {
            _logger.OnLogReceived -= OnLogReceived;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            int lineIndex = TMP_TextUtilities.FindIntersectingLine(_label, Input.mousePosition, null);
            if (lineIndex >= _logger.Logs.Count)
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

            List<DevConsoleLogData> logs = _logger.Logs;
            foreach (DevConsoleLogData data in logs)
            {
                PrintLog(data);
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


        internal void ClearLabel()
        {
            _label.SetText(string.Empty);
        }
    }
}