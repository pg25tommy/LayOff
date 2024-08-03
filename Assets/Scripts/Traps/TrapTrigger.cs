// Created on Wednsday May 8 2024 || Copyright (c) 2024 Names Are Hard Studios || By: Alex Buzmion II, Tiago Corsato
using UnityEngine;
using Photon.Pun;
using Sirenix.OdinInspector;
using FMODUnity;
using System.Collections;
using Photon.Realtime;
using UnityEngine.UI;

public abstract class TrapTrigger : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks, IPunObservable
{
    // Add a reference to the GameManager
    protected GameManager gameManager;

    [field: SerializeField, BoxGroup("Trigger Settings"), Tooltip("Time to activate the trap upon triggering")]
    protected float timeToActivate { get; set; } = 0.5f;
    [field: SerializeField] protected float trapPreEffectLifeTime { get; set; } = 3f;

    [field: SerializeField, BoxGroup("Trigger Settings"), Tooltip("If the trap is activated or not.")]
    protected bool IsActivated { get; set; } = false;

    [field: SerializeField, BoxGroup("Trigger Settings"), Tooltip("The prefab to spawn right beofore the trap is activated (Ex: Bouncing Betty).")]
    protected GameObject trapPreEffectPrefab;

    [field: SerializeField, BoxGroup("Trigger Settings"), Tooltip("The prefab to spawn when the trap is activated.")]
    protected GameObject trapEffectPrefab;

    [field: SerializeField, BoxGroup("Trigger Settings"), Tooltip("The spawn point for the trap effect.")]
    protected GameObject trapEffectSpawnPosition;

    [field: SerializeField, BoxGroup("Trigger Settings"), Tooltip("The spawn point for the trap pre effect.")]
    protected GameObject preEffectSpawnPosition;

    [field: SerializeField] 
    protected GameObject localTrapIndicator;
    [field: SerializeField]
    protected GameObject mesh;
    [SerializeField] public ReplayCamTrigger replayCamInstance; 
    [SerializeField] private GameObject replayCamTriggerPrefab; 
    GameObject replayCam = null;
    protected bool isTrapActive;
 
    [field: SerializeField, BoxGroup("SFX")] public EventReference TrapDestroySFX { get; protected set; }
    private bool IsInitiatingTransfer = false;
    protected GameObject preEffectInstance;
    //! get a reference for the collider
    protected new Collider collider;
    //! owner reference
    protected int triggeringPlayer;
    [SerializeField] public int ownerID;
    [SerializeField] protected float timeToEnable = 3;
    [HideInInspector] public GameObject effectInstance;

    [SerializeField] private Image radialWheel;

    // stat tracking variables 
    #region MonoBehavior Callbacks
    protected virtual void Awake()
    {
        // if (PhotonNetwork.IsMasterClient) {
        // replayCam = PhotonNetwork.InstantiateRoomObject(replayCamTriggerPrefab.name, this.transform.position, Quaternion.identity);
        // }
        // if (replayCam == null) {
        //     Debug.LogWarning("Failed to instantiate replayCam. Check the prefab setup.");
        // } else {
        //     Debug.LogWarning("Successfully instantiated replayCam.");
        // }
        // Initialize the GameManager reference
        gameManager = Object.FindFirstObjectByType<GameManager>();
        PhotonNetwork.AddCallbackTarget(this);
        //collider = GetComponent<Collider>(); //! get the triggers collider
        if(TryGetComponent<Collider>(out collider))
        {
            collider.enabled = false;
        }
        this.ownerID = photonView.CreatorActorNr;
        if (!photonView.IsMine)
        {
            photonView.TransferOwnership(PhotonNetwork.MasterClient);
        }
        // instantiate the replayCam trigger box upon enabling the trap. 
    }
    //! Start disable collider by default, run coroutine to countdown before turning bool isenabled to true, flex time 
    private void Start()
    {
        
        // StartCoroutine(AwaitCamTriggerPV());   
        StartCoroutine(EnableTrigger(timeToEnable));
        // Debug.Log($"{gameObject.name} set by Player {ownerID}");
    }

