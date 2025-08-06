using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework
{
    internal partial class DevLogView : MonoBehaviour
    {
        [SerializeField]
        private UIList logList;
        [SerializeField]
        private Text logDetails;
        [SerializeField]
        private InputField logSearch;
        private int maxLogCount;
        private string lastSearch = "";
        private LogInfo selectedInfo;
        private readonly CachedLinkedList<LogInfo> logInfos = new CachedLinkedList<LogInfo>();
        private readonly List<LogInfo> filteredInfos = new List<LogInfo>();
        private readonly List<LogInfo> resultInfos = new List<LogInfo>();
        private readonly HashSet<LogType> logTypes = new HashSet<LogType>(3);
        private readonly Dictionary<int, int> logCount = new Dictionary<int, int>(3);

        private Action<LogType, int> onLogCountChanged;

        public event Action<LogType, int> OnCountChanged
        {
            add { onLogCountChanged += value; }
            remove { onLogCountChanged -= value; }
        }

        private void Awake()
        {
            maxLogCount = GameSetting.Instance.ConsoleMaxLogCount;
            logList.Init(Resources.Load<DevLogCell>("DevLogCell"));
            logList.OnCellUpdate += OnCellUpdate;
            logSearch.onValueChanged.AddListener(OnSearchChanged);
            Application.logMessageReceived += OnMessageReceived;
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= OnMessageReceived;
        }

        private void OnEnable()
        {
            if (logInfos.Count > 0)
            {
                Refresh();
            }
        }

        public void ClearLog()
        {
            foreach (LogInfo logInfo in logInfos)
            {
                GenericObjectPool.Release(logInfo);
            }

            selectedInfo = null;
            logDetails.text = "";
            logInfos.Clear();
            filteredInfos.Clear();
            resultInfos.Clear();
            ClearLogCount();
            Refresh();
        }

        public void CopyLog()
        {
            if (selectedInfo != null)
            {
                GUIUtility.systemCopyBuffer = selectedInfo.GetDetailsLog(false);
            }
            else
            {
                GUIUtility.systemCopyBuffer = "";
            }
        }

        public void SetLogFilter(LogType logType, bool isOn)
        {
            if (isOn && !logTypes.Contains(logType))
            {
                logTypes.Add(logType);
                Refresh();
            }
            else if (!isOn && logTypes.Contains(logType))
            {
                logTypes.Remove(logType);
                Refresh();
            }
        }

        public void OnLogCellClicked(int index)
        {
            selectedInfo = resultInfos[index];
            logDetails.text = selectedInfo.GetDetailsLog();
        }

        private void AddLogCount(LogType logType, int value)
        {
            logCount.TryGetValue((int)logType, out int count);
            count += value;
            logCount[(int)logType] = count;

            if (isActiveAndEnabled)
            {
                onLogCountChanged?.Invoke(logType, count);
            }
        }

        private void ClearLogCount()
        {
            foreach (int key in logCount.Keys)
            {
                onLogCountChanged?.Invoke((LogType)key, 0);
            }

            logCount.Clear();
        }

        private void Refresh()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            foreach (int key in logCount.Keys)
            {
                onLogCountChanged?.Invoke((LogType)key, logCount[key]);
            }

            filteredInfos.Clear();
            resultInfos.Clear();
            foreach (LogInfo info in logInfos)
            {
                if (logTypes.Contains(info.LogType))
                {
                    filteredInfos.Add(info);
                    if (info.IsMatchPattern(logSearch.text))
                    {
                        resultInfos.Add(info);
                    }
                }
            }

            logList.SetCount(resultInfos.Count);
            logList.UpdateList();
        }

        private void OnCellUpdate(UIListCell listCell)
        {
            DevLogCell logListCell = listCell.Cast<DevLogCell>();
            LogInfo cellInfo = resultInfos[listCell.Index];
            bool isOn = cellInfo == selectedInfo;
            logListCell.SetData(this, cellInfo.GetSimpleLog(), isOn);
        }

        private void OnSearchChanged(string pattern)
        {
            if (pattern == null)
            {
                pattern = "";
            }

            int m = pattern.Length;
            if (m > 0 && pattern[m - 1] > 255)
            {
                logSearch.text = pattern = pattern.Substring(0, m - 1);
            }

            if (pattern == lastSearch)
            {
                return;
            }

            if (pattern.SearchOfBm(lastSearch) != -1)
            {
                for (int i = resultInfos.Count - 1; i >= 0; i--)
                {
                    if (!resultInfos[i].IsMatchPattern(pattern))
                    {
                        resultInfos.RemoveAt(i);
                    }
                }
            }
            else
            {
                resultInfos.Clear();
                for (int i = 0; i < filteredInfos.Count; i++)
                {
                    if (filteredInfos[i].IsMatchPattern(pattern))
                    {
                        resultInfos.Add(filteredInfos[i]);
                    }
                }
            }

            lastSearch = pattern;
            logList.SetCount(resultInfos.Count);
            logList.UpdateList();
        }

        private bool CheckLogOverflow()
        {
            if (logInfos.Count > maxLogCount)
            {
                int halfCount = maxLogCount / 2;
                while (logInfos.Count > halfCount)
                {
                    GenericObjectPool.Release(logInfos.First.Value);
                    logInfos.RemoveFirst();
                }

                ClearLogCount();

                foreach (LogInfo info in logInfos)
                {
                    AddLogCount(info.LogType, 1);
                }

                Refresh();
                return true;
            }

            return false;
        }

        private void OnMessageReceived(string condition, string stackTrace, LogType logType)
        {
            if (string.IsNullOrEmpty(stackTrace))
            {
                stackTrace = DebugUtility.GetStackTrace(3);
            }

            string matchTrace = "";
            if (!string.IsNullOrEmpty(stackTrace))
            {
                string[] traces = stackTrace.Split("\n", StringSplitOptions.RemoveEmptyEntries);
                if (traces.Length > 0)
                {
                    matchTrace = traces[0];
                }
            }

            LogInfo logInfo = null;
            bool isMerged = false;
            foreach (LogInfo info in logInfos)
            {
                if (info.Merge(condition, logType, matchTrace))
                {
                    logInfo = info;
                    isMerged = true;
                    break;
                }
            }

            if (logInfo == null)
            {
                logInfo = GenericObjectPool.Get<LogInfo>();
                logInfo.Init(condition, stackTrace, matchTrace, logType);
                logInfos.AddLast(logInfo);
            }
            else
            {
                logInfos.Remove(logInfo);
                logInfos.AddLast(logInfo);
            }

            if (CheckLogOverflow())
            {
                return;
            }

            AddLogCount(logInfo.LogType, 1);

            if (!isActiveAndEnabled)
            {
                return;
            }

            if (logTypes.Contains(logInfo.LogType))
            {
                if (isMerged)
                {
                    filteredInfos.Remove(logInfo);
                    resultInfos.Remove(logInfo);
                }

                filteredInfos.Add(logInfo);
                if (logInfo.IsMatchPattern(logSearch.text))
                {
                    resultInfos.Add(logInfo);
                }

                logList.SetCount(resultInfos.Count);
                logList.UpdateList();
            }
        }
    }
}
