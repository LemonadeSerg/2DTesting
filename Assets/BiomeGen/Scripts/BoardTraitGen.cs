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
                if (map[x, y].getWallCount() == 3)
                {
                    if (!map[x, y].TopWall)
                    {
                        map[x, y].OrType = BoardCollection.OrientationType.ClearT;
                    }
                    if (!map[x, y].BottomWall)
                    {
                        map[x, y].OrType = BoardCollection.OrientationType.ClearB;
                    }
                    if (!map[x, y].LeftWall)
                    {
                        map[x, y].OrType = BoardCollection.OrientationType.ClearL;
                    }
                    if (!map[x, y].RightWall)
                    {
                        map[x, y].OrType = BoardCollection.OrientationType.ClearR;
                    }
                }
                else
                if (map[x, y].getWallCount() == 2)
                {
                    if (!map[x, y].TopWall)
                    {
                        if (!map[x, y].BottomWall)
                        {
                            map[x, y].OrType = BoardCollection.OrientationType.ClearTB;
                        }
                        if (!map[x, y].LeftWall)
                        {
                            map[x, y].OrType = BoardCollection.OrientationType.ClearTL;
                        }
                        if (!map[x, y].RightWall)
                        {
                            map[x, y].OrType = BoardCollection.OrientationType.ClearTR;
                        }
                    }
                    if (!map[x, y].RightWall)
                    {
                        if (!map[x, y].BottomWall)
                        {
                            map[x, y].OrType = BoardCollection.OrientationType.ClearRB;
                        }
                        if (!map[x, y].LeftWall)
                        {
                            map[x, y].OrType = BoardCollection.OrientationType.ClearRL;
                        }
                    }
                    if (!map[x, y].BottomWall)
                    {
                        if (!map[x, y].LeftWall)
                        {
                            map[x, y].OrType = BoardCollection.OrientationType.ClearBL;
                        }
                    }
                }
                else
                if (map[x, y].getWallCount() == 1)
                {
                    if (map[x, y].TopWall)
                    {
                        map[x, y].OrType = BoardCollection.OrientationType.ClearRBL;
                    }
                    if (map[x, y].RightWall)
                    {
                        map[x, y].OrType = BoardCollection.OrientationType.ClearBLT;
                    }
                    if (map[x, y].BottomWall)
                    {
                        map[x, y].OrType = BoardCollection.OrientationType.ClearTRL;
                    }
                    if (map[x, y].LeftWall)
                    {
                        map[x, y].OrType = BoardCollection.OrientationType.ClearTRB;
                    }
                }
                else
                {
                    map[x, y].OrType = BoardCollection.OrientationType.Clear;
                }
            }
        }
    }
}