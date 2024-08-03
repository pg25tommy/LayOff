// Created on Sun Jun 16 2024 || Copyright© 2024 || By: Alex Buzmion II
using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class KillFeedEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI instigatorText; 
    [SerializeField] private TextMeshProUGUI victimText; 
    [SerializeField] private TextMeshProUGUI trapTypeText; 
    private GameObject entry; 
    private Color instigatorTextColor; 
    private Color victimTextColor; 


    public void UpdateFeedDetails(int instigator, int victim, int trapType, GameObject feedEntry) {
        // Debug.Log("KillFeedEntry: UpdateFeedDetails() called");
        string instigatorName; 
        string victimName;
        entry = feedEntry; // store the gameobject feed entry to disable in the coroutine
        // set instigator text display details
        if ( instigator == PhotonNetwork.LocalPlayer.ActorNumber ) {
            instigatorName = "Your";
            instigatorText.text = $"{instigatorName}";
        }
        else {
            instigatorName = PhotonNetwork.CurrentRoom.Players[instigator].NickName;
            instigatorText.text = $"{instigatorName}'s";
        }
        instigatorTextColor = GetPlayerColor(instigator);
        // set victim text display details
        if (victim == PhotonNetwork.LocalPlayer.ActorNumber) {
            victimName = "You";
        }
        else {
            victimName = PhotonNetwork.CurrentRoom.Players[victim].NickName;
        }
        // get the victim's nickname
        victimTextColor = GetPlayerColor(victim);
        // get the trap type as a string
        string trapTypeName = GetTrapName(trapType);
        // update the text fields
        victimText.text = victimName;
        trapTypeText.text = trapTypeName;

        //update the text colors 
        instigatorText.color = instigatorTextColor;
        victimText.color = victimTextColor;
        Debug.Log($"{instigatorText.text} {victimText.text} {trapTypeText.text}");
        StartCoroutine(ClearDetailsOnDelay());
    }

    private string GetTrapName(int trapType) {
        switch (trapType) {
            case 1: 
                return "Poison Dart"; 
            case 2:
                return "Bouncing Betty";
            case 3:
                return "Skyfall Snare";
            case 4:
                return "Doom Puff";
            default: 
                return "Uknown Trap"; 
        }
    }

    private IEnumerator ClearDetailsOnDelay() {
        yield return new WaitForSeconds(3.5f);
        // update the text fields
        instigatorText.text = "";
        victimText.text = "";
        trapTypeText.text = "";
        entry.SetActive(false);
    }

    private Color GetPlayerColor(int actorNumber)
    {
        switch (actorNumber)
        {
            case 1:
                return Color.red;
            case 2:
                return Color.green;
            case 3:
                return Color.blue;
            case 4:
                return Color.yellow;
            default:
                return Color.black;
        }
    }
}
