using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGen
{
    private int biomeCount;
    public Color[] biomeColors;

    public void init(int biomeCount)
    {
        this.biomeCount = biomeCount;
        biomeColors = new Color[biomeCount];
        for (int i = 0; i < biomeCount; i++)
        {
            biomeColors[i] = new Color(Random.Range(0F, 1F), Random.Range(0, 1F), Random.Range(0, 1F));
        }
    }

    public void growBiomeAll(BoardCollection[,] map)
    {
        while (cleanSpaceCount(map) > 0)
        {
            growBiome(map);
        }
    }

    public void placeBiomeStarts(BoardCollection[,] map)
    {
        for (int i = 1; i < biomeCount; i++)
        {
            map[Random.Range(0, map.GetLength(0) - 1), Random.Range(0, map.GetLength(1) - 1)].biomeID = i;
        }
    }

    public void growBiome(BoardCollection[,] map)
    {
        BoardCollection[,] tempMap = new BoardCollection[map.GetLength(0), map.GetLength(1)];

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                tempMap[x, y] = new BoardCollection();
                tempMap[x, y].biomeID = map[x, y].biomeID;
            }
        }

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (tempMap[x, y].biomeID != 0)
                {
                    if (x > 0)
                        if (tempMap[x - 1, y].biomeID == 0)
                            map[x - 1, y].biomeID = expandChance(tempMap[x, y].biomeID, 1);
                    if (y > 0)
                        if (tempMap[x, y - 1].biomeID == 0)
                            map[x, y - 1].biomeID = expandChance(tempMap[x, y].biomeID, 2);
                    if (x < map.GetLength(1) - 1)
                        if (tempMap[x + 1, y].biomeID == 0)
                            map[x + 1, y].biomeID = expandChance(tempMap[x, y].biomeID, 3);
                    if (y < map.GetLength(1) - 1)
                        if (tempMap[x, y + 1].biomeID == 0)
                            map[x, y + 1].biomeID = expandChance(tempMap[x, y].biomeID, 4);
                }
            }
        }
    }

    public void setBaseBiomeColor(BoardCollection[,] map)
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                map[x, y].color = biomeColors[map[x, y].biomeID];
            }
        }
    }

    public int expandChance(int biomeID, int dir)
    {
        switch (biomeID)
        {
            case 1:
                return dir == 1 ? Random.Range(0, 4) == 1 ? biomeID : 0 :
                    dir == 2 ? Random.Range(0, 4) == 1 ? biomeID : 0 :
                    dir == 3 ? Random.Range(0, 2) == 1 ? biomeID : 0 :
                    dir == 4 ? Random.Range(0, 2) == 1 ? biomeID : 0 : 0;

            case 2:
                return dir == 1 ? Random.Range(0, 2) == 1 ? biomeID : 0 :
                    dir == 2 ? Random.Range(0, 2) == 1 ? biomeID : 0 :
                    dir == 3 ? Random.Range(0, 4) == 1 ? biomeID : 0 :
                    dir == 4 ? Random.Range(0, 4) == 1 ? biomeID : 0 : 0;

            case 3:
                return dir == 1 ? Random.Range(0, 2) == 1 ? biomeID : 0 :
                    dir == 2 ? Random.Range(0, 4) == 1 ? biomeID : 0 :
                    dir == 3 ? Random.Range(0, 2) == 1 ? biomeID : 0 :
                    dir == 4 ? Random.Range(0, 4) == 1 ? biomeID : 0 : 0;

            case 4:
                return dir == 1 ? Random.Range(0, 4) == 1 ? biomeID : 0 :
                    dir == 2 ? Random.Range(0, 2) == 1 ? biomeID : 0 :
                    dir == 3 ? Random.Range(0, 4) == 1 ? biomeID : 0 :
                    dir == 4 ? Random.Range(0, 2) == 1 ? biomeID : 0 : 0;

            default:
                return Random.Range(0, 5) == 1 ? biomeID : 0;
        }
    }

    public int cleanSpaceCount(BoardCollection[,] map)
    {
        int count = 0;
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y].biomeID == 0)
                    count++;
            }
        }
        return count;
    }
}