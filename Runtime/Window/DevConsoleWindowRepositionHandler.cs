using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DevConsole.Window
{
    public class DevConsoleWindowRepositionHandler : MonoBehaviour
    {
        [SerializeField] private Canvas _parentCanvas;
        [SerializeField] private RectTransform _window;

        [Space] 
        [SerializeField] private bool _allowReposition = true;
        [SerializeField] private bool _resetPositionOnOpen = true;
        
        private bool _isDraggable;
        private Vector2 _dragBounds;

        private Vector2 _defaultPosition;

#if UNITY_EDITOR
        private void Reset()
        {
            if (_window == null)
            {
                _window = GetComponent<RectTransform>();
            }
            
            if (_parentCanvas == null)
            {
                _parentCanvas = ParentCanvasFinder.FindParentCanvas(transform);
            }
        }
#endif

        private void Awake()
        {
            _defaultPosition = _window.anchoredPosition;
        }

        private void OnEnable()
        {
            if (_resetPositionOnOpen)
            {
                float offsetWidth = _parentCanvas.pixelRect.width / 4;
                _window.offsetMin = new Vector2(offsetWidth, _window.offsetMin.y);
                _window.offsetMax = new Vector2(-offsetWidth, _window.offsetMax.y);
                _window.anchoredPosition = new Vector2(_window.anchoredPosition.x, _defaultPosition.y);
            }
        }

        private void Start()
        {
            _dragBounds = _parentCanvas.pixelRect.size * 0.5f;
        }

        [UsedImplicitly]
        public void OnPointerDown()
        {
            _isDraggable = true;
        }

        [UsedImplicitly]
        public void OnPointerUp()
        {
            _isDraggable = false;
        }

        [UsedImplicitly]
        public void OnDrag(BaseEventData eventData)
        {
            if (_allowReposition == false)
            {
                return;
            }
            
            if (_isDraggable)
            {
                if (eventData is PointerEventData pointerEventData)
                {
                    _window.anchoredPosition += pointerEventData.delta / _parentCanvas.scaleFactor;
                }
            }

            Vector2 pos = _window.anchoredPosition;

            _window.anchoredPosition = new Vector2(
                Mathf.Clamp(pos.x, -_dragBounds.x, _dragBounds.x),
                Mathf.Clamp(pos.y, -_dragBounds.y, _defaultPosition.y)
            );
        }
    }
}