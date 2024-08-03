// Created on Wed Jul 17 2024 || Copyright© 2024 || By: Alex Buzmion II
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalPlayerStatEntry : MonoBehaviour
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
    [SerializeField] public List<GameObject> mvpTrapHighlighter;
    [SerializeField] public Image playerImage;
    [SerializeField] public Image winStamp;
    [SerializeField] public Image loseStamp;
    [SerializeField] public GameObject winBanner; 
}
