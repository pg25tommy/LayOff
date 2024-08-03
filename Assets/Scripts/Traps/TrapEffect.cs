// Created on Wednsday May 8 2024 || Copyright (c) 2024 Names Are Hard Studios || By: Alex Buzmion II, Tiago Corsato
using System.Collections;
using System.Collections.Generic;
using CharacterMovement;
using FMODUnity;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class TrapEffect : MonoBehaviourPunCallbacks, IPunObservable
{
    [field: SerializeField] protected LayOffGameEvent onDPHitPlayerEvent;
    [field: SerializeField, BoxGroup("VFX")] protected VisualEffectsSO vfxTrapHit;
    [field: SerializeField, BoxGroup("SFX")] protected EventReference countDownSFX;
    [field: SerializeField, BoxGroup("SFX")] protected EventReference explosionSFX;
    [field: SerializeField, BoxGroup("SFX")] protected EventReference _detonatorVA;
    [field: SerializeField, BoxGroup("Trap Effect Settings")] protected float trapEffectLifeTime = 10f;
    [field: SerializeField, BoxGroup("Trap Effect Settings")] protected VisionSystem Vision { get; set; }
    [SerializeField] public ReplayCamTrigger replayCamInstance;
    [SerializeField] protected float moveSpeed = 10f;
    [SerializeField] public int trapType;  // trap type 
    [SerializeField] public List<int> victimIDs; // stores all killed players by this effect (this was originally a list to manage multikill tracking. This will be change from a list and manage multikill recognition in the playerHUD)
    PhotonView victimPV;
    private bool isProcessingKills = false;
    private bool playbackSent = false;
    [SerializeField, BoxGroup("ownership variables")] private int triggerPV;
    [SerializeField, BoxGroup("ownership variables")] public int ownerID; // this traps owner player

    protected virtual void Awake()
    {
        RuntimeManager.PlayOneShot(countDownSFX, transform.position); // FMOD Countdown Sound event playing
        Debug.Log($"Base TrapEfefct class{this.name} ownerID: {ownerID}");
    }

    void Update()
    {
        if (transform.position.y <= 1.5f)
        {
            transform.position += new Vector3(0, 1.5f, 0) * Time.deltaTime * moveSpeed;
        }
    }

    // TODO : Delayed Trap Function

    // Detect Player Collision Function
    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision != null)
            {
                if (collision.gameObject.TryGetComponent<CustomCharacterMovement>(out CustomCharacterMovement player))
                {
                    if (player.IsDead) return;
                    // SendReplay();
                    // onDPHitPlayerEvent.Raise(this, 5.5f);.
                    // Kill Player
                    collision.gameObject.GetComponent<CustomCharacterMovement>().OnDeath(ownerID, trapType);
                    // send kill update passing the colliding player
                    SendKillUpdate(collision);

                    // Spawn VFX
                    SpawnTrapHitVFX(collision.transform);

                    // Spawn SFX
                    SpawnTrapHitSFX();

                    //FMOD SFX and Voice lines
                    RuntimeManager.PlayOneShot(explosionSFX, transform.position); // FMOD Sound event playing
                    RuntimeManager.PlayOneShot(_detonatorVA); //Plays one of the detonator lines at random through the speakers.

                    // if (!isProcessingKills) { // check to start process collisions 
                    //     Debug.Log("Start processing kills");
                    //     isProcessingKills = true;
                    //     StartCoroutine(ProcessCollisions()); 
                    // }

                    
                }
                StartCoroutine(DestroyTrapEffect());
            }
            else
            {
                Debug.Log("Player Hit but not affected");
            }
        }
    }

    // protected virtual void SendReplay() {
    //     if (!playbackSent) {
    //         Debug.Log("Replay Sent");
    //         playbackSent = true;
    //         if (replayCamInstance.cctvCam != null) {
    //             replayCamInstance.cctvCam.InitiateReplay(ownerID, replayCamInstance);
    //         } else {
    //             Debug.LogWarning("No cctvCam assigned");
    //         }
    //     }
    // }

    // function to add the victim to the victimID list and send the RPC to all playerStatsManagers. 
    //! call this function after player.OnDeath() calls
    protected virtual void SendKillUpdate(Collision collision)
    {
        victimPV = collision.transform.parent.GetComponent<PhotonView>(); // retreive the players PV
        int victim = victimPV.OwnerActorNr; // assign the PV OwnerActorNr as victim
        if (!victimIDs.Contains(victim))
        { // ensure the victim is not yet in the list
            victimIDs.Add(victim); // add victim in the list
        }
        victimPV.RPC("RPC_UpdateKills", RpcTarget.All, trapType, ownerID, victimIDs.ToArray()); // send RPC to all to update their stats
        victimIDs.Clear(); // clear the list
    }
    // collider overload
    protected virtual void SendKillUpdate(Collider collider)
    {
        victimPV = collider.transform.parent.GetComponent<PhotonView>(); // retreive the players PV
        int victim = victimPV.OwnerActorNr; // assign the PV OwnerActorNr as victim
        if (!victimIDs.Contains(victim))
        { // ensure the victim is not yet in the list
            victimIDs.Add(victim); // add victim in the list
        }
        victimPV.RPC("RPC_UpdateKills", RpcTarget.All, trapType, ownerID, victimIDs.ToArray()); // send RPC to all to update their stats
        victimIDs.Clear(); // clear the list
    }

    // coroutine handler to ensure all collisions had happened and stored in the victimIDs before sending the data update
    private IEnumerator ProcessCollisions()
    {
        yield return new WaitForSeconds(3);
        // send kill updates after a delay
        Debug.Log("Victim list:");
        foreach (int victim in victimIDs)
        {
            Debug.Log($"Player {victim}");
        }
        victimPV.RPC("RPC_UpdateKills", RpcTarget.All, trapType, ownerID, victimIDs.ToArray());
        victimIDs.Clear();
        isProcessingKills = false; // reset processing to false 
    }
    // Spawn VFX Function
    protected virtual void SpawnTrapHitVFX(Transform vfxSpawnTransform)
    {
        if (vfxTrapHit != null)
        {
            GameObject vfxInstance = Instantiate(vfxTrapHit.vfxPrefab, vfxSpawnTransform.GetComponentInParent<OwnerNetworkComponent>().vfxSpawnPosition.position, Quaternion.identity);
            vfxInstance.GetComponentInChildren<Image>().color = GameManager.Instance.GetPlayerColor(ownerID);
            Destroy(vfxInstance, vfxTrapHit.vfxLifeTime);
        }
        else if (vfxTrapHit == null)
        {
            Debug.LogError($"{vfxTrapHit.name} has not been assigned in the inspector!");
        }
    }

    // Spawn SFX Function
    protected virtual void SpawnTrapHitSFX()
    {

    }

    protected virtual IEnumerator DestroyTrapEffect()
    {
        yield return new WaitForSeconds(trapEffectLifeTime);
        if (GetComponent<PhotonView>().IsMine)
            PhotonNetwork.Destroy(gameObject);
    }

    private void OnValidate()
    {
        Vision = GetComponent<VisionSystem>();
    }

    public void SetTriggerPhotonViewID(int photonViewID)
    {
        this.triggerPV = photonViewID;
        Debug.Log($"SetTriggerPhotonViewID called. triggerPhotonViewID is now {this.triggerPV}");

        // Look up the owner ID of the PhotonView with the cached triggerPhotonViewID
        PhotonView triggerPhotonView = PhotonView.Find(triggerPV);
        if (triggerPhotonView != null)
        {
            if (triggerPhotonView.TryGetComponent<InteractableObject>(out InteractableObject interactable))
            {
                ownerID = interactable.trapOwnerID;
                return;
            }
            ownerID = triggerPhotonView.GetComponent<TrapTrigger>().ownerID;
            Debug.Log($"TriggerPV TrapTrigger ownerID found: {ownerID}");
        }
        else
        {
            Debug.LogError("TriggerPhotonView not found.");
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (!stream.IsWriting)
        {
            ownerID = (int)stream.ReceiveNext();
            return;
        }
        stream.SendNext(ownerID);
    }
}
