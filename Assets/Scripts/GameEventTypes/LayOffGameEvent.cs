// Created on Sun May 05 2024 || Copyright© 2024 || By: Alex Buzmion II
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="LayOff Game Event")]
public class LayOffGameEvent : ScriptableObject
{
    
    public List<LayOffGameEventListener> listeners = new List<LayOffGameEventListener>();

    // Raise event through different method signatures
    // ############################################################

    public void Raise() {
        Raise(null, null);
    }

    public void Raise(object data) {
        Raise(null, data);
    }

    public void Raise(Component sender) {
        Raise(sender, null);
    }

    public void Raise(Component sender, object data) {
        for (int i = listeners.Count -1; i >= 0; i--) {
            listeners[i].OnEventRaised(sender, data);
        }
    }

    // Manage Listeners

    public void RegisterListener(LayOffGameEventListener listener) {
        if (!listeners.Contains(listener))
            listeners.Add(listener);
    }

    public void UnregisterListener(LayOffGameEventListener listener) {
        if (listeners.Contains(listener))
            listeners.Remove(listener);
    }

}
