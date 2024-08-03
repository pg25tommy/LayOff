// Created on Sat Jul 20 2024 || Copyright© 2024 || By: Alex Buzmion II
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ReplayCam : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private Camera cam;
    [SerializeField] private List<Texture2D> frames = new List<Texture2D>();
    private float elapsedTime = 0f;
    private bool isRecording = false;
    private bool isPlaying = false;
    private bool recordingKill = false; 
    private ReplayDisplayWindow replayDisplayWindow;
    public float delayBetweenFrames = 0.05f;
    public RawImage displayWindow;

    private List<Texture2D> texturePool = new List<Texture2D>();
    private int maxPoolSize = 150; 

    private void Awake() {
        replayDisplayWindow = FindFirstObjectByType<ReplayDisplayWindow>();
        displayWindow = replayDisplayWindow.rawImage;
        cam.gameObject.SetActive(isRecording);
        //initialize texture pool 
        for (int i = 0; i < maxPoolSize; i++) {
            Texture2D texture = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
            texturePool.Add(texture);
        }
    }

    public void StartRecording()
    {
        isRecording = true;
        cam.gameObject.SetActive(isRecording);
        frames.Clear();
        elapsedTime = 0;
        Debug.Log($"{this.gameObject.name} started recording");
    }

    public void StopRecording()
    {
        isRecording = false;
        cam.gameObject.SetActive(isRecording);
        Debug.Log($"{this.gameObject.name} stopped recording");
    }

    public void StartPlayback(int ownerID)
    {
        Debug.Log($"StartPlayback called on {this.gameObject.name} by ownerID {ownerID}");
        if (ownerID != PhotonNetwork.LocalPlayer.ActorNumber) {
            Debug.Log($"StartPlayback: Not the owner. LocalPlayer ID: {PhotonNetwork.LocalPlayer.ActorNumber}, Owner ID: {ownerID}");
            return; // only playback for the trap owner 
        }
        replayDisplayWindow.ShowReplayWindow();
        Debug.Log($"StartPlayback: Showing replay window for owner ID {ownerID}");
        if (!isPlaying && frames.Count > 0) {
            StartCoroutine(Playback());
            Debug.Log($"{this.gameObject.name} playback playing");
        }
    }

    void RecordFrame() {
        if (isRecording) {
            Texture2D frame = CaptureFrame();
            // frames.Add(frame);
            if (frames.Count >= maxPoolSize) {
                frames.RemoveAt(0);
            }
            frames.Add(frame);
        }
    }

    Texture2D CaptureFrame() {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = cam.targetTexture;

        Texture2D frame = GetTextureFromPool();
        frame.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        frame.Apply();
        RenderTexture.active = currentRT;

        return frame;
    }

    void DisplayFrame(Texture2D frame)
    {
        displayWindow.texture = frame;
    }

    void LateUpdate() {
        if (!isRecording) return;
        if (!recordingKill) {
            elapsedTime += Time.deltaTime;
        }
        if (elapsedTime > 5f) {
            frames.Clear();
            elapsedTime = 0f;
        }
        RecordFrame();
    }

    IEnumerator Playback() {
        isPlaying = true;
        Debug.Log($"Playback() called and sent by {this.gameObject.name}");
        for (int i = 0; i < frames.Count; i++)
        {
            DisplayFrame(frames[i]);
            yield return new WaitForSeconds(delayBetweenFrames);
        }
        isPlaying = false;
        replayDisplayWindow.HideReplayWindow();
    }
    
    // function called by the trapeffect to stop the recording after a few seconds and play the recording
    public void InitiateReplay(int ownerID, ReplayCamTrigger camTrigger){
        photonView.RPC(nameof(RPC_InitiateReplay), RpcTarget.All, ownerID, camTrigger.photonView.ViewID);
    }

    [PunRPC]
    public void RPC_InitiateReplay(int ownerID, int camTriggerPVID) {
        GameObject camTriggerGO = PhotonView.Find(camTriggerPVID).gameObject;
        ReplayCamTrigger camTrigger = camTriggerGO.GetComponent<ReplayCamTrigger>();
        camTrigger.gameObject.GetComponent<Collider>().enabled = false; 
        StartCoroutine(StopRecordingDelay()); // wait a few seconds to capture the hit and ragdoll
        StartPlayback(ownerID); // start the playback
        PhotonNetwork.Destroy(camTrigger.gameObject);
    }
    private IEnumerator StopRecordingDelay() {
        recordingKill = true;
        yield return new WaitForSeconds(2f);
        StopRecording();
        recordingKill = false;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (!stream.IsWriting) {
            this.isRecording = (bool)stream.ReceiveNext();
            this.isPlaying = (bool)stream.ReceiveNext();
            return;
        }
        stream.SendNext(this.isRecording);
        stream.SendNext(this.isPlaying);
    }

    #region TexturePooling 
        private Texture2D GetTextureFromPool() {
            if (texturePool.Count > 0) {
                Texture2D texture = texturePool[0];
                texturePool.RemoveAt(0);
                return texture;
            }
            return new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
        }

        private void ReturnTextureToPool(Texture2D texture) {
            texturePool.Add(texture);
        }
    #endregion
}

