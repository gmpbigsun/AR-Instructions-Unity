using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MaterialGameEvent : ScriptableObject
{
    /// <summary>
    /// The list of listeners that this event will notify if it is raised.
    /// </summary>
    private readonly List<MaterialGameEventListener> eventListeners = 
        new List<MaterialGameEventListener>();

    public void Raise(Material material)
    {
        for(int i = eventListeners.Count -1; i >= 0; i--)
            eventListeners[i].OnEventRaised(material);
    }

    public void RegisterListener(MaterialGameEventListener listener)
    {
        if (!eventListeners.Contains(listener))
            eventListeners.Add(listener);
    }

    public void UnregisterListener(MaterialGameEventListener listener)
    {
        if (eventListeners.Contains(listener))
            eventListeners.Remove(listener);
    }
}
