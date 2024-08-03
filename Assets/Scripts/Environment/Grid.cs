// Created on Fri May 17 2024 || Copyright© 2024 || By: Alex Buzmion II

using UnityEngine;

public class Grid
{
    private int width; 
    private int height; 
    private int[,] gridArray;

    public Grid(int width, int height) {
        this.width = width; 
        this.height = height; 

        gridArray = new int [width, height];
    }

    // Method to draw the grid
    public void DrawGrid() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Debug.DrawLine(new Vector3(x, 0), new Vector3(x, height), Color.red, 100f);
                Debug.DrawLine(new Vector3(0, y), new Vector3(width, y), Color.red, 100f);
            }
        }
    }
}
