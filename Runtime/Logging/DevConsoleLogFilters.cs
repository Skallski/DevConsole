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

            private static readonly Color32 ColorOn = new Color32(255, 255, 255, 255);
            private static readonly Color32 ColorOff = new Color32(200, 200, 200, 128);

            internal void OnToggled(bool value)
            {
                _image.color = value ? ColorOn : ColorOff;
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