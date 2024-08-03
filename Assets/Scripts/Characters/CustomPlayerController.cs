// Created on Wed May 15 2024 || Copyright© 2024 || By: Alex Buzmion II
using CharacterMovement;
using System.Collections;
using TMPro;
using UnityEngine;

public class CustomPlayerController : PlayerController
{   
    public string PlayerID { get; private set; }
    
    private TextMeshProUGUI textRenderer;
    [SerializeField] private GameObject playerTextRenderer;

    protected override void Awake()
    {
        //textRenderer = playerTextRenderer.GetComponentInChildren<TextMeshProUGUI>();
        base.Awake();
        DisableLookInCameraDirection();
    }

    public void UpdateText(string text)
    {
        playerTextRenderer.SetActive(true);
        textRenderer = playerTextRenderer.GetComponentInChildren<TextMeshProUGUI>();
        textRenderer.text = text;
    }

    public void DisableText()
    {
        textRenderer = playerTextRenderer.GetComponentInChildren<TextMeshProUGUI>();
        textRenderer.text = "";
        playerTextRenderer.SetActive(false);
    }

    public void EnableLookInCameraDirection()
    {
        _lookInCameraDirection = true;
    }

    public void DisableLookInCameraDirection()
    {
        _lookInCameraDirection = false;
    }
}
