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
        [SerializeField] private VisualTreeAsset m_rowTemplate;

        private TemplateContainer m_root;

        private Dictionary<string, PlaySessionInfo> m_playSessionInfos = new Dictionary<string, PlaySessionInfo>();
        private Dictionary<string, EventMetricInfo> m_eventMetricInfos = new Dictionary<string, EventMetricInfo>();

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

            var OpenFolderButton = m_root.Q<Button>("OpenMetricFolder");

            OpenFolderButton.clicked += () =>
            {
                Debug.Log("noCTRL Events Metric Folder Opening...");
                EditorApplication.delayCall += OpenMetricsFolder;

            };

            var registeredEventDataList = GameEventEditorRegistry.Instance.m_registeredEvents;
            var listView = m_root.Q<ListView>("GameEventListView");

            listView.itemsSource = registeredEventDataList;
            listView.makeItem = () => m_rowTemplate.Instantiate();

            listView.bindItem = (element, i) =>
            {
                var row = registeredEventDataList[i];
                element.Q<Label>("eventName").text = row.name;
                element.Q<Label>("listenerCount").text = row.Listeners.Count.ToString();
            };

            listView.fixedItemHeight = 22;
            listView.selectionType = SelectionType.Single;

            var createButton = m_root.Q<Button>("GameEventCreateButton"); createButton.clicked += ShowCreateEventPopup;

            var PlaySessionDropDown = m_root.Q<DropdownField>("PlaySessionDropDown");
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

                PlaySessionDropDown.choices.Add(tempPlaySession.FolderName);
            }

            PlaySessionDropDown.RegisterValueChangedCallback(evt =>
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

            var ResetButton = m_root.Q<Button>("ResetButton");
            ResetButton.clicked += Reset;

        }

        public void Reset()
        {
            var PlaySessionDropDown = m_root.Q<DropdownField>("PlaySessionDropDown");
            var SessionEventDropDown = m_root.Q<DropdownField>("SessionEventDropDown");

            SessionEventDropDown.value = null;
            PlaySessionDropDown.value = null;
        }

        public void OpenMetricsFolder()
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

        public void ClearMetrics()
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
    }
}