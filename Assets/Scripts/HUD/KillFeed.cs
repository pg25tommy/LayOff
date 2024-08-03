// Created on Sun Jun 16 2024 || Copyright© 2024 || By: Alex Buzmion II
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class KillFeed : MonoBehaviourPunCallbacks
{
    [SerializeField] private List<GameObject> feedEntries; 
    public static KillFeed Instance; 

    private void Awake() {
        Instance = this;
    }

    public void UpdateFeed(int instigator, int victim, int trapType) { 
        // Debug.Log("KillFeed: UpdateFeed() called and ran by master client");
        GameObject entry = null; // container to cache the unused/disabled kill feed entry
        foreach (GameObject feedEntry in feedEntries) {
            if (feedEntry.activeSelf != true) { // find the first entry that is not visible in the UI
                entry = feedEntry;
                // Debug.Log($"{entry.name} found");
                entry.SetActive(true); // turn on the feed entry
                // Debug.Log($"entry found visibility {entry.activeSelf}");
                entry.GetComponent<KillFeedEntry>().UpdateFeedDetails(instigator, victim, trapType, entry); // update the details
                break;
            }   
            Debug.Log($"{entry} null");
        }
    }

}
