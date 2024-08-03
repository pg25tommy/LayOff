// Created on Sun Apr 07 2024 || Copyright© 2024 || By: Alex Buzmion II
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class JoinAndCreate : MonoBehaviourPunCallbacks
{
    // this holds a reference to the create input field
    public TextMeshProUGUI createInputField;
    // this holds a reference to the join input field
    public TextMeshProUGUI joinInputField;
    // create a room (automatically joins the room created)
    [SerializeField] string sceneToLoad;

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createInputField.text.ToLower());
    }
    
    // join a room
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInputField.text.ToLower());
    }
    
    // Attempts to join a room that matches the specified filter and creates a room if none found.
    public void JoinRandomOrCreate(){
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    // on join room, load the gameplay scene
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(sceneToLoad);
    }
}
