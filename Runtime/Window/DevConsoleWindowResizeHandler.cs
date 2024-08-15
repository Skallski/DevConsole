using UnityEngine;
using UnityEngine.EventSystems;
using JetBrains.Annotations;

namespace DevConsole.Window
{
    public class DevConsoleWindowResizeHandler : MonoBehaviour
    {
        [SerializeField] private Canvas _parentCanvas;
        [SerializeField] private RectTransform _window;

        [Space] 
        [SerializeField] private bool _allowResize = true;
        [SerializeField] private bool _resetSizeOnOpen = true;
        [SerializeField] private float _topResizeLimit = 150;
        [SerializeField] private float _bottomResizeLimit = 400;

        private bool _isResizable;
        private Vector2 _initialMousePosition;
        private float _initialHeight;
        private Vector2 _initialPosition;
        private float _scaleFactor;

        private float _defaultHeight;

#if UNITY_EDITOR
        private void Reset()
        {
            if (_window == null)
            {
                _window = GetComponent<RectTransform>();
            }
        }
#endif

        private void Awake()
        {
            _defaultHeight = _window.rect.height;
        }

        private void OnEnable()
        {
            if (_resetSizeOnOpen)
            {
                _window.sizeDelta = new Vector2(_window.sizeDelta.x, _defaultHeight);
            }
        }

        private void Start()
        {
            _scaleFactor = _window.localScale.y;
        }

        [UsedImplicitly]
        public void OnPointerDown(BaseEventData eventData)
        {
            if (_allowResize == false)
            {
                return;
            }
            
            _isResizable = true;
            if (eventData is PointerEventData pointerEventData)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _parentCanvas.transform as RectTransform, pointerEventData.position, 
                    _parentCanvas.worldCamera, out _initialMousePosition);
                
                _initialHeight = _window.rect.height;
                _initialPosition = _window.anchoredPosition;
            }
        }

        [UsedImplicitly]
        public void OnPointerUp()
        {
            _isResizable = false;
        }

        [UsedImplicitly]
        public void OnDrag(BaseEventData eventData)
        {
            if (_allowResize == false)
            {
                return;
            }
            
            if (_isResizable && eventData is PointerEventData pointerEventData)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _parentCanvas.transform as RectTransform, pointerEventData.position, 
                    _parentCanvas.worldCamera, out Vector2 localMousePosition);

                float dragDelta = - (localMousePosition.y - _initialMousePosition.y) / _scaleFactor;
                float newHeight = Mathf.Clamp(_initialHeight + dragDelta, _topResizeLimit, _bottomResizeLimit);

                _window.sizeDelta = new Vector2(_window.sizeDelta.x, newHeight);

                float heightDelta = newHeight - _initialHeight;
                _window.anchoredPosition = new Vector2(_window.anchoredPosition.x, 
                    _initialPosition.y - heightDelta);
            }
        }
    }
}
