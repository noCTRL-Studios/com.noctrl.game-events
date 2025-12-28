using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.WSA;

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
                OpenMetricsFolder();
                Debug.Log("noCTRL Events Metric Folder Opening...");
            };

        }

        public void OpenMetricsFolder()
        {
            // TODO: Document this fix
            // Note this Fix for now the / needed to be added to go into the metric folder
            var tempString = GameEventsDefinitions.MetricsDirectory + "/";
            EditorUtility.RevealInFinder(tempString);
        }

        public void ClearMetrics()
        {
            string metricsFolder = GameEventsDefinitions.MetricsDirectory;
            if (!Directory.Exists(metricsFolder)) return;
            string[] jsonFiles = Directory.GetFiles(metricsFolder, "*.json");
            foreach (string file in jsonFiles) File.Delete(file);
            AssetDatabase.Refresh();
            EditorUtility.SetDirty(this);
        }
    }
}