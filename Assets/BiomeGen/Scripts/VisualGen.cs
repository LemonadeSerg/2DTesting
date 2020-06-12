using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualGen
{
    public Texture2D Colormap;
    public Texture2D grid;

    public int gridScale = 10;

    public void init(BoardCollection[,] map)
    {
        Colormap = new Texture2D(map.GetLength(0), map.GetLength(1));
        Colormap.filterMode = FilterMode.Point;

        grid = new Texture2D(map.GetLength(0) * gridScale, map.GetLength(1) * gridScale);
        grid.filterMode = FilterMode.Point;

        for (int x = 0; x < map.GetLength(0) * gridScale; x++)
        {
            for (int y = 0; y < map.GetLength(1) * gridScale; y++)
            {
                grid.SetPixel(x, y, Color.clear);
            }
        }
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                Colormap.SetPixel(x, y, Color.clear);
            }
        }
    }

    public void updateGraphics(BoardCollection[,] map)
    {
        updateMapTexture(map);
        updateGridTexture(map);
    }

    private void updateMapTexture(BoardCollection[,] map)
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                Colormap.SetPixel(x, Colormap.height - y - 1, map[x, y].color);
            }
        }
        Colormap.Apply();
    }

    private void updateGridTexture(BoardCollection[,] map)
    {
        for (int x = 0; x < map.GetLength(0) * gridScale; x++)
        {
            for (int y = 0; y < map.GetLength(1) * gridScale; y++)
            {
                grid.SetPixel(x, y, Color.clear);
            }
        }
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y].topWall)
                {
                    for (int x2 = 0; x2 < map.GetLength(0) * gridScale / map.GetLength(0); x2++)
                    {
                        grid.SetPixel((x * (map.GetLength(0) * gridScale / map.GetLength(0))) + x2, map.GetLength(1) * gridScale - 1 - (y * (map.GetLength(1) * gridScale / map.GetLength(1))), Color.white);
                    }
                }
                if (map[x, y].bottomWall)
                {
                    for (int x2 = 0; x2 < map.GetLength(0) * gridScale / map.GetLength(0); x2++)
                    {
                        grid.SetPixel((x * (map.GetLength(0) * gridScale / map.GetLength(0))) + x2, map.GetLength(1) * gridScale - ((y + 1) * (map.GetLength(1) * gridScale / map.GetLength(1))), Color.white);
                    }
                }
                if (map[x, y].leftWall)
                {
                    for (int y2 = 0; y2 < map.GetLength(1) * gridScale / map.GetLength(1); y2++)
                    {
                        grid.SetPixel(x * (map.GetLength(0) * gridScale / map.GetLength(0)), map.GetLength(1) * gridScale - (y * (map.GetLength(1) * gridScale / map.GetLength(1))) - y2 - 1, Color.white);
                    }
                }
                if (map[x, y].rightWall)
                {
                    for (int y2 = 0; y2 < map.GetLength(1) * gridScale / map.GetLength(1); y2++)
                    {
                        grid.SetPixel(((x + 1) * (map.GetLength(0) * gridScale / map.GetLength(0))) - 1, map.GetLength(1) * gridScale - (y * (map.GetLength(1) * gridScale / map.GetLength(1))) - y2 - 1, Color.white);
                    }
                }
            }
        }

        grid.Apply();
    }
}