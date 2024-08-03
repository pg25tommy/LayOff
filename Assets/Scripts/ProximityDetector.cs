// Created on Fri Jun 28 2024 || Copyright© 2024 || By: Alex Buzmion II
using UnityEngine;

public class ProximityDetector : MonoBehaviour
{
    Collider triggerVolume;
    
    private void Awake() {
        triggerVolume = GetComponent<Collider>();
    }

    
}
