// Created on Wed Apr 10 2024 || Copyright© 2024 || By: Alex Buzmion II
using System.Collections;
using System.Collections.Generic;
using CharacterMovement;
using FMOD.Studio;
using FMODUnity;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

public class FinalDoorBehavior : MonoBehaviour
{
    [SerializeField] private List<GameObject> signs; 
    [SerializeField] private Material redSign;
    [SerializeField] private Material greenSign;
    [field: SerializeField, BoxGroup("SFX")] public EventReference LazorDoorExplosionSFX { get; protected set; }
    [field: SerializeField, BoxGroup("SFX")] public EventReference LazorDoorDisableSFX { get; protected set; }
    [field: SerializeField, BoxGroup("SFX")] public EventReference GenericDeathVA { get; protected set; }
    [SerializeField, BoxGroup("Object References")] private TMPro.TextMeshProUGUI interactText;
    [SerializeField, BoxGroup("Object References")] private GameObject lasers;
    [SerializeField, BoxGroup("Object References")] private ParticleSystem laserInteractionVFX;
    [SerializeField] private GameObject laserGreen; 
    [SerializeField] private GameObject laserRed; 
    [SerializeField] private bool canEnter = false;
    [SerializeField] private float pushBackForce = 5.0f;
    [SerializeField, BoxGroup("Object References")] private Collider doorCollider;
    [SerializeField, BoxGroup("Object References")] private GameObject doorObject; // Add reference to the door GameObject

    // SFX Parameter state search
    private bool isFirstTime = true;

    [SerializeField] private float dissolveSpeed = 0.4f;
    public Material laserMat;
    float dissolve = 1f;

    private void Awake()
    {
        laserMat.SetFloat("_DoorOpen", dissolve);
    }


    // Check if the player has all 3 keycards while inside the trigger box 
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.TryGetComponent<CustomCharacterMovement>(out CustomCharacterMovement character); 
            if (character != null && character.IsDead) {
                UnityEngine.Debug.Log("Character is already dead, returning");
                return;
            }
            // Retrieve the InventoryManager component from the player to check hasAll3Keycards bool 
            other.TryGetComponent<InventoryManager>(out InventoryManager playerinventory);
            if (playerinventory != null)
            {
                if (!playerinventory.hasAllReqKeycards)
                {
                    interactText.text = "You need all 3 keycards to open this door";
                    AccessDenied(other);
                    return;
                }
                // Open the door
                canEnter = true;
                if (canEnter)
                {
                    laserMat.SetFloat("_DoorOpen", 1f);
                    StartCoroutine(DecreaseDissolve());
                    doorCollider.enabled = false;
                    DisableLasers();
                    //HideDoor(); // Hide the door when the player can enter
                }
                interactText.text = "Door opened!";
                return;
            }
        }
    }

    IEnumerator DecreaseDissolve()
    {
        while (dissolve > 0)
        {
            dissolve -= dissolveSpeed * Time.deltaTime;
            laserMat.SetFloat("_DoorOpen", dissolve);
            //Debug.Log("Dissolve: " + dissolve);
            yield return null; // Wait until the next frame
        }
        doorObject.SetActive(false); // Deactivate the door GameObject
    }

    public void SwitchLaser(int mode) {
        if (mode == 1) {
            laserGreen.SetActive(true);
            laserRed.SetActive(false);
            ChangeSignColors(greenSign); // change exit sign colors to green
            return;
        }
        laserGreen.SetActive(false);
        laserRed.SetActive(true);
        ChangeSignColors(redSign); // change exit sign colors to red
    }

    private void OnTriggerExit(Collider other)
    {
        interactText.text = "";
        if (other.gameObject.tag == "Player")
        {
            other.TryGetComponent<InventoryManager>(out InventoryManager playerinventory);
            if (playerinventory != null && !playerinventory.hasAllReqKeycards)
            {
                EnableLasers();
            }
        }
    }

    void EnableLasers()
    {
        lasers.SetActive(true);
        doorCollider.enabled = true;
    }

    void DisableLasers()
    {
        lasers.SetActive(false);
        // Play fmod event only when player first passes through door
        if (isFirstTime)
        {
            PlaySoundWithParameter(LazorDoorDisableSFX, "FinalDoorState", "Clear"); // Clear
            isFirstTime = false; // Parameter change
        }
        else
        {
            PlaySoundWithParameter(LazorDoorDisableSFX, "FinalDoorState", "Hide"); // Hide
        }
        doorCollider.enabled = false;
    }

    void AccessDenied(Collider other)
    {
        // Play SFX
        RuntimeManager.PlayOneShot(LazorDoorExplosionSFX, transform.position);
        // Play VFX
        if (laserInteractionVFX != null)
        {
            Instantiate(laserInteractionVFX, transform.position, Quaternion.identity);
        }
        if (!other.gameObject.CompareTag("Player")) return;
        CustomCharacterMovement player = other.gameObject.GetComponent<CustomCharacterMovement>();
        if (player != null)
        {
            player.OnDeath(0, 1);
            RuntimeManager.PlayOneShot(GenericDeathVA);
        }
        else
        {
            UnityEngine.Debug.Log("Player Hit but not affected");
        }
        // Push back Player
        Vector3 normalDir = Vector3.Normalize(other.transform.position - transform.position);
        other.GetComponent<Rigidbody>().AddForce(normalDir * pushBackForce, ForceMode.Impulse);
    }

    void HideDoor()
    {
        doorObject.SetActive(false); // Deactivate the door GameObject
        
    }

    void PlaySoundWithParameter(EventReference sound, string parameterName, string parameterValue)
    {
        Debug.Log($"Playing sound: {sound}, Parameter: {parameterName} = {parameterValue}");
        EventInstance instance = RuntimeManager.CreateInstance(sound);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        instance.setParameterByNameWithLabel(parameterName, parameterValue);
        instance.start();
        instance.release();
    }
    // Changes the sign colors upon collecting 3 keycards. 
    void ChangeSignColors(Material signColor){ 
        if (signs is null) return; 
        foreach (GameObject sign in signs) {
            sign.GetComponent<MeshRenderer>().material = signColor;
        }
    }
}
