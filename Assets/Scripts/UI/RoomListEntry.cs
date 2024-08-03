using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomListEntry : MonoBehaviour
{
    public TextMeshProUGUI RoomNameText;
    public TextMeshProUGUI RoomPlayersText;
    public Button JoinRoomButton;
    [SerializeField] public GameObject joinInputField; 

    private string roomName;

    public void Start()
    {
        // JoinRoomButton.onClick.AddListener(() =>
        // {
        //     if (PhotonNetwork.InLobby)
        //     {
        //         PhotonNetwork.LeaveLobby();
        //     }

        //     PhotonNetwork.JoinRoom(roomName);
        // });
    }

    public void Initialize(string name, byte currentPlayers, byte maxPlayers, GameObject inputField)
    {
        roomName = name;

        RoomNameText.text = name;
        RoomPlayersText.text = currentPlayers + " / " + maxPlayers;
        joinInputField = inputField;
    }

    public void UpdateJoinRoomTextField() {
        if (joinInputField != null) {
            joinInputField.GetComponent<TMP_InputField>().text= roomName.Trim((char)8203);
           Debug.Log($"RoomListEntry clicked. passing {roomName}, to {joinInputField.name}, with text ");
        }
    }
}
