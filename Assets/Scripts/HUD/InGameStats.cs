// Created on Wed Jun 12 2024 || Copyright© 2024 || By: Alex Buzmion II
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameStats : MonoBehaviourPunCallbacks
{
    public static InGameStats Instance;
    private Player[] playerList;
    private GameObject playerEntryPrefab; 
    private int playerCount = 0;

    [SerializeField] public TextMeshProUGUI roomName; 
    [SerializeField] public TextMeshProUGUI playerID; 
    [SerializeField] public TextMeshProUGUI keycardCount; 
    [SerializeField] public TextMeshProUGUI deathCount;
    [SerializeField] public TextMeshProUGUI killsCount;
    [SerializeField] public TextMeshProUGUI trapsCount;
    [SerializeField] public Image playerColorImage;

    private void Awake() {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    private void Start() {
        playerID.text = $"{PhotonNetwork.LocalPlayer.NickName}";
        playerID.color = GetPlayerColor(PhotonNetwork.LocalPlayer.ActorNumber);
        playerColorImage.color = GetPlayerColor(PhotonNetwork.LocalPlayer.ActorNumber);
        roomName.text = PhotonNetwork.CurrentRoom.Name;
    }

    public void UpdatePlayerList() {
        if (playerList != null && playerList.Length != playerCount) {
            foreach (Player player in playerList) {
                
            }
        }
    }

    public void Initialize() {
        
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
