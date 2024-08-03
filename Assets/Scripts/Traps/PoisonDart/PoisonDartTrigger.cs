using UnityEngine;
using Photon.Pun;
using CharacterMovement;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;

public class PoisonDartTrigger : TrapTrigger
{
    Ray ray;
    RaycastHit rayCastHit;
    int trapType = 1;
    int trapPosition = 0;
    bool killUpdateSent = false; 
    public LayerMask shoveLayer;
    public LayerMask floorLayer;
    public LayerMask wallLayer;
    public LayerMask propsLayer;
    public LayerMask interactablesLayer;

    public float rayDistance = 10f;
    public GameObject laserEndPoint;

    public LineRenderer lineRenderer;

    public LayOffGameEvent onPDHitPlayerEvent;

    [SerializeField] protected EventReference explosionSFX;
    [SerializeField] protected EventReference _poisonDartVA;

    public Material localLaserMaterial;

    public GameObject localTrapIndicator2;
    private bool playbackSent;

    protected override void Awake()
    {
        if (this.transform.position.y >= 1.5f) {
            trapPosition = 1; 
        }
        base.Awake();
        laserEndPoint.transform.localPosition = new Vector3(0, 0.6f, rayDistance);
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, trapEffectSpawnPosition.transform.position);
        lineRenderer.SetPosition(1, laserEndPoint.transform.position);

        localTrapIndicator2.SetActive(false);
    }
    public void SendReplay() {
        if (!playbackSent) {
            Debug.Log("Replay Sent");
            playbackSent = true;
            replayCamInstance.cctvCam.InitiateReplay(ownerID, replayCamInstance);
        }
    }
    private void Update()
    {
        if(isTrapActive)
        {
            if (this.ownerID == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                //AnimationCurve curve = new AnimationCurve();
                //curve.AddKey(1f, 1f);
                //lineRenderer.widthCurve = curve;
                lineRenderer.material = localLaserMaterial;
                localTrapIndicator2.SetActive(true);
                localTrapIndicator2.GetComponent<SpriteRenderer>().color = GameManager.Instance.GetPlayerColor(this.ownerID);
            }
            else
            {
                localTrapIndicator2.SetActive(false);
            }

            #region HandleLaserRaycast

            ray = new Ray(trapEffectSpawnPosition.transform.position, trapEffectSpawnPosition.transform.forward);

            if (Physics.Raycast(ray, out rayCastHit, rayDistance, ~shoveLayer + ~floorLayer + ~propsLayer + ~interactablesLayer))
            {
                lineRenderer.SetPosition(1, rayCastHit.point);

                if (rayCastHit.collider.CompareTag("Player"))
                {
                    CustomCharacterMovement player = rayCastHit.collider.gameObject.GetComponent<CustomCharacterMovement>();
                    if (player.IsDead) return;
                    // SendReplay();
                    if (player != null) {
                        if (trapPosition == 1 && !player.IsCrouching) { // if trap is set at shoulder height and is not crouching, kill player
                            StartCoroutine(ActivateTrap(rayCastHit.collider.gameObject));
                            RuntimeManager.PlayOneShot(explosionSFX, transform.position); // FMOD Sound event playing
                            Debug.Log($"player is crouching {player.IsCrouching}");
                            player.OnDeath(ownerID, trapType);
                            RuntimeManager.PlayOneShot(_poisonDartVA); // Plays a random voiceline for the poison dart from the speakers
                            if (killUpdateSent) return; // ensure kill update is only sent once
                            killUpdateSent = true;
                            SendKillUpdate(rayCastHit.collider); // send the kill update
                            return; 
                        } 
                        // if trap is set at ankle height and there is still player collision kill. 
                        StartCoroutine(ActivateTrap(rayCastHit.collider.gameObject));
                        RuntimeManager.PlayOneShot(explosionSFX, transform.position); // FMOD Sound event playing
                        Debug.Log($"player is crouching {player.IsCrouching}");
                        player.OnDeath(ownerID, trapType);

                        if (killUpdateSent) return; // ensure kill update is only sent once
                        killUpdateSent = true;
                        SendKillUpdate(rayCastHit.collider); // send the kill update
                    }
                
                }
            }

            #endregion
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    protected override IEnumerator ActivateTrap(GameObject player)
    {
        return base.ActivateTrap(player);
    }

    protected override void SpawnTrapEffect()
    {
        base.SpawnTrapEffect();
        if (effectInstance != null)
        {
            effectInstance.GetComponent<Rigidbody>().velocity = transform.forward * effectInstance.GetComponent<PoisonDartProjectile>().speed;
        }
    }

    protected override void DestroyTrigger()
    {
        base.DestroyTrigger();
    }

    // copied and modified from TrapEffect base class due to kill implementation of PoisonDart 
    protected virtual void SendKillUpdate(Collider collider) {
        PhotonView victimPV = collider.transform.parent.GetComponent<PhotonView>(); // retreive the players PV
        List<int> victimIDs = new List<int>();
        int trapType = 1;
        int victim = victimPV.OwnerActorNr; // assign the PV OwnerActorNr as victim
        if (!victimIDs.Contains(victim)) { // ensure the victim is not yet in the list
            victimIDs.Add(victim); // add victim in the list
        }
        victimPV.RPC("RPC_UpdateKills", RpcTarget.All, trapType, ownerID, victimIDs.ToArray()); // send RPC to all to update their stats
        victimIDs.Clear(); // clear the list
    }
}
