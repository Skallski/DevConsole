using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Console.Commands
{
    public static class ConsoleModifiableVariableHandler
    {
        internal static FieldInfo GetModifiableField(string variableName)
        {
            FieldInfo[] allFields = FindModifiableFields();
            
            foreach (FieldInfo fieldInfo in allFields)
            {
                ConsoleModifiableVariableAttribute variableAttribute = 
                    (ConsoleModifiableVariableAttribute) fieldInfo.GetCustomAttribute(typeof(ConsoleModifiableVariableAttribute));
            
                if (variableAttribute != null)
                {
                    if (variableAttribute.VariableName.Equals(variableName))
                    {
                        return fieldInfo;
                    }
                }
            }

            return null;
        }

        private static FieldInfo[] FindModifiableFields()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            List<Type> types = new List<Type>();

            foreach (var assembly in assemblies)
            {
                types.AddRange(assembly.GetTypes());
            }

            return types
                .Where(type => type.IsSubclassOf(typeof(MonoBehaviour)) || type.IsSubclassOf(typeof(ScriptableObject)))
                .SelectMany(type => type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(field => field.IsDefined(typeof(ConsoleModifiableVariableAttribute), true))
                    .ToArray())
                .ToArray();
        }

        private static Object GetInstanceOfField(FieldInfo fieldInfo)
        {
            Type declaringType = fieldInfo.DeclaringType;
            if (declaringType == null)
            {
                return null;
            }
            
            if (typeof(ScriptableObject).IsAssignableFrom(declaringType))
            {
                CreateAssetMenuAttribute createAssetMenu = declaringType.GetCustomAttribute<CreateAssetMenuAttribute>();
                if (createAssetMenu != null)
                {
                    return Resources.Load(createAssetMenu.menuName);
                }
            }
            else if (typeof(MonoBehaviour).IsAssignableFrom(declaringType))
            {
                return Object.FindObjectOfType(declaringType);
            }

            return null;
        }

        internal static bool GetModifiableFieldValue(string variableName, out object value)
        {
            FieldInfo fieldInfo = GetModifiableField(variableName);
            return GetModifiableFieldValue(fieldInfo, out value);
        }

        internal static bool GetModifiableFieldValue(FieldInfo fieldInfo, out object value)
        {
            value = null;
            
            if (fieldInfo == null)
            {
                return false;
            }
            
            var instance = GetInstanceOfField(fieldInfo);
            if (instance == null)
            {
                return false;
            }
            
            value = fieldInfo.GetValue(instance);
            return true;
        }

        internal static bool SetModifiableFieldValue(string variableName, object value)
        {
            FieldInfo fieldInfo = GetModifiableField(variableName);
            
            return SetModifiableFieldValue(fieldInfo, value);
        }

        internal static bool SetModifiableFieldValue(FieldInfo fieldInfo, object value)
        {
            if (fieldInfo == null)
            {
                return false;
            }

            var instance = GetInstanceOfField(fieldInfo);
            if (instance == null)
            {
                return false;
            }

            if (fieldInfo.IsLiteral == false && fieldInfo.IsInitOnly == false)
            {
                Type fieldType = fieldInfo.FieldType;
                if (fieldType == typeof(int))
                {
                    fieldInfo.SetValue(instance, Convert.ToInt32(value));
                }
                else if (fieldType == typeof(float))
                {
                    fieldInfo.SetValue(instance, Convert.ToSingle(value));
                }
                else if (fieldType == typeof(string))
                {
                    fieldInfo.SetValue(instance, Convert.ToString(value));
                }
                else
                {
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}