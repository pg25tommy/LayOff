// Created on Sun Apr 07 2024 || Copyright© 2024 || By: Alex Buzmion II
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(PhotonView))]
public class RoomBehavior : MonoBehaviourPun
{
    #region Private Fields
    [SerializeField] public List<CustomPlayerController> _playerCountInRoom;
    [SerializeField] private List<InteractableObject> interactableObjects = new List<InteractableObject>();
    [SerializeField] private Collider[] roomColliders; // array to hold multiple colliders that makes up the room
    [SerializeField] private GameObject roomHighlighter;
    [SerializeField] private float _timeToGas = 20;
    private Coroutine _countdownCoroutine;
    private Coroutine _blinkCoroutine;
    public float roomHighlightDuration = 5f;
    [SerializeField] private bool isHallway = false;
    // [SerializeField] private Camera minimapCam;
    [SerializeField] GameObject minimapIcon;
    [SerializeField] public ReplayCam roomCam;
    #endregion

    #region Monobehavior Callbacks
    void Awake()
    {
        if (roomHighlighter != null) {
        roomHighlighter.SetActive(false);
        }
        if (roomColliders.Length == 0)
        {
            roomColliders = GetComponents<Collider>();
        }
        // find and move interactable objects within the room bounds to be a child of the room object
        //MoveInteractableObjectsToRoom();
        // minimapCam = FindAnyObjectByType<MinimapFollowCam>().GetComponent<Camera>();
    }

    private void DetectInteractableObjects()
    {
        foreach (Collider roomCollider in roomColliders) {
            Collider[] colliders = Physics.OverlapBox(roomCollider.bounds.center, roomCollider.bounds.extents, roomCollider.transform.rotation);
            foreach (Collider collider in colliders) {
                InteractableObject interactableObject = collider.GetComponent<InteractableObject>();
                if (interactableObject != null && !interactableObjects.Contains(interactableObject)) {
                    interactableObjects.Add(interactableObject);
                    // Debug.Log($"Added {interactableObject.name} in the room {this.gameObject.name}");
                }
            }
        }
    }

    // void OnTriggerEnter(Collider other) {
    //     //if (isHallway) return;
    //     // get the player controller component
    //     CustomPlayerController player = other.GetComponent<CustomPlayerController>();

    //     // check if the playerController is not null and the player's PhotonView ID is not already in the list
    //     if (player != null && !_playerCountInRoom.Exists(pc => pc.PlayerID == player.PlayerID)) {
    //         _playerCountInRoom.Add(player);

    //         // start the countdown coroutine only if it's not already running
    //         if (_countdownCoroutine == null) {
    //             _countdownCoroutine = StartCoroutine(StartCountDown(_timeToGas));
    //             // logs the time to gas the room ran in the Couroutine
    //             // Debug.Log("Kill Protocol Initiated. Room will gas in " + _timeToGas + " seconds.");
    //         }
    //     }
    // }

    // void OnTriggerExit(Collider other) {
    //     //if (isHallway) return;
    //     // if a gameobject with a PlayerController component exits the trigger volume, subtract 1 from the player count in the room
    //     CustomPlayerController player = other.GetComponent<CustomPlayerController>();
    //     if (player != null) {
    //         _playerCountInRoom.Remove(player);
    //         // Stop the countdown coroutine if there are no players in the room
    //         if (_playerCountInRoom.Count == 0 && _countdownCoroutine != null) {
    //             StopCoroutine(_countdownCoroutine);
    //             _countdownCoroutine = null;
    //             // Debug.Log("Kill Protocol terminated");
    //         }
    //     }
    // }

    public void KillAllInRoom()
    {
        // Debug.Log("All players in the room have died.");
    }
    #endregion

    #region Coroutines
    // Start counting down the time to gas the room
    // count only if there is at least 1 player in the room constantly 
    IEnumerator StartCountDown(float timeToGas)
    {
        while (_playerCountInRoom.Count > 0)
        {
            yield return new WaitForSeconds(timeToGas);
            KillAllInRoom();
        }
    }

    // Coroutine for the blinking effect
    IEnumerator BlinkHighlighter(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            roomHighlighter.SetActive(!roomHighlighter.activeSelf);
            yield return new WaitForSeconds(0.5f); // Blink interval
            elapsed += 0.5f;
        }
        roomHighlighter.SetActive(true);
    }
    #endregion
    // Kill all players in the room

    public void TurnOffHighlighter()
    {
        roomHighlighter.SetActive(false);
    }

    #region PunRPC
    [PunRPC]
    public void RPC_StartBlinkingHighlighter(float duration)
    {
        // Debug.Log($"{roomHighlighter.name} blinking!");
        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
        }
        _blinkCoroutine = StartCoroutine(BlinkHighlighter(duration));
    }
    #endregion
}
