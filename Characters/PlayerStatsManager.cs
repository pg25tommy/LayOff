// Created on Sun May 26 2024 || Copyright© 2024 || By: Alex Buzmion II
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;
using System.Collections.Generic;

public class PlayerStatsManager : MonoBehaviourPunCallbacks
{
    #region Private Fields
    private bool trackFirst = true;
    private bool trackSecond = true;
    private bool trackThird = true;
    private InventoryManager inventoryManager;
    //todo utilize this for easier iteration of all players in the room and all the custom properties 
    Player[] players = PhotonNetwork.PlayerList;
    private Player owner;
    public readonly string[] propertyKeys = {
            "keycardsCollected", "trapsSet", "trap1", "trap2", "trap3", "trap4",
            "kills", "trap1kill", "trap2kill", "trap3kill", "trap4kill",
            "deaths", "killedbyp1", "killedbyp2", "killedbyp3", "killedbyp4"
        };
    private string[] playerNames = { "Redd", "Lime", "Navy", "Gold" };

    #endregion

    #region Public Fields
    public const string KeycardsCollected = "keycardsCollected";
    public const string TrapsSet = "trapsSet"; // todo convert this to a hashtable that breaks out traptype and quantity set
    public const string Trap1 = "trap1";
    public const string Trap2 = "trap2";
    public const string Trap3 = "trap3";
    public const string Trap4 = "trap4";
    public const string Kills = "kills"; // todo convert this to a hashtable that breaks out kills by traptype
    public const string Trap1Kill = "trap1kill";
    public const string Trap2Kill = "trap2kill";
    public const string Trap3Kill = "trap3kill";
    public const string Trap4Kill = "trap4kill";
    public const string Deaths = "deaths"; // todo convert this to a hashtable that breaks out deaths by traptype and by who
    public const string KilledByP1 = "killedbyp1";
    public const string KilledByP2 = "killedbyp2";
    public const string KilledByP3 = "killedbyp3";
    public const string KilledByP4 = "killedbyp4";
    #endregion

    #region MonoBehavior Callbacks
    void Awake()
    {
        inventoryManager = GetComponentInChildren<InventoryManager>();
    }

    void Start()
    {
        InitializeCustomProperties(); // pun callback to initialize custom properties of a local player
        ResetCustomProperties();
        if (CloudSave.Instance != null) {
        CloudSave.Instance.playerStats.player = PhotonNetwork.LocalPlayer.ActorNumber;
        }
    }
    #endregion

