// Created on Sun May 05 2024 || Copyright© 2024 || By: Alex Buzmion II
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CustomGameEvent : UnityEvent<Component, object> {}

public class LayOffGameEventListener : MonoBehaviour
{

    [Tooltip("Event to register with.")]
    public LayOffGameEvent gameEvent;

    [Tooltip("Response to invoke when Event with GameData is raised.")]
    public CustomGameEvent response;

    private void OnEnable() {
        gameEvent.RegisterListener(this);
    }

    private void OnDisable() {
        gameEvent.UnregisterListener(this);
    }

    public void OnEventRaised(Component sender, object data) {
        response.Invoke(sender, data);
    }

}
