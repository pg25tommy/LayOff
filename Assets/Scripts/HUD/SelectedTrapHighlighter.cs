// Created on Thu Jul 11 2024 || Copyright© 2024 || By: Alex Buzmion II
using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class SelectedTrapHighlighter : MonoBehaviourPun
{
    public static SelectedTrapHighlighter Instance; 
    [SerializeField] GameObject trap1; 
    [SerializeField] GameObject trap2; 
    [SerializeField] GameObject trap3; 
    [SerializeField] GameObject trap4;
    [SerializeField] TextMeshProUGUI title; 
    [SerializeField] TextMeshProUGUI description; 
    [SerializeField] TextMeshProUGUI avoidText; 
    RectTransform rectTrans; 
    private Vector3 stepSize = new Vector3 (.1f,.1f,.1f); 

    private void Awake() {
        Hide();
        Instance = this;
        rectTrans = GetComponent<RectTransform>();
    }

    public void UpdateTrapSelected(int trap) {
        // Debug.Log($"UpdateTrapSelected called with trap: {trap}");

        this.gameObject.SetActive(true);
        // Debug.Log("GameObject set to active");

        switch (trap) {
            case 1: 
                // Debug.Log("Updated parent to trap 1");
                this.rectTrans.SetParent(trap1.transform, false);
                title.text = "Poison Dart";
                description.text = "Set on walls and triggered when a laser detects collision, shooting a poison dart;";
                avoidText.text = "Avoid by crouching or jumping over.";
                break;
            case 2: 
                // Debug.Log("Updated parent to trap 2");
                this.rectTrans.SetParent(trap2.transform, false);
                title.text = "Bouncing Betty";
                description.text = "Set on floors and triggered, springing up a mine that shoots vertical lasers;";
                avoidText.text = "Avoid by crouching.";
                break;
            case 3: 
                // Debug.Log("Updated parent to trap 3");
                this.rectTrans.SetParent(trap3.transform, false);
                title.text = "Skyfall Snare";
                description.text = "Set on floors and triggered, launching a giant metal ball in a straight line;";
                avoidText.text = "Avoid by getting out of the way.";
                break;
            case 4: 
                // Debug.Log("Updated parent to trap 4");
                this.rectTrans.SetParent(trap4.transform, false);
                title.text = "Doom Puff";
                description.text = "Set inside interactable objects, unleashes a cloud explosion after a few seconds;";
                avoidText.text = "Avoid by dashing away.";
                break;
            default:
                // Debug.Log("Invalid trap selected");
                break;
        }

        StartCoroutine(AnimateBig()); 
        // Debug.Log("Started AnimateBig coroutine");

        // Set the local position of the rectTrans to zero on the X axis
        Vector3 newPos = rectTrans.localPosition;
        newPos.x = 0f;
        rectTrans.localPosition = newPos; 
        // Debug.Log("Local position of rectTrans set to zero on the X axis");

        StartCoroutine(CountDownToHide());
        // Debug.Log("Started CountDownToHide coroutine");
    }

    public void Hide() {
        // Debug.Log("Hide method called");
        this.gameObject.SetActive(false);
    }

    private IEnumerator AnimateBig() {
        // Debug.Log("AnimateBig coroutine started");
        this.gameObject.transform.localScale = new Vector3 (.5f,0,1); 
        while (this.gameObject.transform.localScale.x <= 1.2 ) {
            this.gameObject.transform.localScale += stepSize; 
            yield return null;
        }
        this.gameObject.transform.localScale = new Vector3(1,1,1); 
        // Debug.Log("AnimateBig coroutine completed");
    }
    
    private IEnumerator CountDownToHide() {
        // Debug.Log("CountDownToHide coroutine started");
        yield return new WaitForSeconds(5);
        Hide();
        // Debug.Log("CountDownToHide coroutine completed, called Hide method");
    }
}
