using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGeneration
{
    public void wallOffBiomes(BoardCollection[,] map)
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (x == map.GetLength(0) - 1)
                    map[x, y].rightWall = true;
                if (x == 0)
                    map[x, y].leftWall = true;
                if (y == map.GetLength(1) - 1)
                    map[x, y].bottomWall = true;
                if (y == 0)
                    map[x, y].topWall = true;
                if (x > 0)
                    if (map[x - 1, y].biomeID != map[x, y].biomeID)
                    {
                        map[x, y].leftWall = true;
                    }
                if (y > 0)
                    if (map[x, y - 1].biomeID != map[x, y].biomeID)
                    {
                        map[x, y].topWall = true;
                    }
                if (x < map.GetLength(0) - 1)
                    if (map[x + 1, y].biomeID != map[x, y].biomeID)
                    {
                        map[x, y].rightWall = true;
                    }
                if (y < map.GetLength(1) - 1)
                    if (map[x, y + 1].biomeID != map[x, y].biomeID)
                    {
                        map[x, y].bottomWall = true;
                    }
            }
        }
    }

    public void genAllInternallWalls(int biomeCount, BoardCollection[,] map)
    {
        for (int i = 0; i < biomeCount; i++)
        {
            genBoardWall(i, map);
        }
    }

    private void genBoardWall(int biomeID, BoardCollection[,] map)
    {
        if (getSpaceWithFreeWallsCount(biomeID, 3, map) > 0)
            setWalls(getSpaceWithFreeWalls(biomeID, 3, map), map);
        else if (getSpaceWithFreeWallsCount(biomeID, 2, map) > 0)
            setWalls(getSpaceWithFreeWalls(biomeID, 2, map), map);
        else if (getSpaceWithFreeWallsCount(biomeID, 1, map) > 0)
            setWalls(getSpaceWithFreeWalls(biomeID, 1, map), map);
        if (getSpaceWithFreeWallsCount(biomeID, 3, map) > 0 || getSpaceWithFreeWallsCount(biomeID, 2, map) > 0 || getSpaceWithFreeWallsCount(biomeID, 1, map) > 0)
            genBoardWall(biomeID, map);
    }

    public void setWalls(Vector2Int pos, BoardCollection[,] map)
    {
        List<Vector2> freePos = new List<Vector2>();

        if (pos.x < map.GetLength(0) - 1)
            if (map[pos.x + 1, pos.y].getWallCount() < 3 && map[pos.x, pos.y].weight >= 1)
                freePos.Add(new Vector2(+1, 0));
        if (pos.x > 0)
            if (map[pos.x - 1, pos.y].getWallCount() < 3 && map[pos.x, pos.y].weight >= 1)
                freePos.Add(new Vector2(-1, 0));
        if (pos.y < map.GetLength(1) - 1)
            if (map[pos.x, pos.y + 1].getWallCount() < 3 && map[pos.x, pos.y].weight >= 1)
                freePos.Add(new Vector2(0, +1));
        if (pos.y > 0)
            if (map[pos.x, pos.y - 1].getWallCount() < 3 && map[pos.x, pos.y].weight >= 1)
                freePos.Add(new Vector2(0, -1));

        if (freePos.Count > 0)
        {
            int Rand = Random.Range(0, freePos.Count);
            if (pos.x + freePos[Rand].x > pos.x)
            {
                map[pos.x + (int)freePos[Rand].x, pos.y + (int)freePos[Rand].y].leftWall = true;
                map[pos.x, pos.y].rightWall = true;
            }
            if (pos.x + freePos[Rand].x < pos.x)
            {
                map[pos.x + (int)freePos[Rand].x, pos.y + (int)freePos[Rand].y].rightWall = true;
                map[pos.x, pos.y].leftWall = true;
            }
            if (pos.y + freePos[Rand].y > pos.y)
            {
                map[pos.x + (int)freePos[Rand].x, pos.y + (int)freePos[Rand].y].topWall = true;
                map[pos.x, pos.y].bottomWall = true;
            }
            if (pos.y + freePos[Rand].y < pos.y)
            {
                map[pos.x + (int)freePos[Rand].x, pos.y + (int)freePos[Rand].y].bottomWall = true;
                map[pos.x, pos.y].topWall = true;
            }
        }
    }

    public Vector2Int getSpaceWithFreeWalls(int biomeID, int weight, BoardCollection[,] map)
    {
        Vector2Int vec2 = new Vector2Int();
        List<Vector2> freePos = new List<Vector2>();
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y].biomeID == biomeID && map[x, y].getWallCount() < 4 - map[x, y].weight && map[x, y].weight == weight)
                {
                    freePos.Add(new Vector2(x, y));
                }
            }
        }
        if (freePos.Count > 0)
        {
            int Rand = Random.Range(0, freePos.Count);
            return new Vector2Int((int)freePos[Rand].x, (int)freePos[Rand].y);
        }
        return vec2;
    }

    public int getSpaceWithFreeWallsCount(int biomeID, int weight, BoardCollection[,] map)
    {
        int count = 0;
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y].biomeID == biomeID && map[x, y].weight == weight && map[x, y].getWallCount() < 4 - map[x, y].weight)
                {
                    count++;
                }
            }
        }
        return count;
    }
}