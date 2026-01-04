using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UIElements;

// System as data:
// One folder per play session
// One JSON per event
// One metadata file describing the session

namespace NoCtrl.GameEvents.Editor
{
    public class GameEventDashboard : EditorWindow
    {
        [SerializeField] private VisualTreeAsset m_uxml;
        private TemplateContainer m_root;

        private DateTime sessionStartTime;
        private DateTime sessionEndTime;

        private List<EventMetricRow> m_eventMetricRowRows = new List<EventMetricRow>();
        private List<ActiveEventRow> m_activeEventRowRows = new List<ActiveEventRow>();

        private Dictionary<string, PlaySessionInfo> m_playSessionInfos = new Dictionary<string, PlaySessionInfo>();
        private Dictionary<string, EventMetricInfo> m_eventMetricInfos = new Dictionary<string, EventMetricInfo>();

        private MultiColumnListView m_gameEventMultiColumnListView;
        private MultiColumnListView m_playSessionListView;

        private DropdownField m_playSessionDropDown;
        private DropdownField m_sessionEventDropDown;

        private Toggle m_metricTrackingToggle;
        private Toggle m_debugConsoleLoggingToggle;

        // Menu window
        [MenuItem("noCTRL Studios/Game Events Dashboard %#d")]
        public static void ShowWindow()
        {
            GetWindow(typeof(GameEventDashboard), false, "noCTRL Game Events Dashboard");
        }
        
        public void CreateGUI()
        {
            if (m_uxml == null)
            {
                Debug.LogError("Dashboard UXML not assigned.");
                return;
            }

            m_root = m_uxml.CloneTree();
            rootVisualElement.Add(m_root);

            Build_MetricsTrackingToggle();
            Build_DebugConsoleLoggingToggle();
            Build_ClearButton();
            Build_OpenFolderButton();
Build_CreateGameEventButton();
Build_GameEventMultiColumnListView();
Build_PlaySessionDropDown();
Build_SessionEventDropDown();
Build_ResetButton();
        }

        public void BeginSession()
        {
            // sessionStartTime = DateTime.UtcNow;
        }

        public void EndSession()
        {
            // sessionEndTime = DateTime.UtcNow;

            string projectName = Application.productName;
            string metricsFolder = GameEventsDefinitions.MetricsDirectory;

            if (!Directory.Exists(metricsFolder)) return;

            string[] sessionFolders = Directory.GetDirectories(metricsFolder, $"{projectName}_Session_*");

            foreach (string folder in sessionFolders)
            {
                var tempPlaySession = new PlaySessionInfo(folder);

                if (!m_playSessionInfos.ContainsKey(tempPlaySession.FolderName))
                {
                    m_playSessionInfos.Add(tempPlaySession.FolderName, tempPlaySession);
                }

                m_playSessionDropDown.choices.Add(tempPlaySession.FolderName);
            }
        }

