// Created on Sun Jun 30 2024 || Copyright© 2024 || By: Alex Buzmion II
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PlayerRoomEntryList : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerEntryPrefab;
    [SerializeField] private GameObject insideRoomParent;
    [SerializeField] public Dictionary<int, GameObject> playerEntryList;
    [SerializeField] public TextMeshProUGUI roomNameText;
    public static PlayerRoomEntryList Instance;

    #region MonobehaviorCallbacks
    private void Awake() {
        Instance = this;
    }

    public void AddAllEntries()
    {
        if (playerEntryList == null) {
            playerEntryList = new Dictionary<int, GameObject>();
        }

        foreach (Player player in PhotonNetwork.PlayerList) {
            AddEntry(player);
        }
    }

    public void AddEntry(Player player) {
        GameObject entry = Instantiate(playerEntryPrefab);
        entry.transform.SetParent(insideRoomParent.transform);
        entry.TryGetComponent<PlayerRoomEntry>(out PlayerRoomEntry playerEntry);
        if (playerEntry != null) {
            playerEntry.SetPlayerNameAndImage(player.ActorNumber);
            if (player.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber) {
                playerEntry.readyButton.interactable = false; 
            }
        }
        else {
            Debug.Log("No PlayerRoomEntry found");
        }
        playerEntryList.Add(player.ActorNumber, entry);
    }
    #endregion

    #region PunCallbacks

    public override void OnMasterClientSwitched(Player newMasterClient) {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber) {
            
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {
        GameObject entry;
        if (playerEntryList.TryGetValue(targetPlayer.ActorNumber, out entry)) {
            object isPlayerReady; 
            if (changedProps.TryGetValue("isReady", out isPlayerReady)) {
                entry.GetComponent<PlayerRoomEntry>().SetPlayerReady((bool)isPlayerReady);
                Debug.Log($"{targetPlayer.ActorNumber} isReady status from OnPlayerPropertiesUpdated is: {isPlayerReady}");
            }
        }
        if (CheckPlayersReady()) {
            PunLauncher.Instance.StartMatch();
            DisableReadyButtons();
            Debug.Log("StartMatch called");
        }
    }

    private void DisableReadyButtons() {
        foreach (KeyValuePair<int, GameObject> entry in playerEntryList) {
            int playerId = entry.Key;
            GameObject playerEntry = entry.Value;
            playerEntry.GetComponent<PlayerRoomEntry>().readyButton.interactable = false;
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer) {
        Debug.Log("New Player Entered");
        AddEntry(newPlayer);
        Debug.Log("Calling AddEntry()");
    }

    public override void OnLeftRoom() {
        foreach (GameObject entry in playerEntryList.Values)  {
            Destroy(entry.gameObject);
        }
        playerEntryList.Clear();
        playerEntryList = null;
    }
    public override void OnPlayerLeftRoom(Player otherPlayer) {
        if (playerEntryList.TryGetValue(otherPlayer.ActorNumber, out GameObject entry)) {
            Destroy(entry);
            playerEntryList.Remove(otherPlayer.ActorNumber);
        }
    }

    private bool CheckPlayersReady() {
        if (!PhotonNetwork.IsMasterClient) return false; 
        if (PhotonNetwork.PlayerList.Length != PhotonNetwork.CurrentRoom.MaxPlayers) return false;
        foreach (Player p in PhotonNetwork.PlayerList) {
            object isPlayerReady; 
            if (p.CustomProperties.TryGetValue("isReady", out isPlayerReady)) {
                if (!(bool)isPlayerReady) return false;
            }
            else return false; 
            Debug.Log($"Player is Ready: {isPlayerReady}");
        }
        Debug.Log("Checked all Players, returning True");
        return true;
    }
    #endregion
}