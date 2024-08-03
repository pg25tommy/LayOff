// Created on Wed May3 2024 || Copyright© 2024 || By: Alex Buzmion II
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Newtonsoft.Json;
using System;

public class CloudSave : MonoBehaviour
{
    public static CloudSave Instance { get; set; }
    private bool UCSInitialized = false;
    private bool AttemptToSave = false;
    private string playerID;
    public int count;
    public string fileName;
    public List<FloatPair> movePosData = new List<FloatPair>();
    public Dictionary<string, object> movePosDict = new Dictionary<string, object>();
    public Dictionary<string, object> playerData = new Dictionary<string, object>();
    public PlayerStats playerStats;
    // [SerializeField] public TextMeshProUGUI username;
    // [SerializeField] public TextMeshProUGUI password;

    private async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        await InitializeUnityServices();

        if (AuthenticationService.Instance.IsSignedIn)
        {
            playerID = AuthenticationService.Instance.PlayerId;
            // Debug.Log($"Player already signed in as {playerID}");
        }
        else
        {
            await SignInAsGuest();
        }

        ResetPlayerStats();
    }

    private async Task InitializeUnityServices()
    {
        if (!UCSInitialized)
        {
            try
            {
                await UnityServices.InitializeAsync();
                UCSInitialized = true;
                // Debug.Log("Unity Services Initialized");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize Unity Services: {ex}");
            }
        }
        else
        {
            // Debug.Log("CloudSave already initialized");
        }
    }

    public async Task SignInAsGuest()
    {
        try
        {
            if (AuthenticationService.Instance != null)
            {
                if (AuthenticationService.Instance.IsSignedIn)
                {
                    // Debug.Log($"Player already signed in as {AuthenticationService.Instance.PlayerId}");
                }
                else
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    playerID = AuthenticationService.Instance.PlayerId;
                    // Debug.Log($"Signed in as {playerID}");
                }
                await ListAllFiles();
            }
            else
            {
                Debug.LogError("AuthenticationService.Instance is null. Ensure Unity Services are initialized and configured correctly.");
            }
        }
        catch (AuthenticationException ex)
        {
            if (ex.Message.Contains("INVALID_SESSION_TOKEN"))
            {
                Debug.LogWarning("Session token is invalid. Clearing session token and retrying sign-in.");
                AuthenticationService.Instance.ClearSessionToken();
                await SignInAsGuest(); // retry sign-in
            }
            Debug.LogError($"Failed to sign in: {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Unexpected error during sign in: {ex}");
        }
    }

    //? For later implementation
    // public async void SignInAsUser(){
    //     try {
    //         if (!IsSigningOn){
    //             // Regex used to sanitize invisible characters from the username and password fields
    //             string user = Regex.Replace(username.text, @"\p{C}+", string.Empty).Trim();
    //             string pass = Regex.Replace(password.text, @"\p{C}+", string.Empty).Trim();

    //             IsSigningOn = true;
    //             await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(user, pass);
    //             Debug.Log($"Signed in as {AuthenticationService.Instance.PlayerId}");
    //             IsSigningOn = false;
    //             await ListAllFiles();
    //             //increment count used to create a new unique file name
    //             count++;

    //         }
    //     }
    //     catch {
    //         Debug.Log("Failed to sign in as user");
    //     }
    // }

    [Button(Name = "Save Data")]
    // Method to save player data to CloudSave for everything else except for the player movement data
    // approx 170-200b per save. with a max of 5MB, can save up to 30k files per player.
    public async void SaveData()
    {
        if (AttemptToSave) return;
        AttemptToSave = true;
        playerData.Clear(); // ensure player data is clean before adding new data
        fileName = $"{count}";
        // add the player stats to the playerData dictionary
        playerData.Add(fileName, playerStats);
        await CloudSaveService.Instance.Data.Player.SaveAsync(playerData);
        // Debug.Log($"Saved data {string.Join(',', playerData)}");
        count += 1;
        // Debug.Log($"increment count/filename after saving: {count}");
        AttemptToSave = false;
        playerData.Clear(); // ensure player data is clean before adding new data
        ResetPlayerStats();
    }

    [Button(Name = "Load Data")]
    public async void LoadPlayerData()
    {
        try
        {
            // specify which session or key you want to load, e.g., by session ID
            var loadData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "session1" });
            // Debug.Log("Data loaded successfully.");
        }
        catch (Exception ex)
        {
            Debug.Log($"Failed to load data: {ex.Message}");
        }
    }

    // function that saves a file to Unity cloudsave

    [Button(Name = "Save Json File")]
    public async Task SavePlayerFile()
    {
        // additional settings required to force converting the json file
        string jsonFileName = "posData" + count + ".json";
        playerStats.dateAndTime = DateTime.Now;
        var settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        //convert to JSON
        string json = JsonConvert.SerializeObject(movePosData, settings);
        //convert to byte array
        byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(json);
        try
        {
            // save the file to the cloud
            await CloudSaveService.Instance.Files.Player.SaveAsync(jsonFileName, byteArray);
            // Debug.Log("File saved successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

    }
    // pulls the list of all files and increments the count to create a new file name everytime

    private async Task ListAllFiles()
    {
        try
        {
            // pull the list of all files saved in the cloud
            var results = await CloudSaveService.Instance.Data.Player.ListAllKeysAsync();
            count = results.Count;
            // Debug.Log($"{nameof(ListAllFiles)}() Retreived all cloud save files and found: {results.Count}");
            count += 1;
            // Debug.Log("added 1 to count value:" + count);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to list files {e}");
        }
    }

    // method to delete a file from the cloudsave
    [Button(Name = "Delete File")]
    private async Task DeleteFile(string key)
    {
        try
        {
            await CloudSaveService.Instance.Files.Player.DeleteAsync(key);

            Debug.Log("File deleted!");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to delete file: {e}");
        }
    }

    // stats to track for now
    // todo move to its own class/file
    [Serializable]
    public class PlayerStats
    {
        public string roomID = "";
        [SerializeField] public DateTime dateAndTime = DateTime.Now; 
        public int player = 0;
        public int Wins = 0;
        public int Losses = 0;
        public float TimePlayed = 0;
        public int KeycardsCollected = 0;
        public float timeFirstKeyCollected = 0;
        public float timeSecondKeyCollected = 0;
        public float timeThirdKeyCollected = 0;
        public int TimesDashed = 0;
        public float timeOfFirstDash = 0;
        public int TimesShoved = 0;
        public float timeOfFirstShove = 0;
        public int TimesCrouched = 0;
        public float timeOfFirstCrouch = 0;
        public int TrapsSet = 0;
        public float timeOfFirstTrapSet = 0;
        public Dictionary<string, int> TrapsSetByType = new Dictionary<string, int> {
            {"Trap1", 0},
            {"Trap2", 0},
            {"Trap3", 0},
            {"Trap4", 0}
        };
        public int Kills = 0;
        public float timeOfFirstKill = 0;
        public Dictionary<string, Dictionary<string, int>> KillsByTrap = new Dictionary<string, Dictionary<string, int>>
        {
            { "Player1", new Dictionary<string, int> {
                    { "Trap1", 0 },
                    { "Trap2", 0 },
                    { "Trap3", 0 },
                    { "Trap4", 0 }
                }
            },
            { "Player2", new Dictionary<string, int> {
                    { "Trap1", 0 },
                    { "Trap2", 0 },
                    { "Trap3", 0 },
                    { "Trap4", 0 }
                }
            },
            { "Player3", new Dictionary<string, int> {
                    { "Trap1", 0 },
                    { "Trap2", 0 },
                    { "Trap3", 0 },
                    { "Trap4", 0 }
                }
            },
            { "Player4", new Dictionary<string, int> {
                    { "Trap1", 0 },
                    { "Trap2", 0 },
                    { "Trap3", 0 },
                    { "Trap4", 0 }
                }
            },
        };
        public int Deaths = 0;
        public float timeOfFirstDeath = 0;
        [SerializeField]
        public Dictionary<string, Dictionary<string, int>> DeathsByCause = new Dictionary<string, Dictionary<string, int>>
        {
            { "Player1", new Dictionary<string, int> {
                    { "Trap1", 0 },
                    { "Trap2", 0 },
                    { "Trap3", 0 },
                    { "Trap4", 0 }
                }
            },
            { "Player2", new Dictionary<string, int> {
                    { "Trap1", 0 },
                    { "Trap2", 0 },
                    { "Trap3", 0 },
                    { "Trap4", 0 }
                }
            },
            { "Player3", new Dictionary<string, int> {
                    { "Trap1", 0 },
                    { "Trap2", 0 },
                    { "Trap3", 0 },
                    { "Trap4", 0 }
                }
            },
            { "Player4", new Dictionary<string, int> {
                    { "Trap1", 0 },
                    { "Trap2", 0 },
                    { "Trap3", 0 },
                    { "Trap4", 0 }
                }
            },
            { "Environment", new Dictionary<string, int> {
                    {"FinalDoors", 0},
                    {"RoomKills", 0},
                    {"KillBox", 0},
                    {"GMKills", 0},
                }
            }
        };
        public int TrapsTriggered = 0;
        public Dictionary<string, int> TrapsTriggeredByType = new Dictionary<string, int> {
            {"trap1", 0},
            {"trap2", 0},
            {"trap3", 0},
            {"trap4", 0},
        };
        public List<PlayerActionTracking> PlayerActions = new List<PlayerActionTracking>();
    }

    private void ResetPlayerStats()
    {
        playerStats = new PlayerStats();
    }

    // struct to hold the x and z values of the player movement excluding other Vector2 default data (magnitude, normalize) 
    public struct FloatPair
    {
        public float x;
        public float y;

        public FloatPair(float x, float z)
        {
            this.x = x;
            this.y = z;
        }
    }


    [Serializable] // class that tracks a players action performed, its position and time (Time.timeSinceLevelLoaded)
    public class PlayerActionTracking
    {
        public string Action { get; set; }
        public FloatPair Position { get; set; }
        public float Time { get; set; }

        public PlayerActionTracking(string action, FloatPair position, float time)
        {
            Action = action;
            Position = position;
            Time = time;
        }
    }
}
