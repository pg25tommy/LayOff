// Created on Sun May 26 2024 || Copyright© 2024 || By: Alex Buzmion II
using System.Collections;
using TMPro;
using UnityEngine;

public class TextRenderer : MonoBehaviour
{
    public static TextRenderer TRInstance; 
    [SerializeField] private GameObject TRGO; 
    [SerializeField] private TextMeshProUGUI textRenderer; 
    bool isVisible = false; 
    void Awake() {
        TRInstance = this; 
    }
    public void UpdateText(string text) {
        if (textRenderer != null) {
            textRenderer.text = text;
            StartCoroutine(CleanUp());
        }
    }

    public void UpdateTextRenderer(string msg, bool append = true) {
    if (textRenderer == null) return;

    if (append) {
        textRenderer.text += "\n" + msg;
    } else {
        textRenderer.text = msg;
    }
}

    private IEnumerator CleanUp() {
        yield return new WaitForSeconds(3);
        textRenderer.text = "";
    }
    
    public void ToggleVisibility() {
        isVisible = !isVisible;
        TRGO.SetActive(isVisible);
    }
}
