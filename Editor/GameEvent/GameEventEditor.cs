using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameEvent))]
public class GameEventEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GameEvent gameEvent = (GameEvent)target;

        // Button to Raise all test event
        if (GUILayout.Button("Debug Test : Raise All Event"))
        {
            gameEvent.RaiseAll();
        }

        // Lists out the Listeners for this event and allows for testing their invoked response
        for (int i = 0; i < gameEvent.Listeners.Count; ++i)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Listener " + i, gameEvent.Listeners[i].ListenerName);

            if (GUILayout.Button("Debug Test : Raise", GUILayout.Width(160)))
            {
                gameEvent.Listeners[i].OnEventRaised();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
