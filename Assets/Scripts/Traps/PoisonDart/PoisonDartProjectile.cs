// Created on Sun Mar 17 2024 || Copyright (c) 2024 Names Are Hard Studios || By: Alex Buzmion II
using UnityEngine;
using Photon.Pun;
using System.Collections;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Rigidbody))]

public class PoisonDartProjectile : TrapEffect
{
    public float speed = 100.0f; // the speed of the projectile
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float maxDistance = 10f; // the damage of the projectile
    private Vector3 spawnPosition;

    // Start is called before the first frame update
    private new void Awake()
    {
        // Launch Poison Dart
        rb.velocity = transform.forward * speed;
        spawnPosition = transform.position;
        StartCoroutine(DestroyTrapEffect());
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
