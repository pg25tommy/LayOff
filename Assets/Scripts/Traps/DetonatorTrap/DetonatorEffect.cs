// Created on Sun Mar 17 2024 || Copyright (c) 2024 Names Are Hard Studios || By: Alex Buzmion II
using System.Collections;
using FMODUnity;
using UnityEngine;
using Photon.Pun;
using CharacterMovement;
using Sirenix.OdinInspector;

public class DetonatorEffect : TrapEffect
{
    [SerializeField] protected float effectSpeed = 10f;

    private bool isGrowing = true;
    public float dissipationTime = 1f; // rate of dissipation
    
    protected override void Awake() {
        base.Awake();
        trapType = 4;
    }
    private void Update() 
    {
        // Gas VFX logic
        transform.position -= new Vector3(0, 1, 0) * Time.deltaTime * dissipationTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other != null)
            {
                if(other.GetComponent<CustomCharacterMovement>().IsDead) return;
                //onDPHitPlayerEvent.Raise(this, 5.5f);

                // Kill Player
                other.gameObject.GetComponent<CustomCharacterMovement>().OnDeath(ownerID, trapType);

                // Spawn VFX
                SpawnTrapHitVFX(other.transform);

                // Spawn SFX
                SpawnTrapHitSFX();


                // StartCoroutine(DestroyTrapEffect());
            }
            else
            {
                Debug.Log("Player Hit but not affected");
            }
            SendKillUpdate(other.GetComponent<Collider>());
        }
    }

    protected override void SpawnTrapHitVFX(Transform vfxSpawnTransform)
    {
        base.SpawnTrapHitVFX(vfxSpawnTransform);
    }

    protected override IEnumerator DestroyTrapEffect()
    {
        return base.DestroyTrapEffect();
    }
}