// Created on Wednesday April 06 2024 || Copyright (c) 2024 Names Are Hard Studios || By: Tommy Minter / Tiago Corsato
using System.Collections;
using CharacterMovement;
using CharacterSelect;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Keycard : MonoBehaviourPunCallbacks, IPunObservable
{
    #region Private Fields
    private Vector3 startPosition;
    [SerializeField] Collider collider;
    [SerializeField] Collider collectionTrigger;
    [SerializeField] GameObject highlighter;
    Transform highlighterTransform;
    [SerializeField] private CustomCharacterMovement owner;
    [SerializeField] private bool isCollected = false;
    [SerializeField] private GameObject vfxCollection;
    [SerializeField] private GameObject vfxTextCollection;
    PhotonView playerPV;
    [SerializeField] private Camera minimapCam;
    [SerializeField] GameObject minimapIcon;

    #endregion

    #region Monobehavior Callbacks

    private void Awake()
    {
        highlighterTransform = highlighter.GetComponent<Transform>();
        minimapCam = FindAnyObjectByType<MinimapFollowCam>().GetComponent<Camera>();
    }
    private void Start()
    {
        // add force impulse upon instantiation
        GetComponent<Rigidbody>().AddForce(new Vector3(.5f, 1, 0) * 1, ForceMode.Impulse);
        StartCoroutine(nameof(Hover));
    }

    private void LateUpdate()
    {
        minimapIcon.transform.rotation = Quaternion.Euler(minimapIcon.transform.rotation.x, minimapIcon.transform.rotation.y, -minimapCam.transform.rotation.z);
    }
    // Called when the keycard collides with another object
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        // Check if the object collided with is a player
        playerPV = other.GetComponentInParent<PhotonView>();
        other.gameObject.TryGetComponent<CustomCharacterMovement>(out CustomCharacterMovement player);
        if (player != null)
        {

            if (player.IsDead) return; // if already dead, return
            if (this.isCollected) return; // if the card is collected return (network efficiency)
            this.isCollected = true; // set keycard collection 
                                     // get the playerStats component from the player
            PlayerStatsManager playerStatsManager = playerPV.gameObject.GetComponentInParent<PlayerStatsManager>();
            if (playerStatsManager == null) return; // returns if no playerStatsManager found
                                                    // Debug.Log($"Adding 1 keycard count to {playerStatsManager.gameObject.name} with PhotonView ID of Player: {playerPV}");
            playerStatsManager.UpdateStat(PlayerStatsManager.KeycardsCollected, "add");

            SpawnVFX(vfxCollection, player.transform.position);
            player.GetComponent<CustomCharacterMovement>().EnableKeycardVFX();
            SpawnTextVFX(vfxTextCollection, player.GetComponent<CustomCharacterMovement>().vfxSpawnPosition);
            // destroy card on collision with Player
            photonView.RPC(nameof(RPC_DestroyCard), RpcTarget.MasterClient);
        }
    }

    Vector3 vfxOffset = new Vector3(0, 0.7f, 0);
    // Can be called to spawn any VFX with a prefab, spawn position and life time.
    private void SpawnVFX(GameObject vfxPrefab, Vector3 vfxSpawnTransform)
    {
        if (vfxPrefab != null)
        {
            GameObject vfxInstance = PhotonNetwork.Instantiate(vfxPrefab.name, vfxSpawnTransform + vfxOffset, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning($"{vfxPrefab.name} has not been assigned in the inspector!");
        }
    }

    public void SpawnTextVFX(GameObject vfxPrefab, GameObject vfxSpawnPosition)
    {
        if (vfxPrefab is not null)
        {
            GameObject vfxInstance = Instantiate(vfxPrefab, vfxSpawnPosition.transform.position, Quaternion.identity);
            vfxInstance.transform.SetParent(vfxSpawnPosition.transform);
        }
    }
    #endregion

    #region Coroutines    
    private IEnumerator Hover()
    {
        yield return new WaitForSeconds(1);
        collectionTrigger.enabled = true;
        yield return new WaitForSeconds(3);
        collider.enabled = false;
        highlighterTransform.position = transform.position;
        highlighter.SetActive(true);
        float hoverHeight = .25f; // The amount to hover up and down
        float hoverSpeed = 1.5f;  // The speed of the hover movement

        Vector3 startPos = transform.parent.position + new Vector3(0, 1, 0);
        transform.rotation = new Quaternion(0, 0, 0, 0);

        while (!isCollected)
        {
            float newY = startPos.y + Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            yield return null; // Wait for the next frame
        }
    }
    #endregion

    #region PhotonCallback
    [PunRPC]
    public void RPC_DestroyCard()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Debug.Log($"RPC DestroyCard called, destroying {gameObject}");
            // PhotonNetwork.Destroy(highlighter);
            PhotonNetwork.Destroy(gameObject.transform.parent.gameObject);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (!stream.IsWriting)
        {
            isCollected = (bool)stream.ReceiveNext();
            return;
        }
        stream.SendNext(isCollected);
    }
    #endregion
}
