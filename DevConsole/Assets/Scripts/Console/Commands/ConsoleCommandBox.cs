using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

namespace Console.Commands
{
    public class ConsoleCommandBox : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_InputField _inputField;
        
        private readonly List<string> _cachedCommands = new List<string>();
        private int _currentCachedCommandIndex;

#if UNITY_EDITOR
        private void Reset()
        {
            if (_inputField == null)
            {
                _inputField = GetComponentInChildren<TMPro.TMP_InputField>();
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
                ExecuteCommand();
            }

            if (_cachedCommands.Count > 0)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (_currentCachedCommandIndex > 0)
                    {
                        _currentCachedCommandIndex--;
                    }

                    _inputField.text = _cachedCommands[_currentCachedCommandIndex];
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
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
        }

        [UsedImplicitly]
        public void ClearCommandsCache()
        {
            _cachedCommands.Clear();
            _currentCachedCommandIndex = _cachedCommands.Count;
        }

        [UsedImplicitly]
        public void ExecuteCommand()
        {
            ExecuteCommand(_inputField.text);
            
            _cachedCommands.Add(_inputField.text);
            _currentCachedCommandIndex = _cachedCommands.Count;
            
            _inputField.text = string.Empty;
        }

        private static void ExecuteCommand(string text)
        {
            if (text.StartsWith("/") == false)
            {
                goto NotValidCommand;
            }
            
            string[] parts = text.Split(' ');
            if (parts == null || parts.Length == 0)
            {
                goto NotValidCommand;
            }

            var command = parts[0];
            switch (command)
            {
                // example: /set [variableName] [value]
                case "/set":
                {
                    if (parts.Length < 3)
                    {
                        Debug.LogError("/set command has not enough arguments!");
                        return;
                    }
                    
                    SetVariable(parts[1], parts[2]);
                    break;
                }
                // example: /get [variableName]
                case "/get":
                {
                    GetVariable(parts[1]);
                    break;
                }
                // example: /getAll
                case "/getAll":
                {
                    GetAllVariables();
                    break;
                }
                default:
                {
                    goto NotValidCommand;
                }
            }
            
            return;
            
            NotValidCommand:
            {
                Debug.LogError("Not valid command!");
                return;
            }
        }
        
        private static void SetVariable(string variableName, object value)
        {
            var fieldInfo = ConsoleModifiableVariableHandler.GetModifiableField(variableName);
            if (fieldInfo == null)
            {
                Debug.LogError($"'{variableName}' is not valid variable!");
                return;
            }

            if (ConsoleModifiableVariableHandler.SetModifiableFieldValue(fieldInfo, value))
            {
                Debug.LogAssertion($"'{variableName}' value set: '{value}'");
            }
            else
            {
                Debug.LogError($"'{value}' is not valid value for variable '{variableName}'!");
            }
        }

        private static void GetVariable(string variableName)
        {
            if (ConsoleModifiableVariableHandler.GetModifiableFieldValue(variableName, out object value))
            {
                Debug.Log($"'{variableName}' = '{value}'");
            }
            else
            {
                Debug.LogError($"'{variableName}' is not valid variable!");
            }
        }

        private static void GetAllVariables()
        {
            StringBuilder sb = new StringBuilder();
            
            var fields = ConsoleModifiableVariableHandler.FindModifiableFields();
            foreach (var field in fields)
            {
                sb.Append($"{ConsoleModifiableVariableHandler.GetNameOfField(field)}, ");
            }
            
            Debug.Log($"Variables: {sb.ToString()}");
        }
    }
}