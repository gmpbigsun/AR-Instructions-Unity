using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BoolGameEvent : ScriptableObject
{
    /// <summary>
    /// The list of listeners that this event will notify if it is raised.
    /// </summary>
    private readonly List<BoolGameEventListener> eventListeners = 
        new List<BoolGameEventListener>();

    public void Raise(bool boolean)
    {
        for(int i = eventListeners.Count -1; i >= 0; i--)
            eventListeners[i].OnEventRaised(boolean);
    }

    public void RegisterListener(BoolGameEventListener listener)
    {
        if (!eventListeners.Contains(listener))
            eventListeners.Add(listener);
    }

    public void UnregisterListener(BoolGameEventListener listener)
    {
        if (eventListeners.Contains(listener))
            eventListeners.Remove(listener);
    }
}
