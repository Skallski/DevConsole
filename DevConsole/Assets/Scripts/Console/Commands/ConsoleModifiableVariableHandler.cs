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
        internal class ModifiableVariableData
        {
            public readonly FieldInfo FieldInfo;
            public readonly Object Instance;
            public readonly object InitialValue;

            public ModifiableVariableData(FieldInfo fieldInfo, Object instance)
            {
                FieldInfo = fieldInfo;
                Instance = instance;
                InitialValue = fieldInfo.GetValue(instance);
            }
        }

        private static readonly List<ModifiableVariableData> CachedModifiableVariableDataSet =
            new List<ModifiableVariableData>();
        
        /// <summary>
        /// Gets data of modifiable variable with certain name
        /// </summary>
        /// <param name="variableName"> modifiable variable name (the name inside attribute) </param>
        /// <returns></returns>
        internal static ModifiableVariableData GetModifiableVariable(string variableName)
        {
            List<ModifiableVariableData> allFields = FindAllModifiableVariables();
            
            foreach (ModifiableVariableData data in allFields)
            {
                FieldInfo fieldInfo = data.FieldInfo;
                
                ConsoleModifiableVariableAttribute variableAttribute = 
                    fieldInfo.GetCustomAttribute<ConsoleModifiableVariableAttribute>();
                
                if (variableAttribute != null)
                {
                    if (variableAttribute.VariableName.Equals(variableName))
                    {
                        return data;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets all data of each modifiable variable
        /// </summary>
        /// <returns></returns>
        internal static List<ModifiableVariableData> FindAllModifiableVariables()
        {
            if (CachedModifiableVariableDataSet.Count == 0)
            {
                IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies();
                List<ModifiableVariableData> fields = new List<ModifiableVariableData>();

                foreach (var assembly in assemblies)
                {
                    var types = assembly.GetTypes();
                    foreach (var type in types)
                    {
                        if (type.IsSubclassOf(typeof(MonoBehaviour)) || type.IsSubclassOf(typeof(ScriptableObject)))
                        {
                            var typeFields = 
                                type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                .Where(field => field.IsDefined(typeof(ConsoleModifiableVariableAttribute), true));

                            foreach (var field in typeFields)
                            {
                                fields.Add(new ModifiableVariableData(field, GetInstanceOfField(field)));
                            }
                        }
                    }
                }

                CachedModifiableVariableDataSet.AddRange(fields);
            }

            return CachedModifiableVariableDataSet;
        }
        
        private static Object GetInstanceOfField(FieldInfo fieldInfo)
        {
            Type declaringType = fieldInfo.DeclaringType;
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

        internal static string GetNameOfField(FieldInfo fieldInfo)
        {
            return fieldInfo.GetCustomAttribute<ConsoleModifiableVariableAttribute>().VariableName;
        }

        /// <summary>
        /// Gets value of modifiable variable
        /// </summary>
        /// <param name="variableName"> modifiable variable name (the name inside attribute) </param>
        /// <param name="value"> value to return </param>
        /// <returns> true or false, whether the value has ben get successfully or not </returns>
        internal static bool GetModifiableVariableValue(string variableName, out object value)
        {
            ModifiableVariableData modifiableVariableData = GetModifiableVariable(variableName);
            return GetModifiableVariableValue(modifiableVariableData, out value);
        }

        /// <summary>
        /// Gets value of modifiable variable
        /// </summary>
        /// <param name="data"> modifiable variable data </param>
        /// <param name="value"> value to return </param>
        /// <returns> true or false, whether the value has ben get successfully or not </returns>
        internal static bool GetModifiableVariableValue(ModifiableVariableData data, out object value)
        {
            value = null;

            if (data == null)
            {
                return false;
            }

            FieldInfo fieldInfo = data.FieldInfo;
            if (fieldInfo == null)
            {
                return false;
            }

            Object instance = data.Instance;
            if (instance == null)
            {
                return false;
            }
            
            value = fieldInfo.GetValue(instance);
            
            return true;
        }

        /// <summary>
        /// Sets value of modifiable variable
        /// </summary>
        /// <param name="variableName"> modifiable variable name (the name inside attribute) </param>
        /// <param name="value"> value to set </param>
        /// <returns> true or false, whether the value has ben set successfully or not </returns>
        internal static bool SetModifiableVariableValue(string variableName, object value)
        {
            ModifiableVariableData modifiableVariableData = GetModifiableVariable(variableName);

            return SetModifiableVariableValue(modifiableVariableData, value);
        }

        /// <summary>
        /// Sets value of modifiable variable
        /// </summary>
        /// <param name="data"> modifiable variable data </param>
        /// <param name="value"> value to set </param>
        /// <returns> true or false, whether the value has ben set successfully or not </returns>
        internal static bool SetModifiableVariableValue(ModifiableVariableData data, object value)
        {
            if (data == null)
            {
                return false;
            }

            FieldInfo fieldInfo = data.FieldInfo;
            if (fieldInfo == null)
            {
                return false;
            }

            Object instance = data.Instance;
            if (instance == null)
            {
                return false;
            }

            if (fieldInfo.IsLiteral == false && fieldInfo.IsInitOnly == false)
            {
                Type fieldType = fieldInfo.FieldType;
                
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
                catch (FormatException e)
                {
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}