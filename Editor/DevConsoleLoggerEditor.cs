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
            
            if (GUILayout.Button("Test Log")) Debug.Log("test info");
            if (GUILayout.Button("Test LogWarning")) Debug.LogWarning("test warning");
            if (GUILayout.Button("Test LogError")) Debug.LogError("test error");

            if (GUI.enabled == false)
            {
                GUI.enabled = true;
            }

            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }
    }
}