using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInOut : MonoBehaviour
{
    public CanvasGroup _canvasGroup;
    private bool _fadeIn = false;
    private bool _fadeOut = false;

    [SerializeField] private float _fadeStartTime;
    [SerializeField] private float _timeToFade;

    // Update is called once per frame
    public void Update()
    {
        if(_fadeIn && _canvasGroup.alpha < 1)
        {
            _canvasGroup.alpha += _timeToFade * Time.deltaTime;
            if(_canvasGroup.alpha >= 1) _fadeIn = false;
        }

        if(_fadeOut && _canvasGroup.alpha >= 0)
        {
            _canvasGroup.alpha -= _timeToFade * Time.deltaTime;
            if(_canvasGroup.alpha == 0) _fadeOut = false;
        }
    }

    public void ApplyFadeInOut()
    {
        StartCoroutine(FadeInOutCoroutine());
    }

    private IEnumerator FadeInOutCoroutine()
    {
        yield return new WaitForSeconds(_fadeStartTime);
        _fadeIn = true;
        yield return new WaitForSeconds(_timeToFade);
        _fadeOut = true;
    }
}
