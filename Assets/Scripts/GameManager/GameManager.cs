// Created on Wednesday May 01 2024 || Copyright (c) 2024 Names Are Hard Studios || By: Alex Buzmion II / Tiago Corsato
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using UnityEngine.Events;
using CharacterMovement;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using FMODUnity;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField, BoxGroup("Ceiling")] public GameObject Ceiling;

    private int gameScene = 1; // some checks require checking if the current scene is/isnt gamescene update this value of the gamescene index changed

    [ShowInInspector, BoxGroup("Players In Room")] private int playersInSession;
    [SerializeField, BoxGroup("Player Count")] private TextMeshProUGUI playerCountText;
    [SerializeField, BoxGroup("Player Count")] private Image playerCountFill;
    [SerializeField, BoxGroup("Events & Delegates")] private UnityEvent PlayerJoinedRoom;
    [SerializeField] public GameObject finalRoomHighlighter;
    [SerializeField] private EventReference _gameOverVA;
    [SerializeField] private Sprite[] playerHeadshots;
    [SerializeField] private Color[] playerColors;
    public UnityEvent StartTimer;
    public UnityEvent SpawnKeyCards;
    public UnityEvent switchActionMap;

    [HideInInspector]
    public static GameManager Instance { get; set; }

    // singleton check to ensure that there is only one instance of the GameManager in Awake
    protected virtual void Awake()
    {
        Instance = this;

        if (Ceiling != null)
        {
            Ceiling.SetActive(true);
        }
    }

    private void Start()
    {
        // Start Timer
        StartTimer.Invoke();

        // Spawn Keycards
        SpawnKeyCards.Invoke();

        // playerList = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
    }

    public void GameWon(Component component, object data)
    {
        if (component is PhotonView winnerPV)
        {
            int winnerId = winnerPV.Owner.ActorNumber;
            photonView.RPC("EndGame", RpcTarget.All, winnerId);
        }
    }

    private void UpdatePlayerCountInHUD()
    {
        if (PhotonNetwork.CurrentRoom == null) return;
        playersInSession = PhotonNetwork.CurrentRoom.PlayerCount;

        if (SceneManager.GetActiveScene().buildIndex != gameScene) return;
        playerCountText.text = playersInSession.ToString();

        float fillAmount = (float)playersInSession / 4f;
        playerCountFill.fillAmount = Mathf.Clamp01(fillAmount);
    }

    IEnumerator HighlightIcon(GameObject icon, float time)
    {
        icon.SetActive(true);
        yield return new WaitForSeconds(time);
        icon.SetActive(false);
    }

    [PunRPC]
    public void EndGame(int winnerId)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        HUDManager.Instance.playerStatsPanel.SetActive(true);
        string elapsedTime;
        if (PhotonNetwork.LocalPlayer.ActorNumber == winnerId)
        {
            CloudSave.Instance.playerStats.Wins += 1;
            Debug.Log($"RPC EndGame called, with winner {winnerId}");
            // This is the winner
            HUDManager.Instance.playerStatsTitle.text = "WIN";
            UIEndGame.Instance.endGameStatus.text = $"You Got Out!";
            UIEndGame.Instance.UpdateElapsedTime(TimerManager.instance.timeRemaining, TimerManager.instance.startTimer);
            TimerManager.instance.timerIsRunning = false;
        }
        else
        {
            // This is everyone else
            CloudSave.Instance.playerStats.Losses += 1;
            HUDManager.Instance.playerStatsTitle.text = "LOSE";
            UIEndGame.Instance.endGameBanner.color = Color.red;
            RuntimeManager.PlayOneShot(_gameOverVA);
            if (TimerManager.instance.timeRemaining <= .5f)
            {
                UIEndGame.Instance.endGameStatus.text = "Out of Time!";
                UIEndGame.Instance.UpdateElapsedTime(0, 330);

            }
            else
            {
                UIEndGame.Instance.endGameStatus.text = $"Player {winnerId} got out!";
                UIEndGame.Instance.UpdateElapsedTime(TimerManager.instance.timeRemaining, TimerManager.instance.startTimer);
            }
        }
        UIEndGame.Instance.AnimateBanner();
        string winnerNickname = GetPlayerName(winnerId);
        UIEndGame.Instance.InitEndGameScreenData(winnerNickname);
        elapsedTime = UIEndGame.Instance.ConvertElapsedTime(TimerManager.instance.timeRemaining, TimerManager.instance.startTimer);
        CloudSave.Instance.playerStats.TimePlayed = Time.timeSinceLevelLoad;
        HUDManager.Instance.playerHUD.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        switchActionMap.Invoke();
        CloudSave.Instance.SaveData();
    }
    //? call this with parameter passing actor numbers from Photon, returning the player color for the actor
    public Color GetPlayerColor(int actorNumber)
    {
        switch (actorNumber)
        {
            case 1:
                return playerColors[0];
            case 2:
                return playerColors[1];
            case 3:
                return playerColors[2];
            case 4:
                return playerColors[3];
            default:
                return Color.black;
        }
    }
    //? call this with parameter passing actor numbers from Photon, returning the player avatar for the actor
    public Sprite GetPlayerImage(int actorNumber)
    {
        switch (actorNumber)
        {
            case 1:
                return playerHeadshots[0];
            case 2:
                return playerHeadshots[1];
            case 3:
                return playerHeadshots[2];
            case 4:
                return playerHeadshots[3];
            default:
                return null;
        }
    }

    private string GetPlayerName(int winnerID) {
        foreach (Player player in PhotonNetwork.PlayerList) {
            if (player.ActorNumber == winnerID) {
                return player.NickName;
            }
        }
        return null;
    }

    public Color GetHexColor(string hexColor) {
        if (ColorUtility.TryParseHtmlString(hexColor, out Color color)) {
            return color;
        } else {
            Debug.LogError($"Invalid hex color string: {hexColor}");
            return Color.magenta;
        }
    }
}

