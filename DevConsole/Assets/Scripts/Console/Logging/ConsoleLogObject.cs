using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Console.Logging
{
    public class ConsoleLogObject : MonoBehaviour, IPointerUpHandler
    {
        public static event Action<ConsoleLogObject> OnConsoleLogSelected;
        private static readonly Color SelectedBackgroundColor = new Color32(255, 255, 255, 42);
        private static readonly Color DeselectedBackgroundColor = new Color32(255, 255, 255, 0);

        [SerializeField] private UnityEngine.UI.Image _background;
        [SerializeField] private TMPro.TextMeshProUGUI _typeLabel;
        [SerializeField] private TMPro.TextMeshProUGUI _textLabel;
        
        internal string Text { get; private set; }
        internal string StackTrace { get; private set; }
        internal bool Selected { get; private set; }

        private void OnEnable()
        {
            OnConsoleLogSelected += OnConsoleLogSelectedCallback;
        }

        private void OnDisable()
        {
            OnConsoleLogSelected -= OnConsoleLogSelectedCallback;
        }

        internal void Setup(string text, string stackTrace, LogType type)
        {
            Text = text;
            StackTrace = stackTrace;

            var displayedTypeText = type.ToString().ToUpper();
            _typeLabel.SetText(type switch
            {
                LogType.Error => $"<color=red>{displayedTypeText}</color>",
                LogType.Assert => $"<color=green>{displayedTypeText}</color>",
                LogType.Warning => $"<color=yellow>{displayedTypeText}</color>",
                LogType.Log => $"<color=white>{displayedTypeText}</color>",
                LogType.Exception => $"<color=red>{displayedTypeText}</color>",
                _ => string.Empty
            });

            _textLabel.SetText(text);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            SetSelected(!Selected);
            
            OnConsoleLogSelected?.Invoke(this);
        }

        private void OnConsoleLogSelectedCallback(ConsoleLogObject _consoleLogObject)
        {
            if (_consoleLogObject != this)
            {
                SetSelected(false);
            }
        }

        private void SetSelected(bool value)
        {
            Selected = value;

            _background.color = value 
                ? SelectedBackgroundColor 
                : DeselectedBackgroundColor;
        }
    }
}