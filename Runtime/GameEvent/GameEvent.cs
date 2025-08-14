
using System.Collections.Generic;
using UnityEngine;

//TODO: Finish features as needed

[CreateAssetMenu]
public class GameEvent : ScriptableObject
{
    public List<GameEventListener> Listeners => m_listeners;

    [SerializeField] private bool DebugLogging;

    private List<GameEventListener> m_listeners = new List<GameEventListener>();

    // Raise Loops backward in case the event includes removing the response
    // this should account for and avoid the out of bounds exception.
    public void RaiseAll()
    {
        for (int i = m_listeners.Count - 1; i >= 0; --i)
            m_listeners[i].OnEventRaised();

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
}
