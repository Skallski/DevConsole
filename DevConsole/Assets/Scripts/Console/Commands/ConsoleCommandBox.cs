using JetBrains.Annotations;
using UnityEngine;

namespace Console.Commands
{
    public class ConsoleCommandBox : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_InputField _inputField;

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

            // TODO: up arrow scrolls to last archived command
            // TODO: down arrow scrolls to next archived command
            // if (Input.GetKeyDown(KeyCode.UpArrow))
            // {
            //     if (_archivedCommands.Count == 0)
            //     {
            //         return;
            //     }
            //     
            //     _archivedCommands
            // }
            // else if (Input.GetKeyDown(KeyCode.DownArrow))
            // {
            //     
            // }
        }


        [UsedImplicitly]
        public void ExecuteCommand()
        {
            ExecuteCommand(_inputField.text);
            _inputField.text = string.Empty;
        }

        private static void ExecuteCommand(string text)
        {
            if (text.StartsWith("/") == false)
            {
                // TODO: no such command
                return;
            }
            
            string[] parts = text.Split(' ');
            if (parts == null || parts.Length == 0)
            {
                // TODO: no such command
                return;
            }

            var command = parts[0];
            
            if (command.Equals("/set")) // example: /set [variableName] [value]
            {
                SetVariable(parts[1], parts[2]);
            }
            else if (command.Equals("/get")) // example: /get [variableName]
            {
                GetVariable(parts[1]);
            }
        }
        
        private static void SetVariable(string variableName, object value)
        {
            System.Reflection.FieldInfo fieldInfo = ConsoleModifiableVariableHandler.GetModifiableField(variableName);
            if (fieldInfo == null)
            {
                Debug.LogError($"{variableName} is not valid variable!");
                return;
            }

            if (ConsoleModifiableVariableHandler.SetModifiableFieldValue(fieldInfo, value))
            {
                Debug.LogAssertion($"{variableName} value set: {value}");
            }
            else
            {
                Debug.LogError($"{value} is not valid value for {variableName}!");
            }
        }

        private static void GetVariable(string variableName)
        {
            if (ConsoleModifiableVariableHandler.GetModifiableFieldValue(variableName, out object value))
            {
                Debug.Log($"{variableName} = {value}");
            }
            else
            {
                Debug.LogError($"{variableName} is not valid variable!");
            }
        }
    }
}