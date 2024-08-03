// Created on Fri May 17 2024 || Copyright© 2024 || By: Alex Buzmion II
using UnityEngine;

public class RoomGrid : MonoBehaviour
{
    void Awake() {
        // get the world scale of the game object
        Vector3 worldScale = transform.lossyScale;
        int gridWidth = Mathf.RoundToInt(worldScale.x);
        int gridHeight = Mathf.RoundToInt(worldScale.z);

        // create a grid instance with the world scale dimensions
        Grid grid = new Grid(gridWidth, gridHeight);
        Debug.Log($"New Grid created with {gridWidth} and {gridHeight}");

        // draw the grid
        grid.DrawGrid();
    }
}
