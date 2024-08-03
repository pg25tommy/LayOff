// Created on Sat Jun 29 2024 || Copyright© 2024 || By: Alex Buzmion II
using CharacterMovement;
using Photon.Pun;
using UnityEngine;

public class WorldButtonInteractable : MonoBehaviour
{
    [SerializeField] GameObject highlighter;
    [SerializeField] LayOffGameEvent onWinButtonPressed;

    private void OnTriggerEnter(Collider other) {
        if (highlighter != null) {
            highlighter.SetActive(true);
        }
    }

    public void Interact(CustomCharacterMovement playerCCM) {
        // Debug.Log($"{playerCCM} received ");
        // todo Add SFX for win button pressed
        PhotonView winnerPV = playerCCM.GetComponentInParent<PhotonView>();
        onWinButtonPressed.Raise(winnerPV, null);
   }

   private void OnTriggerExit(Collider other) {
        if (highlighter != null) {
            highlighter.SetActive(false);
        }
    }
}
