using System.Reflection;
using DevConsole.Logging;
using UnityEditor;
using UnityEngine;

namespace DevConsole.Editor
{
    [CustomEditor(typeof(DevConsoleLogger))]
    public class DevConsoleLoggerEditor : UnityEditor.Editor
    {
        private DevConsoleLogger _devConsoleLogger;
        
        private void OnEnable()
        {
            _devConsoleLogger = target as DevConsoleLogger;
        }

        public override void OnInspectorGUI()
        {
            if (_devConsoleLogger == null)
            {
                return;
            }
            
            serializedObject.Update();
            EditorGUILayout.BeginVertical();

            DrawDefaultInspector();
            EditorGUILayout.Space();
            
            GUI.enabled = Application.isPlaying;
            
            if (GUILayout.Button("Test Log"))
            {
                _devConsoleLogger
                    .GetType()
                    .GetMethod("TestLog", BindingFlags.Instance | BindingFlags.NonPublic)
                    ?.Invoke(_devConsoleLogger, default);
            }
            
            if (GUILayout.Button("Test LogWarning"))
            {
                _devConsoleLogger
                    .GetType()
                    .GetMethod("TestLogWarning", BindingFlags.Instance | BindingFlags.NonPublic)
                    ?.Invoke(_devConsoleLogger, default);
            }
            
            if (GUILayout.Button("Test LogError"))
            {
                _devConsoleLogger
                    .GetType()
                    .GetMethod("TestLogError", BindingFlags.Instance | BindingFlags.NonPublic)
                    ?.Invoke(_devConsoleLogger, default);
            }

            if (GUI.enabled == false)
            {
                GUI.enabled = true;
            }

            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }
    }
}