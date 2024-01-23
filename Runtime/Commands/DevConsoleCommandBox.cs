using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

namespace DevConsole.Commands
{
    public class DevConsoleCommandBox : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_InputField _inputField;
        
        private readonly List<string> _cachedCommands = new List<string>();
        private int _currentCachedCommandIndex;
        
        protected enum CommandResult
        {
            None,
            Ok,
            NotEnoughArguments,
            NotValidCommand,
            NotValidArgument,
            NotValidVariable,
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
        public void Execute()
        {
            Execute(_inputField.text);
            
            _cachedCommands.Add(_inputField.text);
            _currentCachedCommandIndex = _cachedCommands.Count;
            
            _inputField.text = string.Empty;
        }
        
        private void Execute(string text)
        {
            if (text.StartsWith("/") == false)
            {
                PrintCommandResult(CommandResult.NotValidCommand);
                return;
            }
            
            string[] parts = text.Split(' ');
            if (parts == null || parts.Length == 0)
            {
                PrintCommandResult(CommandResult.NotValidCommand);
                return;
            }

            HandleCommand(parts);
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
                        PrintCommandResult(CommandResult.NotEnoughArguments, command);
                    }
                    
                    SetVariable(commandTextParts[1], commandTextParts[2]);
                    break;
                }
                // example: /reset [variableName]
                case "/reset":
                {
                    ResetVariable(commandTextParts[1]);
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
                    PrintCommandResult(CommandResult.NotValidCommand, command);
                    break;
                }
            }
            
            HandleCommandInternal(commandTextParts);
        }

        protected virtual void HandleCommandInternal(IReadOnlyList<string> commandTextParts) {}

        protected static void PrintCommandResult(CommandResult result, params object[] parameters)
        {
            switch (result)
            {
                case CommandResult.None:
                    break;
                case CommandResult.Ok:
                    Debug.Log($"Ok: {parameters[0]}");
                    break;
                case CommandResult.NotEnoughArguments:
                    Debug.LogError($"Command: {parameters[0]} has not enough arguments!");
                    break;
                case CommandResult.NotValidCommand:
                    Debug.LogError($"Command: {parameters[0]} is not valid!");
                    break;
                case CommandResult.NotValidArgument:
                    Debug.LogError($"Command: {parameters[0]}, argument {parameters[1]} is not valid!");
                    break;
                case CommandResult.NotValidVariable:
                    Debug.LogError($"Command: {parameters[0]}, variable: {parameters[1]} is not valid!");
                    break;
            }
        }

        private static void SetVariable(string variableName, object value)
        {
            DevConsoleModifiableVariableHandler.ModifiableVariableData data = 
                DevConsoleModifiableVariableHandler.GetModifiableVariable(variableName);
            if (data == null)
            {
                PrintCommandResult(CommandResult.NotValidVariable, $"/set {variableName}", variableName);
                return;
            }

            if (DevConsoleModifiableVariableHandler.SetModifiableVariableValue(data, value))
            {
                PrintCommandResult(CommandResult.Ok, $"'{variableName}' value set to '{value}'");
            }
            else
            {
                PrintCommandResult(CommandResult.NotValidArgument, $"/set {variableName}", value);
            }
        }
        
        private static void ResetVariable(string variableName)
        {
            DevConsoleModifiableVariableHandler.ModifiableVariableData data = 
                DevConsoleModifiableVariableHandler.GetModifiableVariable(variableName);
            if (data == null)
            {
                PrintCommandResult(CommandResult.NotValidVariable, $"/reset {variableName}", variableName);
                return;
            }

            if (DevConsoleModifiableVariableHandler.SetModifiableVariableValue(data, data.InitialValue))
            {
                PrintCommandResult(CommandResult.Ok, $"'{variableName}' value reset to '{data.InitialValue}'");
            }
            else
            {
                Debug.LogError($"'{variableName}' cannot be reset!");
            }
        }

        private static void GetVariable(string variableName)
        {
            if (DevConsoleModifiableVariableHandler.GetModifiableVariableValue(variableName, out object value))
            {
                PrintCommandResult(CommandResult.Ok, $"'{variableName}' = '{value}'");
            }
            else
            {
                PrintCommandResult(CommandResult.NotValidVariable, $"/get {variableName}", variableName);
            }
        }

        private static void GetAllVariables()
        {
            StringBuilder sb = new StringBuilder();
            
            List<DevConsoleModifiableVariableHandler.ModifiableVariableData> dataSets = 
                DevConsoleModifiableVariableHandler.FindAllModifiableVariables();
            foreach (var data in dataSets)
            {
                sb.Append($"{DevConsoleModifiableVariableHandler.GetNameOfField(data.FieldInfo)}, ");
            }
            
            PrintCommandResult(CommandResult.Ok, $"Variables: {sb}");
        }
    }
}