using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DevConsole.Commands
{
    /// <summary>
    /// This class handles command execution
    /// </summary>
    public class DevConsoleCommandHandler : MonoBehaviour
    {
        private List<(Predicate<string> predicate, Action<string> action)> _commandActions;

        private void Start()
        {
            SetupCommandActions();
        }

        /// <summary>
        /// Sets up list of commands and their actions
        /// Override this method in order to add more commands
        /// </summary>
        protected virtual void SetupCommandActions()
        {
            _commandActions = new List<(Predicate<string> predicate, Action<string> action)>
            {
                (command => command.StartsWith("/set "), command =>
                {
                    Match match = Regex.Match(command, @"\/set\s+([a-zA-Z_][a-zA-Z0-9_]*)\s+(\S+)");
                    if (match.Success)
                    {
                        string variableName = match.Groups[1].Value;
                        object value = match.Groups[2].Value;
                        SetVariable(variableName, value);
                    }
                    else
                    {
                        Debug.LogError("Invalid /set command format. Usage: /set <variable> <value>");
                    }
                }),
                (command => Regex.IsMatch(command, @"^\/get(\s+[a-zA-Z_][a-zA-Z0-9_]*)?$"), command =>
                {
                    Match match = Regex.Match(command, @"\/get\s+([a-zA-Z_][a-zA-Z0-9_]*)");
                    if (match.Success)
                    {
                        string variableName = match.Groups[1].Value;
                        GetVariable(variableName);
                    }
                    else
                    {
                        Debug.LogError("Invalid /get command format. Usage: /get <variable>");
                    }
                }),
                (command => command.Equals("/getAll"), _ => GetAllVariables())
            };
        }

        /// <summary>
        /// Checks for matched command from commands list
        /// </summary>
        /// <param name="command"> passed command </param>
        internal void HandleCommand(string command)
        {
            if (command.StartsWith("/") == false)
            {
                Debug.LogError("Valid command should start with '/'!");
                return;
            }

            bool isHandled = false;

            foreach ((Predicate<string> commandPredicate, Action<string> commandAction) in _commandActions)
            {
                if (commandPredicate(command))
                {
                    commandAction(command);
                    isHandled = true;
                    break;
                }
            }

            if (isHandled == false)
            {
                Debug.LogError($"Command '{command}' not found!");
            }
        }
        
        /// <summary>
        /// Sets dev console variable with new value and logs the result
        /// </summary>
        /// <param name="variableName"> variable to modify </param>
        /// <param name="value"> value to set </param>
        private static void SetVariable(string variableName, object value)
        {
            if (DevConsoleFieldHandler.TrySetValue(variableName, value, out string logMessage))
            {
                Debug.Log($"{variableName} [{value.GetType()}] => {value}");
            }
            else
            {
                Debug.LogError(logMessage);
            }
        }

        /// <summary>
        /// Logs dev console variable and its value
        /// </summary>
        /// <param name="variableName"> variable to modify </param>
        private static void GetVariable(string variableName)
        {
            if (DevConsoleFieldHandler.TryGetValue(variableName, out object value))
            {
                Debug.Log($"{variableName} [{value.GetType()}] = {value}");
            }
            else
            {
                Debug.LogError($"Variable '{variableName}' not found!");
            }
        }

        /// <summary>
        /// Logs all dev console variables
        /// </summary>
        private static void GetAllVariables()
        {
            (string, Type)[] fields = DevConsoleFieldHandler.GetAllFields();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < fields.Length; i++)
            {
                (string fieldName, Type fieldType) = fields[i];

                sb.Append(i + 1 >= fields.Length 
                    ? $"{fieldName} [{fieldType}]" 
                    : $"{fieldName} [{fieldType}], ");
            }

            Debug.Log(sb.ToString());
        }
    }
}