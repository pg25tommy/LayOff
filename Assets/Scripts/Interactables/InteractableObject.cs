using CharacterMovement;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using System.Collections;
using TMPro;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class InteractableObject : MonoBehaviourPunCallbacks, IPunObservable
{
    #region Private Fields
    [BoxGroup("Events"), SerializeField] private GameObject eventPrefab; // event prefab
    // private TrapSpawner trapSpawner; // Reference to the TrapSpawner
    #endregion

    #region Public Fields
    public bool hasTrap = false; // Flag to indicate if the trap is set
    [BoxGroup("VFX")] public GameObject vfxTextInteractPrefab;
    [BoxGroup("VFX")] public GameObject vfxTextInteractSpawnPosition;
    [BoxGroup("VFX")] public GameObject highlighter;

    [SerializeField] private string InteractSFX = "event:/SFX/Player/PlayerInteractRule";
    [SerializeField] private string GetSFX = "event:/SFX/Player/PlayerGetRule";
    public bool hasKeyCard = false;
    [BoxGroup("Interaction"), Tooltip("Has to be attached to this object. It is not instantiated.")] public GameObject vfxTextPrompt; // Prompt that shows up when player gets close to an interactable object
    [BoxGroup("Interaction")] public string promptTextInteract = "Interact [E]";        // This string is used in the "vfxTextPromt" TextMeshPro Component
    [BoxGroup("Interaction")] public string promptTextPlaceTrap = "Trap [3]";       // This string is used in the "vfxTextPromt" TextMeshPro Component
    [BoxGroup("Interaction")] public string promptTextPlacedTrap = "Placed Trap!";      // This string is used in the "vfxTextPromt" TextMeshPro Component
    [BoxGroup("Interaction")] public bool canInteract = true;
    [SerializeField] private new GameObject light;
    public float effectDelay = 3f;
    public int trapOwnerID;
    public GameObject trapInstance;
    [SerializeField] public ReplayCamTrigger replayCamInstance; 
    [SerializeField] Player player;
    [SerializeField] PhotonView playerPV;
    [SerializeField] PlayerStatsManager playerSM;
    CustomCharacterMovement customCharacterMovement;
    #endregion

    #region Monobehavior Callbacks
    void Awake()
    {
        this.player = null;
        this.playerPV = null;
        this.playerSM = null;
        if (highlighter != null)
        {
            highlighter.SetActive(false);
        }
        // this.trapSpawner = GetComponent<TrapSpawner>(); // Get the TrapSpawner component
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (player != null)
            {
                // Debug.Log("Player already assigned, returning...");
                return;
            }
            // photonView.RPC(nameof(RPC_UpdateInteractingPlayer), RpcTarget.MasterClient, other.GetComponent<PhotonView>().Owner, other.GetComponent<PhotonView>(), playerPV.GetComponentInParent<PlayerStatsManager>());
            this.player = other.GetComponent<PhotonView>().Owner; // get the local player number
            this.playerPV = other.GetComponent<PhotonView>();
            this.playerSM = playerPV.GetComponentInParent<PlayerStatsManager>();

            // ensure to only show highlighter and text prompt to the local client only
            if (playerPV != null && playerPV.IsMine)
            {
                highlighter.SetActive(true);
                // Debug.Log($"Player interacting: {player} with object {this.gameObject.name}, PV interacting: {playerPV}, SM interacting: {playerSM.gameObject.name}");
                this.playerSM.GetComponentInChildren<CustomCharacterMovement>().EnableVFXPrompt();
            }
            return;
        }
    }

    // Called when the player leaves the interaction zone
    void OnTriggerExit(Collider other)
    {
        if (playerPV != null && playerPV.IsMine )
        {
            highlighter.SetActive(false);

            this.playerSM.GetComponentInChildren<CustomCharacterMovement>().DisableVFXPrompt();
            //if (vfxTextPrompt != null)
            //{
            //    vfxTextPrompt.SetActive(false);
            //}
        }
        canInteract = true;
        // ensure to only show highlighter and text prompt to the local client only
        if (player == null) return;
        player = null;
        playerPV = null;
        playerSM = null;
        // Debug.Log($"player: {player} left {this.gameObject.name}, playerPV: {playerPV}, playerSM: {playerSM}");
    }

    public void Interact()
    {
        if (!canInteract) return;
        // log interaction data
        if (playerSM != null) {
            playerSM.GetComponentInChildren<CustomCharacterMovement>().LogAction("Interacted");
        }
        if (vfxTextInteractPrefab != null)
        {
            PlaySoundWithParameter(InteractSFX, "hastrap", "False"); //FMOD
            SpawnTextVFX(vfxTextInteractPrefab, playerSM.GetComponentInChildren<CustomCharacterMovement>().vfxSpawnPosition, false);
            highlighter.SetActive(false);
        }
        // check if it has a trap
        if (hasTrap)
        {
            PlaySoundWithParameter(InteractSFX, "hastrap", "True"); //FMOD
            photonView.RPC(nameof(RequestDestroy), RpcTarget.MasterClient, trapInstance.GetComponent<PhotonView>().ViewID);
            StartCoroutine(DelayTrapTriggerDestruction());
            photonView.RPC(nameof(RPC_BlinkAndDetonateTrap), RpcTarget.All); return;
        }
        else
        {
            if (playerSM != null) {
                playerSM.GetComponentInChildren<CustomCharacterMovement>().DisableVFXPrompt();
            }
            // Show prompt if trap can be placed
            //if (vfxTextPrompt != null)
            //{
            //    vfxTextPrompt.SetActive(false);
            //}
        }
        CheckIfHasKeyCard(playerSM);
        canInteract = false;
    }

    private IEnumerator DelayTrapTriggerDestruction() {
        yield return new WaitForSeconds(3);
        if (trapInstance != null) {
            photonView.RPC(nameof(RequestDestroy), RpcTarget.MasterClient, trapInstance.GetComponent<PhotonView>().ViewID);
        }
    }
    
    public void CheckIfHasKeyCard(PlayerStatsManager playerStatsManager)
    {
        if (playerStatsManager != null) {
            CustomCharacterMovement characterMovement = playerStatsManager.gameObject.GetComponentInChildren<CustomCharacterMovement>();
            if (!hasKeyCard && characterMovement != null) {
                SpawnTextVFX(characterMovement.vfxEmptyLootPrefab, characterMovement.vfxSpawnPosition, true);
                PlaySoundWithParameter(GetSFX, "hasKey", "False"); //FMOD
            }
            if (playerStatsManager == null || !this.hasKeyCard) return; // returns if the player inventory is null or the object does not have a keycard
            SpawnTextVFX(characterMovement.vfxSuccessLootPrefab, characterMovement.vfxSpawnPosition, true);
            characterMovement.EnableKeycardVFX();
            PlaySoundWithParameter(GetSFX, "hasKey", "True"); //FMOD
            playerStatsManager.UpdateStat(PlayerStatsManager.KeycardsCollected, "add");
            photonView.RPC(nameof(RPC_ResetHasKeycard), RpcTarget.AllBuffered);
            hasKeyCard = false; // set the hasKeyCard bool to false
        }
        // Debug.Log($"Adding 1 keycard count to {playerStatsManager.gameObject.name} with PhotonView ID of Player: {playerPV}");
        // Debug.Log("Keycard picked up.");
        // todo VFX/SFX to signify the keycard has been picked up
    }

    private void TriggerTrapEffect()
    {
        // Check if the event prefab is assigned
        if (eventPrefab != null)
        {
            // Use PhotonNetwork.Instantiate to ensure the effect is instantiated across all clients
            GameObject dpInstance = PhotonNetwork.Instantiate(eventPrefab.name, transform.position, transform.rotation);
            dpInstance.GetComponent<TrapEffect>().SetTriggerPhotonViewID(photonView.ViewID);
            dpInstance.GetComponent<TrapEffect>().replayCamInstance = this.replayCamInstance;
        }
        else
        {
            Debug.LogWarning("Event prefab is not assigned.");
        }

        // Reset the trap flag after triggering
        if (photonView != null)
        {
            photonView.RPC("RPC_ResetTrap", RpcTarget.MasterClient);
        }
        else
        {
            Debug.LogWarning("PhotonView is not assigned.");
        }
    }

    public void SpawnTextVFX(GameObject vfxPrefab, GameObject vfxSpawnPosition, bool setAsChild)
    {
        if (vfxPrefab is not null)
        {
            GameObject vfxInstance = Instantiate(vfxPrefab, vfxSpawnPosition.transform.position, Quaternion.identity);
            if(setAsChild)
            {
                vfxInstance.transform.SetParent(vfxSpawnPosition.transform);
            }
        }
    }

    // method to retrieve the room this interactable object is in 
    public RoomBehavior GetRoomHighlighter()
    {
        RaycastHit roomHit;
        LayerMask floorMask = LayerMask.GetMask("RoomBehavior");
        RoomBehavior room = null;
        if (Physics.Raycast(this.transform.position, -this.transform.up, out roomHit, 20f, floorMask))
        {
            room = roomHit.collider.gameObject.GetComponent<RoomBehavior>();
            if (room == null)
            {
                room = roomHit.collider.gameObject.GetComponentInParent<RoomBehavior>();
            }
            if (room != null)
            {
                // Debug.Log($"Room found {room.name}");
            }
            else
            {
                // Debug.Log("RoomBehavior component not found on the hit object.");
                // Debug.Log($"raycast hit = {roomHit.collider.gameObject.name}");
            }
            return room;
        }
        else
        {
            // Debug.Log("No room found");
            return null;
        }
    }
        public void SetTrap(InteractableObject io, GameObject trap, int trapOwner) {
            if (photonView != null) {
                int ioID = io.GetComponent<PhotonView>().ViewID;
                int trapID = trap.GetComponent<PhotonView>().ViewID;
                photonView.RPC("RPC_SetTrap", RpcTarget.All, ioID, trapID, trapOwner);
            }
            else {
                Debug.LogWarning("PhotonView is not assigned.");
            }
        }
    #endregion

    #region PunCallbacks
    [PunRPC]
    private void RPC_SetTrap(int ioID, int trapID, int trapOwner)
    {
        GameObject ioGO = PhotonView.Find(ioID).gameObject; 
        GameObject trapGO = PhotonView.Find(trapID).gameObject;
        InteractableObject io = ioGO.GetComponent<InteractableObject>();
        trapGO.transform.SetParent(ioGO.transform);
        io.trapInstance = trapGO;
        this.trapOwnerID = trapOwner;
        this.hasTrap = true; // Set the trap flag to true
        io.GetReplayCamInstance();
        // TextRenderer.TRInstance.UpdateText($"Trap has been set on the object with trapOwnerID {trapOwnerID}.");
    }
    [PunRPC]
    private void RPC_ResetTrap() // Reset the trap flag
    {
        this.hasTrap = false;
        // TextRenderer.TRInstance.UpdateText("Trap triggered and reset.");
        this.trapOwnerID = 0;
    }
    [PunRPC]
    private void RPC_ResetHasKeycard() // Reset the trap flag
    {
        this.hasKeyCard = false;
        // TextRenderer.TRInstance.UpdateText($"RPC keycard collection called and bool reset to {hasKeyCard}.");
        GetRoomHighlighter().TurnOffHighlighter();
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
    private void RPC_UpdateInteractingPlayer(Player photonPlayer, PhotonView playerPhotonView, PlayerStatsManager playerStatsManager) {
        this.player = photonPlayer;
        this.playerPV = playerPhotonView;
        this.playerSM = playerStatsManager;
    }

    //Synchronize the hasTrap variable across the Photon network
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (!stream.IsWriting)
        {
            this.hasTrap = (bool)stream.ReceiveNext();
            this.hasKeyCard = (bool)stream.ReceiveNext();
            this.player = (Player)stream.ReceiveNext();
            // this.playerPV = (PhotonView)stream.ReceiveNext();
            // this.playerSM = (PlayerStatsManager)stream.ReceiveNext();
            this.trapOwnerID = (int)stream.ReceiveNext();
            return;
        }
        stream.SendNext(this.hasTrap);
        stream.SendNext(this.hasKeyCard);
        stream.SendNext(this.player);
        // stream.SendNext(this.playerPV);
        // stream.SendNext(this.playerSM);
        stream.SendNext(this.trapOwnerID);
    }

    [PunRPC]
    void RPC_BlinkAndDetonateTrap()
    {
        StartCoroutine(BlinkAndDetonateTrap(effectDelay));
    }
    #endregion 

    #region Coroutines
    IEnumerator BlinkAndDetonateTrap(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            light.SetActive(!light.activeSelf);
            yield return new WaitForSeconds(0.5f); // Blink interval
            elapsed += 0.5f;
        }
        light.SetActive(false);
        TriggerTrapEffect();
    }

    void PlaySoundWithParameter(string soundPath, string parameterName, string parameterValue)
    {
        EventInstance instance = RuntimeManager.CreateInstance(soundPath);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        instance.setParameterByNameWithLabel(parameterName, parameterValue);
        instance.start();
        instance.release();
    }

    public void GetReplayCamInstance() {
        var trap = GetComponentInChildren<DetonatorTrigger>();
        if (trap != null) {
            Debug.Log($"{trap.name} trap found");
            StartCoroutine(Delay(trap));
        } else {
            Debug.Log("No DetonatorTrigger found");
        }
    }

    private IEnumerator Delay(DetonatorTrigger trap) {
        yield return new WaitForSeconds(3.5f);
        this.replayCamInstance = trap.replayCamInstance;
    }
    #endregion
}
