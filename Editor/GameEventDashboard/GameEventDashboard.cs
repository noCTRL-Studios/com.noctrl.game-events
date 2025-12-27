using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

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