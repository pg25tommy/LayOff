// Created on tuesday Apr 09 2024 || Copyright© 2024 || By: Tommy Minter
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Collections;
using Sirenix.OdinInspector;

public class TimerManager : MonoBehaviourPunCallbacks
{
    [BoxGroup("Refs"), SerializeField] private TMP_Text timerText;  // customizable timer text

    private GameObject timerObj;
    [BoxGroup("Timer Settings")] public float timeRemaining = 600; // time remaining in seconds
    [BoxGroup("Timer Settings")] public float startTimer = 600; // timer in seconds 
    [BoxGroup("Timer Settings")] public bool timerIsRunning = false; // timer running state
    
    [BoxGroup("Timer Settings")] public float timerSizeDelay = 1.5f;
    public string animTimerIncrease;
    public string animTimerDecrease;
    private Animator animator;
    public float turnRedTime = 30f;
    public static TimerManager instance;

    private double lastSyncTime = 0; // Time at last sync to control update rate
    
    private void Awake()
    {
        instance = this;
        timerObj = timerText.transform.parent.gameObject;

        animator = timerText.GetComponentInParent<Animator>();
    }

    private void Start()
    {
        StartCoroutine(DecreaseTimer(timerSizeDelay));
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 1) return;
        if (PhotonNetwork.IsMasterClient && timerIsRunning) 
        {
            if (timeRemaining <= 50)
            {
                // Increase Timer size
                animator.Play(animTimerIncrease);
            }
            if(timeRemaining <= turnRedTime)
            {
                timerText.color = Color.red;
            }

            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                // Sync timer every second or significant change
                if (PhotonNetwork.Time - lastSyncTime > 1)
                {
                    GameManager.Instance.photonView.RPC("UpdateTimer", RpcTarget.All, timeRemaining);
                    lastSyncTime = PhotonNetwork.Time;
                }
            }
            if(timeRemaining <= 0)
            {
                StopTimer();
                GameManager.Instance.photonView.RPC("EndGame", RpcTarget.All, 5);
            }
        }
    }

    [PunRPC]
    public void StartTimer()
    {
        timerIsRunning = true;
        timeRemaining = startTimer;
        lastSyncTime = PhotonNetwork.Time; // Start sync timing
    }

    [PunRPC]
    public void UpdateTimer(float syncedTime) {
        timeRemaining = syncedTime;
        DisplayTime(timeRemaining);
    }

    public void StopTimer() {
        timeRemaining = 0;
        timerIsRunning = false;
        Debug.Log("Attempting to stop timer via RPC");
        GameManager.Instance.photonView.RPC("StopTimerRPC", RpcTarget.All);
    }

    [PunRPC]
    public void StopTimerRPC() {
        timerIsRunning = false; // Ensure all clients stop the timer
    }
   
    IEnumerator DecreaseTimer(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Decrease timer size
        animator.Play(animTimerDecrease);
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1; 

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
