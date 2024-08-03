using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class DebugMenu : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private TextMeshProUGUI debugText;
    [SerializeField]
    private TextMeshProUGUI playerLocationText;
    [SerializeField]
    private TextMeshProUGUI buildVersionText;
    [SerializeField]
    private Canvas debugCanvas;
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private GameObject inventoryManagerPrefab;
    [SerializeField]
    private Transform playerTransform;

    // Variables to hold position data
    private InventoryManager inventoryManager;
    private Queue<string> messages = new Queue<string>();
    private List<FloatPair> movePosData = new List<FloatPair>();
    private Dictionary<string, List<FloatPair>> movePosDict = new Dictionary<string, List<FloatPair>>();
    private Vector3 lastPosition;

    // Register log message handling and find the local player on enable
    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
        FindLocalPlayer();
    }

    // Unregister log message handling on disable
    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    // Set the build version text on start
    void Start()
    {
        buildVersionText.text = "Build Version: " + Application.version;
    }


    void Update()
    {
        // Toggle debug canvas visibility with the '0' key
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            debugCanvas.enabled = !debugCanvas.enabled;
            Cursor.lockState = debugCanvas.enabled ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = debugCanvas.enabled;
        }

        // Update player position text and track movement if playerTransform is assigned
        if (playerTransform != null)
        {
            Vector3 currentPosition = playerTransform.position;
            playerLocationText.text = $"Position: {currentPosition.x:F2}, {currentPosition.y:F2}, {currentPosition.z:F2}";

            // Check if the player has moved more than a small threshold
            if (Vector3.Distance(currentPosition, lastPosition) > 0.01f)
            {
                AddMovePos(currentPosition.x, currentPosition.z);
                lastPosition = currentPosition;
                // Debug.Log($"Position Updated: {currentPosition}");
            }
        }
        else
        {
            // Debug.LogWarning("Player Transform is not assigned.");
            FindLocalPlayer(); // Try to find the player transform dynamically if not assigned
        }
    }

    // Handle and display log messages
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Log)
        {
            if (messages.Count >= 3)
            {
                messages.Dequeue();
            }

            messages.Enqueue(logString);
            debugText.text = string.Join("\n", messages.ToArray());
        }
    }

    // Add a new position to the movePosData list and movePosDict dictionary
    public void AddMovePos(float x, float z)
    {
        x = Mathf.Round(x * 100f) / 100f;
        z = Mathf.Round(z * 100f) / 100f;
        FloatPair newPos = new FloatPair(x, z);
        movePosData.Add(newPos);
        movePosDict[Time.time.ToString()] = new List<FloatPair>(movePosData);
        // Debug.Log($"Position added: {x}, {z}");
    }

    // Log the "Destroy All Traps" event
    public void DestroyAllTraps()
    {
        Debug.Log("Destroy All Traps event triggered");
    }

 // Spawn a dummy object in front of the player
    public void SpawnDummy()
    {
        if (playerTransform != null)
        {
            // Calculate spawn position in front of the player
            Vector3 spawnPosition = playerTransform.position + playerTransform.forward * 2;

            // Instantiate a new dummy prefab at the calculated position
            PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);

            Debug.Log("Spawn Dummy event triggered");
        }
        else
        {
            Debug.LogError("Player transform is not assigned. Cannot spawn dummy.");
        }
    }
    // Reload the current level
    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Instantiate the inventory manager and set all keycards as collected
    public void SetAllKeycardsCollected()
    {
        inventoryManager = Instantiate(inventoryManagerPrefab).GetComponent<InventoryManager>();
        inventoryManager.hasAllReqKeycards = true;
        Debug.Log("All keycards collected");
    }

    // Find and assign the local player's transform
    private void FindLocalPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            PhotonView photonView = player.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                playerTransform = player.transform;
                lastPosition = playerTransform.position;
                Debug.Log($"Local Player Transform assigned: {playerTransform.position}");
                break;
            }
        }

        if (playerTransform == null)
        {
            Debug.LogError("Local Player Transform not assigned. Make sure the local player has the 'Player' tag and PhotonView component.");
        }
    }
}

// Struct to hold float pairs for position data
[System.Serializable]
public struct FloatPair
{
    public float x;
    public float z;

    public FloatPair(float x, float z)
    {
        this.x = x;
        this.z = z;
    }
}
