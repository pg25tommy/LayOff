// Created on Tue May 21 2024 || Copyright© 2024 || By: Alex Buzmion II
using System.Collections;
using UnityEngine;

public class RoomHighlighter : MonoBehaviour
{
    [SerializeField] private float blinkInterval = 0.5f;
    [SerializeField] private bool isBlinking = false;
    private Coroutine blinkCoroutine;

    void Awake() {
        gameObject.SetActive(false);
    }

    public void StartBlinking()
    {
        if (!isBlinking)
        {
            Debug.Log("Starting blinking...");
            isBlinking = true;
            blinkCoroutine = StartCoroutine(Blink());
        }
    }

    public void StopBlinking()
    {
        if (isBlinking)
        {
            Debug.Log("Stopping blinking...");
            isBlinking = false;
            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
                blinkCoroutine = null;
            }
            gameObject.SetActive(false);
            
        }
    }

    private IEnumerator Blink()
    {
        while (isBlinking)
        {
            gameObject.SetActive(!gameObject.activeSelf);
            yield return new WaitForSeconds(blinkInterval);
        }
        gameObject.SetActive(true);
    }
}
