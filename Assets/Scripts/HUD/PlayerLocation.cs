using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerLocationDisplay : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI locationText;
    [SerializeField] private GameObject playerPrefab; // Serialized field to manually assign the player prefab

    private Transform playerTransform;

    void Start()
    {
        if (playerPrefab == null)
        {
            InvokeRepeating("FindLocalPlayer", 0f, 0.5f);
            Debug.Log("Searching for local player...");
        }
        else
        {
            Debug.Log("Player prefab assigned in the inspector.");
            playerTransform = playerPrefab.transform;
        }
    }

    void FindLocalPlayer()
    {
        if (playerTransform == null) // Only search if we haven't found the player yet
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var player in players)
            {
                PhotonView photonView = player.GetComponent<PhotonView>();
                if (photonView != null && photonView.IsMine)
                {
                    playerPrefab = player; // Set the local player's prefab
                    playerTransform = player.transform; // Set the local player's transform
                    Debug.Log("Local player found: " + playerPrefab.name);
                    break;
                }
            }
        }
    }

    void Update()
    {
        if (playerTransform == null)
        {
            FindLocalPlayer(); // Continuously search for the player until found
        }
        else
        {
            UpdateLocationText();
        }
    }

    void UpdateLocationText()
    {
        if (locationText != null && playerTransform != null)
        {
            Vector3 playerPosition = playerTransform.position;
            locationText.text = $"X: {playerPosition.x:F2}, Y: {playerPosition.y:F2}, Z: {playerPosition.z:F2}";
        }
    }
}
