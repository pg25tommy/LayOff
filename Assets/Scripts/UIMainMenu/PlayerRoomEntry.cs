// Created on Sun Jun 30 2024 || Copyright© 2024 || By: Alex Buzmion II
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRoomEntry : MonoBehaviourPunCallbacks
{
    [SerializeField] public TextMeshProUGUI playerName;
    [SerializeField] public Image playerAvatar;
    [SerializeField] public Image readyIcon;
    [SerializeField] public TextMeshProUGUI readyText;
    [SerializeField] public Button readyButton;
    [SerializeField] List<Color> colors = new List<Color>();
    [SerializeField] private ColorBlock colorBlock;

    private const string readyState = "isReady";
    public bool IsReady = false; // set to true but will be updated false in SetPlayerReady()
    private int ownerID;
    [SerializeField] private List<Sprite> playerAvatars;

    private void Awake() {
        IsReady = false; 
        colorBlock = readyButton.colors;
        if (!PhotonNetwork.LocalPlayer.IsLocal){
            readyButton.interactable = false;
        }
        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(readyState)) {
            Hashtable propToSet = new Hashtable { { readyState, IsReady } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(propToSet);
            return;
        }
        SetCustomProperty("isReady", IsReady); 
    }

    // hooked to the ready button
    public void ReadyButtonClicked() {
        IsReady = !IsReady;
        // set custom property isReady; 
        SetCustomProperty("isReady", IsReady); 
    }

    public void SetPlayerNameAndImage(int actorNumber)
    {
        switch (actorNumber)
        {
            case 1:
                playerName.text = "Elise";
                break;
            case 2:
                playerName.text = "Hazel";
                break;
            case 3:
                playerName.text = "Sofia";
                break;
            case 4:
                playerName.text = "Amelia";
                break;
            default:
                playerName.text = "NaN";
                break;
        }
        playerAvatar.sprite = playerAvatars[actorNumber - 1];
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        if (otherPlayer.IsLocal) {
            OnDisable();
        }
    }

    public override void OnDisable() {
        base.OnDisable();
        Destroy(this);
    }

    public void SetCustomProperty(string propertyKey, bool value)
    {
        Hashtable propsToSet = new Hashtable { { propertyKey, value } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(propsToSet);
        // log to ensure properties are being set
        Debug.Log($"Set property {propertyKey} to {value}");
    }

    public void SetPlayerReady(bool playerReady) {
        Debug.Log($"readyIcon setActive({playerReady})");
        readyIcon.gameObject.SetActive(playerReady);
        readyText.text = playerReady ? "Ready!" : "Ready?";
        colorBlock.normalColor = playerReady ? colors[0] : colors[1];
        Debug.Log($"IsReady {playerReady}");
    }
}