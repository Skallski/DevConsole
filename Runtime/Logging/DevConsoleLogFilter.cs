using UnityEngine;
using UnityEngine.UI;

namespace DevConsole.Logging
{
    public class DevConsoleLogFilter : MonoBehaviour
    {
        internal static event System.Action<LogType, bool> FilterToggled;
        
        [SerializeField] private Image _image;
        [SerializeField] private Toggle _toggle;
        [field: SerializeField] internal LogType LogType { get; private set; }

        internal bool IsOn => _toggle.isOn;
        
#if UNITY_EDITOR
        private void Reset()
        {
            if (_image == null) _image = GetComponent<Image>();
            if (_toggle == null) _toggle = GetComponent<Toggle>();
        }
#endif

        private void OnEnable()
        {
            _toggle.onValueChanged.AddListener(Toggle);
        }

        private void OnDisable()
        {
            _toggle.onValueChanged.RemoveListener(Toggle);
        }
        
        private void Toggle(bool value)
        {
            var color = _image.color;

            _image.color = value 
                ? new Color(color.r, color.g, color.b, 1) 
                : new Color(color.r, color.g, color.b, 0.5f);
            
            FilterToggled?.Invoke(LogType, _toggle.isOn);
        }
    }
}