    #region PunCallbacks
    private void InitializeCustomProperties()
    {
        foreach (string propKey in propertyKeys)
        {
            if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(propKey))
            {
                SetCustomProperty(propKey, 0);
            }
        }
        photonView.Owner.NickName = playerNames[photonView.Owner.ActorNumber - 1];
        // Debug.Log($" This clients' nickname is {photonView.Owner.NickName}");
        if (CloudSave.Instance != null) {
        CloudSave.Instance.playerStats.roomID = PhotonNetwork.CurrentRoom.Name;
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        if (otherPlayer.IsLocal)
        {
            ResetCustomProperties();
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        //Debug.Log($"OnPlayerPropertiesUpdate called by {targetPlayer.NickName} updated value: {changedProps}");
        if (targetPlayer.IsLocal)
        {
            // LogStats();
            foreach (string propKey in propertyKeys)
            {
                if (changedProps.ContainsKey(propKey))
                {
                    int newValue = (int)changedProps[propKey];
                    // handles specific UI updates, add more cases as stats are needed
                    switch (propKey)
                    {
                        case "keycardsCollected":
                            KeycardManager.Instance.UpdateKeyCardCountHUD(newValue);
                            inventoryManager.keycardsCollected = newValue;
                            inventoryManager.CheckKeyCardReq();
                            // InGameStats.Instance.keycardCount.text = newValue.ToString();
                            if (CloudSave.Instance != null) {
                            CloudSave.Instance.playerStats.KeycardsCollected = GetCustomProperty(propKey);
                            }
                            break;
                        case "deaths":
                            UIEndGame.Instance.deaths.text = newValue.ToString();
                            // InGameStats.Instance.deathCount.text = newValue.ToString();
                            break;
                        case "kills":
                            UIEndGame.Instance.kills.text = newValue.ToString();
                            // InGameStats.Instance.killsCount.text = newValue.ToString();
                            break;
                        case "trapsSet":
                            UIEndGame.Instance.trapsSet.text = newValue.ToString();
                            // InGameStats.Instance.trapsCount.text = newValue.ToString();
                            break;
                        default: // for everything else, skip
                            break;
                    }
                }
            }
        }
    }
    #endregion

    #region Getters & Setters
    public int GetCustomProperty(string propertyKey)
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(propertyKey, out object value))
        {
            return (int)value;
        }
        return 0;
    }
    // Setting custom property without expected value parameter
    public void SetCustomProperty(string propertyKey, int value)
    {
        Hashtable propsToSet = new Hashtable { { propertyKey, value } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(propsToSet);
        // log to ensure properties are being set
        //Debug.Log($"Set property {propertyKey} to {value}");
    }
    // overload to set custom property with expected value parameter
    public void SetCustomProperty(string propertyKey, int newValue, int expectedValue)
    {
        Hashtable propsToSet = new Hashtable { { propertyKey, newValue } };
        Hashtable expectedProps = new Hashtable { { propertyKey, expectedValue } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(propsToSet, expectedProps);
    }

    #endregion

    public void AddKeyCardCount()
    {
        int keycards = GetCustomProperty(PlayerStatsManager.KeycardsCollected) + 1;
        SetCustomProperty(PlayerStatsManager.KeycardsCollected, keycards);
    }

    // resets the properties 
    public void ResetCustomProperties()
    {
        // Debug.Log("ResetCustomProperties is called on Start");
        foreach (string propKey in propertyKeys)
        {
            SetCustomProperty(propKey, 0);
        }
    }

    public void LogStats()
    {
        foreach (string propKey in propertyKeys)
        {
            Debug.Log($"{propKey}, {GetCustomProperty(propKey)}");
        }
    }
    public void UpdateKills(int trapType, int ownerID, int[] victims)
    {
        // Debug.Log($"UpdateKills called with trapType: {trapType}, ownerID: {ownerID}, victims count: {victims.Length}");
        if (PhotonNetwork.LocalPlayer.ActorNumber != ownerID)
        {
            // Debug.Log($"Owner mismatch: photonView.OwnerActorNr = {PhotonNetwork.LocalPlayer.ActorNumber}, ownerID = {ownerID}");
            return; // Check if the trap ownerID matches this PlayerStatsManager owner
        }
        #region CloudSave data
        if (CloudSave.Instance.playerStats.Kills <= 0)
        {
            CloudSave.Instance.playerStats.timeOfFirstKill = Time.timeSinceLevelLoad;
        }
        CloudSave.Instance.playerStats.Kills += 1;
        CloudSave.Instance.playerStats.KillsByTrap[$"Player{victims[0]}"][$"Trap{trapType}"] += 1;
        #endregion
        int currentKillVal = GetCustomProperty($"trap{trapType}Kill");
        int updatedKillVal = currentKillVal + victims.Length;
        string propToUpdate = $"trap{trapType}Kill";
        SetCustomProperty(propToUpdate, updatedKillVal); // update the specific trap type kills

        //update total kills
        int currentTotalKills = GetCustomProperty(Kills);
        int updatedTotalKills = currentTotalKills + victims.Length;
        SetCustomProperty(Kills, updatedTotalKills);

        // Send RPC to master client to update kill feed
        photonView.RPC(nameof(RPC_UpdateKillFeed), RpcTarget.All, ownerID, victims, trapType);
    }

    // generic stat updater if stats only need to be increased by 1
    public void UpdateStat(string propToUpdate, string operation)
    {
        int currentStatVal = GetCustomProperty(propToUpdate);
        int updatedStatVal;

        switch (operation)
        { // future cases can include multiply / divide / modulo 
            case "subtract":
                updatedStatVal = currentStatVal - 1;
                break;
            case "add":
                updatedStatVal = currentStatVal + 1;
                if (propToUpdate == "keycardsCollected") {
                    #region CloudSave data
                        if (CloudSave.Instance.playerStats.KeycardsCollected == 0 && trackFirst && Time.timeSinceLevelLoad > 1) {
                            CloudSave.Instance.playerStats.timeFirstKeyCollected = Time.timeSinceLevelLoad;
                            trackFirst = false;
                        }
                        if (CloudSave.Instance.playerStats.KeycardsCollected == 1 && trackSecond) {
                            CloudSave.Instance.playerStats.timeSecondKeyCollected = Time.timeSinceLevelLoad;
                            trackSecond = false;
                        }
                        if (CloudSave.Instance.playerStats.KeycardsCollected == 2 && trackThird) {
                            CloudSave.Instance.playerStats.timeThirdKeyCollected = Time.timeSinceLevelLoad;
                            trackThird = false;
                        }
                    #endregion
                }
                break;
            default:
                updatedStatVal = currentStatVal + 1;
                break;
        }
        SetCustomProperty(propToUpdate, updatedStatVal);
    }

    public void UpdateDeaths(int playerNr, int trapType)
    {
        int currentDeathVal;
        int updatedDeathVal;
        if (playerNr == 0)
        {
            string cause;
            switch (trapType)
            {
                case 0:
                    cause = "FinalDoors";
                    break;
                case 1:
                    cause = "RoomKills";
                    break;
                case 2:
                    cause = "KillBox";
                    break;
                default:
                    cause = "GMKills";
                    break;
            }
            CloudSave.Instance.playerStats.DeathsByCause[$"Environment"][cause] += 1;
            return;
        }
        currentDeathVal = GetCustomProperty($"killedbyp{playerNr}");
        updatedDeathVal = currentDeathVal + 1;
        SetCustomProperty($"killedbyp{playerNr}", updatedDeathVal);

        currentDeathVal = GetCustomProperty(Deaths);
        updatedDeathVal = currentDeathVal + 1;
        SetCustomProperty(Deaths, updatedDeathVal);

        #region CloudSave data
        if (CloudSave.Instance.playerStats.Deaths <= 0)
        {
            CloudSave.Instance.playerStats.timeOfFirstDeath = Time.timeSinceLevelLoad; ;
        }
        CloudSave.Instance.playerStats.DeathsByCause[$"Player{playerNr}"][$"Trap{trapType}"] += 1;
        CloudSave.Instance.playerStats.Deaths += 1;
        #endregion
    }
    [PunRPC]
    public void RPC_UpdateKills(int trapType, int ownerID, int[] victimIDs)
    {
        UpdateKills(trapType, ownerID, victimIDs);
    }

    [PunRPC]
    private void RPC_UpdateKillFeed(int ownerID, int[] victims, int trapType)
    {
        foreach (var victimID in victims)
        {
            KillFeed.Instance.UpdateFeed(ownerID, victimID, trapType);
        }
    }
}
