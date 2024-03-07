using UnityEngine;

namespace DevConsole.Logging
{
    public class DevConsoleLogStackTraceDisplay : MonoBehaviour
    {
        [SerializeField] private RectTransform _stackTraceTransform;
        [SerializeField] private float _basePositionY = -30;
        [SerializeField] private float _baseHeight = 60;
        [SerializeField] private TMPro.TextMeshProUGUI _stackTraceLabel;

        private DevConsoleLogData _selectedLog;

        private void OnEnable()
        {
            DevConsoleLogger.OnClear += ClearStackTrace;
            DevConsoleLogLabel.OnLogClick += OnLogClick;
        }
        
        private void OnDisable()
        {
            DevConsoleLogger.OnClear -= ClearStackTrace;
            DevConsoleLogLabel.OnLogClick -= OnLogClick;
        }
        
        private void OnLogClick(DevConsoleLogData data)
        {
            if (data == null)
            {
                return;
            }

            _selectedLog?.SetSelected(false);

            if (data == _selectedLog)
            {
                _selectedLog = null;

                ClearStackTrace();
            }
            else
            {
                data.SetSelected(true);
                _selectedLog = data;

                ShowStackTrace();
            }
        }

        private void ShowStackTrace()
        {
            _stackTraceTransform.gameObject.SetActive(true);
            _stackTraceLabel.SetText(_selectedLog?.StackTrace ?? string.Empty);

            float height = _stackTraceLabel.preferredHeight;
            if (height < 60)
            {
                height = 60;
            }

            _stackTraceTransform.sizeDelta = new Vector2(_stackTraceTransform.sizeDelta.x, height);

            float desiredPositionY = _basePositionY - (height - _baseHeight) / 2f;
            _stackTraceTransform.anchoredPosition =
                new Vector2(_stackTraceTransform.anchoredPosition.x, desiredPositionY);
        }

        private void ClearStackTrace()
        {
            _stackTraceLabel.SetText(string.Empty);
            
            _stackTraceTransform.sizeDelta = new Vector2(_stackTraceTransform.sizeDelta.x, _baseHeight);
            _stackTraceTransform.anchoredPosition = new Vector2(_stackTraceTransform.anchoredPosition.x, _basePositionY);
            
            _stackTraceTransform.gameObject.SetActive(false);
        }
    }
}