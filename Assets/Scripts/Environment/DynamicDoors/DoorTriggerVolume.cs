// Created on Sun Apr 07 2024 || Copyright© 2024 || By: Alex Buzmion II
using System.Collections;
using FMODUnity;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class DoorTriggerVolume : MonoBehaviour
{
    [SerializeField, BoxGroup("VFX")] private VisualEffectsSO vfxDoorInteraction;
    [SerializeField, BoxGroup("VFX")] private string vfxDoorOpenText = "PSSS!";
    [SerializeField, BoxGroup("VFX")] private string vfxDoorCloseText = "WHAM!"; 

    [SerializeField] private float animTime = 1;
    [SerializeField] private float elapsedTime = 0;
    [SerializeField] private bool isInteractable;
    [SerializeField] public Transform doorTransform;

    [SerializeField, BoxGroup("Door Open")] private float doorTimer = 20; 
    [SerializeField, BoxGroup("Door Open")] private Vector3 doorOpenPosition;

    [SerializeField, BoxGroup("Door Close")] private Vector3 doorClosePosition;
    [SerializeField, BoxGroup("Door Close")] private float closeTime = 3;

    [SerializeField, BoxGroup("Trap Constraint Settings")] public GameObject pdTrapPos01;
    [SerializeField, BoxGroup("Trap Constraint Settings")] public GameObject pdTrapPos02;

    [SerializeField, BoxGroup("FMOD References")] private EventReference _doorOpenSFX;
    [SerializeField, BoxGroup("FMOD References")] private EventReference _doorClosingSFX;

    private void OnEnable() {
        doorClosePosition = transform.transform.position;
    }   

    private void Awake() 
    {
        isInteractable = true;
        
        if (doorTransform.gameObject.tag == "ZDoor") 
        {
            doorOpenPosition = doorTransform.transform.position + new Vector3 (0,0,1.81f);
        }
        else if (doorTransform.gameObject.tag == "ZDoor2") 
        {
            doorOpenPosition = doorTransform.position + new Vector3 (0,0,-1.81f);
        }
        else {
            doorOpenPosition = doorTransform.transform.position + new Vector3 (1.81f,0,0);
        }

    }
    
    private void OnTriggerStay(Collider other) 
    {
        if(!isInteractable) return; // If the door is not interactable, return
        //if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E)) // If the player is in the trigger volume and presses E
        if (other.CompareTag("Player")) // If the player is in the trigger volume and presses E
        {
            if (vfxDoorInteraction != null) {
            SpawnDoorVFX(other.transform, vfxDoorOpenText);
            }
            StartCoroutine(DoorOpen(animTime));
        }
    }

    // IEnumerator to open the door when player interacts with the door
    // animate the door opening by sliding the door to the right
    IEnumerator DoorOpen(float _animTimer) 
    {
        isInteractable = false; // Set the door to not interactable
        elapsedTime = 0;
        RuntimeManager.PlayOneShot(_doorOpenSFX, transform.position);
        // While the time elapsed is less than the total duration
        while (elapsedTime < _animTimer) 
        {
            // Calculate the new position based on elapsed time
            doorTransform.position = Vector3.Lerp(doorClosePosition, doorOpenPosition, (elapsedTime / _animTimer));
            elapsedTime += Time.deltaTime; // Increase elapsed time by the time taken to render this frame
            yield return null; // Wait until next frame
        }
        doorTransform.position = doorOpenPosition;
        StartCoroutine(DoorClose(animTime, closeTime));
    }
    
    // counter to re-open the door after the door timer has passed
    // enabling the box collider again
    IEnumerator DoorClose(float _animTimer, float _closeTimeDelay) 
    {
        yield return new WaitForSeconds(_closeTimeDelay); // Wait for the door timer delay before starting to close
        float elapsedTime = 0;
        RuntimeManager.PlayOneShot(_doorClosingSFX, transform.position);
        while (elapsedTime < _animTimer)  // this while check needs to be changed from a time check. Positions is not consistent
        {
            doorTransform.position = Vector3.Lerp(doorOpenPosition, doorClosePosition, elapsedTime / _animTimer);
            elapsedTime += Time.deltaTime;
            if (elapsedTime > 2.0f)
            {
                // todo hook for closing door rumble VFX
            }
            yield return null;
        }
        doorTransform.position = doorClosePosition;
        // VFX for door slam/shut
        //SpawnDoorVFX(transform, vfxDoorCloseText);
        StartCoroutine(CDToInteract(doorTimer)); // Start cooldown for re-interaction after door closes
    }

    // counter before the door can be interacted with again
    IEnumerator CDToInteract(float _doorTimer)
    {
        yield return new WaitForSeconds(_doorTimer);
        isInteractable = true;
        // todo add VFX here for door light signifier 
    }

    private void SpawnDoorVFX(Transform vfxSpawnTransform, string vfxText)
    {
        if (vfxDoorInteraction != null)
        {
            GameObject vfxInstance = Instantiate(vfxDoorInteraction.vfxPrefab, vfxSpawnTransform.GetComponentInParent<OwnerNetworkComponent>().vfxSpawnPosition.position, Quaternion.identity);
            //vfxInstance.GetComponentInChildren<TextMeshProUGUI>().text = vfxText;
            Destroy(vfxInstance, vfxDoorInteraction.vfxLifeTime);
        }
        else if (vfxDoorInteraction == null)
        {
            Debug.LogError($"{vfxDoorInteraction.name} has not been assigned in the inspector!");
        }
    }
}