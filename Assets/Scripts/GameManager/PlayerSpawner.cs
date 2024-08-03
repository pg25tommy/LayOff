// Created on Sun Apr 07 2024 || Copyright© 2024 || By: Alex Buzmion II
using System.Collections.Generic;
using CharacterMovement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] List<Transform> spawnPoints;
    [SerializeField] GameObject spawnPointParent;

    private void Awake()
    {
        if (spawnPointParent != null)
        {
            foreach (Transform child in spawnPointParent.transform)
            {
                spawnPoints.Add(child);
            }
        }
    }

    private void Start() {
        if (PhotonNetwork.InLobby || PhotonNetwork.InRoom) {
            SpawnPlayer(null, null);
        }
    }

    public void SpawnPlayer(Component component, object data) {
        if (spawnPoints == null) return; 
        // chooses a random spawn point in one of the spawn points in the list
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoints[Random.Range(0, spawnPoints.Count)].position, Quaternion.identity);
        string stringkey; 
        
        switch (PhotonNetwork.LocalPlayer.ActorNumber) {
            case 1:
                stringkey = "red"; 
                break;
            case 2:
                stringkey = "green"; 
                break;
            case 3:
                stringkey = "blue"; 
                break;
            case 4:
                stringkey = "yellow";
                break;
            default:
                stringkey = "red";
                break;
        }
        // TextRenderer.TRInstance.UpdateText($"This clients actor number is {PhotonNetwork.LocalPlayer.ActorNumber} and color {stringkey}");
        player.GetComponentInChildren<MaterialColorChanger>().ChangePlayerMaterial(stringkey);
    }

    public void OnPlayerDeath(Component sender, object data) {
        if (data is GameObject player) {
            RespawnPlayer(player);
        }
        else {
            Debug.Log("Data is not a GameObject");
        }
    }

    public void RespawnPlayer(GameObject player) {
        // get the parent of this gameobject and move it to a random spawn point
        Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
        Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        if (playerRigidbody != null) {
            playerRigidbody.MovePosition(randomPoint.position);
        } else {
            player.transform.position = randomPoint.position;
        }
        player.transform.position = randomPoint.transform.position;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
    }
}
