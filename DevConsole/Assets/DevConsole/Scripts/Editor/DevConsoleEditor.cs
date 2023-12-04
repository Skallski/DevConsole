using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DevConsole.Editor
{
    [CustomEditor(typeof(DevConsoleController), true)]
    public class DevConsoleEditor : UnityEditor.Editor
    {
        private DevConsoleController _devConsoleController;

        private SerializedProperty _consoleOpenKeycode;
        private SerializedProperty _content;
        private SerializedProperty _onOpen;
        private SerializedProperty _onClose;
        
        private void OnEnable()
        {
            _devConsoleController = target as DevConsoleController;

            _consoleOpenKeycode = serializedObject.FindProperty("_consoleOpenKeycode");
            _content = serializedObject.FindProperty("_content");
            _onOpen = serializedObject.FindProperty("_onOpen");
            _onClose = serializedObject.FindProperty("_onClose");
        }

        public override void OnInspectorGUI()
        {
            if (_devConsoleController == null)
            {
                return;
            }
            
            EditorGUILayout.PropertyField(_consoleOpenKeycode);
            EditorGUILayout.PropertyField(_content);
            
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_onOpen);
            EditorGUILayout.PropertyField(_onClose);

            if (Application.isPlaying == false)
            {
                return;
            }

            EditorGUILayout.Space();
            EditorGUI.DrawRect(
                EditorGUILayout.GetControlRect(false, 1), new Color(0f, 0f, 0f, 0.3f));
            EditorGUILayout.Space();

            PropertyInfo isOpenedPropertyInfo = _devConsoleController.GetType()
                .GetProperty("IsOpened", BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (isOpenedPropertyInfo == null)
            {
                return;
            }
            
            if ((bool) isOpenedPropertyInfo.GetValue(_devConsoleController))
            {
                if (GUILayout.Button("Close"))
                {
                    _devConsoleController.GetType()
                        .GetMethod("ForceClose", BindingFlags.NonPublic | BindingFlags.Instance)
                        ?.Invoke(_devConsoleController, default);
                }
            }
            else
            {
                if (GUILayout.Button("Open"))
                {
                    _devConsoleController.GetType()
                        .GetMethod("ForceOpen", BindingFlags.NonPublic | BindingFlags.Instance)
                        ?.Invoke(_devConsoleController, default);
                }
            }
        }
    }
}