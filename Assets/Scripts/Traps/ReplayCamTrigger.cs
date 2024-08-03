// Created on Sat Jul 20 2024 || Copyright© 2024 || By: Alex Buzmion II
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class ReplayCamTrigger : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] RoomBehavior room; // the room this trap is in
    [SerializeField] public ReplayCam cctvCam; // the cam in the room where the trap is
    [SerializeField] bool recordingStarted; // whether the recording had already started
    [SerializeField] PhotonView playerPV; 
    public Collider collider;
    //todo exlude own player from recording its own trap 


    private void Awake() {
        // get the room to get access to the rooms camera
        collider = this.gameObject.GetComponent<Collider>();
        collider.enabled = false;
        recordingStarted = false;
        room = GetRoom();
        if (room != null) {
           StartCoroutine(RetrieveCam());
        }
    }

    private IEnumerator RetrieveCam() {
        for (int i = 0; i < 10; i++) {
            cctvCam = room.roomCam;
            Debug.LogWarning("attempted to look for cam in room");
            yield return new WaitForSeconds(.01f);
        }
        Debug.LogWarning("Cam in room found");
    }

    private void OnTriggerEnter(Collider other) {
        if (playerPV == null) {
            playerPV = other.GetComponent<PhotonView>();
        }
        if (!recordingStarted) {
            recordingStarted = true;
            cctvCam.StartRecording();
            Debug.Log($"{playerPV.Owner.NickName} started recording with {cctvCam.name}");
        }
    }

    public void OnTriggerExit(Collider other) {
        if (playerPV != null || playerPV.IsMine) {
            recordingStarted = false;
            cctvCam.StopRecording();
            Debug.Log($"{playerPV.Owner.NickName} stopped recording with {cctvCam.name}");
            playerPV = null;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (!stream.IsWriting) {
            this.recordingStarted = (bool)stream.ReceiveNext();
            return;
        }
        stream.SendNext(this.recordingStarted);
    }

    public RoomBehavior GetRoom()
    {
        RaycastHit roomHit;
        LayerMask floorMask = LayerMask.GetMask("RoomBehavior");
        RoomBehavior room = null;
        if (Physics.Raycast(this.transform.position, -this.transform.up, out roomHit, 20f, floorMask)) {
            room = roomHit.collider.gameObject.GetComponent<RoomBehavior>();
            if (room == null) {
                room = roomHit.collider.gameObject.GetComponentInParent<RoomBehavior>();
            }
            if (room != null){
                Debug.LogWarning($"Room found {room.name}");
            }
            else {
                Debug.LogWarning("RoomBehavior component not found on the hit object.");
                // Debug.Log($"raycast hit = {roomHit.collider.gameObject.name}");
            }
            return room;
        }
        else {
            Debug.Log("No room found");
            return null;
        }
    }

}
