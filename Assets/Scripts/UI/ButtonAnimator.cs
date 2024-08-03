// Created on Fri Jul 05 2024 || Copyright© 2024 || By: Alex Buzmion II
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAnimator : MonoBehaviour
{
    [SerializeField] List<GameObject> buttonList;
    private List<Transform> buttonTransformList = new List<Transform>();
    [SerializeField]private float moveHideDirection = 500f;
    [SerializeField]private float moveUnhideDirection = -500f;
    [SerializeField]private float stepSize = 70f;
    [SerializeField]private bool isHidden = false;
    public static ButtonAnimator Instance; 
    int screenWidth;
    
    private void Awake() {
        Instance = this;
        foreach (GameObject button in buttonList) {
            buttonTransformList.Add(button.transform);
        }
        
        screenWidth = Screen.width;
        switch (screenWidth) { // todo add more screen reso types later
            case 1002: 
                break;
            case 1366: 
                break;
            case 2560: 
                break;
            case 3840:
                moveHideDirection += 450f;
                moveUnhideDirection -= 450;
                stepSize += 70;
                break; 
            default:
                break; 
        }
    }

    private void Start() {
        foreach (Transform button in buttonTransformList) {
            button.transform.position = new Vector3 (moveUnhideDirection, button.transform.position.y, button.transform.position.z);
        }
    }
    
    public void Hide() {
        StartCoroutine(HideButtons());
    }

    public void Unhide() {
        StartCoroutine(UnhideButtons());
    }

    private IEnumerator HideButtons() {
        foreach (Transform buttonTransform in buttonTransformList) {
            Vector3 targetPosition = buttonTransform.position + new Vector3(moveUnhideDirection, 0, 0);
            while (buttonTransform.position.x > targetPosition.x) {
                buttonTransform.position += new Vector3(-stepSize, 0, 0);
                yield return new WaitForSeconds(0.01f);
            }
            buttonTransform.position = targetPosition;
        }
        isHidden = true;
    }

    private IEnumerator UnhideButtons() {
        foreach (Transform buttonTransform in buttonTransformList) {
            Vector3 targetPosition = buttonTransform.position + new Vector3(moveHideDirection, 0, 0);
            while (buttonTransform.position.x < targetPosition.x) {
                buttonTransform.position += new Vector3(stepSize, 0, 0);
                yield return new WaitForSeconds(0.01f); // Adjust timing as needed
            }
            buttonTransform.position = targetPosition;
        }
        isHidden = false;
    }
}