    //! coroutine delay to trip the trap
    private IEnumerator EnableTrigger(float timeToEnable)
    {
        isTrapActive = false;
        localTrapIndicator.SetActive(false);
        radialWheel.fillAmount = 0f;
        while(radialWheel.fillAmount < 1f)
        {
            radialWheel.fillAmount += 0.4f * Time.deltaTime;
            yield return null;
        }

        if(collider is not null)
            collider.enabled = true;
        radialWheel.enabled = false;
        isTrapActive = true;

        if (this.ownerID == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            //Enable mesh and indicator
            if (localTrapIndicator is not null)
            {
                localTrapIndicator.SetActive(true);
                localTrapIndicator.GetComponent<SpriteRenderer>().color = GameManager.Instance.GetPlayerColor(this.ownerID);
            }
            if (mesh is not null)
            {
                mesh.SetActive(true);
            }
        }
        else
        {
            if (localTrapIndicator is not null)
            {
                localTrapIndicator.SetActive(false);
            }
            if (mesh is not null)
            {
                mesh.SetActive(false);
            }
        }
        if (replayCamInstance != null) {
            replayCamInstance.collider.enabled = true;
        }
        // saves the replaycam trigger that the effect will reference 
    }

    private IEnumerator AwaitCamTriggerPV() {
        while (replayCamInstance == null) {
            if (PhotonNetwork.IsMasterClient) {
                PhotonView replayCamPVID = replayCam.GetComponent<PhotonView>();
                if (replayCamPVID != null) {
                replayCamInstance = replayCam.GetComponent<ReplayCamTrigger>();
                photonView.RPC(nameof(AssignReplayCamTriggerInstance), RpcTarget.All, replayCamPVID.ViewID);
                } else
                {
                    Debug.LogWarning("PhotonView component is missing from replayCam.");
                }
                yield return new WaitForSeconds(.01f);
            }
            else {
                yield return null;

            }
        }
    }

    [PunRPC]
    public void AssignReplayCamTriggerInstance(int viewID) {
        PhotonView view = PhotonView.Find(viewID);
        if (view != null) {
            replayCamInstance = view.GetComponent<ReplayCamTrigger>();
            Debug.LogWarning("ReplayCamInstance assigned via RPC");
        } else {
            Debug.LogWarning("PhotonView not found for viewID: " + viewID);
        }
    }

