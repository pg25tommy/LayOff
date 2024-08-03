using CharacterMovement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTHologram : MonoBehaviour
{
    public TrapSpawner trapSpawner;
    public CustomPlayerController playerController;

    public float range = 2f;

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, trapSpawner.transform.position);

        //if (distance <= trapSpawner.distanceToPlaceTrap)
        //{
            // Make sure the hologram is not on a wall
            if (Physics.Raycast(transform.position, Vector3.forward, out trapSpawner.cameraHit, range, trapSpawner.wallMask | trapSpawner.propsMask | trapSpawner.interactablesMask) ||
                Physics.Raycast(transform.position, Vector3.back, out trapSpawner.cameraHit, range, trapSpawner.wallMask | trapSpawner.propsMask | trapSpawner.interactablesMask) ||
                Physics.Raycast(transform.position, Vector3.left, out trapSpawner.cameraHit, range, trapSpawner.wallMask | trapSpawner.propsMask | trapSpawner.interactablesMask) ||
                Physics.Raycast(transform.position, Vector3.right, out trapSpawner.cameraHit, range, trapSpawner.wallMask | trapSpawner.propsMask | trapSpawner.interactablesMask))
            {
                trapSpawner.canPlaceTrap = false;
                playerController.UpdateText("Invalid placement");
            }
            else
            {
                trapSpawner.canPlaceTrap = true;
                playerController.DisableText();
            }
        //}
        //else if (distance > trapSpawner.distanceToPlaceTrap)
        //{
            //trapSpawner.canPlaceTrap = false;
            //playerController.UpdateText("Too far!");
        //}
    }
}