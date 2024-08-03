// Created on Fri Jul 05 2024 || Copyright© 2024 || By: Alex Buzmion II
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MaterialColorChanger : MonoBehaviourPun
{
    [SerializeField] MaterialData variantData; 
    [SerializeField] Renderer[] renderersM1;
    [SerializeField] Renderer[] renderersM2;
    [SerializeField] Renderer[] renderersM3;
    private Dictionary<string, List<Material>> materialDictionary;
    [SerializeField] Cloth skirtCloth; 
    private void Awake() {
        materialDictionary = new Dictionary<string, List<Material>>();
        foreach (var entry in variantData.materialEntries) {
            materialDictionary.Add(entry.key, entry.materials);
        }
    }

    public void ChangePlayerMaterial(string colorKey) {
        photonView.RPC(nameof(RPC_ChangePlayerMaterial), RpcTarget.AllBuffered, colorKey);
    }

    [PunRPC]
    public void RPC_ChangePlayerMaterial(string colorKey) {
        if (materialDictionary.ContainsKey(colorKey)) {
            List<Material> materials = materialDictionary[colorKey];
            for (int i = 0; i < renderersM1.Length; i++) {
                if (i < materials.Count) {
                    renderersM1[i].material = materials[0];
                }
            }
            for (int i = 0; i < renderersM2.Length; i++) {
                if (i < materials.Count) {
                    renderersM2[i].material = materials[1];
                }
            }
            for (int i = 0; i < renderersM3.Length; i++) {
                if (i < materials.Count) {
                    renderersM3[i].material = materials[2];
                }
            }

        }
    }

    public void EnableDisableCloth() {
        skirtCloth.enabled = false; 
        skirtCloth.enabled = true; 
    }

}
