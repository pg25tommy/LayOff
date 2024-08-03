using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VFX", menuName = "Scriptables/VFX")]
public class VisualEffectsSO : ScriptableObject
{
    public GameObject vfxPrefab;
    public float vfxLifeTime;
}
