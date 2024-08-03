// Created on Thu May 30 2024 || Copyright© 2024 || By: Alex Buzmion II
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class UIEndGame : MonoBehaviourPunCallbacks
{
    public static UIEndGame Instance;
    #region Serialized Fields
    [SerializeField] public TextMeshProUGUI endGameStatus;
    [SerializeField] public TextMeshProUGUI elapsedTime;
    [SerializeField] public TextMeshProUGUI kills;
    [SerializeField] public TextMeshProUGUI deaths;
    [SerializeField] public TextMeshProUGUI trapsSet;
    [SerializeField] public TextMeshProUGUI MVPTrap;
    [SerializeField] public TextMeshProUGUI BulliedPlayer;
    [SerializeField] public Image endGameBanner;
    [SerializeField] public LocalPlayerStatEntry localPlayerStatEntry;
    [SerializeField] private List<OtherPlayerStatEntry> otherPlayerStatEntries;
    [SerializeField] private OtherPlayerStatEntry otherPlayerStatEntry1;
    [SerializeField] private OtherPlayerStatEntry otherPlayerStatEntry2;
    [SerializeField] private OtherPlayerStatEntry otherPlayerStatEntry3;
    [SerializeField] private List<Sprite> playerImages;
    [SerializeField] private List<GameObject> mvpTrapIcon;

    [SerializeField] List<PlayerStats> playerStats;
    #endregion

    #region  Public Fields
    public float timeRemaining;
    public float startTimer;
    #endregion

    #region Monobehavior Callbacks
    void Awake()
    {
        Instance = this;
        playerStats = new List<PlayerStats>();
    }

    public void AnimateBanner()
    {
        StartCoroutine(ModifyTransparency());
    }
    private IEnumerator ModifyTransparency()
    {
        float maxAlpha = 0.5f;
        float minAlpha = 0.2f;
        float stepSize = 0.02f;
        bool isIncreasing = false;

        while (true)
        {
            Color color = endGameBanner.color;
            float targetAlpha = isIncreasing ? maxAlpha : minAlpha;

            if (isIncreasing)
            {
                color.a += stepSize;
                if (color.a >= maxAlpha)
                {
                    color.a = maxAlpha;
                    isIncreasing = false;
                }
            }
            else
            {
                color.a -= stepSize;
                if (color.a <= minAlpha)
                {
                    color.a = minAlpha;
                    isIncreasing = true;
                }
            }
            endGameBanner.color = color;
            yield return new WaitForSeconds(0.1f); // Adjust the duration as needed
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (PhotonNetwork.LocalPlayer.IsLocal)
        {

        }
    }

    public void UpdateElapsedTime(float timeLeft, float startTime)
    {
        elapsedTime.text = ConvertElapsedTime(timeLeft, startTime);
    }

    public string ConvertElapsedTime(float timeLeft, float startTime)
    {
        float endTime = startTime - timeLeft;
        float minutes = Mathf.FloorToInt(endTime / 60);
        float seconds = Mathf.FloorToInt(endTime % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    #endregion

    #region StatsDataDisplay
    // struct used to search for ALL playerstats to display on the EndGame screen
    public struct PlayerStats
    {
        public string playerName;
        public int win;
        public int totalKills;
        public int totalDeaths;
        public int totalTrapsSet;
        public int trap1Set;
        public int trap2Set;
        public int trap3Set;
        public int trap4Set;
        public int trap1Kills;
        public int trap2Kills;
        public int trap3Kills;
        public int trap4Kills;
    }
    // called by the Game manager when the win button is pressed or if timer runs out
    public void InitEndGameScreenData(string winnerNickName) 
    {
        playerStats = GetPlayerStatsForEndGame();
        DisplayLocalPlayerStats(winnerNickName);
    }

    // method to get player stats for the end game screen from Photon.Player.CustomProperties 
    public List<PlayerStats> GetPlayerStatsForEndGame()
    {
        List<PlayerStats> endGameStats = new List<PlayerStats>();
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            PlayerStats stats = new PlayerStats // create a new PlayerStats Struct for each player/iteration
            {
                playerName = player.NickName,
                totalKills = GetPlayerCustomProperty(player, PlayerStatsManager.Kills),
                totalDeaths = GetPlayerCustomProperty(player, PlayerStatsManager.Deaths),
                totalTrapsSet = GetPlayerCustomProperty(player, PlayerStatsManager.TrapsSet),
                trap1Set = GetPlayerCustomProperty(player, PlayerStatsManager.Trap1),
                trap2Set = GetPlayerCustomProperty(player, PlayerStatsManager.Trap2),
                trap3Set = GetPlayerCustomProperty(player, PlayerStatsManager.Trap3),
                trap4Set = GetPlayerCustomProperty(player, PlayerStatsManager.Trap4),
                trap1Kills = GetPlayerCustomProperty(player, PlayerStatsManager.Trap1Kill),
                trap2Kills = GetPlayerCustomProperty(player, PlayerStatsManager.Trap2Kill),
                trap3Kills = GetPlayerCustomProperty(player, PlayerStatsManager.Trap3Kill),
                trap4Kills = GetPlayerCustomProperty(player, PlayerStatsManager.Trap4Kill),
            };
            endGameStats.Add(stats); // add all retreived stats in the player stats
        }
        return endGameStats;
    }

    // helper method to get a custom property for a specific player
    private int GetPlayerCustomProperty(Player player, string propertyKey)
    {
        if (player.CustomProperties.TryGetValue(propertyKey, out object value))
        {
            return (int)value;
        }
        return 0;
    }
    // function to update all local player stats (big ID card for local player)
    private void DisplayLocalPlayerStats(string winner) {
        int otherPlayerIndex = 0;
        foreach (PlayerStats stats in playerStats) { //for each cached PlayerStats collected from Photon Custom Properties
            // handles data population of the Local player card
            if (stats.playerName == PhotonNetwork.LocalPlayer.NickName) {
                localPlayerStatEntry.playerNickname.text = stats.playerName;
                localPlayerStatEntry.totalDeaths.text = $"x{stats.totalDeaths}";
                localPlayerStatEntry.totalTrapsSet.text = $"x{stats.totalTrapsSet}";
                localPlayerStatEntry.totalKills.text = $"x{stats.totalKills}";
                localPlayerStatEntry.trap1Set.text = $"x{stats.trap1Set}";
                localPlayerStatEntry.trap2Set.text = $"x{stats.trap2Set}";
                localPlayerStatEntry.trap3Set.text = $"x{stats.trap3Set}";
                localPlayerStatEntry.trap4Set.text = $"x{stats.trap4Set}";
                localPlayerStatEntry.trap1Kill.text = $"x{stats.trap1Kills}";
                localPlayerStatEntry.trap2Kill.text = $"x{stats.trap2Kills}";
                localPlayerStatEntry.trap3Kill.text = $"x{stats.trap3Kills}";
                localPlayerStatEntry.trap4Kill.text = $"x{stats.trap4Kills}";
                int mvpTrap = GetMVPTrap(PhotonNetwork.LocalPlayer);
                if (mvpTrap > 0) { // ensure that there is an MVP traptype identified
                    localPlayerStatEntry.mvpTrapHighlighter[mvpTrap - 1].SetActive(true); // enables the highlighter for the MVP trap
                }
                Color footerImageColor; // variable to cache the footer color
                if (stats.playerName == winner) { // handle win specific data/UI elements for local player card
                    localPlayerStatEntry.winStamp.gameObject.SetActive(true);
                    localPlayerStatEntry.footerText.text = "Top Agent";
                    footerImageColor = GameManager.Instance.GetHexColor("#00713C");
                    localPlayerStatEntry.winBanner.GetComponent<Image>().color = Color.yellow;
                }
                else { // handle lose specific data/UI elements for local player card
                    localPlayerStatEntry.loseStamp.gameObject.SetActive(true);
                    localPlayerStatEntry.footerText.text = "Ex-Employee";
                    footerImageColor = GameManager.Instance.GetHexColor("#464646"); 
                    // localPlayerStatEntry.gameObject.GetComponent<Image>().color = GameManager.Instance.GetHexColor("#CBCBCB");
                    localPlayerStatEntry.winBanner.GetComponent<Image>().color = Color.red;
                }
                    localPlayerStatEntry.footerImage.color = footerImageColor;
                switch (stats.playerName) { // switch case to update players image in the right card
                    case "Elise":
                        localPlayerStatEntry.playerImage.sprite = playerImages[0];
                        break;
                    case "Hazel":
                        localPlayerStatEntry.playerImage.sprite = playerImages[1];
                        break;
                    case "Sofia":
                        localPlayerStatEntry.playerImage.sprite = playerImages[2];
                        break;
                    case "Amelia":
                        localPlayerStatEntry.playerImage.sprite = playerImages[3];
                        break;
                    default:
                        break;

                }
            } // almost similar to the local player. this handles the other players data population
            else if (otherPlayerIndex < otherPlayerStatEntries.Count) {
                OtherPlayerStatEntry otherPlayer = otherPlayerStatEntries[otherPlayerIndex];
                otherPlayer.playerNickname.text = stats.playerName;
                otherPlayer.totalKills.text = $"x{stats.totalKills}";
                otherPlayer.totalDeaths.text = $"x{stats.totalDeaths}";
                otherPlayer.totalTrapsSet.text = $"x{stats.totalTrapsSet}";
                otherPlayer.trap1Set.text = $"x{stats.trap1Set}";
                otherPlayer.trap2Set.text = $"x{stats.trap2Set}";
                otherPlayer.trap3Set.text = $"x{stats.trap3Set}";
                otherPlayer.trap4Set.text = $"x{stats.trap4Set}";
                otherPlayer.trap1Kill.text = $"x{stats.trap1Kills}";
                otherPlayer.trap2Kill.text = $"x{stats.trap2Kills}";
                otherPlayer.trap3Kill.text = $"x{stats.trap3Kills}";
                otherPlayer.trap4Kill.text = $"x{stats.trap4Kills}";
                Color footerImageColor;
                if (stats.playerName == winner) {
                    otherPlayer.winStampImage.gameObject.SetActive(true);
                    otherPlayer.footerText.text = "Top Agent";
                    footerImageColor = GameManager.Instance.GetHexColor("#00713C");
                    otherPlayer.winnerBorder.SetActive(true);
                }
                else {
                    otherPlayer.LoseStampImage.gameObject.SetActive(true);
                    otherPlayer.footerText.text = "Ex-Employee";
                    footerImageColor = GameManager.Instance.GetHexColor("#464646");
                    otherPlayer.gameObject.GetComponent<Image>().color = GameManager.Instance.GetHexColor("#CBCBCB");
                }
                otherPlayer.footerImage.color = footerImageColor;
                otherPlayerIndex++;
                switch (stats.playerName) {
                    case "Elise":
                        otherPlayer.playerImage.sprite = playerImages[0];
                        break;
                    case "Hazel":
                        otherPlayer.playerImage.sprite = playerImages[1];
                        break;
                    case "Sofia":
                        otherPlayer.playerImage.sprite = playerImages[2];
                        break;
                    case "Amelia":
                        otherPlayer.playerImage.sprite = playerImages[3];
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void UpdateCardImage(int playerNumber, bool localPlayer)
    {
        if (localPlayer)
        {
            // localPlayerStatEntry.playerImage
        }
    }

    public int GetMVPTrap(Player player)
    {
        int trapTypes = 5; // update this if more traps are introduced in the game
        float mvpKillPercentage = -1; // reference for the mvpKill%
        int mvpTrapType = 0;  // iterator and stores the current mvpTrap 
        for (int i = 1; i < trapTypes; i++)
        { // cycle through the trapTypes
            int totalSets = GetPlayerCustomProperty(player, $"trap{i}"); // get # of sets
            int totalKills = GetPlayerCustomProperty(player, $"trap{i}kill"); // get # of kills 
            Debug.Log($"trap {i} sets: {totalSets}, trap{i} kills: {totalKills}");
            if (totalSets == 0) continue; // avoid division by 0
            float killPercentage = (float)totalKills / totalSets; // get the % of kills to sets

            if (i == 0 || killPercentage > mvpKillPercentage)
            { // caches in the first max % and updates if a new % is higher than previous
                mvpKillPercentage = killPercentage;
                mvpTrapType = i;
            }
            Debug.Log($"Trap {i}, with kill % {killPercentage}");
        }
        return mvpTrapType;
    }

    public IEnumerator ScaleLocalPlayerCard(Vector2 targetSize, float targetFontSize)
    {
        RectTransform rectTransform = localPlayerStatEntry.GetComponent<RectTransform>();
        Vector2 initialSize = rectTransform.sizeDelta;
        float initialFontSize = 32f; // initial font size
        float elapsedTime = 0f;
        float animationDuration = 0.2f; // adjust as needed

        while (elapsedTime < animationDuration)
        {
            rectTransform.sizeDelta = Vector2.Lerp(initialSize, targetSize, elapsedTime / animationDuration);
            foreach (TextMeshProUGUI tmp in localPlayerStatEntry.GetComponentsInChildren<TextMeshProUGUI>())
            {
                tmp.fontSize = Mathf.Lerp(initialFontSize, targetFontSize, elapsedTime / animationDuration);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.sizeDelta = targetSize;
        foreach (TextMeshProUGUI tmp in localPlayerStatEntry.GetComponentsInChildren<TextMeshProUGUI>())
        {
            tmp.fontSize = targetFontSize;
        }
    }
    #endregion
}
