using TMPro;
using UnityEngine;

namespace DevConsole
{
    public class DevConsoleVersionHandler : MonoBehaviour
    {
        [System.Serializable]
        private class DevConsoleVersionData
        {
            public string version;
            public string displayName;
        }

        [SerializeField] private TextAsset _packageFile;
        [SerializeField] private TextMeshProUGUI _label;

#if UNITY_EDITOR
        private void Reset()
        {
            if (_label == null)
            {
                _label = GetComponent<TextMeshProUGUI>();
            }
        }
#endif

        private void Start()
        {
            DevConsoleVersionData data = JsonUtility.FromJson<DevConsoleVersionData>(_packageFile.text);
            if (data != null)
            {
                _label.SetText($"{data.displayName} v{data.version}");
            }
        }
    }
}