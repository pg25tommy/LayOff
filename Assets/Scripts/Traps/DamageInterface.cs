using FMODUnity;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class DamageInterface : MonoBehaviour
{
    [field: SerializeField, BoxGroup("Source")] public float EffectiveRange { get; protected set; } = 5f;// distance at which AI will use weapon
    [field: SerializeField, BoxGroup("Source")] public float DamageAmount { get; protected set; } = 5f; // The amount of damage that the source will be doing
    [field: SerializeField, BoxGroup("Source")] public float Cooldown { get; protected set; } = 0.5f; // The cooldown of the source
    [field: SerializeField, BoxGroup("Source")] public float Duration { get; protected set; } = 0.5f; // The duration of the damage by the source

    //The last time damage was done
    private float _lastAttackTime;

    //Method that checks if the weapon can apply the damage
    public bool TryAttack(Vector3 aimPosition, int team, GameObject instigator)
    {
        // current time is less that next attack time
        if (Time.time < _lastAttackTime + Cooldown) return false;
        // set last time to now
        _lastAttackTime = Time.time;

        Attack(aimPosition, team, instigator);

        return true;
    }

    //Base method to do damage, should be overrided by the inherited classes to implement their own damage logic
    protected virtual void Attack(Vector3 aimPosition, int team, GameObject instigator)
    {
        
    }
}
