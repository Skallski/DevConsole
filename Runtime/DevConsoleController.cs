using UnityEngine;
using UnityEngine.Events;

namespace DevConsole
{
    public class DevConsoleController : MonoBehaviour
    {
        [SerializeField] private KeyCode _consoleOpenKeycode = KeyCode.Tilde;
        [SerializeField] private GameObject _content;

        [SerializeField] private UnityEvent _onOpen;
        [SerializeField] private UnityEvent _onClose;
        
        internal bool IsOpened => _content.activeSelf;
        
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
        private void Start()
        {
            if (IsOpened)
            {
                _content.SetActive(false);
            }
        }

        private void Update()
        {
            if (IsOpened)
            {
                return;
            }

            if (Input.GetKeyDown(_consoleOpenKeycode))
            {
                Open();
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
            
            _onOpen?.Invoke();
        }

        private void ForceClose()
        {
            _content.SetActive(false);
            
            _onClose?.Invoke();
        }
    }
}