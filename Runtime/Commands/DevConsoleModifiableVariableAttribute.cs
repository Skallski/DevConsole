using System;
using UnityEngine;

namespace DevConsole.Commands
{
    /// <summary>
    /// Attribute that allows variables to be visible and modifiable inside Dev Console
    /// <example>
    /// [ConsoleModifiableVariable("consoleVariableName")] private int _someVariable;
    /// </example>>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DevConsoleModifiableVariableAttribute : PropertyAttribute
    {
        public readonly string VariableName;

        public DevConsoleModifiableVariableAttribute(string variableName)
        {
            VariableName = variableName;
        }
    }
}