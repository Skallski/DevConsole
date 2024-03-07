using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace DevConsole.Commands
{
    public class DevConsoleCommandPrompt : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_InputField _inputField;

        [SerializeField] private bool _allowCachingCommands = true;
        private readonly List<string> _cachedCommands = new List<string>();
        private int _currentCachedCommandIndex;
        
        protected enum DevCommandResult
        {
            None,
            Ok,
            NotEnoughArguments,
            NotValidCommand,
            NotValidArgument,
            NotValidFieldOrArgument
        }

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
            
            if (text.StartsWith("/") == false)
            {
                PrintCommandResult(DevCommandResult.NotValidCommand);
                return;
            }
            
            string[] parts = text.Split(' ');
            if (parts == null || parts.Length == 0)
            {
                PrintCommandResult(DevCommandResult.NotValidCommand);
                return;
            }

            HandleCommand(parts);
            
            _cachedCommands.Add(text);
            _currentCachedCommandIndex = _cachedCommands.Count;
            
            _inputField.text = string.Empty;
        }

        private void HandleCommand(IReadOnlyList<string> commandTextParts)
        {
            string command = commandTextParts[0];
            switch (command)
            {
                // example: /set [variableName] [value]
                case "/set":
                {
                    if (commandTextParts.Count < 3)
                    {
                        PrintCommandResult(DevCommandResult.NotEnoughArguments, command);
                    }
                    
                    SetVariable(commandTextParts[1], commandTextParts[2]);
                    break;
                }
                // example: /get [variableName]
                case "/get":
                {
                    GetVariable(commandTextParts[1]);
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
                    PrintCommandResult(DevCommandResult.NotValidCommand, command);
                    break;
                }
            }
            
            HandleCommandInternal(commandTextParts);
        }

        protected virtual void HandleCommandInternal(IReadOnlyList<string> commandTextParts) {}

        protected static void PrintCommandResult(DevCommandResult result, params object[] parameters)
        {
            switch (result)
            {
                case DevCommandResult.None:
                    break;
                case DevCommandResult.Ok:
                    Debug.Log($"Ok: {parameters[0]}");
                    break;
                case DevCommandResult.NotEnoughArguments:
                    Debug.LogError($"Command '{parameters[0]}' has not enough arguments!");
                    break;
                case DevCommandResult.NotValidCommand:
                    Debug.LogError($"Invalid command: '{parameters[0]}'!");
                    break;
                case DevCommandResult.NotValidArgument:
                    Debug.LogError($"Invalid argument {parameters[0]} for command: '{parameters[1]}'!");
                    break;
                case DevCommandResult.NotValidFieldOrArgument:
                    Debug.LogError($"Invalid argument {parameters[0]} or field name {parameters[1]} for command: '{parameters[2]}'!");
                    break;
            }
        }

        private static void SetVariable(string variableName, object value)
        {
            if (DevConsoleFieldHandler.SetValue(variableName, value))
            {
                PrintCommandResult(DevCommandResult.Ok, 
                    $"({DevConsoleFieldHandler.GetType(variableName)}){variableName} set to [{value}]");
            }
            else
            {
                PrintCommandResult(DevCommandResult.NotValidFieldOrArgument, value, variableName, "/set");
            }
        }

        private static void GetVariable(string variableName)
        {
            if (DevConsoleFieldHandler.GetValue(variableName, out object value))
            {
                PrintCommandResult(DevCommandResult.Ok,
                    $"({DevConsoleFieldHandler.GetType(variableName)}){variableName} is [{value}]");
            }
            else
            {
                PrintCommandResult(DevCommandResult.NotValidArgument, $"{variableName}", "/get");
            }
        }

        private static void GetAllVariables()
        {
            string names = DevConsoleFieldHandler.GetAllFieldNames();
            
            PrintCommandResult(DevCommandResult.Ok, names);
        }
    }
}