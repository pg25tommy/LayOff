using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using Photon.Pun;
using CharacterMovement;
using UnityEngine.Rendering.HighDefinition;

public class BouncingBettyEffect : TrapEffect
{

    [SerializeField] protected EventReference _bouncingBettyVA;
    [SerializeField] protected float effectSpeed = 10f;
    [SerializeField] protected float timeToExplode = 1f;

    [SerializeField] private float roomSize = 30f; // The size of the room
    [SerializeField] public LayerMask floorMask;

    private List<CustomCharacterMovement> visibleTargets;

    private LocalVolumetricFog _fogEffect;

    public LineRenderer lineRendererPrefab;

    public List<LineRenderer> lineRenderers = new List<LineRenderer>();

    protected override void Awake()
    {
        base.Awake();
        trapType = 2; // set trap type to 2
        _fogEffect = GetComponent<LocalVolumetricFog>();


        // Initialize a line renderer for each ray
        for (int i = 0; i < numberOfRays; i++)
        {
            //if (PhotonNetwork.IsMasterClient)
            //{ 
            //    GameObject lr = PhotonNetwork.Instantiate(lineRendererPrefab.gameObject.name, transform.position, Quaternion.identity);
            //    lr.transform.SetParent(this.transform);
            //    lr.GetComponent<LineRenderer>().positionCount = 2;
            //    lineRenderers.Add(lr.GetComponent<LineRenderer>());
            //}
            foreach(LineRenderer lr in lineRenderers)
            {
                //lr.gameObject.SetActive(true);
            }
        }

        StartCoroutine(DestroyTrapEffect());
    }

    private void Update()
    {
        LaserRayCast();

        //Vector3 startPoint = transform.position; // Start point of the explosion
        //RaycastHit hit;
        //if (Physics.Raycast(startPoint, -transform.up, out hit, 100f, floorMask))
        //{
        //    if (hit.transform.gameObject.TryGetComponent(out RoomBehavior currentRoom))
        //    {
        //        roomSize = currentRoom.GetComponent<BoxCollider>().size.x;
        //    }
        //}
        //else
        //{
        //    Vector3 vectorIncrease = new Vector3(roomSize, 0f, roomSize);
        //    transform.localScale = vectorIncrease;
        //    Vision._range = vectorIncrease;
        //    _fogEffect.parameters.size = new Vector3(roomSize, 1f, roomSize);
        //}


        //if (transform.localScale.x < roomSize && transform.localScale.z < roomSize)
        //{
        //    // If the scale of the effect is less than the room size
        //    float scaleIncrease = Mathf.Clamp(5 * Time.deltaTime * effectSpeed, 0, roomSize - transform.localScale.x); // clamp the scale increase to the room size minus the current scale
        //    Vector3 vectorIncrease = new Vector3(scaleIncrease, 0f, scaleIncrease);
        //    transform.localScale += vectorIncrease;
        //    Vision._range += vectorIncrease;
        //    _fogEffect.parameters.size = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
        //}

        //visibleTargets = Vision.GetVisibleTargets();
        //for (int i = 0; i < visibleTargets.Count; i++)
        //{
        //    if (visibleTargets[i] is not null && !visibleTargets[i].IsDead && !visibleTargets[i].IsCrouching)
        //    {
        //        visibleTargets[i].OnDeath();
        //    }
        //}
    }

    public int numberOfRays = 8;
    public float rayDistance = 10f;
    public LayerMask playerLayer;
    public LayerMask ignoreLayer;

    // protected override void SendReplay()
    // {
    //     base.SendReplay();
    // }
    void LaserRayCast()
    {
        // Calculate angle between each ray
        float angleBetweenRays = 360f / numberOfRays;
        Vector3 raycastOrigin = transform.position;

        for (int i = 0; i < numberOfRays; i++)
        {
            // Calculate direction of the current ray
            float angle = i * angleBetweenRays;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;

            RaycastHit hit;
            Vector3 endPosition = raycastOrigin + direction * rayDistance;

            // Perform a raycast, including the player
            if (Physics.Raycast(raycastOrigin, direction, out hit, rayDistance, ~ignoreLayer))
            {
                if (((1 << hit.collider.gameObject.layer) & playerLayer) != 0)
                {
                    // TODO : Kill Player
                    hit.collider.TryGetComponent<CustomCharacterMovement>(out CustomCharacterMovement player);
                    if (player != null && !player.IsDead) {
                        // SendReplay();
                        player.OnDeath(ownerID, trapType);
                        SendKillUpdate(hit.collider);
                        RuntimeManager.PlayOneShot(explosionSFX, transform.position); // FMOD Sound event playing
                        RuntimeManager.PlayOneShot(_bouncingBettyVA); // Plays a random voiceline for the bouncing Betty from the speakers
                    }
                }
                else
                {
                    // Hit a non-player object, set end position to hit point
                    endPosition = hit.point;
                }

                //Debug.Log("Hit: " + hit.collider.name);
                Debug.DrawLine(raycastOrigin, hit.point, Color.red);
            }

            // Perform a second raycast ignoring the player
            RaycastHit hitNonPlayer;
            if (Physics.Raycast(raycastOrigin, direction, out hitNonPlayer, rayDistance, ~ignoreLayer & ~playerLayer))
            {
                endPosition = hitNonPlayer.point;
            }

            // Set the line renderer to the end point
            lineRenderers[i].SetPosition(0, raycastOrigin);
            lineRenderers[i].SetPosition(1, endPosition);

            if (endPosition == raycastOrigin + direction * rayDistance)
            {
                Debug.DrawRay(raycastOrigin, direction * rayDistance, Color.green);
            }
        }
    }

    [PunRPC]
    protected virtual void RequestDestroy(int viewID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Request Destroy called inside master client");
            PhotonView objectToDelete = PhotonView.Find(viewID);
            if (objectToDelete != null)
            {
                PhotonNetwork.Destroy(objectToDelete.gameObject);
            }
        }
    }

    protected override IEnumerator DestroyTrapEffect()
    {
        return base.DestroyTrapEffect();
    }
}
