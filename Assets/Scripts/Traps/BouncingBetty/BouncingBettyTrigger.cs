// Created on Wednsday May 8 2024 || Copyright (c) 2024 Names Are Hard Studios || By: Alex Buzmion II, Tiago Corsato
using System.Collections;
using UnityEngine;
using FMODUnity;

public class BouncingBettyTrigger : TrapTrigger
{
    // [SerializeField]
    // private GameObject trapEffectPrefab; 
    [SerializeField] private float launchHeight = 0.2f; // Default launch height
    [SerializeField] private float launchSpeed = 6f; // Default launch speed
    [SerializeField] private float _spawnDistance = 2f;
    [SerializeField] protected EventReference ActivateSFX;


    protected override void Awake()
    {
        base.Awake();
    }
    protected override void OnTriggerEnter(Collider other)
    {
         if(other.tag == "Player" && !IsActivated)
         {
             Vector3 spawnPosition =  other.gameObject.transform.position + (other.gameObject.transform.forward * _spawnDistance); 
             trapEffectSpawnPosition.transform.position = new Vector3(spawnPosition.x, trapEffectSpawnPosition.transform.position.y, spawnPosition.z);
             RuntimeManager.PlayOneShot(ActivateSFX, transform.position); // FMOD Sound event playing

         }
        base.OnTriggerEnter(other);
    }

    protected override void SpawnTrapEffect()
    {
        // Instantiate the first prefab
        //GameObject trapEffectInstance = Instantiate(trapEffectPrefab, transform.position, Quaternion.identity);
        // Add a Rigidbody and set its velocity to launch it upwards
        trapPreEffectPrefab.GetComponent<Rigidbody>().velocity = Vector3.up * launchSpeed; // launch speed 
        base.SpawnTrapEffect();

    }

    protected override void DestroyTrigger()
    {
        base.DestroyTrigger();
    }

    protected override void OnPreEffectSpawned() {
        StartCoroutine(BBAnimate());
    }

    private IEnumerator BBAnimate() {
        Vector3 startPos = preEffectInstance.transform.position;
        Vector3 endPos = trapEffectSpawnPosition.transform.position;
        float step = 0.25f; // step for each frame


        while (Vector3.Distance(preEffectInstance.transform.position, endPos) > 0.01f) {
            preEffectInstance.transform.position = Vector3.MoveTowards(preEffectInstance.transform.position, endPos, step);
            yield return null;
        }
        preEffectInstance.transform.position = endPos; // ensures the final position is set and always constant
    }
}