        // ------------------------ Services ------------------------

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case
                    PlayModeStateChange.EnteredPlayMode:
                    BeginSession();
                    break;
                case
                    PlayModeStateChange.ExitingPlayMode:
                    EndSession();
                    break;

            }
        }

        private void Reset()
        {
            if (m_root == null) 
                return;

            var PlaySessionDropDown = m_root.Q<DropdownField>("PlaySessionDropDown");
            var SessionEventDropDown = m_root.Q<DropdownField>("SessionEventDropDown");

            SessionEventDropDown.value = null;
            PlaySessionDropDown.value = null;
            m_eventMetricRowRows.Clear();
            var PlaySessionListView = m_root.Q<MultiColumnListView>("PlaySessionListView");
            PlaySessionListView.Rebuild();

            string projectName = Application.productName;
            string metricsFolder = GameEventsDefinitions.MetricsDirectory;

            if (!Directory.Exists(metricsFolder)) return;

            string[] sessionFolders = Directory.GetDirectories(metricsFolder, $"{projectName}_Session_*");

            foreach (string folder in sessionFolders)
            {
                var tempPlaySession = new PlaySessionInfo(folder);

                if (!m_playSessionInfos.ContainsKey(tempPlaySession.FolderName))
                {
                    m_playSessionInfos.Add(tempPlaySession.FolderName, tempPlaySession);
                }

                m_playSessionDropDown.choices.Add(tempPlaySession.FolderName);
            }
        }

        private void OpenMetricsFolder()
        {
            var path = GameEventsDefinitions.MetricsDirectory; 
            if (!Directory.Exists(path)) 
            { 
                Directory.CreateDirectory(path); 
            }
            // TODO: Document this fix
            // Note this Fix for now the / needed to be added to go into the metric folder
            var tempString = path + "/";
            EditorUtility.RevealInFinder(tempString);
        }
        
        private void ClearMetrics()
        {
            string metricsFolder = GameEventsDefinitions.MetricsDirectory;
            if (!Directory.Exists(metricsFolder)) return;

            // Find all session folders for this project
            string projectName = Application.productName;
            string[] sessionFolders= Directory.GetDirectories(metricsFolder, $"{projectName}_Session_*");

            foreach (string folder in sessionFolders) 
                Directory.Delete(folder, true); // true = recursive delete

m_playSessionInfos.Clear();
m_eventMetricInfos.Clear();

var PlaySessionDropDown = m_root.Q<DropdownField>("PlaySessionDropDown");
var SessionEventDropDown = m_root.Q<DropdownField>("SessionEventDropDown");

SessionEventDropDown.value = null;
SessionEventDropDown.choices.Clear();
            PlaySessionDropDown.value = null;
            PlaySessionDropDown.choices.Clear();

            AssetDatabase.Refresh();
            EditorUtility.SetDirty(this);
        }

        private void ShowCreateEventPopup()
        {
            string path = EditorUtility.SaveFilePanelInProject("Create Game Event", "NewGameEvent", "asset",
                "Choose where to save the new GameEvent asset.");

            if (string.IsNullOrEmpty(path)) return;

            var asset = AssetDatabase.LoadAssetAtPath<GameEvent>(path);
            asset = CreateInstance<GameEvent>();

            // Create the event
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets(); // Save to data
            EditorGUIUtility.PingObject(asset); // ping project view
            Selection.activeObject = asset; // set active
        }

        private void OnMetricsToggleChanged(ChangeEvent<bool> evt)
        {
            GameEventEditorRegistry.Instance.MetricTracking = evt.newValue;
            Debug.Log($"Metrics recording is now {(evt.newValue ? "ENABLED" : "DISABLED")}");
        }

        private void OnDebugConsoleLoggingToggleToggleChanged(ChangeEvent<bool> evt)
        {
            GameEventEditorRegistry.Instance.DebugConsoleLogging = evt.newValue;
            Debug.Log($"Debug Console Logging recording is now {(evt.newValue ? "ENABLED" : "DISABLED")}");
        }

        // ------------------------ Builders ------------------------

        private void Build_DebugConsoleLoggingToggle()
        {
            m_debugConsoleLoggingToggle = m_root.Q<Toggle>("DebugConsoleLoggingToggle");

            // Read the current value from your config/service
            m_debugConsoleLoggingToggle.SetValueWithoutNotify(GameEventEditorRegistry.Instance.DebugConsoleLogging);
            // Register callback
            m_debugConsoleLoggingToggle.RegisterValueChangedCallback(OnDebugConsoleLoggingToggleToggleChanged);
        }

        private void Build_MetricsTrackingToggle()
        {
            m_metricTrackingToggle = m_root.Q<Toggle>("MetricTrackingToggle");

            // Read the current value from your config/service
            m_metricTrackingToggle.SetValueWithoutNotify(GameEventEditorRegistry.Instance.MetricTracking);
            // Register callback
            m_metricTrackingToggle.RegisterValueChangedCallback(OnMetricsToggleChanged);
        }

        private void Build_ClearButton()
        {
            var clearButton = m_root.Q<Button>("ClearMetricData");
            if (clearButton == null)
            {
                Debug.LogError("ClearMetricData button not found in UXML.");
                return;
            }
            clearButton.clicked += () =>
            {
                ClearMetrics();
                Debug.Log("Game Event metrics cleared.");
            };
        }

        private void Build_OpenFolderButton()
        {
            var OpenFolderButton = m_root.Q<Button>("OpenMetricFolder");

            OpenFolderButton.clicked += () =>
            {
                Debug.Log("noCTRL Events Metric Folder Opening...");
                EditorApplication.delayCall += OpenMetricsFolder;

            };
        }

        private void Build_CreateGameEventButton()
        {
            var createButton = m_root.Q<Button>("GameEventCreateButton"); 
            createButton.clicked += ShowCreateEventPopup;
        }

        private void Build_GameEventMultiColumnListView()
        {
            var registeredEventDataList = GameEventEditorRegistry.Instance.m_registeredEvents;
            m_gameEventMultiColumnListView = m_root.Q<MultiColumnListView>("GameEventListView");

            // Convert JSON metrics into ListView rows
            m_activeEventRowRows.Clear();
            foreach (var gameEvent in registeredEventDataList)
            {
                if (gameEvent == null)
                    continue;

                m_activeEventRowRows.Add(new ActiveEventRow
                {
                    EventName = gameEvent.name,
                    UniqueListeners = gameEvent.Listeners.Count
                });
            }

            m_gameEventMultiColumnListView.columns["EventName"].bindCell = (element, rowIndex) =>
            {
                var row = m_activeEventRowRows[rowIndex]; (element as Label).text = row.EventName;
            };

            m_gameEventMultiColumnListView.columns["NumberOfListeners"].bindCell = (element, rowIndex) =>
            {
                var row = m_activeEventRowRows[rowIndex]; (element as Label).text = row.UniqueListeners.ToString();
            };

            // Assign data + refresh
            m_gameEventMultiColumnListView.itemsSource = m_activeEventRowRows;
            m_gameEventMultiColumnListView.Rebuild();
        }

        private void Build_PlaySessionDropDown()
        {
m_playSessionDropDown             = m_root.Q<DropdownField>("PlaySessionDropDown");

            string projectName = Application.productName;
            string metricsFolder = GameEventsDefinitions.MetricsDirectory;

            if (!Directory.Exists(metricsFolder)) return;

            string[] sessionFolders = Directory.GetDirectories(metricsFolder, $"{projectName}_Session_*");

            foreach (string folder in sessionFolders)
            {
                var tempPlaySession = new PlaySessionInfo(folder);

                if (!m_playSessionInfos.ContainsKey(tempPlaySession.FolderName))
                {
                    m_playSessionInfos.Add(tempPlaySession.FolderName, tempPlaySession);
                }

                m_playSessionDropDown.choices.Add(tempPlaySession.FolderName);
            }

            m_playSessionDropDown.RegisterValueChangedCallback(evt =>
            {
                if (string.IsNullOrEmpty(evt.newValue))
                    return; // Ignore null/empty selections

                var SessionEventDropDown = m_root.Q<DropdownField>("SessionEventDropDown");

                // Look up the full session info using the display name
                PlaySessionInfo sessionInfo = m_playSessionInfos[evt.newValue];

                // Use the FULL PATH for file loading
                string[] sessionFiles = Directory.GetFiles(sessionInfo.FullPath, "*.json");

                SessionEventDropDown.choices.Clear();
                SessionEventDropDown.value = null;

                foreach (string file in sessionFiles)
                {
                    var tempEventMetricInfo = new EventMetricInfo(file);

                    if (!m_eventMetricInfos.ContainsKey(tempEventMetricInfo.FileName))
                        m_eventMetricInfos.Add(tempEventMetricInfo.FileName, tempEventMetricInfo);

                    SessionEventDropDown.choices.Add(tempEventMetricInfo.FileName);
                }
            });
        }

        private void Build_SessionEventDropDown()
        {
            m_sessionEventDropDown = m_root.Q<DropdownField>("SessionEventDropDown");

            m_sessionEventDropDown.RegisterValueChangedCallback(evt =>
            {
                if (string.IsNullOrEmpty(evt.newValue))
                    return;

                // TODO: finish reading Json data when this value changes and display in the playSessionListView

                if (!m_eventMetricInfos.ContainsKey(evt.newValue))
                    return;

                List<GameEvent.RaiseMetric> eventMetrics = m_eventMetricInfos[evt.newValue].GetMetrics();

                m_eventMetricRowRows.Clear();
                foreach (GameEvent.RaiseMetric metric in eventMetrics)
                {
                    m_eventMetricRowRows.Add(new EventMetricRow
                    {
                        TimeStamp = metric.TimeStamp(),
                        UniqueListeners = metric.ListenerCount()
                    });
                }

                m_playSessionListView = m_root.Q<MultiColumnListView>("PlaySessionListView");

                m_playSessionListView.columns["TimeStamp"].bindCell = (element, rowIndex) =>
                {
                    var row = m_eventMetricRowRows[rowIndex]; (element as Label).text = row.TimeStamp;
                };

                m_playSessionListView.columns["NumberListeners"].bindCell = (element, rowIndex) =>
                {
                    var row = m_eventMetricRowRows[rowIndex]; (element as Label).text = row.UniqueListeners.ToString();
                };
                m_playSessionListView.itemsSource = m_eventMetricRowRows;
                m_playSessionListView.Rebuild();
            });
        }

        private void Build_ResetButton()
        {
            var ResetButton = m_root.Q<Button>("ResetButton");
            ResetButton.clicked += Reset;
        }
    }
}