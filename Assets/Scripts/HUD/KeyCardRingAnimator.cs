// Created on Sun Jul 14 2024 || Copyright© 2024 || By: Alex Buzmion II
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCardRingAnimator : MonoBehaviour
{
    [SerializeField] GameObject innerRing; 
    [SerializeField] GameObject middleRing; 
    [SerializeField] GameObject outerRing; 
    [SerializeField] float initScale = .41f;  
   private Vector3 maxScale = new Vector3 (.3f,.3f,.3f);
   private Vector3 stepSize = new Vector3(0.01f, 0.01f, 0.01f);
   [SerializeField] List<GameObject> ringList; 

   private void Awake() {
        IncreaseRingSize(); // set ring size to the max size onset
   }
    // 
    private void OnEnable() {
        StartCoroutine(IndicateRings()); // when active, start animating the ring through coroutine
    }
    private IEnumerator IndicateRings() {
        foreach (GameObject ring in ringList) { // iterate through all the rings and animate decrease size from max to desired size
            while (ring.transform.localScale.x >= initScale) {
                ring.transform.localScale -= stepSize;
                yield return null; 
            }
            ring.transform.localScale = new Vector3(initScale, initScale, initScale); // ensure final scale is the exact scale
        }
        IncreaseRingSize(); // increase ring sizes again and re-run animation 
        StartCoroutine(IndicateRings());
    }

    private void IncreaseRingSize() {
        foreach (GameObject ring in ringList) {
            ring.transform.localScale += maxScale;
        }
    }
}
