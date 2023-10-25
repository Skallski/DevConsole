using JetBrains.Annotations;
using UnityEngine;

namespace Console.Logging
{
    public class ConsoleLogs : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI _stackTrace;

        private void OnEnable()
        {
            ConsoleLogObject.OnConsoleLogSelected += PrintStackTrace;
        }

        private void OnDisable()
        {
            ConsoleLogObject.OnConsoleLogSelected -= PrintStackTrace;
        }

        private void PrintStackTrace(ConsoleLogObject _consoleLogObject)
        {
            _stackTrace.SetText(_consoleLogObject.Selected 
                ? _consoleLogObject.StackTrace 
                : string.Empty);
        }
        
        internal void Clear()
        {
            _stackTrace.SetText(string.Empty);
        }
    }
}