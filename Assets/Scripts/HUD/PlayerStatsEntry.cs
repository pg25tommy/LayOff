// Created on Wed Jun 12 2024 || Copyright© 2024 || By: Alex Buzmion II
using System.Drawing;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class PlayerStatsEntry : MonoBehaviourPunCallbacks
{
    public int playerNumber;
    public Image playerColorImage;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI keycardCount;
    public TextMeshProUGUI killCount;
    public TextMeshProUGUI deathCount;
    public TextMeshProUGUI trapsSetCount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Initialize() {

    }
}
