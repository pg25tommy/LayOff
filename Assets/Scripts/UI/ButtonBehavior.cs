// Created on Sun Mar 17 2024 || Copyright (c) 2024 Names Are Hard Studios || By: Alex Buzmion II
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonBehavior : MonoBehaviour
{
    // takes in a string for the scene name and loads the scene
    public void LoadScene(string sceneName) {
        Debug.Log($"Loading Scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    // checks if the application is running in the editor and quits the game, otherwise it quits the application
    public void QuitGame() {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif

        Application.Quit();
        return;
 
    }

    public void CheckMaxPlayerCount(TMP_InputField inputField) {
        int parsedValue;
        if (int.TryParse(inputField.text, out parsedValue)) {
            if (parsedValue > 4 || parsedValue == 0) {
                StartCoroutine(AnimateError(inputField));
            }
            return;
        } else {
            Debug.LogWarning("Invalid input. Please enter a valid number.");
            StartCoroutine(AnimateError(inputField));
        }
    }

    public IEnumerator AnimateError(TMP_InputField field) {
        Vector3 originalSize = field.gameObject.transform.localScale;
        Vector3 stepSize = new Vector3(.01f, .01f, .01f);
        field.interactable = false;
        for (int i = 0; i < 10; i++) {
            field.gameObject.transform.localScale += stepSize; 
            yield return new WaitForSeconds(.015f);
        }
        field.text = "4";
        field.interactable = true;
        field.gameObject.transform.localScale = originalSize;
    }
}
