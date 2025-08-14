
using UnityEngine;
using UnityEngine.Events;

//TODO: Finish features as needed

public class GameEventListener : MonoBehaviour
{
    public string ListenerName; // For readability and debugging
    public GameEvent Event;
    public UnityEvent Response;

    public void OnEventRaised()
    {
        Response.Invoke();
    }

    private void OnEnable()
    {
        if (Event != null)
        {
            Event.RegisterListener(this);
        }
    }

    private void OnDisable()
    {
        if (Event != null)
        {
            Event.UnregisterListener(this);
        }
    }
}
