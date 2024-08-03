//Copyright (c) 2024 Names Are Hard, All Rights Reserved
// Import necessary libraries
using Photon.Pun;
using UnityEngine;

// Define the Item class
public class Item : MonoBehaviour
{
    // called when the item enters a trigger collider
    private void OnCollisionEnter(Collision collision)
    {
        collision.gameObject.TryGetComponent<InventoryManager>(out InventoryManager playerinventory);
        if (playerinventory == null) {
            return;
        }
        // If the object that entered the trigger has the tag "Player"
        if (collision.gameObject.tag =="Player")
        {
            // Update the HUD to show the gun image
            // Destroy the item game object
            Destroy(gameObject);
        }
    }

}