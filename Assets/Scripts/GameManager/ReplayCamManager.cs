// Created on Tue May 07 2024 || Copyright© 2024 || By: Alex Buzmion II
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class ReplayCamManager : MonoBehaviour
{
    [SerializeField, BoxGroup("PlaceholderVideos")] public List<RawImage> videoList; 
    // takes in the data passed by the invoking trap and triggers the replay cam
    public void ReplayDeathInCam(Component trap, object replayTime) {
        //todo replay real death in cam
        //! This is a placeholder only. The actual implementation will be done in the future.
        // checks what trap invoked the replay cam 
        if (trap is Component) {
            if (trap.GetType() == typeof(BouncingBettyEffect)) {
                videoList[0].gameObject.SetActive(true);
            }
            if (trap.GetType() == typeof(PoisonDartTrigger)) {
                videoList[1].gameObject.SetActive(true);
            }
            if (trap.GetType() == typeof(DetonatorEffect)) {
                videoList[2].gameObject.SetActive(true);
            }
            if (trap.GetType() == typeof(CeilingTrapEffect)) {
                videoList[3].gameObject.SetActive(true);
            }
        }
        if (replayTime is float) {
            StartCoroutine(TurnOffReplayCam(trap, (float)replayTime));
        }
    }

    private IEnumerator TurnOffReplayCam(Component trap, float replayTime) {
        yield return new WaitForSeconds(replayTime);
        if (trap is Component) {
            if (trap.GetType() == typeof(BouncingBettyEffect)) {
                videoList[0].gameObject.SetActive(false);
            }
            if (trap.GetType() == typeof(PoisonDartTrigger)) {
                videoList[1].gameObject.SetActive(false);
            }
            if (trap.GetType() == typeof(DetonatorEffect)) {
                videoList[2].gameObject.SetActive(false);
            }
            if (trap.GetType() == typeof(CeilingTrapEffect)) {
                videoList[3].gameObject.SetActive(false);
            }
        }
        //replayCam.gameObject.SetActive(false);
    }

}
