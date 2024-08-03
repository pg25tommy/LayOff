// Created on Wed Jul 17 2024 || Copyright© 2024 || By: Alex Buzmion II
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OtherPlayerStatEntry : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI playerNickname; 
    [SerializeField] public TextMeshProUGUI totalKills; 
    [SerializeField] public TextMeshProUGUI totalDeaths; 
    [SerializeField] public TextMeshProUGUI totalTrapsSet; 
    [SerializeField] public TextMeshProUGUI trap1Set; 
    [SerializeField] public TextMeshProUGUI trap2Set; 
    [SerializeField] public TextMeshProUGUI trap3Set; 
    [SerializeField] public TextMeshProUGUI trap4Set; 
    [SerializeField] public TextMeshProUGUI trap1Kill; 
    [SerializeField] public TextMeshProUGUI trap2Kill; 
    [SerializeField] public TextMeshProUGUI trap3Kill; 
    [SerializeField] public TextMeshProUGUI trap4Kill; 
    [SerializeField] public TextMeshProUGUI footerText; 
    [SerializeField] public Image footerImage;
    [SerializeField] public Image playerImage;
    [SerializeField] public Image winStampImage;
    [SerializeField] public Image LoseStampImage;
    [SerializeField] public GameObject winnerBorder;

    [SerializeField] public List<TextMeshProUGUI> dataTMPs;
    private float originalFontSize = 24; 
    private float targetFontSize = 32;
    private RectTransform rectTransform;
    private Vector2 originalSize;
    private Vector2 targetSize = new Vector2(550, 770); 
    private float animationDuration = 0.2f;
    private bool isScaled = false;

    private void Start(){
        rectTransform = GetComponent<RectTransform>();
        originalSize = rectTransform.sizeDelta;
    }

    // public void OnPointerClick(PointerEventData eventData) {
    //     if (!isScaled) {
    //         isScaled = true;
    //         StopAllCoroutines();
    //         StartCoroutine(ScaleToSize(targetSize, targetFontSize));
    //         StartCoroutine(UIEndGame.Instance.ScaleLocalPlayerCard(new Vector2(420, 587), 24f));
    //     }
    // }

    // public void OnPointerExit(PointerEventData eventData) {
    //     isScaled = false;
    //     StopAllCoroutines();
    //     StartCoroutine(ScaleToSize(originalSize, originalFontSize));
    //     StartCoroutine(UIEndGame.Instance.ScaleLocalPlayerCard(new Vector2(550, 770), 32f));
    // }


    // private IEnumerator ScaleToSize(Vector2 targetSize, float targetFontSize) {
    //     Vector2 initialSize = rectTransform.sizeDelta;
    //     float elapsedTime = 0f;

    //     while (elapsedTime < animationDuration) {
    //         rectTransform.sizeDelta = Vector2.Lerp(initialSize, targetSize, elapsedTime / animationDuration);
    //         foreach (TextMeshProUGUI tmp in dataTMPs) {
    //             tmp.fontSize = Mathf.Lerp(originalFontSize, targetFontSize, elapsedTime / animationDuration);
    //         }
    //         elapsedTime += Time.deltaTime;
    //         yield return null;
    //     }

    //     rectTransform.sizeDelta = targetSize;
    //     foreach (TextMeshProUGUI tmp in dataTMPs) {
    //         tmp.fontSize = targetFontSize;
    //     }
    // }

    
}
