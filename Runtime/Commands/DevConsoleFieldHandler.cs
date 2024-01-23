using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DevConsole.Commands
{
    public static class DevConsoleFieldHandler
    {
        internal class DevConsoleFieldData
        {
            public readonly FieldInfo FieldInfo;
            public readonly Object Instance;
            public readonly object InitialValue;
            public readonly string VariableName;
            public readonly Type FieldType;

            public DevConsoleFieldData(FieldInfo fieldInfo, Object instance)
            {
                FieldInfo = fieldInfo;
                Instance = instance;

                if (fieldInfo != null)
                {
                    InitialValue = fieldInfo.GetValue(instance);
                    VariableName = fieldInfo.GetCustomAttribute<DevConsoleFieldAttribute>().VariableName;
                    FieldType = fieldInfo.FieldType;
                }
                else
                {
                    InitialValue = null;
                    VariableName = string.Empty;
                    FieldType = null;
                }
            }
        }

        private static readonly List<DevConsoleFieldData> DevConsoleFieldsCache =
            new List<DevConsoleFieldData>();
        
        /// <summary>
        /// Gets data of modifiable variable with certain name
        /// </summary>
        /// <param name="variableName"> modifiable variable name (the name inside attribute) </param>
        /// <returns></returns>
        private static DevConsoleFieldData GetDevConsoleVariable(string variableName)
        {
            List<DevConsoleFieldData> allFields = GetAllDevConsoleFields();
            return allFields.FirstOrDefault(field => field.VariableName.Equals(variableName));
        }

        /// <summary>
        /// Gets each modifiable variable's data
        /// </summary>
        /// <returns> List that contains every modifiable variable's data </returns>
        private static List<DevConsoleFieldData> GetAllDevConsoleFields()
        {
            if (DevConsoleFieldsCache.Count == 0)
            {
                IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies();
                List<DevConsoleFieldData> fields = new List<DevConsoleFieldData>();

                foreach (var assembly in assemblies)
                {
                    var types = assembly.GetTypes();
                    foreach (var type in types)
                    {
                        if (type.IsSubclassOf(typeof(MonoBehaviour)) || type.IsSubclassOf(typeof(ScriptableObject)))
                        {
                            var typeFields = type
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
            // Returns instance of seeking FieldIngo
            Object GetInstanceOfField(MemberInfo info)
            {
                Type declaringType = info.DeclaringType;
                if (declaringType == null)
                {
                    return null;
                }
            
                if (typeof(ScriptableObject).IsAssignableFrom(declaringType)) // for variable stored inside ScriptableObject
                {
                    CreateAssetMenuAttribute createAssetMenu = declaringType.GetCustomAttribute<CreateAssetMenuAttribute>();
                    if (createAssetMenu != null)
                    {
                        return Resources.Load(createAssetMenu.menuName);
                    }
                }
                else if (typeof(MonoBehaviour).IsAssignableFrom(declaringType)) // for variable stored inside MonoBehaviour
                {
                    return Object.FindObjectOfType(declaringType);
                }

                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool IsValid(DevConsoleFieldData data)
        {
            return data != null && data.FieldInfo != null && data.Instance != null;
        }

        #region INTERNAL FIELDS
        /// <summary>
        /// 
        /// </summary>
        /// <param name="variableName"></param>
        /// <returns></returns>
        internal static Type GetType(string variableName)
        {
            DevConsoleFieldData devConsoleFieldData = GetDevConsoleVariable(variableName);
            
            return IsValid(devConsoleFieldData) 
                ? devConsoleFieldData.FieldType 
                : null;
        }
        
        /// <summary>
        /// Gets value of modifiable variable
        /// </summary>
        /// <param name="variableName"> modifiable variable name (the name inside attribute) </param>
        /// <param name="value"> value to return </param>
        /// <returns> true or false, whether the value has ben get successfully or not </returns>
        internal static bool GetValue(string variableName, out object value)
        {
            value = null;
            DevConsoleFieldData devConsoleFieldData = GetDevConsoleVariable(variableName);
            
            if (IsValid(devConsoleFieldData))
            {
                value = devConsoleFieldData.FieldInfo.GetValue(devConsoleFieldData.Instance);

                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Sets value of modifiable variable
        /// </summary>
        /// <param name="variableName"> modifiable variable name (the name inside attribute) </param>
        /// <param name="value"> value to set </param>
        /// <returns> true or false, whether the value has ben set successfully or not </returns>
        internal static bool SetValue(string variableName, object value)
        {
            DevConsoleFieldData devConsoleFieldData = GetDevConsoleVariable(variableName);
            if (IsValid(devConsoleFieldData) == false)
            {
                return false;
            }
            
            FieldInfo fieldInfo = devConsoleFieldData.FieldInfo;
            Object instance = devConsoleFieldData.Instance;
            Type fieldType = devConsoleFieldData.FieldType;

            if (fieldInfo.IsLiteral == false && fieldInfo.IsInitOnly == false)
            {
                try
                {
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
                }
                catch (FormatException)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="initialValue"></param>
        /// <returns></returns>
        internal static bool ResetValue(string variableName, out object initialValue)
        {
            initialValue = null;
            
            DevConsoleFieldData devConsoleFieldData = GetDevConsoleVariable(variableName);
            if (IsValid(devConsoleFieldData) == false)
            {
                return false;
            }
            
            initialValue = devConsoleFieldData.InitialValue;

            return SetValue(variableName, initialValue);
        }

        /// <summary>
        /// Gets all names of modifiable variables
        /// </summary>
        /// <returns> string that contains field name of each modifiable variable </returns>
        internal static string GetAllFieldNames()
        {
            StringBuilder sb = new StringBuilder();
            
            List<DevConsoleFieldData> allFields = GetAllDevConsoleFields();
            IEnumerable<DevConsoleFieldData> safeFields = allFields.Where(data => data != null);
            foreach (var data in safeFields)
            {
                sb.Append($"{data.VariableName}, ");
            }

            return sb.ToString();
        }
        #endregion
    }
}