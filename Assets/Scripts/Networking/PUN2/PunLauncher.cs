// Created on Sun Apr 07 2024 || Copyright© 2024 || By: Alex Buzmion II
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using TMPro;
using UIComponents;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WebSocketSharp;

public class PunLauncher : MonoBehaviourPunCallbacks
{   
    #region Private Serializable Fields
        [SerializeField, BoxGroup("MainMenu")] GameObject roomOptionsPanel;
        [SerializeField, BoxGroup("MainMenu")] GameObject mainMenuPanel;
        [SerializeField, BoxGroup("MainMenu")] GameObject titlePanel;
        [SerializeField, BoxGroup("MainMenu")] GameObject insideRoomPanel;
        [SerializeField, BoxGroup("MainMenu")] GameObject insideRoomEntries;
        [SerializeField, BoxGroup("MainMenu")] GameObject joinRoomOption;
        [SerializeField, BoxGroup("MainMenu")] Button createRoomBackButton;
        [SerializeField, BoxGroup("MainMenu")] Button insideRoomBackButton;
        [SerializeField] public int maxPlayers;
        [SerializeField] public int countdown; 
        [SerializeField] private TextMeshProUGUI textRenderer;
        [SerializeField] TMP_InputField maxPlayersInput; 
        [SerializeField] private GameObject RoomListEntryPrefab;
        [SerializeField] private GameObject RoomListContent;
        [SerializeField] private GameObject JoinInputField;
        [SerializeField] private GameObject CreateInputField;
        [SerializeField] private PlayerRoomEntryList pList;

        [SerializeField, BoxGroup("SFX")] EventReference _objectiveVA; 
        
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    #endregion

    #region Private Fields
        // private string gameVersion = "0.0.1"; 
        private RoomOptions roomOptions = new RoomOptions();
        private Dictionary<string, RoomInfo> cachedRoomList;
        private Dictionary<string, GameObject> roomListEntries;
        private int gameScene = 1; // some checks require checking if the current scene is/isnt gamescene update this value of the gamescene index changed
        private static System.Random random = new System.Random();
    #endregion

    #region Public Fields
        [HideInInspector] public static PunLauncher Instance { get; set; }
        public SwitchActionMaps switchActionMap;
        public LayOffGameEvent onPunJoinedRoom;
        public TMP_InputField createInputField;
        public TextMeshProUGUI joinInputField;
    #endregion

