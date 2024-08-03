// Created on Sun Mar 17 2024 || Copyright (c) 2024 Names Are Hard Studios || By: Alex Buzmion II
using System.Collections;
using Photon.Pun;
using UnityEngine;
using FMODUnity;

public class CeilingTrapEffect : TrapEffect
{
    [SerializeField] protected EventReference explosionSFX;
    [SerializeField] protected EventReference _ceilingTrapVA;

    public LayOffGameEvent onSSHitPlayerEvent;
    private LineRenderer lineRenderer;
    public Transform hanger;

    protected override void Awake()
    {
        base.Awake();
        trapType = 3;
        lineRenderer = GetComponent<LineRenderer>();
        StartCoroutine(DestroyTrapEffect());
        lineRenderer.positionCount = 2;
    }

    private void Update()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, hanger.position);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if(collision.gameObject.CompareTag("Player")) {
            if(collision != null)  {
                RuntimeManager.PlayOneShot(_ceilingTrapVA); // FMOD play random voiceline when it hits the player
                RuntimeManager.PlayOneShot(explosionSFX); // FMOD Sound event playing

            }
        }
        Vector3 normalDir = Vector3.Normalize(transform.position);
        collision.gameObject.GetComponent<Rigidbody>().AddForce(normalDir * 35.0f, ForceMode.Impulse);
    }

    protected override void SpawnTrapHitVFX(Transform vfxSpawnTransform)
    {
        base.SpawnTrapHitVFX(vfxSpawnTransform);
    }

    protected override IEnumerator DestroyTrapEffect()
    {
        yield return new WaitForSeconds(trapEffectLifeTime);
        if (GetComponentInParent<PhotonView>().IsMine)
            PhotonNetwork.Destroy(transform.parent.gameObject);
    }
}
