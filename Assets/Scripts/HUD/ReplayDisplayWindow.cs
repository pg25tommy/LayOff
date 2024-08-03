// Created on Sun Jul 21 2024 || Copyright© 2024 || By: Alex Buzmion II
using UnityEngine;
using UnityEngine.UI;

public class ReplayDisplayWindow : MonoBehaviour
{
    public static ReplayDisplayWindow Instance;
    public RawImage rawImage;

    private void Awake() {
        Instance = this;
        rawImage = GetComponent<RawImage>();
        // HideReplayWindow();
    }

    void Start() {
    }

    public void ShowReplayWindow() {
        rawImage.enabled = true;
    }
    public void HideReplayWindow() {
        rawImage.enabled = false;
    }
}