    #region MonoBehaviorCallBacks
        private void Awake() {
            //? Singleton Patter to ensure only 1 PUN manager exists throughout the game
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }
            Instance = this as PunLauncher;
            //DontDestroyOnLoad(gameObject);
            switchActionMap = FindFirstObjectByType<SwitchActionMaps>();
            //? call to connect to master server
            PhotonNetwork.AutomaticallySyncScene = true;
            if (CreateInputField != null) {
                createInputField = CreateInputField.GetComponent<TMP_InputField>();
            }
            cachedRoomList = new Dictionary<string, RoomInfo>();
            roomListEntries = new Dictionary<string, GameObject>();
        }
        
        public void CreateRoomButtonClicked() {
            string code = GetRandomString(Random.Range(4,8));
            if (createInputField != null) {
                createInputField.text = code;
            }
        }
        
        public string GetRandomString(int length) {
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        void Connect() {
            //? connect to PUN first with the App ID
            switchActionMap.SwitchMap("UI");
            if (!PhotonNetwork.IsConnected) {
                PhotonNetwork.ConnectUsingSettings();
            }
            // PhotonNetwork.GameVersion = gameVersion;
            if (SceneManager.GetActiveScene().buildIndex == gameScene) return;
            UpdateTextRenderer("Connecting. . .");
        }
        private void Start() {
            Connect();
        }

        //called by create room button
        public void CreateRoom() {
            if (maxPlayers > 4 || maxPlayers == 0) {
                ButtonAnimator.Instance.Unhide();
                return;
            }
            if (createInputField.text.Length <= 1) {
                UpdateTextRenderer("Invalid Room name, must be longer");
                StartCoroutine(ClearTextRendered());
                return;
            }
            if (!string.IsNullOrEmpty(maxPlayersInput.text)) { // cehck if max player is set by a player, if not default to 4
                maxPlayers = int.Parse(maxPlayersInput.text);
            } else {
                Text placeholderText = maxPlayersInput.placeholder as Text;
                if (placeholderText != null) {
                    maxPlayers = int.Parse(placeholderText.text);
                }
            }
            roomOptions.PublishUserId = true; 
            roomOptions.MaxPlayers = maxPlayers; 
            roomOptions.PlayerTtl = 0;
            PhotonNetwork.CreateRoom(createInputField.text.Trim((char)8203), roomOptions);
            titlePanel.SetActive(false);
            roomOptionsPanel.SetActive(false);
        }

        // called by join room button
        public void JoinRoom() {
            if (joinInputField.text.Length < 2) {
                UpdateTextRenderer("Please choose or type a room name");
                return;
            }
            UpdateTextRenderer("Loading. . .");
            PhotonNetwork.JoinRoom(joinInputField.text.Trim((char)8203));
            joinRoomOption.SetActive(false);
        }

        public void LeaveRoom(){
            PhotonNetwork.LeaveRoom();
        }
        public void UpdateTextRenderer(string msg) {
            if(textRenderer == null) return;
            textRenderer.text = msg;
        }

        public void OnRoomListButtonClicked() {
            if(!PhotonNetwork.InLobby){
                PhotonNetwork.JoinLobby();
            }
        }

        public void StartMatch() {
            photonView.RPC(nameof(RPC_DisableBackButtons), RpcTarget.All);
            Debug.Log("Match Begins!");
            StartCoroutine(CountDownToMatch());
        }


        public IEnumerator CountDownToMatch() {
            // todo add voiceline explaining the reason here
            RuntimeManager.PlayOneShot(_objectiveVA);
            // todo enable visuals for the voice line. story for the lay-off, etc. 
            for (int i = countdown; i != 0; i--) {
                photonView.RPC(nameof(RPC_UpdateTextRenderer), RpcTarget.All, $"Match starting in {i}. . .");
                yield return new WaitForSeconds(1);
            }
            if (switchActionMap != null) {
                switchActionMap.SwitchMap("Player");
            }
            UpdateTextRenderer("Get Ready! ");
            onPunJoinedRoom.Raise(null, null); // game event listened by the game manager in the game scene to spawn the player 
            PhotonNetwork.LoadLevel("GameScene");
            PhotonNetwork.CurrentRoom.IsVisible = false; // close the room 
        }
    #endregion

    #region MonoBehaviorPunCallbacks
    //? after connection established to the master server 
    public override void OnConnectedToMaster() {   
            if (SceneManager.GetActiveScene().buildIndex != gameScene) {
                // TextRenderer.TRInstance.UpdateTextRenderer("Connected to master", true);
            }
            //base.OnConnectedToMaster();
            //? Connect to the lobby right after 
            PhotonNetwork.JoinLobby();
            // only updates in the main menu scene and not when launched in the gamescene
            
            UpdateTextRenderer ("");
            if (ButtonAnimator.Instance != null) {
                ButtonAnimator.Instance.Unhide();
            }
            if (mainMenuPanel != null){
                mainMenuPanel.SetActive(true);
                if (switchActionMap != null){
                    switchActionMap.SwitchMap("UI");
                }
            }
        }
        
        public override void OnJoinedLobby() {   
            if (SceneManager.GetActiveScene().buildIndex != gameScene) {
                if (PhotonNetwork.InLobby) {
                    // TextRenderer.TRInstance.UpdateTextRenderer("Successfully Joined Lobby", true);
                    // TextRenderer.TRInstance.UpdateTextRenderer($"Current lobby: {PhotonNetwork.CurrentLobby.Name}",true);
                } else {
                    // TextRenderer.TRInstance.UpdateTextRenderer($"Not connected to any lobby", true);
                }
            }
            // whenever this joins a new lobby, clear any previous room lists
            cachedRoomList.Clear();
            ClearRoomListView();
            // if playing the scene from the game manager [2] create room named 1.     
            if (SceneManager.GetActiveScene().buildIndex == gameScene) {
                PhotonNetwork.CreateRoom(Random.Range(0, 200).ToString());
            }
        }

        public override void OnDisconnected(DisconnectCause cause) {
            if (switchActionMap != null){
                switchActionMap.SwitchMap("UI");
            }
        }

        public override void OnJoinedRoom() {
            // joining (or entering) a room invalidates any cached lobby room list (even if LeaveLobby was not called due to just joining a room)
            cachedRoomList.Clear();
            UpdateTextRenderer("Waiting for all players to be ready");
            if (insideRoomPanel != null) {
                insideRoomEntries.SetActive(true);
                pList.AddAllEntries();
                pList.roomNameText.text = PhotonNetwork.CurrentRoom.Name;
            }
        }

        public override void OnLeftRoom() {
            if (SceneManager.GetActiveScene().buildIndex != gameScene) {
                insideRoomEntries.SetActive(false); 
                titlePanel.SetActive(true);
                UpdateTextRenderer("");
                base.OnLeftRoom();
                return;
            }
            SceneManager.LoadScene(0);
        }
        public override void OnJoinRoomFailed(short returnCode, string message) {
            UpdateTextRenderer($"Error{returnCode}: {message}");
            Debug.Log("Join Room Failed");
        }

        public override void OnCreateRoomFailed(short returnCode, string message) {
            UpdateTextRenderer($"Error{returnCode}: {message}");
            Debug.Log("Create Room Failed");
        }
        // automatically called when a room is created
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            if (SceneManager.GetActiveScene().buildIndex != gameScene) {
                // if (TextRenderer.TRInstance != null) {
                //         TextRenderer.TRInstance.UpdateTextRenderer("Room List Update: Received " + roomList.Count + " rooms.", true);
                // }

                foreach (var room in roomList) {
                    string roomInfo = "Room: " + room.Name + " Players: " + room.PlayerCount + "/" + room.MaxPlayers;
                    Debug.Log(roomInfo);
                    // if (TextRenderer.TRInstance != null) {
                    //     TextRenderer.TRInstance.UpdateTextRenderer(roomInfo, true);
                    // }
                }
            }
            ClearRoomListView(); 
            UpdateCachedRoomList(roomList); 
            UpdateRoomListView(); 
        }

        private void ClearRoomListView()
        {
            foreach (GameObject entry in roomListEntries.Values) {
                Destroy(entry.gameObject);
            }

            roomListEntries.Clear();
        }

        private void UpdateRoomListView()
        {
            foreach (RoomInfo info in cachedRoomList.Values) {
                if (RoomListEntryPrefab != null) {
                    GameObject entry = Instantiate(RoomListEntryPrefab);
                    if (RoomListContent != null) {
                        entry.transform.SetParent(RoomListContent.transform);
                        entry.transform.localScale = Vector3.one;
                        entry.GetComponent<RoomListEntry>().Initialize(info.Name, (byte)info.PlayerCount, (byte)info.MaxPlayers, JoinInputField);
                        roomListEntries.Add(info.Name, entry);
                    }
                }
            }
        }

        private void UpdateCachedRoomList(List<RoomInfo> roomList) {
            foreach (RoomInfo info in roomList) {
                // Remove room from cached room list if it got closed, became invisible or was marked as removed
                if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)  {
                    if (cachedRoomList.ContainsKey(info.Name)) {
                        cachedRoomList.Remove(info.Name);
                        // TextRenderer.TRInstance.UpdateTextRenderer($"Removed room from cache {info.Name}", true);
                    }
                    continue;
                }

                // Update cached room info
                if (cachedRoomList.ContainsKey(info.Name)) {
                    cachedRoomList[info.Name] = info;
                    // TextRenderer.TRInstance.UpdateTextRenderer($"Updated room in cache {info.Name}", true);
                }
                // Add new room info to cache
                else {
                    cachedRoomList.Add(info.Name, info);
                    // TextRenderer.TRInstance.UpdateTextRenderer($"Added room to cache {info.Name}", true);
                }
            }
        }

        IEnumerator ClearTextRendered() {
            yield return new WaitForSeconds(3);
            UpdateTextRenderer("");
        }

        [PunRPC]
        public void RPC_UpdateTextRenderer(string text) {
            textRenderer.text = text;
        }

        [PunRPC]
        public void RPC_DisableBackButtons() {
            insideRoomBackButton.interactable = false;
        }
    #endregion
}
