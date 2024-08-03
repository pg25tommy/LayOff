// Created on Sun Mar 17 2024 || Copyright (c) 2024 Names Are Hard Studios || By: Alex Buzmion II
using System;
using System.Collections;
using FMODUnity;
using NUnit.Framework;
using UnityEngine;
using Photon.Pun;

public class DetonatorTrigger : TrapTrigger
{
    protected override void Awake() {
        base.Awake();
        this.replayCamInstance = base.replayCamInstance;
    }
    protected override void SpawnTrapEffect() 
    {
        base.SpawnTrapEffect();
    }

    protected override void DestroyTrigger()
    {
        base.DestroyTrigger();
    }
}
