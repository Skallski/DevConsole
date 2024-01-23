using System;
using UnityEngine;

namespace DevConsole.Commands
{
    /// <summary>
    /// Attribute that allows variables to be visible and modifiable inside Dev Console
    /// <example>
    /// [DevConsoleField("consoleVariableName")] private int _someVariable;
    /// </example>>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DevConsoleFieldAttribute : PropertyAttribute
    {
        public readonly string VariableName;

        public DevConsoleFieldAttribute(string variableName)
        {
            VariableName = variableName;
        }
    }
}