using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class HeatmapGen
{
    public void GenAllHeatMap(int biomeCount, BoardCollection[,] map)
    {
        for (int i = 1; i < biomeCount; i++)
        {
            genBiomeHeatMap(i, 4, map);
        }
    }

    private void genBiomeHeatMap(int biome, int maxWeight, BoardCollection[,] map)
    {
        while (freeSpaceFromBiome(biome, map) > 0)
            generateHeatmapVein(biome, maxWeight, map);
    }

    public void generateHeatmapVein(int biomeID, int weight, BoardCollection[,] map)
    {
        List<Vector2> freePoss = new List<Vector2>();
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y].biomeID == biomeID && map[x, y].weight == 0)
                {
                    freePoss.Add(new Vector2Int(x, y));
                }
            }
        }

        if (freePoss.Count > 0)
        {
            int Rand = Random.Range(0, freePoss.Count);
            map[(int)freePoss[Rand].x, (int)freePoss[Rand].y].weight = weight;
            downThePath(new Vector2Int((int)freePoss[Rand].x, (int)freePoss[Rand].y), map);
        }
    }

    public void applyReduction(BoardCollection[,] map)
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y].weight == 4)
                    if (Random.Range(0, 5) == 1)
                        map[x, y].weight = 2;
                if (map[x, y].weight == 3)
                    if (Random.Range(0, 3) == 1)
                        map[x, y].weight = 2;
            }
        }
    }

    private void downThePath(Vector2Int pos, BoardCollection[,] map)
    {
        if (pos.y >= 0 && pos.x >= 0 && pos.x < map.GetLength(0) && pos.y < map.GetLength(1) && map[pos.x, pos.y].weight > 2)
            if (getConnectedWeightedBiome(pos, map[pos.x, pos.y].biomeID, 0, map) > 0)
            {
                List<Vector2> freePoss = new List<Vector2>();
                if (pos.x > 0)
                    if (map[pos.x - 1, pos.y].biomeID == map[pos.x, pos.y].biomeID && map[pos.x - 1, pos.y].weight == 0)
                        freePoss.Add(new Vector2(-1, 0));
                if (pos.y > 0)
                    if (map[pos.x, pos.y - 1].biomeID == map[pos.x, pos.y].biomeID && map[pos.x, pos.y - 1].weight == 0)
                        freePoss.Add(new Vector2(0, -1));
                if (pos.x < map.GetLength(0) - 1)
                    if (map[pos.x + 1, pos.y].biomeID == map[pos.x, pos.y].biomeID && map[pos.x + 1, pos.y].weight == 0)
                        freePoss.Add(new Vector2(+1, 0));
                if (pos.y < map.GetLength(0) - 1)
                    if (map[pos.x, pos.y + 1].biomeID == map[pos.x, pos.y].biomeID && map[pos.x, pos.y + 1].weight == 0)
                        freePoss.Add(new Vector2(0, +1));
                int Rand = Random.Range(0, freePoss.Count);
                map[pos.x + (int)freePoss[Rand].x, pos.y + (int)freePoss[Rand].y].weight = Mathf.Clamp(map[pos.x, pos.y].weight + map[pos.x, pos.y].heatGrowthChange(), 3, 4);
                downThePath(new Vector2Int(pos.x + (int)freePoss[Rand].x, pos.y + (int)freePoss[Rand].y), map);
            }
    }

    public int freeSpaceFromBiome(int biomeID, BoardCollection[,] map)
    {
        int spaces = 0;
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y].biomeID == biomeID && map[x, y].weight == 0)
                    spaces++;
            }
        }
        return spaces;
    }

    private int getConnectedWeightedBiome(Vector2Int pos, int biomeID, int weight, BoardCollection[,] map)
    {
        int count = 0;
        if (pos.x > 0)
            if (map[pos.x - 1, pos.y].biomeID == map[pos.x, pos.y].biomeID && map[pos.x - 1, pos.y].weight == weight)
                count++;
        if (pos.y > 0)
            if (map[pos.x, pos.y - 1].biomeID == map[pos.x, pos.y].biomeID && map[pos.x, pos.y - 1].weight == weight)
                count++;
        if (pos.x < map.GetLength(0) - 1)
            if (map[pos.x + 1, pos.y].biomeID == map[pos.x, pos.y].biomeID && map[pos.x + 1, pos.y].weight == weight)
                count++;
        if (pos.y < map.GetLength(1) - 1)
            if (map[pos.x, pos.y + 1].biomeID == map[pos.x, pos.y].biomeID && map[pos.x, pos.y + 1].weight == weight)
                count++;
        return count;
    }
}