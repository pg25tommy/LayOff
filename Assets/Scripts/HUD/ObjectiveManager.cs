using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Sirenix.OdinInspector;
using Photon.Pun;

public class ObjectiveManager : MonoBehaviourPun
{
    [SerializeField] private TextMeshProUGUI HUDText;
    [SerializeField] private float textDuration = 5f;
    public string objectiveText = "1. Loot Props\n2. Get 3 KeyCards\n3. Open Laser Door \n4. Press the button to Win";

    private void Awake()
    {
        if (HUDText.gameObject.activeSelf)
        {
            HUDText.text = objectiveText;
        }
        StartCoroutine(TurnOffHudText());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StopCoroutine(TurnOffHudText());
            ToogleText();
        }

        if (!HUDText.gameObject.activeSelf)
        {
            StopAllCoroutines();
        }
    }

    public void ToogleText()
    {
        HUDText.gameObject.SetActive(!HUDText.gameObject.activeSelf);
        if(HUDText.gameObject.activeSelf)
        {
            HUDText.text = objectiveText;
        }
        StartCoroutine(TurnOffHudText());
    }

    IEnumerator TurnOffHudText()
    {
        yield return new WaitForSeconds(textDuration);
        HUDText.gameObject.SetActive(false);
    }
}
