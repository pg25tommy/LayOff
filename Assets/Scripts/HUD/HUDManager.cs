// Created on Sat Jun 01 2024 || Copyright© 2024 || By: Alex Buzmion II
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using Photon.Realtime;
using Photon.Pun;

public class HUDManager : MonoBehaviour
{
    #region Serialized Fields
        // private
        [SerializeField] private RawImage replayCam;
        
        [SerializeField, BoxGroup("ActionBar")] private Image trap1;
        [SerializeField, BoxGroup("ActionBar")] private Image trap2;
        [SerializeField, BoxGroup("ActionBar")] private Image trap3;
        [SerializeField, BoxGroup("ActionBar")] private Image trap4;
        [SerializeField, BoxGroup("ActionBar")] private Image dash;
        [SerializeField, BoxGroup("ActionBar")] private Image shove;
        [SerializeField, BoxGroup("ActionBar")] private Image crouch;
        [SerializeField, BoxGroup("ActionBar")] private Image jump;
        // public 
        [SerializeField] public GameObject playerHUD;
        [SerializeField] public Image disableOverlay;
        [SerializeField] public GameObject playerStatsPanel;
        [SerializeField] public GameObject minimapBorder;
        [SerializeField] public GameObject minimapBorderBG;
        [SerializeField] public FadeInOut _fadeInOut;
    #endregion

    #region Private Fields
        // trapCD checks
        [HideInEditorMode] public bool trap1CD = false;
        [HideInEditorMode] public bool trap2CD = false;
        [HideInEditorMode]public bool trap3CD = false;
        [HideInEditorMode]public bool trap4CD = false;
        [HideInEditorMode]public bool dashCD = false;
        [HideInEditorMode]public bool shoveCD = false;
    #endregion

    #region Public Properties
        public static HUDManager Instance;
        public TextMeshProUGUI playerStatsTitle;
        public UnityEvent switchActionMap;
    #endregion

    #region MonoBehavior Callbacks
        void Awake() {
            Instance = this;
            playerStatsTitle = playerStatsPanel.GetComponentInChildren<TextMeshProUGUI>();
            _fadeInOut = GetComponent<FadeInOut>();
        }

        public void StartTrapCooldown(int trap, float cooldownTime) {
            switch (trap) {
                case 1:
                    if (trap1CD) return;
                    trap1CD = true;
                    StartCoroutine(StartTrapCooldown(trap1, cooldownTime, trap));
                    break;
                case 2:
                    if (trap2CD) return;
                    trap2CD = true;
                    StartCoroutine(StartTrapCooldown(trap2, cooldownTime, trap));
                    break;
                case 3:
                    if (trap3CD) return;
                    trap3CD = true;
                    StartCoroutine(StartTrapCooldown(trap3, cooldownTime, trap));
                    break;
                case 4:
                    if (trap4CD) return;
                    trap4CD = true;
                    StartCoroutine(StartTrapCooldown(trap4, cooldownTime, trap));
                    break;
                case 5:
                    if (dashCD) return;
                    dashCD = true;
                    StartCoroutine(StartTrapCooldown(dash, cooldownTime, trap));
                    break;
                case 6:
                    if (shoveCD) return;
                    shoveCD = true;
                    StartCoroutine(StartTrapCooldown(shove, cooldownTime, trap));
                    break;
                default:
                    break;
            }
        }

        // method to log stats for all players at the end of the game
        public void LogAllPlayerStats() {
            foreach (Player player in PhotonNetwork.PlayerList) {
                string playerName = player.NickName;
                int keycardsCollected = GetCustomProperty(player, "keycardsCollected");
                int trapsSet = GetCustomProperty(player, "trapsSet");
                int kills = GetCustomProperty(player, "kills");
                int deaths = GetCustomProperty(player, "deaths");

                Debug.Log($"Player: {playerName}\nKeycards Collected: {keycardsCollected}\nTraps Set: {trapsSet}\nKills: {kills}\nDeaths: {deaths}");
            }
        }

        private int GetCustomProperty(Player player, string propertyKey) {
            if (player.CustomProperties.TryGetValue(propertyKey, out object value)) {
                return (int)value;
            }
            return 0;
        }
    #endregion

    #region Coroutines
        IEnumerator StartTrapCooldown(Image trap, float cooldownTime, int trapCDIndex) {
            trap.fillAmount = 0;
            while (trap.fillAmount < 1) {
                trap.fillAmount += Time.deltaTime / cooldownTime;
                yield return null;
            }
            switch (trapCDIndex) {
                case 1:
                    trap1CD = false;
                    break;
                case 2:
                    trap2CD = false;
                    break;
                case 3:
                    trap3CD = false;
                    break;
                case 4:
                    trap4CD = false;
                    break;
                case 5:
                    dashCD = false;
                    break;
                case 6:
                    shoveCD = false;
                    break;
            }
        }

        public void ApplyFadeInOut()
        {
            _fadeInOut.ApplyFadeInOut();
        }

        public bool GetCD(int trapIndex) {
            bool isOnCD; 
            switch (trapIndex) {
                case 1: 
                    isOnCD = trap1CD;
                    break;
                case 2:
                    isOnCD = trap2CD;
                    break;
                case 3: 
                    isOnCD = trap3CD;
                    break;
                case 4: 
                    isOnCD = trap4CD;
                    break;
                default: 
                    isOnCD = false;
                    break;
            }
            return isOnCD;
        }

        public void NotifyHUD() {
            StartCoroutine(AnimateMinimapBorder()); 
        }
        private IEnumerator AnimateMinimapBorder() {
            minimapBorderBG.SetActive(true);
            Vector3 reducedScale = new Vector3 (.83f, .83f, .83f); // cache start scale 
            Vector3 stepSize = new Vector3 (.01f, .01f, .01f); // cache the step size to scale the object
            while (minimapBorderBG.transform.localScale.x < 1) {
                minimapBorderBG.transform.localScale += stepSize;
                yield return null;
            }
            minimapBorderBG.transform.localScale = new Vector3(1, 1, 1);

            for (int i = 0; i < 10; i++) { // animate 10 times, from reduced scale to normal scale
                minimapBorder.transform.localScale = reducedScale;
                while (minimapBorder.transform.localScale.x < 1) {
                    minimapBorder.transform.localScale += stepSize;
                    yield return null;
                }
                minimapBorder.transform.localScale = new Vector3(1,1,1); 
                yield return new WaitForSeconds(.5f); 
            }
            while (minimapBorderBG.transform.localScale.x > reducedScale.x) {
                minimapBorderBG.transform.localScale -= stepSize;
                yield return null; 
            }
            minimapBorderBG.transform.localScale = reducedScale;
        }
    #endregion
}
