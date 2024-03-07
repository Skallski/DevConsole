using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DevConsole.Logging
{
    public class DevConsoleLogFilters : MonoBehaviour
    {
        [Serializable]
        private struct DevConsoleLogFilter
        {
            [SerializeField] private Image _image;
            [field:SerializeField] internal Toggle Toggle { get; private set; }
            [field:SerializeField] internal LogType LogType { get; private set; }

            internal void OnToggled(bool value)
            {
                Color currentColor = _image.color;
                _image.color = value 
                    ? new Color(currentColor.r, currentColor.g, currentColor.b, 1) 
                    : new Color(currentColor.r, currentColor.g, currentColor.b, 0.5f);
            }
        }

        [SerializeField] private DevConsoleLogFilter[] _filters;

        private readonly HashSet<LogType> _filteredLogTypes = new HashSet<LogType>();
        internal static event Action FilterToggled;

        private void OnEnable()
        {
            DevConsoleLogger.OnClear += ClearFilters;
            
            foreach (DevConsoleLogFilter filter in _filters)
            {
                filter.Toggle.onValueChanged.AddListener(isToggled =>
                {
                    filter.OnToggled(isToggled);
                    ApplyFilter(filter.LogType, isToggled);
                });
            }
        }

        private void OnDisable()
        {
            DevConsoleLogger.OnClear -= ClearFilters;
            
            foreach (DevConsoleLogFilter filter in _filters)
            {
                filter.Toggle.onValueChanged.RemoveListener(isToggled =>
                {
                    filter.OnToggled(isToggled);
                    ApplyFilter(filter.LogType, isToggled);
                });
            }
        }

        private void Start()
        {
            ClearFilters();
        }

        private void ApplyFilter(LogType logType, bool isToggled)
        {
            if (isToggled)
            {
                if (_filteredLogTypes.Contains(logType) == false)
                {
                    _filteredLogTypes.Add(logType);
                }
            }
            else
            {
                if (_filteredLogTypes.Contains(logType))
                {
                    _filteredLogTypes.Remove(logType);
                }
            }
            
            FilterToggled?.Invoke();
        }

        internal DevConsoleLogData[] GetFilteredLogs(IEnumerable<DevConsoleLogData> logs)
        {
            return logs.Where(log => _filteredLogTypes.Any(filter => log.LogType == filter)).ToArray();
        }

        private void ClearFilters()
        {
            _filteredLogTypes.Clear();
            foreach (DevConsoleLogFilter filter in _filters)
            {
                _filteredLogTypes.Add(filter.LogType);
            }
        }
    }
}