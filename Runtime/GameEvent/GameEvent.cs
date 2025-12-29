
using System.Collections.Generic;

using UnityEngine;
using System;
using System.IO;


#if UNITY_EDITOR
using Codice.Client.Common;
using UnityEditor;
using NoCtrl.GameEvents;
#endif

[CreateAssetMenu]
public class GameEvent : ScriptableObject
{
    #region EditorMetric

#if UNITY_EDITOR

    [Serializable]
    public struct RaiseMetric
    {
        [SerializeField] private string m_timeStamp; // ISO 8601 format
        [SerializeField] private int m_numberListeners;
        [SerializeField] private List<string> m_listenerNameList;

        public RaiseMetric(List<string> listenersNameList)
        {
            m_timeStamp = System.DateTime.UtcNow.ToString("o"); // ISO 8601
            m_numberListeners = listenersNameList.Count;
m_listenerNameList = listenersNameList;
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this, prettyPrint: true);
        }

        public static RaiseMetric FromJson(string json)
        {
            return JsonUtility.FromJson<RaiseMetric>(json);
        }
    }
    
    // Container so JsonUtility can serialize a list of metrics
    [Serializable]
    private class RaiseMetricCollection
    {
        public List<RaiseMetric> metrics;
    }

    private List<RaiseMetric> m_raiseMetrics = new List<RaiseMetric>();

#endif
    #endregion

    [SerializeField] private bool DebugLogging;

    public List<GameEventListener> Listeners => m_listeners;
    private List<GameEventListener> m_listeners = new List<GameEventListener>();

    // Raise Loops backward in case the event includes removing the response
    // this should account for and avoid the out of bounds exception.
    public void RaiseAll()
    {
        List<string> listenerList = new List<string>(m_listeners.Count);
        for (int i = m_listeners.Count - 1; i >= 0; --i)
        {
            listenerList.Add(m_listeners[i].ListenerName);
            m_listeners[i].OnEventRaised();
        }

#if UNITY_EDITOR

        m_raiseMetrics.Add(new RaiseMetric(listenerList));
        if (GameEventEditorRegistry.Instance.DebugConsoleLogging == true)
        Debug.Log("Event Metric added " + m_raiseMetrics.Count + " Metrics...");

#endif
        if (DebugLogging)
            Debug.Log("Event Raised! Notifying " + m_listeners.Count + " Listeners...");
    }

    public void RaiseSpecific(GameEventListener listener)
    {
        if (m_listeners.Contains(listener))
        {
            listener.OnEventRaised();

            if (DebugLogging)
                Debug.Log("Event Raised! Notifying Listener " + listener.ListenerName);
        }
    }

    public void RegisterListener(GameEventListener listener)
    {
        if (!m_listeners.Contains(listener))
        {
            m_listeners.Add(listener);
        }
    }

    public void UnregisterListener(GameEventListener listener)
    {
        if (m_listeners.Contains(listener))
        {
            m_listeners.Remove(listener);
        }
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        NoCtrl.GameEvents.GameEventEditorRegistry.Instance.Register(this);
#endif
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        NoCtrl.GameEvents.GameEventEditorRegistry.Instance.Unregister(this);
#endif
    }

    public void SerializeMetricData(string sessionFolder)
    {
#if UNITY_EDITOR
        // Dump all metrics to json and out to (Assets/com.noctrl.game-events/Editor/GameEventMetrics/<eventName>_<timestamp>.json)
        try
        {
            if (m_raiseMetrics.Count > 0)
            {
                string metricsDir = GameEventsDefinitions.MetricsDirectory + "/" +sessionFolder;
                if (!Directory.Exists(metricsDir))
                    Directory.CreateDirectory(metricsDir);

                var collection = new RaiseMetricCollection { metrics = m_raiseMetrics };
                string json = JsonUtility.ToJson(collection, prettyPrint: true);
                string fileName = $"{name}_{DateTime.UtcNow:yyyy-MM-ddTHH-mm-ss.fff}.json";
                string filePath = Path.Combine(metricsDir, fileName);

                File.WriteAllText(filePath, json);
                // Refresh the AssetDatabase so the new file appears in the Project view
                AssetDatabase.Refresh();

                Debug.Log($"[GameEvent] Metrics written to: {filePath}");

                // Clear the metrics for the session
                ClearMetricData();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[GameEvent] Failed to write metrics: {ex}");
        }


#endif
    }

    public List<RaiseMetric> GetMetrics()
    {
        return m_raiseMetrics;
    }

    public void ClearMetricData()
    {
        // Clear the metrics for the session
        m_raiseMetrics.Clear();
    }
}
