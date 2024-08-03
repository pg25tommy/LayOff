using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraLookAt : MonoBehaviour
{
    [SerializeField] GameObject playerCam;
    [SerializeField] private string cameraName;

    private void Update()
    {
        playerCam = GameObject.Find(cameraName);
        if (playerCam != null) {
            transform.LookAt(playerCam.transform.position, Vector3.up);
        }
    }
}
