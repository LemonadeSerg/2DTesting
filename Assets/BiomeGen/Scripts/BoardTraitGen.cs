using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardTraitGen
{
    public void traitMarkBiome(BoardCollection[,] map)
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                map[x, y].connectedToOther = false;
                map[x, y].outerShell = false;

                if (x == 0 || x == map.GetLength(0) - 1 || y == 0 || y == map.GetLength(1) - 1)
                    map[x, y].outerShell = true;
                if (x > 0)
                    if (map[x - 1, y].biomeID != map[x, y].biomeID)
                        map[x, y].connectedToOther = true;
                if (y > 0)
                    if (map[x, y - 1].biomeID != map[x, y].biomeID)
                        map[x, y].connectedToOther = true;
                if (x < map.GetLength(0) - 1)
                    if (map[x + 1, y].biomeID != map[x, y].biomeID)
                        map[x, y].connectedToOther = true;
                if (y < map.GetLength(1) - 1)
                    if (map[x, y + 1].biomeID != map[x, y].biomeID)
                        map[x, y].connectedToOther = true;
            }
        }
    }
}