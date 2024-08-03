// Created on Wednsday May 8 2024 || Copyright (c) 2024 Names Are Hard Studios || By: Alex Buzmion II, Tiago Corsato
using UnityEngine;
using FMODUnity;

public class CeilingTrapTrigger : TrapTrigger
{
    [SerializeField] private float _spawnDistance = 3f;    
    Collider player;

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !IsActivated)
        {
            player = other;
        }
        base.OnTriggerEnter(other);
    }

    protected override void SpawnTrapEffect()
    {
        base.SpawnTrapEffect();

        if (player != null && effectInstance != null)
        {
            trapEffectSpawnPosition.transform.position = new Vector3(player.transform.position.x, trapEffectSpawnPosition.transform.position.y, player.transform.position.z);

            Vector3 targetDirection = player.transform.position - trapEffectSpawnPosition.transform.position;

            //Quaternion newRotation = Quaternion.LookRotation(-targetDirection);

            effectInstance.transform.eulerAngles = new Vector3(effectInstance.transform.eulerAngles.x, player.transform.eulerAngles.y, effectInstance.transform.eulerAngles.z);
            effectInstance.transform.position = new Vector3(player.transform.position.x, trapEffectSpawnPosition.transform.position.y, player.transform.position.z);
        }
    }


    protected override void DestroyTrigger()
    {
        base.DestroyTrigger();
    }
}
