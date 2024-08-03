using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DTHologram : MonoBehaviour
{
    public TrapSpawner trapSpawner;
    public CustomPlayerController playerController;

    public RaycastHit hit;

    void Update()
    {
        float distance = Vector3.Distance(transform.position, trapSpawner.transform.position);

        if (distance <= trapSpawner.distanceToPlaceTrap)
        {
            if (Physics.Raycast(trapSpawner.startPoint, trapSpawner.camObj.transform.forward, out hit, trapSpawner.castDistance, trapSpawner.interactablesMask))
            {
                trapSpawner.isSnapping = true;

                if (Physics.Raycast(trapSpawner.camObj.transform.position, trapSpawner.camObj.transform.forward, out trapSpawner.cameraHit, 50f, trapSpawner.interactablesMask))
                {
                    Transform targetPos = hit.collider.gameObject.GetComponent<InteractableObject>().gameObject.transform;
                    if (targetPos == null) return; 

                    trapSpawner.canPlaceTrap = true;
                    transform.position = targetPos.position; // Snap position to prop
                    transform.rotation = targetPos.rotation; // Snap rotation to prop
                    Debug.DrawLine(trapSpawner.startPoint, hit.point, Color.red);
                    playerController.DisableText();
                }
            }
            else
            {
                trapSpawner.isSnapping = false;
                trapSpawner.canPlaceTrap = false;
                Debug.DrawRay(trapSpawner.startPoint, transform.forward * trapSpawner.castDistance, Color.green);
                playerController.UpdateText("Place inside objects");
            }
        }
        else if (distance > trapSpawner.distanceToPlaceTrap)
        {
            //trapSpawner.canPlaceTrap = false;
            trapSpawner.isSnapping = false;
            Debug.DrawRay(trapSpawner.startPoint, transform.forward * trapSpawner.castDistance, Color.green);
            //playerController.UpdateText("Too far!");
        }
    }
}
