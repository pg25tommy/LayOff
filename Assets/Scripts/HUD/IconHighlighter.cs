// Created on Thu Apr 11 2024 || Copyright© 2024 || By: Alex Buzmion II
using System.Collections;
using UnityEngine;

public class IconHighlighter : MonoBehaviour
{
    [SerializeField] private RectTransform imageTransform; // assign the RectTransform of the image in the inspector
    [SerializeField] private float pulseDuration = 1.0f; // duration of each size pulse
    [SerializeField] private Vector2 minSize = new Vector2(35, 35f); // Minimum size
    [SerializeField] private Vector2 maxSize = new Vector2(40, 40f); // Maximum size
    private Coroutine pulseCoroutine; // Reference to the coroutine

    private void OnEnable()
    {
        // Check if the gameObject is active and if the coroutine is not already running
        if (gameObject.activeInHierarchy && pulseCoroutine == null)
        {
            pulseCoroutine = StartCoroutine(PulseSize());
        }
    }

    private void OnDisable()
    {
        // Stop the coroutine when the gameObject is no longer active
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
        }
    }

    IEnumerator PulseSize()
    {
        float elapsedTime = 0f;
        while (gameObject.activeInHierarchy) // Loop while the object is active
        {
            float t = (Mathf.Sin(elapsedTime / pulseDuration * Mathf.PI * 2) + 1f) / 2f;
            imageTransform.sizeDelta = Vector2.Lerp(minSize, maxSize, t);

            elapsedTime += Time.deltaTime;
            if (elapsedTime > pulseDuration)
            {
                elapsedTime = 0f;
            }

            yield return null;
        }
    }
}
