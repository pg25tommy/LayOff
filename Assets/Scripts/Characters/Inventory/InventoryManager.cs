//Copyright (c) 2024 Names Are Hard, All Rights Reserved | Created by: Tommmy Minter, Alex Buzmion II
using FMODUnity;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

// Define the InventoryManager class
public class InventoryManager : MonoBehaviourPunCallbacks
{
    #region Private Fields
    private PlayerStatsManager playerStatsManager;
    [SerializeField] private EventReference _allKeycardsCollectedVA;
    #endregion

    #region Public Fields
    [SerializeField] public bool hasAllReqKeycards = false; // Bool to track if all 3 keycards have been collected
    [SerializeField] public int keycardsRequired = 3; // Number of keycards required to open the door
    
    public int keycardsCollected;

    public GameObject keycardPing;
    [BoxGroup("Player Ping Settings"), SerializeField] private float pingDuration = 5f;
    [BoxGroup("Player Ping Settings"), SerializeField] private float pingCooldown = 10f;
    [BoxGroup("Player Ping Settings")] public Material yellowPing;
    [BoxGroup("Player Ping Settings")] public Material orangePing;
    [BoxGroup("Player Ping Settings")] public Material redPing;

    public GameObject playerNav;

    public static InventoryManager Instance;

    #endregion

    private void Awake()
    {
        Instance = this;

        keycardPing.GetComponent<MeshRenderer>().enabled = false;

    }

    #region Monobehavior Callbacks
    void Start()
    {
        playerStatsManager = GetComponentInParent<PlayerStatsManager>();
        // TextRenderer.TRInstance.UpdateText($"found playerstatsManager {playerStatsManager}");
        keycardsCollected = playerStatsManager.GetCustomProperty(PlayerStatsManager.KeycardsCollected);
    }


    [Button("Drop KeyCard")] // for testing purposes
                             // todo call this on death
    public void DropKeyCard()
    {
        //if (keycardsCollected == 0) return;
        // instantiate a keycard 
        // photonView.RPC(nameof(RPC_Ping), RpcTarget.AllBuffered);
    }

    public void CheckKeyCardReq()
    {
        // Check if the player has collected all keycards required
        if (playerStatsManager.GetCustomProperty(PlayerStatsManager.KeycardsCollected) >= keycardsRequired)
        {
            hasAllReqKeycards = true; // set to true if has all required keycards are collected
            // TextRenderer.TRInstance.UpdateText("All Keycards Collected! Head to the laser rooms!");
            MinimapFollowCam.Instance.ZoomOut();
            if (photonView.IsMine) {
                SwitchLasersOnAllDoors(1);
                GameManager.Instance.finalRoomHighlighter.GetComponent<RoomBehavior>().RPC_StartBlinkingHighlighter(5f);
                RuntimeManager.PlayOneShot(_allKeycardsCollectedVA);
            }
            return;
        }
        hasAllReqKeycards = false; // set to false if has all required keycards are collected
        if (photonView.IsMine) {
            SwitchLasersOnAllDoors(0);
            GameManager.Instance.finalRoomHighlighter.GetComponent<RoomBehavior>().TurnOffHighlighter();
        }
    }

    private void SwitchLasersOnAllDoors(int mode)
    {
        FinalDoorBehavior[] doors = FindObjectsOfType<FinalDoorBehavior>();
        foreach (FinalDoorBehavior door in doors)
        {
            door.SwitchLaser(mode);
        }
    }

    #endregion

    #region PunCallbacks
    // [PunRPC]
    // private void RPC_Ping()
    // {
    //     Debug.Log("RPC_Ping is called");
    //     StartCoroutine(Ping(pingDuration));
    // }

    #endregion
}
