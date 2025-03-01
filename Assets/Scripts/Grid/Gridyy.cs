using UnityEngine;

public class Gridyy
{
    private int width;
    private int height;
    private float cellSize;
    private float cellSpacing;
    private int[,] gridArray;

    public Gridyy(int width, int height, float cellSize, float cellSpacing)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.cellSpacing = cellSpacing;

        gridArray = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

            }
        }
    }


}
