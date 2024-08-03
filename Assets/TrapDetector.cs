using UnityEngine;
using Photon.Pun;
using System.Collections;

public class TrapDetector : MonoBehaviourPunCallbacks
{
    public float maxDistance = 2f;
    public LayerMask trapLayer;
    public LayerMask ignoreLayer;

    [SerializeField] private GameObject vfx_ExclamationMark;
    public float radius = 1f;

    RaycastHit hit;

    void Update()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        RaycastHit hit;
        if (Physics.SphereCast(origin, radius, direction, out hit, maxDistance, trapLayer))
        {
            // Collision detected
            Debug.DrawRay(origin, direction * hit.distance, Color.red);
            StopAllCoroutines();
            StartCoroutine(EnableVFX());
        }
        else
        {
            // No collision detected
            Debug.DrawRay(origin, direction * maxDistance, Color.green);
        }
    }

    IEnumerator EnableVFX()
    {
        if(photonView.IsMine)
        {
            vfx_ExclamationMark.SetActive(true);
            //vfx_ExclamationMark.GetComponent<SpriteRenderer>().color = GameManager.Instance.GetPlayerColor(PhotonNetwork.LocalPlayer.ActorNumber);
            yield return new WaitForSeconds(1f);
            vfx_ExclamationMark.SetActive(false);
        }
    }
}
