using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DevConsole.Commands
{
    public static class DevConsoleFieldHandler
    {
        private class DevConsoleFieldData
        {
            internal readonly FieldInfo FieldInfo;
            internal readonly Object Instance;
            internal readonly string VariableName;
            internal readonly Type FieldType;

            internal DevConsoleFieldData(FieldInfo fieldInfo, Object instance)
            {
                FieldInfo = fieldInfo;
                Instance = instance;

                if (fieldInfo != null)
                {
                    VariableName = fieldInfo.GetCustomAttribute<DevConsoleFieldAttribute>().VariableName;
                    FieldType = fieldInfo.FieldType;
                }
                else
                {
                    VariableName = string.Empty;
                    FieldType = null;
                }
            }
        }

        private static readonly List<DevConsoleFieldData> DevConsoleFieldsCache = new List<DevConsoleFieldData>();

        /// <summary>
        /// Gets data of variable
        /// </summary>
        /// <param name="variableName"> name of variable (the name inside attribute) </param>
        /// <param name="data"></param>
        /// <returns> true or false, whether the data is found and valid </returns>
        private static bool TryGetDevConsoleVariable(string variableName, out DevConsoleFieldData data)
        {
            IEnumerable<DevConsoleFieldData> allFields = GetAllDevConsoleVariables();
            data = allFields.FirstOrDefault(field => field.VariableName.Equals(variableName));

            return data != null && data.FieldInfo != null && data.Instance != null;
        }

        /// <summary>
        /// Gets data of all variables
        /// </summary>
        /// <returns> List that contains every modifiable variable's data </returns>
        private static IEnumerable<DevConsoleFieldData> GetAllDevConsoleVariables()
        {
            if (DevConsoleFieldsCache.Count == 0)
            {
                IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies();
                List<DevConsoleFieldData> fields = new List<DevConsoleFieldData>();

                foreach (var assembly in assemblies)
                {
                    Type[] types = assembly.GetTypes();
                    foreach (var type in types)
                    {
                        if (type.IsSubclassOf(typeof(MonoBehaviour)) || type.IsSubclassOf(typeof(ScriptableObject)))
                        {
                            IEnumerable<FieldInfo> typeFields = type
                                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                .Where(field => field.IsDefined(typeof(DevConsoleFieldAttribute), true));

                            fields.AddRange(typeFields.Select(fieldInfo =>
                                new DevConsoleFieldData(fieldInfo, GetInstanceOfField(fieldInfo))));
                        }
                    }
                }

                DevConsoleFieldsCache.AddRange(fields);
            }

            return DevConsoleFieldsCache;
            
            // INNER METHOD
            // Returns instance of seeking FieldInfo
            Object GetInstanceOfField(MemberInfo info)
            {
                Type declaringType = info.DeclaringType;
                if (declaringType == null)
                {
                    return null;
                }
            
                if (typeof(MonoBehaviour).IsAssignableFrom(declaringType)) // for variable stored inside MonoBehaviour
                {
                    return Object.FindObjectOfType(declaringType);
                }
                // else if (typeof(ScriptableObject).IsAssignableFrom(declaringType)) // for variable stored inside ScriptableObject
                // {
                //     CreateAssetMenuAttribute createAssetMenu = declaringType.GetCustomAttribute<CreateAssetMenuAttribute>();
                //     if (createAssetMenu != null)
                //     {
                //         return Resources.Load(createAssetMenu.menuName);
                //     }
                // }
                
                return null;
            }
        }

        #region INTERNAL METHODS
        /// <summary>
        /// Gets value of variable
        /// </summary>
        /// <param name="variableName"> name of variable (the name inside attribute) </param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool TryGetValue(string variableName, out object value)
        {
            if (TryGetDevConsoleVariable(variableName, out DevConsoleFieldData data))
            {
                value = data.FieldInfo.GetValue(data.Instance);
                return true;
            }
            
            value = null;
            return false;
        }

        /// <summary>
        /// Sets value of variable
        /// </summary>
        /// <param name="variableName"> name of variable to modify (the name inside attribute) </param>
        /// <param name="value"> value to set </param>
        /// <param name="message"></param>
        internal static bool TrySetValue(string variableName, object value, out string logMessage)
        {
            logMessage = string.Empty;
            
            if (TryGetDevConsoleVariable(variableName, out DevConsoleFieldData data) == false)
            {
                logMessage = $"Variable '{variableName}' not found!";
                return false;
            }
            
            FieldInfo fieldInfo = data.FieldInfo;
            Object instance = data.Instance;
            Type fieldType = data.FieldType;
            
            if (fieldInfo.IsLiteral || fieldInfo.IsInitOnly)
            {
                logMessage = $"Variable '{variableName}' cannot be a literal or init only!";
                return false;
            }
            
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
                logMessage = $"Variable '{variableName}' has unsupported type of {fieldType}";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets all names of modifiable variables
        /// </summary>
        /// <returns> string that contains field name of each modifiable variable </returns>
        internal static (string, Type)[] GetAllFields()
        {
            IEnumerable<DevConsoleFieldData> allFields = GetAllDevConsoleVariables();
            IEnumerable<DevConsoleFieldData> safeFields = allFields.Where(data => data != null);
            return safeFields.Select(data => (data.VariableName, data.FieldType)).ToArray();
        }
        #endregion
    }
}