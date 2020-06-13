using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewWallGeneration
{
    public int RandomWallCount;
    private int Rand;

    public bool noMore;

    public void addPerimeterBreakoffs(BoardCollection[,] map)
    {
        for (int x2 = 0; x2 < map.GetLength(0); x2++)
        {
            for (int y2 = 0; y2 < map.GetLength(0); y2++)
            {
                if ((map[x2, y2].OuterShell || map[x2, y2].ConnectedToOther) && map[x2, y2].getWallCount() < 3)
                {
                    int Rand = Random.Range(1, 5);

                    if (Rand == 1 && !map[x2, y2].RightWall && map[x2 + 1, y2].getWallCount() < 2)
                    {
                        map[x2, y2].RightWall = true;
                        map[x2 + 1, y2].LeftWall = true;
                    }
                    if (Rand == 2 && !map[x2, y2].LeftWall && map[x2 - 1, y2].getWallCount() < 2)
                    {
                        map[x2 - 1, y2].RightWall = true;
                        map[x2, y2].LeftWall = true;
                    }
                    if (Rand == 3 && !map[x2, y2].TopWall && map[x2, y2 - 1].getWallCount() < 2)
                    {
                        map[x2, y2].TopWall = true;
                        map[x2, y2 - 1].BottomWall = true;
                    }
                    if (Rand == 4 && !map[x2, y2].BottomWall && map[x2, y2 + 1].getWallCount() < 2)
                    {
                        map[x2, y2 + 1].TopWall = true;
                        map[x2, y2].BottomWall = true;
                    }
                }
            }
        }
    }

    public bool found = false;
    public bool wallGened = false;

    public void buildWall(BoardCollection[,] map, int wallBreakPoints)
    {
        wallGened = true;
        found = false;
        for (int x2 = 0; x2 < map.GetLength(0); x2++)
        {
            for (int y2 = 0; y2 < map.GetLength(0); y2++)
            {
                buildWall(x2, y2, map, wallBreakPoints);
            }
        }
        if (found)
            wallGened = false;
    }

    public void buildWall(int x, int y, BoardCollection[,] map, int wallBreakPoints)
    {
        List<Vector2> boardPos = new List<Vector2>();
        List<string> wallDir = new List<string>();
        if (map[x, y].getWallCount() > 0)
        {
            //Check start Connectiions to Bottom Wall
            if (map[x, y].BottomWall && y > 0)
            {
                //Check if valid Up on the Left and Right side are valid
                if (!map[x, y].TopWall && y > 0)
                {
                    //Check to place on right side
                    if (!map[x, y].RightWall)
                    {
                        //Check to see if it will connect to another wall ignore if so
                        if (!map[x + 1, y - 1].BottomWall && !map[x + 1, y - 1].LeftWall)
                        {
                            boardPos.Add(new Vector2(x, y));
                            wallDir.Add("R");
                            //valid 1
                        }
                    }
                    //Check to place to left side
                    if (!map[x, y].LeftWall)
                    {
                        //Check to see if it will connect to another wall ignore if so
                        if (!map[x - 1, y - 1].BottomWall && !map[x - 1, y - 1].RightWall)
                        {
                            boardPos.Add(new Vector2(x, y));
                            wallDir.Add("L");
                            //valid 2
                        }
                    }
                }
                //Check if right wall conflict
                if (x < map.GetLength(0) - 2)
                {
                    //Check if valid Left free
                    if (!map[x + 1, y].BottomWall)
                    {
                        //Check to see if it will connect to another wall ignore if so
                        if (!map[x + 2, y].BottomWall && !map[x + 2, y].LeftWall && !map[x + 2, y + 1].TopWall && !map[x + 2, y + 1].LeftWall)
                        {
                            boardPos.Add(new Vector2(x + 1, y));
                            wallDir.Add("D");
                            //Valid 3
                        }
                    }
                }
                //Check if left wall conflict
                if (x > 1)
                {
                    //Check if valid Right free
                    if (!map[x - 1, y].BottomWall)
                    {
                        //Check to see if it will connect to another wall ignore if so
                        if (!map[x - 2, y].BottomWall && !map[x - 2, y].RightWall && !map[x - 2, y + 1].TopWall && !map[x - 2, y + 1].RightWall)
                        {
                            boardPos.Add(new Vector2(x - 1, y));
                            wallDir.Add("D");
                            //Valid 3
                        }
                    }
                }
            }

            //Check Valid start to Top Wall
            if (map[x, y].TopWall && y < map.GetLength(1) - 1)
            {
                //Check if valid Up on the Left and Right side are valid
                if (!map[x, y].BottomWall)
                {
                    //Check to place on right side
                    if (!map[x, y].RightWall)
                    {
                        //Check to see if it will connect to another wall ignore if so
                        if (!map[x + 1, y + 1].TopWall && !map[x + 1, y + 1].LeftWall)
                        {
                            boardPos.Add(new Vector2(x, y));
                            wallDir.Add("R");
                            //valid 1
                        }
                    }
                    //Check to place to left side
                    if (!map[x, y].LeftWall)
                    {
                        //Check to see if it will connect to another wall ignore if so
                        if (!map[x - 1, y + 1].TopWall && !map[x - 1, y + 1].RightWall)
                        {
                            boardPos.Add(new Vector2(x, y));
                            wallDir.Add("L");
                            //valid 2
                        }
                    }
                }
                //Check if right wall conflict
                if (x < map.GetLength(0) - 2)
                {
                    //Check if valid Left free
                    if (!map[x + 1, y].TopWall)
                    {
                        //Check to see if it will connect to another wall ignore if so
                        if (!map[x + 2, y].TopWall && !map[x + 2, y].LeftWall && !map[x + 2, y - 1].BottomWall && !map[x + 2, y - 1].LeftWall)
                        {
                            boardPos.Add(new Vector2(x + 1, y));
                            wallDir.Add("U");
                            //Valid 3
                        }
                    }
                }
                //Check if left wall conflict
                if (x > 1)
                {
                    //Check if valid Right free
                    if (!map[x - 1, y].TopWall)
                    {
                        //Check to see if it will connect to another wall ignore if so
                        if (!map[x - 2, y].TopWall && !map[x - 2, y].RightWall && !map[x - 2, y - 1].BottomWall && !map[x - 2, y - 1].RightWall)
                        {
                            boardPos.Add(new Vector2(x - 1, y));
                            wallDir.Add("U");
                            //Valid 3
                        }
                    }
                }
            }

            if (map[x, y].RightWall && x > 0 && y > 0 && y < map.GetLength(1) - 1)
            {
                if (!map[x, y].LeftWall)
                {
                    if (!map[x, y].TopWall)
                    {
                        if (!map[x, y - 1].LeftWall)
                        {
                            if (!map[x - 1, y - 1].BottomWall)
                            {
                                boardPos.Add(new Vector2(x, y));
                                wallDir.Add("U");
                            }
                        }
                    }
                    if (!map[x, y].BottomWall && y < map.GetLength(1) - 1)
                    {
                        if (!map[x, y + 1].LeftWall)
                        {
                            if (!map[x - 1, y + 1].TopWall)
                            {
                                boardPos.Add(new Vector2(x, y));
                                wallDir.Add("D");
                            }
                        }
                    }
                }
                if (!map[x, y - 1].RightWall && y < map.GetLength(1) - 2)
                {
                    if (!map[x + 1, y - 1].TopWall)
                    {
                        if (!map[x + 1, y - 2].LeftWall)
                        {
                            if (!map[x, y - 2].BottomWall)
                            {
                                boardPos.Add(new Vector2(x, y - 1));
                                wallDir.Add("R");
                            }
                        }
                    }
                }
                if (!map[x, y + 1].RightWall && y < map.GetLength(1) - 2)
                {
                    if (!map[x + 1, y + 1].BottomWall)
                    {
                        if (!map[x + 1, y + 2].LeftWall)
                        {
                            if (!map[x, y + 2].TopWall)
                            {
                                boardPos.Add(new Vector2(x, y + 1));
                                wallDir.Add("R");
                            }
                        }
                    }
                }
            }

            if (map[x, y].LeftWall && x > 0 && y > 0 && y < map.GetLength(1) - 1)
            {
                if (!map[x, y].RightWall)
                {
                    if (!map[x, y].TopWall)
                    {
                        if (!map[x, y - 1].RightWall)
                        {
                            if (!map[x + 1, y - 1].BottomWall)
                            {
                                boardPos.Add(new Vector2(x, y));
                                wallDir.Add("U");
                            }
                        }
                    }
                    if (!map[x, y].BottomWall && y < map.GetLength(1) - 1)
                    {
                        if (!map[x, y + 1].RightWall)
                        {
                            if (!map[x + 1, y + 1].TopWall)
                            {
                                boardPos.Add(new Vector2(x, y));
                                wallDir.Add("D");
                            }
                        }
                    }
                }
                if (!map[x, y - 1].LeftWall && y < map.GetLength(1) - 2)
                {
                    if (!map[x - 1, y - 1].TopWall)
                    {
                        if (!map[x - 1, y - 2].RightWall)
                        {
                            if (!map[x, y - 2].BottomWall)
                            {
                                boardPos.Add(new Vector2(x, y - 1));
                                wallDir.Add("L");
                            }
                        }
                    }
                }
                if (!map[x, y + 1].LeftWall && y < map.GetLength(1) - 2)
                {
                    if (!map[x - 1, y + 1].BottomWall)
                    {
                        if (!map[x - 1, y + 2].RightWall)
                        {
                            if (!map[x, y + 2].TopWall)
                            {
                                boardPos.Add(new Vector2(x, y + 1));
                                wallDir.Add("L");
                            }
                        }
                    }
                }
            }
        }

        //Place Wall Randomly selcted from valid placements
        if (boardPos.Count > 0)
        {
            found = true;
            int Rand = Random.Range(0, boardPos.Count);
            int Rand2 = 1;
            if (map[x, y].OuterShell || map[x, y].ConnectedToOther)
                Rand2 = Random.Range(0, 100);
            else
                Rand2 = Random.Range(0, 10);

            if (wallDir[Rand] == "R" && map[x, y].getWallRightChance() && Rand2 == 1)
            {
                map[(int)boardPos[Rand].x, (int)boardPos[Rand].y].RightWall = true;
                map[(int)boardPos[Rand].x + 1, (int)boardPos[Rand].y].LeftWall = true;
            }
            if (wallDir[Rand] == "L" && map[x, y].getWallLeftChance() && Rand2 == 1)
            {
                map[(int)boardPos[Rand].x - 1, (int)boardPos[Rand].y].RightWall = true;
                map[(int)boardPos[Rand].x, (int)boardPos[Rand].y].LeftWall = true;
            }
            if (wallDir[Rand] == "U" && map[x, y].getWallTopChance() && Rand2 == 1)
            {
                map[(int)boardPos[Rand].x, (int)boardPos[Rand].y].TopWall = true;
                map[(int)boardPos[Rand].x, (int)boardPos[Rand].y - 1].BottomWall = true;
            }
            if (wallDir[Rand] == "D" && map[x, y].getWallBotChance() && Rand2 == 1)
            {
                map[(int)boardPos[Rand].x, (int)boardPos[Rand].y + 1].TopWall = true;
                map[(int)boardPos[Rand].x, (int)boardPos[Rand].y].BottomWall = true;
            }
        }
    }

    public void addFloatingWall(BoardCollection[,] map, int max)
    {
        noMore = false;
        int x = Random.Range(0, map.GetLength(0));
        int y = Random.Range(0, map.GetLength(1));

        if (map[x, y].getWallCount() == 0)
        {
            Rand = Random.Range(0, 4);
            if (Rand == 0 && RandomWallCount <= max && !map[x + 1, y].OuterShell && !map[x + 1, y].ConnectedToOther)
            {
                map[x, y].RightWall = true;
                map[x + 1, y].LeftWall = true;
                //map[x, y].updateBoardTexture();
                //map[x + 1, y].updateBoardTexture();
                RandomWallCount++;
            }
            if (Rand == 1 && RandomWallCount <= max && !map[x - 1, y].OuterShell && !map[x - 1, y].ConnectedToOther)
            {
                map[x - 1, y].RightWall = true;
                map[x, y].LeftWall = true;
                //map[x - 1, y].updateBoardTexture();
                //map[x, y].updateBoardTexture();
                RandomWallCount++;
            }
            if (Rand == 2 && RandomWallCount <= max && !map[x, y - 1].OuterShell && !map[x, y - 1].ConnectedToOther)
            {
                map[x, y].TopWall = true;
                map[x, y - 1].BottomWall = true;
                //map[x, y].updateBoardTexture();
                //map[x, y - 1].updateBoardTexture();
                RandomWallCount++;
            }
            if (Rand == 3 && RandomWallCount <= max && !map[x, y + 1].OuterShell && !map[x, y + 1].ConnectedToOther)
            {
                map[x, y + 1].TopWall = true;
                map[x, y].BottomWall = true;
                //map[x, y + 1].updateBoardTexture();
                //map[x, y].updateBoardTexture();
                RandomWallCount++;
            }

            if (RandomWallCount >= max)
                noMore = true;
        }
    }

    public void wallOffBiomes(BoardCollection[,] map)
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (x == map.GetLength(0) - 1)
                {
                    map[x, y].RightWall = true;
                    map[x, y].OuterShell = true;
                }
                if (x == 0)
                {
                    map[x, y].LeftWall = true;
                    map[x, y].OuterShell = true;
                }
                if (y == map.GetLength(1) - 1)
                {
                    map[x, y].BottomWall = true;
                    map[x, y].OuterShell = true;
                }
                if (y == 0)
                {
                    map[x, y].TopWall = true;
                    map[x, y].OuterShell = true;
                }
                if (x > 0)
                    if (map[x - 1, y].BiomeID != map[x, y].BiomeID)
                    {
                        map[x, y].LeftWall = true;
                        map[x, y].ConnectedToOther = true;
                    }
                if (y > 0)
                    if (map[x, y - 1].BiomeID != map[x, y].BiomeID)
                    {
                        map[x, y].TopWall = true;
                        map[x, y].ConnectedToOther = true;
                    }
                if (x < map.GetLength(0) - 1)
                    if (map[x + 1, y].BiomeID != map[x, y].BiomeID)
                    {
                        map[x, y].RightWall = true;
                        map[x, y].ConnectedToOther = true;
                    }
                if (y < map.GetLength(1) - 1)
                    if (map[x, y + 1].BiomeID != map[x, y].BiomeID)
                    {
                        map[x, y].BottomWall = true;
                        map[x, y].ConnectedToOther = true;
                    }
            }
        }
    }
}