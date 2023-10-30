using UnityEngine;

namespace Console
{
    public class Console : MonoBehaviour
    {
        [SerializeField] private GameObject _content;
        
        public bool IsOpened => _content.activeSelf;
        
#if UNITY_EDITOR
        private void Reset()
        {
            if (_content == null)
            {
                for (int i = 0, c = transform.childCount; i < c; i++)
                {
                    var child = transform.GetChild(i);
                    if (child.name.Equals("Content"))
                    {
                        _content = child.gameObject;
                        break;
                    }
                }
            }
        }
#endif
        public void Update()
        {
            if (IsOpened == false)
            {
                if (Input.GetKeyDown(KeyCode.Tilde) || 
                    Input.GetKeyDown(KeyCode.Slash) || 
                    Input.GetKeyDown(KeyCode.Backslash))
                {
                    Open();
                }
            }
        }
        
        [ContextMenu(nameof(Open))]
        public void Open()
        {
            if (IsOpened == false)
            {
                // TODO: invoke opening animation
            
                ForceOpen();
            }
        }

        [ContextMenu(nameof(Close))]
        public void Close()
        {
            if (IsOpened)
            {
                // TODO: invoke closing animation
                
                ForceClose();
            }
        }

        private void ForceOpen()
        {
            _content.SetActive(true);
        }

        private void ForceClose()
        {
            _content.SetActive(false);
        }
    }
}