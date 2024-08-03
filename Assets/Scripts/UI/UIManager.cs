using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject UIMovement;
    public GameObject UITrapPlacement;

    public static UIManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void ToggleControlsUI(bool movementUI, bool trapPlacementUI)
    {
        UIMovement.SetActive(movementUI);
        UITrapPlacement.SetActive(trapPlacementUI);
    }
}