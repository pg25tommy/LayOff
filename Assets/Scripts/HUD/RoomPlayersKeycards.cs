// Created on Fri Jun 21 2024 || Copyright© 2024 || By: Alex Buzmion II
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class RoomPlayersKeycards : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerEntryPrefab; 
    [SerializeField] private GameObject statsPanel; 
    private Dictionary<int, GameObject> playerEntries;

    public static RoomPlayersKeycards Instance; 

    private void Awake() {
        playerEntries = new Dictionary<int, GameObject>();
        Instance = this; 
    }

    private void Start() {
        AddAllEntries(); 
    }

    private void AddAllEntries() {
        foreach (Player player in PhotonNetwork.PlayerList) {
            if (player.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber) {
                AddEntry(player);
            }
        }
    }

    public void AddEntry(Player player) {
        if (playerEntries.ContainsKey(player.ActorNumber)) return;

        Color pColor = GameManager.Instance.GetPlayerColor(player.ActorNumber);
        GameObject entry = Instantiate(playerEntryPrefab);
        entry.transform.SetParent(statsPanel.transform);

        PlayerStatsEntry playerEntry = entry.GetComponent<PlayerStatsEntry>();

        player.CustomProperties.TryGetValue("keycardsCollected", out object value);
        playerEntry.keycardCount.text = value != null ? $"{value}/3" : "0/3";
        playerEntry.keycardCount.color = pColor;
        playerEntry.playerColorImage.sprite = GameManager.Instance.GetPlayerImage(player.ActorNumber);

        playerEntries.Add(player.ActorNumber, entry);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        if (playerEntries.TryGetValue(otherPlayer.ActorNumber, out GameObject entry)) {
            Destroy(entry);
            playerEntries.Remove(otherPlayer.ActorNumber);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        if (newPlayer.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber) {
            AddEntry(newPlayer);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) {
        if (!changedProps.ContainsKey("keycardsCollected")) return;
        if (playerEntries.TryGetValue(targetPlayer.ActorNumber, out GameObject entry)) {
            TextMeshProUGUI pKeyCards = entry.GetComponent<PlayerStatsEntry>().keycardCount;
            targetPlayer.CustomProperties.TryGetValue("keycardsCollected", out object value);
            pKeyCards.text = value != null ? $"{value}/3" : "0/3";
        }
    }
}
