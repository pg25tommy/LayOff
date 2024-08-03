using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;
using CharacterMovement;
using UnityEngine.InputSystem;

public class OwnerNetworkComponent : MonoBehaviourPunCallbacks
{
    public CinemachineFreeLook camFl;
    public Camera camObj;
    public Transform vfxSpawnPosition;
    public GameObject minimapIndicator;
    public PlayerStatsManager playerStatsManager;
    public CustomPlayerController customPlayerController;  
    public PlayerInput playerInput;

    private void Start()
    {
        if(!GetComponent<PhotonView>().IsMine)
        {
            // Turn off other cameras
            camFl.enabled = false;
            camObj.enabled = false;
            vfxSpawnPosition.gameObject.SetActive(false);
            minimapIndicator.SetActive(false);
            playerStatsManager.enabled = false;
            customPlayerController.enabled = false;
        }
        else
        {
            camFl.enabled = true;
            camObj.enabled = true;
            vfxSpawnPosition.gameObject.SetActive(true);
            minimapIndicator.SetActive(true);
            playerStatsManager.enabled = true;
           customPlayerController.enabled = true;
           playerInput.enabled = false; // this playerInput switch on and off fixes a bug that locks players movement onset of the game.
           playerInput.enabled = true;
        }
    }
}
