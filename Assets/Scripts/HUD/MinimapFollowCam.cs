// Created on Fri Jun 28 2024 || Copyright© 2024 || By: Alex Buzmion II
using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class MinimapFollowCam : MonoBehaviourPunCallbacks
{
    public static MinimapFollowCam Instance; 
    public Transform player; 
    private Camera cam;
    private Transform target;
    public bool isZoomedOut = true; 
    [SerializeField] private float originalZoom = 10f; 
    [SerializeField] private float zoomOutSize = 50f; 
    [SerializeField] private float zoomStep = .5f;
    [SerializeField] private GameObject zoomOutFollow;

    private void Awake() {
        Instance = this;
        cam = GetComponent<Camera>();
        target = zoomOutFollow.transform;
    }

    public void AssignPlayer(Transform spawnedPlayer) {
        player = spawnedPlayer;
    }

    private void LateUpdate() {
        target = isZoomedOut ? zoomOutFollow.transform : player; 
        Vector3 newPosition = target.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
        //? turned off the camera rotation for now due to feedback
        // if (player != null) {
        //     transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0);
        // } 
    }

    public void ResetZoom() {
        isZoomedOut = false; 
        StartCoroutine(SmoothStep(originalZoom)); 
    }

    private IEnumerator SmoothStep(float targetSize){
        while (Mathf.Abs(cam.orthographicSize - targetSize) > 0.01f){
            cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, targetSize, zoomStep);
            yield return null;
        }
        cam.orthographicSize = targetSize;
    }

    public void ZoomOut() {
        isZoomedOut = true; 
        StartCoroutine(SmoothStep(zoomOutSize));
    }
}
