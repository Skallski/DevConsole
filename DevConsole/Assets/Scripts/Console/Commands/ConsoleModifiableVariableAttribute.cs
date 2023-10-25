using System;
using UnityEngine;

namespace Console.Commands
{
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