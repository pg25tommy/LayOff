// Created on Thu Jul 04 2024 || Copyright© 2024 || By: Alex Buzmion II
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Material Data", menuName = "LayOff/ScriptableObjects/MaterialData")]
public class MaterialData : ScriptableObject
{
    [System.Serializable]
    public class MaterialEntry {
        public string key; 
        public List<Material> materials; 
    }

    public List<MaterialEntry> materialEntries;
}
