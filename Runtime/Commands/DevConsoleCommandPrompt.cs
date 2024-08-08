using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace DevConsole.Commands
{
    public class DevConsoleCommandPrompt : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_InputField _inputField;
        [SerializeField] private DevConsoleCommandHandler _commandHandler;
        
        [Space]
        [SerializeField] private bool _allowCachingCommands = true;
        private readonly List<string> _cachedCommands = new List<string>();
        private int _currentCachedCommandIndex;
        
#if UNITY_EDITOR
        private void Reset()
        {
            if (_inputField == null)
            {
                _inputField = GetComponentInChildren<TMPro.TMP_InputField>();
            }
            
            if (_commandHandler == null)
            {
                _commandHandler = GetComponent<DevConsoleCommandHandler>();
            }
        }
#endif

        private void OnEnable()
        {
            _inputField.ActivateInputField();
            _inputField.Select();

            _currentCachedCommandIndex = _cachedCommands.Count;
        }

        private void OnDisable()
        {
            _inputField.DeactivateInputField();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Execute();
            }
            
            if (_allowCachingCommands == false || _cachedCommands.Count <= 0)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow)) // move to previous executed command
            {
                if (_currentCachedCommandIndex > 0)
                {
                    _currentCachedCommandIndex--;
                }

                _inputField.text = _cachedCommands[_currentCachedCommandIndex];
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow)) // move to next executed command if there is any
            {
                if (_currentCachedCommandIndex < _cachedCommands.Count)
                {
                    _currentCachedCommandIndex++;
                }

                _inputField.text = _currentCachedCommandIndex == _cachedCommands.Count 
                    ? string.Empty 
                    : _cachedCommands[_currentCachedCommandIndex];
            }
        }

        [UsedImplicitly]
        public void ClearCommandsCache()
        {
            if (_cachedCommands.Count > 0)
            {
                _cachedCommands.Clear();
                _currentCachedCommandIndex = _cachedCommands.Count;
                
                Debug.Log("Cleared cached commands");
            }
        }

        [UsedImplicitly]
        public void Execute()
        {
            Execute(_inputField.text);
        }
        
        private void Execute(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            _commandHandler.HandleCommand(text);
            
            _cachedCommands.Add(text);
            _currentCachedCommandIndex = _cachedCommands.Count;
            
            _inputField.text = string.Empty;
        }
    }
}