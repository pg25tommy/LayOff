using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ToonLightData : MonoBehaviour
{
    [SerializeField] private Light _light;

    private void OnValidate()
    {
        if(_light == null) _light = GetComponent<Light>();
    }

    private void Update()
    {
        if (_light == null) return;
        Shader.SetGlobalColor("_MainLightColor", _light.color);
        Shader.SetGlobalFloat("_MainLightIntensity", _light.intensity);
    }
}