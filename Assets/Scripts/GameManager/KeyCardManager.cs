// Created on Wednesday May 01 2024 || Copyright (c) 2024 Names Are Hard Studios || By: Tommy Minter /Alex Buzmion II / Tiago Corsato
using CharacterMovement;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeycardManager : MonoBehaviourPunCallbacks
{
    #region Private Fields
    [SerializeField] private float spawnTimeDelay = 60f;
    [SerializeField, BoxGroup("Interactable Objects")] private List<InteractableObject> spawnPoints = new List<InteractableObject>(); // List of spawn points for the keycards
    [SerializeField, BoxGroup("Objects with Keycards")] private List<InteractableObject> objWithKeycard;
    [SerializeField, BoxGroup("KeyCard Count HUD Updater")] private TextMeshProUGUI keycardCountText;
    [SerializeField, BoxGroup("KeyCard Count HUD Updater")] private Image keycardCountFill;
    [SerializeField, BoxGroup("KeyCard Count HUD Updater")] private Image playerAvatar;
    [SerializeField, BoxGroup("KeyCard Count HUD Updater")] private GameObject keycardCountHighlight;
    [SerializeField] GameObject keycardPrefab;
    #endregion

    #region Public Fields
    public int keycardsCollected = 0; // Tracks the number of keycards collected.
    public int totalKeycards = 3; // Total keycards to collect.
    [HideInInspector] public static KeycardManager Instance;
    #endregion

    #region Monobehavior Methods
    private void Awake()
    {
        Instance = this;
        spawnPoints = FindObjectsByType<InteractableObject>(FindObjectsSortMode.InstanceID).ToList(); //!important that the objects searched for are sorted by instanceID
        //Debug.Log($"Total spawn points: {spawnPoints.Count}");
        keycardCountText.color = GameManager.Instance.GetPlayerColor(PhotonNetwork.LocalPlayer.ActorNumber);
        playerAvatar.sprite = GameManager.Instance.GetPlayerImage(PhotonNetwork.LocalPlayer.ActorNumber);
    }

    public void SpawnKeycards()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (spawnPoints.Count > 0)
            {
                int randomIndex = Random.Range(0, spawnPoints.Count);
                photonView.RPC("RPC_SetKeycardObject", RpcTarget.AllBuffered, randomIndex);
            }
            else
            {
                // Debug.Log("No more spawnpoints available");
            }
        }
    }

    // receive data from a game event invoked 
    public void KeycardCollected(Component sender, object data)
    {
        if (sender is InventoryManager inventoryManager)
        {
            UpdateKeyCardCountHUD((int)data);
        }
        // todo fix removing the object from the list to prevent spawning in the same location again
        // use the sender data to remove the IO from the list of objWithKeycard 
        if (sender is InteractableObject ioCardTaken)
        {
            Debug.Log($"Keycard taken from {ioCardTaken.name}");
            objWithKeycard.Remove(ioCardTaken);
            spawnPoints.Add(ioCardTaken);
            Debug.Log($"Removed from list: {ioCardTaken.name}");
        }
    }

    public void UpdateKeyCardCountHUD(int keycardsCollected)
    {
        keycardCountText.text = $"{keycardsCollected}/3";
        keycardCountFill.fillAmount = (float)keycardsCollected / totalKeycards;
        StartCoroutine(HighlightIcon(keycardCountHighlight, 2f));
    }
    #endregion

    #region Coroutines
    IEnumerator HighlightIcon(GameObject icon, float time)
    {
        icon.SetActive(true);
        yield return new WaitForSeconds(time);
        icon.SetActive(false);
    }

    IEnumerator TimerToNextCardSpawn(float timeToSpawn)
    {
        yield return new WaitForSeconds(timeToSpawn);
        SpawnKeycards();
    }
    #endregion

    #region PunCallbacks 
    [PunRPC]
    // RPC Call to assign an interactable object by the master client only and synchronize it across all clients
    public void RPC_SetKeycardObject(int randomIndex)
    {
        if (spawnPoints != null && randomIndex >= 0 && randomIndex < spawnPoints.Count)
        {
            InteractableObject selectedObject = spawnPoints[randomIndex];
            objWithKeycard.Add(selectedObject);
            spawnPoints.Remove(selectedObject);
            selectedObject.hasKeyCard = true;
            // gets the room the interactable object is in
            RoomBehavior room = selectedObject.GetRoomHighlighter();
            if (room != null)
            { // if room is not null cast the RPC for the room to start blinking
                // Debug.Log($"Room found {room.name}");
                room.photonView.RPC("RPC_StartBlinkingHighlighter", RpcTarget.All, 5f);
            } 
            if (MinimapFollowCam.Instance !=null) {
               MinimapFollowCam.Instance.ZoomOut();
            }
            HUDManager.Instance.NotifyHUD(); 
        }
        StartCoroutine(TimerToNextCardSpawn(spawnTimeDelay));
    }

    [PunRPC]
    private void RPC_DropKeycard(Vector3 position)
    {
        // Debug.Log("RPC_DropKeycard called");
        PhotonNetwork.InstantiateRoomObject(keycardPrefab.name, position, Quaternion.identity);

        InventoryManager.Instance.CheckKeyCardReq();
    }

    public void DropKeyCard(Vector3 position)
    {

        photonView.RPC(nameof(RPC_DropKeycard), RpcTarget.MasterClient, position);
    }
    #endregion
}