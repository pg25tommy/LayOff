// Created on Mon May 06 2024 || Copyright© 2024 || By: Alex Buzmion II
using CharacterMovement;
using UnityEngine;

public class KillBoxBehavior : MonoBehaviour
{
    int playerNr = 0;
    int trapType = 3; 
    private void OnCollisionEnter(Collision other) {
        // invoke damage to player 
        if (!other.gameObject.CompareTag("Player")) return;
        CustomCharacterMovement player = other.gameObject.GetComponent<CustomCharacterMovement>();
        if (player != null) {
            player.OnDeath(0, trapType);
        } else {
            Debug.Log("Player Hit but not affected");
        }
    }
}
