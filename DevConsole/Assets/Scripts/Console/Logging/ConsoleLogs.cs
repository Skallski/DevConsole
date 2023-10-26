using UnityEngine;

namespace Console.Logging
{
    public class ConsoleLogs : MonoBehaviour
    {
        [SerializeField] private RectTransform _stackTraceTransform;
        [SerializeField] private float _basePositionY = -30;
        [SerializeField] private float _baseHeight = 60;
        [SerializeField] private TMPro.TextMeshProUGUI _stackTraceLabel;

        private void OnEnable()
        {
            ConsoleLogObject.OnConsoleLogSelected += PrintStackTrace;
        }

        private void OnDisable()
        {
            ConsoleLogObject.OnConsoleLogSelected -= PrintStackTrace;
        }

        private void PrintStackTrace(ConsoleLogObject consoleLogObject)
        {
            if (consoleLogObject.Selected)
            {
                _stackTraceTransform.gameObject.SetActive(true);
                _stackTraceLabel.SetText(consoleLogObject.StackTrace);

                var height = _stackTraceLabel.preferredHeight;
                if (height < 60)
                {
                    height = 60;
                }
                
                _stackTraceTransform.sizeDelta = new Vector2(_stackTraceTransform.sizeDelta.x, height);
                
                var desiredPositionY = _basePositionY - (height - _baseHeight) / 2f;
                _stackTraceTransform.anchoredPosition = new Vector2(_stackTraceTransform.anchoredPosition.x, desiredPositionY);
            }
            else
            {
                Clear();
            }
        }
        
        internal void Clear()
        {
            _stackTraceLabel.SetText(string.Empty);
            
            _stackTraceTransform.sizeDelta = new Vector2(_stackTraceTransform.sizeDelta.x, _baseHeight);
            _stackTraceTransform.anchoredPosition = new Vector2(_stackTraceTransform.anchoredPosition.x, _basePositionY);
            
            _stackTraceTransform.gameObject.SetActive(false);
        }
    }
}