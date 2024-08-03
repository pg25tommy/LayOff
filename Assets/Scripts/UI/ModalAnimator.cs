// Created on Sun Jul 07 2024 || Copyright© 2024 || By: Alex Buzmion II
using System.Collections;
using UnityEngine;
using FMODUnity;

public class ModalAnimator : MonoBehaviour
{
    [SerializeField] GameObject createRoomModal;
    [SerializeField] GameObject joinRoomModal;
    [SerializeField] GameObject optionsModal;
    [SerializeField] GameObject creditsModal;
    [SerializeField] GameObject insideRoomModal;
    [SerializeField] protected EventReference MainUISFX;
    private Vector3 stepSize =  new Vector3(.2f, .2f, .2f);
    

    public void EnlargeModal(string modal) {
        RuntimeManager.PlayOneShot(MainUISFX, transform.position); // FMOD Sound event playing
        
        switch (modal) {
            case "createRoom":
                StartCoroutine(Animate(createRoomModal));
                break;    
            case "joinRoom":
                StartCoroutine(Animate(joinRoomModal));
                break;
            case "options":
                StartCoroutine(Animate(optionsModal));
                break;
            case "credits":
                StartCoroutine(Animate(creditsModal));
                break;
            case "insideRoom":
                StartCoroutine(Animate(creditsModal));
                break;
            default:
                RuntimeManager.PlayOneShot(MainUISFX, transform.position); // FMOD Sound event playing
                break;
        }
    }

    private IEnumerator Animate(GameObject modal) {
        RuntimeManager.PlayOneShot(MainUISFX, transform.position); // FMOD Sound event playing
        modal.transform.localScale = new Vector3(0, 0,0); 
        while (modal.transform.localScale.x <= 1.2 ) {
            modal.transform.localScale += stepSize; 
            yield return null;
        }
        modal.transform.localScale = new Vector3(1,1,1); 
    }

    private IEnumerator Hide(GameObject modal) {
        while (modal.transform.localScale.x > 0) {
            modal.transform.localScale -= stepSize; 
            yield return null; 
        }
    }
}
