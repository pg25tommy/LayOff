// Created on Wed Jun 26 2024 || Copyright© 2024 || By: Alex Buzmion II
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class KeycardSpriteAtlas : MonoBehaviour
{
    [SerializeField] SpriteAtlas atlas; 
    [SerializeField] string spriteName; 
    // Start is called before the first frame update
    void Start() {
        GetComponent<Image>().sprite = atlas.GetSprite(spriteName);
    }
}
