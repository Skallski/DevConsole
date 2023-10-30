using System;
using UnityEngine;

namespace Console.Commands
{
    /// <summary>
    /// Attribute that allows variables to be visible and modifiable inside Dev Console
    /// <example>
    /// [ConsoleModifiableVariable("consoleVariableName")] private int _someVariable;
    /// </example>>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ConsoleModifiableVariableAttribute : PropertyAttribute
    {
        public readonly string VariableName;

        public ConsoleModifiableVariableAttribute(string variableName)
        {
            VariableName = variableName;
        }
    }
}