    protected virtual void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    // This will call the trigger and spawn the TrapEffect
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 20) return;
        if (other.tag == "Player" && !IsActivated || other.gameObject.layer == 12 && !IsActivated)
        {// If the object that collides with the trap is a player and the trap is not activated
            Debug.Log("Trap triggered by: " + other.name);
            StartCoroutine(HandleTrapTrigger());
        }
    }

    protected virtual IEnumerator HandleTrapTrigger()
    {
        IsActivated = true;
        if (PhotonNetwork.IsMasterClient)
        {
            // Debug.Log("Master client handling trap trigger");
            yield return SpawnPreTrapEffect();
            //yield return new WaitForSeconds(timeToActivate);
            photonView.RPC(nameof(RPC_AnimateTrigger), RpcTarget.All);
            SpawnTrapEffect();
            yield return new WaitForSeconds(3);
            RuntimeManager.PlayOneShot(TrapDestroySFX, transform.position); // FMOD Sound event playing
            photonView.RPC(nameof(RequestDestroy), RpcTarget.MasterClient, photonView.ViewID);
        }
    }

    protected virtual IEnumerator ActivateTrap(GameObject player)
    {
        yield return new WaitForSeconds(timeToActivate);
        SpawnTrapEffect();
        //DestroyTrigger();
        photonView.RPC(nameof(RequestDestroy), RpcTarget.MasterClient, photonView.ViewID);
    }

    // This will spawn an aditional effect before the actual trap effect
    protected virtual IEnumerator SpawnPreTrapEffect()
    {
        if (trapPreEffectPrefab != null && PhotonNetwork.IsMasterClient)
        {
            // Spawn VFX before Trap Effect
            preEffectInstance = PhotonNetwork.Instantiate(trapPreEffectPrefab.name, preEffectSpawnPosition.transform.position, Quaternion.identity);
            OnPreEffectSpawned();
            // Debug.Log($"Spawned Trap Effect: {this.name}");
            preEffectInstance.transform.SetParent(gameObject.transform);

            yield return new WaitForSeconds(trapPreEffectLifeTime);

            if (preEffectInstance.GetComponent<PhotonView>().IsMine)
            {
                PhotonNetwork.Destroy(preEffectInstance);
            }
        }
    }

    // call back to play to be implemented by the child class 
    protected virtual void OnPreEffectSpawned() { }

    protected virtual void SpawnTrapEffect()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (trapEffectPrefab != null)
        {
            // Debug.Log("Spawning Trap Effect...");
            GameObject effectInstance = PhotonNetwork.Instantiate(trapEffectPrefab.name, trapEffectSpawnPosition.transform.position, Quaternion.identity);
            // Debug.Log("Effect instance created, finding TrapEffect component...");

            TrapEffect effect = effectInstance.GetComponentInChildren<TrapEffect>();
            if (effect != null)
            {
                // send the PV of the trigger to the effect to have the effect set its ownerID
                effect.SetTriggerPhotonViewID(photonView.ViewID);
                effect.replayCamInstance = this.replayCamInstance;
            }
            else
            {
                Debug.LogWarning("No TrapEffect found on instantiated object.");
            }
        }
        else
        {
            Debug.LogWarning("Trap effect prefab is null.");
        }
    }

    public int GetOwnerID()
    {
        return this.ownerID;
    }

    // Animate trtigger being stepped on
    protected virtual void AnimateTrigger() {
        Debug.Log("AnimateTrigger called");
        this.gameObject.SetActive(true);
    }
    //!DONT DELETE
    protected virtual void DestroyTrigger()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
    #endregion

    #region PunCallbacks
    [PunRPC]
    protected virtual void RPC_AnimateTrigger() {
        AnimateTrigger();
    }

    [PunRPC]
    protected virtual void RequestDestroy(int viewID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonView objectToDelete = PhotonView.Find(viewID);
            // Debug.Log($"Request sent to Masterclient to destroy {objectToDelete.gameObject.name}");
            if (objectToDelete != null)
            {
                PhotonNetwork.Destroy(objectToDelete.gameObject);
            }
        }
    }

    [PunRPC]
    protected virtual void RPC_SpawnTrapEffect()
    {
        SpawnTrapEffect();
    }

    void IPunOwnershipCallbacks.OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        if (targetView != photonView) return;
        // Debug.Log($"Ownership requested for View ID: {targetView.ViewID} by Player ID: {requestingPlayer.UserId}");

        if (PhotonNetwork.IsMasterClient)
        {
            // Debug.Log($"Master Client is taking ownership of View ID: {targetView.ViewID}");
            photonView.TransferOwnership(PhotonNetwork.MasterClient);
        }
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        if (targetView == photonView)
        {
            // Debug.Log($"Ownership transferred for View ID: {targetView.ViewID} from Player ID: {previousOwner?.UserId ?? "None"} to Player ID: {targetView.Owner.UserId}");
        }
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
        if (targetView == photonView)
        {
            // Debug.LogWarning($"Ownership transfer failed for View ID: {targetView.ViewID} requested by Player ID: {senderOfFailedRequest.UserId}");
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (!stream.IsWriting)
        {
            ownerID = (int)stream.ReceiveNext();
            IsActivated = (bool)stream.ReceiveNext();
            return;
        }
        stream.SendNext(ownerID);
        stream.SendNext(IsActivated);
    }
    #endregion
}