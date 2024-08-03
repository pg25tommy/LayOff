using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PDHologram : MonoBehaviour
{
    public TrapSpawner trapSpawner;
    public CustomPlayerController playerController;

    void Update()
    {
        if (trapSpawner == null)
        {
            Debug.LogError("TrapSpawner was not assigned.");
            return;
        }

        float distance = Vector3.Distance(transform.position, trapSpawner.transform.position);

        if (distance <= trapSpawner.distanceToPlaceTrap)
        {
            CheckConstraints();
        }
        else
        {
            //trapSpawner.canPlaceTrap = false;
            trapSpawner.isSnapping = false;
            //playerController.UpdateText("Too far!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        LayerMask trapLayer = LayerMask.GetMask("Traps");
        if(other.gameObject.layer == 12)
        {
            trapSpawner.canPlaceTrap = false;
            Debug.Log("Found Trap!");
        }

        Debug.Log(other.gameObject.layer);
    }

    RaycastHit spawnerHit;
    RaycastHit hologramHit;

    void CheckConstraints()
    {
        if (trapSpawner == null || trapSpawner.camObj == null)
        {
            Debug.LogError("TrapSpawner or camObj was not assigned.");
            return;
        }


        // Raycast to detect Doors
        if (Physics.Raycast(trapSpawner.startPoint, trapSpawner.transform.forward, out spawnerHit, trapSpawner.castDistance, trapSpawner.doorMask))
        {
            trapSpawner.isSnapping = true;
            HandleDoorRaycast(spawnerHit);
        }
        // Raycast to detect Walls
        else if (Physics.Raycast(trapSpawner.startPoint, trapSpawner.transform.forward, out spawnerHit, trapSpawner.castDistance, trapSpawner.wallMask))
        {
            trapSpawner.isSnapping = true;
            HandleWallRaycast(spawnerHit);
        }
        else
        {
            trapSpawner.isSnapping = false;
            trapSpawner.canPlaceTrap = false;
            Debug.DrawRay(trapSpawner.startPoint, transform.forward * trapSpawner.castDistance, Color.green);
            playerController.UpdateText("Walls/Doors only!");
        }
    }

    void HandleDoorRaycast(RaycastHit spawnerHit)
    {
        if (Physics.Raycast(trapSpawner.camObj.transform.position, trapSpawner.camObj.transform.forward, out trapSpawner.cameraHit, 50f, trapSpawner.wallMask))
        {
            Transform targetPos = spawnerHit.collider.gameObject.GetComponent<DoorTriggerVolume>().pdTrapPos01.transform;

            if (trapSpawner.cameraHit.point.y > spawnerHit.point.y)
            {
                // Higher
                targetPos = spawnerHit.collider.gameObject.GetComponent<DoorTriggerVolume>().pdTrapPos01.transform;
            }
            else if (trapSpawner.cameraHit.point.y < spawnerHit.point.y)
            {
                // Lower
                targetPos = spawnerHit.collider.gameObject.GetComponent<DoorTriggerVolume>().pdTrapPos02.transform;
            }

            trapSpawner.canPlaceTrap = true;
            transform.position = targetPos.position; // Snap position to door
            transform.rotation = targetPos.rotation; // Snap rotation to door
            Debug.DrawLine(trapSpawner.startPoint, spawnerHit.point, Color.red);
            playerController.DisableText();
        }
    }

    void HandleWallRaycast(RaycastHit spawnerHit)
    {
        transform.forward = spawnerHit.normal;

        if (Physics.Raycast(trapSpawner.camObj.transform.position, trapSpawner.camObj.transform.forward, out trapSpawner.cameraHit, 50f, trapSpawner.wallMask))
        {
            trapSpawner.yPos = trapSpawner.cameraHit.point.y > spawnerHit.point.y ? 1.5f : 0.5f;
        }

        Vector3 xzPos = new Vector3(spawnerHit.point.x, trapSpawner.yPos, spawnerHit.point.z);
        transform.position = xzPos;

        if (Physics.Raycast(transform.position, transform.forward, out hologramHit, 50f, trapSpawner.doorMask))
        {
            trapSpawner.canPlaceTrap = Physics.Raycast(transform.position, transform.forward, out hologramHit, 50f, trapSpawner.wallMask);
        }
        else
        {
            trapSpawner.canPlaceTrap = true;
            playerController.DisableText();
        }
    }
}
