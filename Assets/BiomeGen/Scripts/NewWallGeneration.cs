using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewWallGeneration
{
    public int RandomWallCount;
    private int Rand;

    public bool noMore;

    public void buildWall(BoardCollection[,] map, int wallBreakPoints)
    {
        //int x = Random.Range(0, map.GetLength(0));
        //int y = Random.Range(0, map.GetLength(1));
        //buildWall(x, y, map, wallBreakPoints);
        for (int x2 = 0; x2 < map.GetLength(0); x2++)
        {
            for (int y2 = 0; y2 < map.GetLength(0); y2++)
            {
                buildWall(x2, y2, map, wallBreakPoints);
            }
        }
    }

    public void buildWall(int x, int y, BoardCollection[,] map, int wallBreakPoints)
    {
        //Chance to try and build wall
        if (Random.Range(0, 40) == 1)
        {
            List<Vector2> boardPos = new List<Vector2>();
            List<string> wallDir = new List<string>();
            if (map[x, y].getWallCount() > 0)
            {
                //Check start Connectiions to Bottom Wall
                if (map[x, y].bottomWall && y > 0)
                {
                    //Check if valid Up on the Left and Right side are valid
                    if (!map[x, y].topWall && y > 0)
                    {
                        //Check to place on right side
                        if (!map[x, y].rightWall)
                        {
                            //Check to see if it will connect to another wall ignore if so
                            if (!map[x + 1, y - 1].bottomWall && !map[x + 1, y - 1].leftWall)
                            {
                                boardPos.Add(new Vector2(x, y));
                                wallDir.Add("R");
                                //valid 1
                            }
                        }
                        //Check to place to left side
                        if (!map[x, y].leftWall)
                        {
                            //Check to see if it will connect to another wall ignore if so
                            if (!map[x - 1, y - 1].bottomWall && !map[x - 1, y - 1].rightWall)
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
                        if (!map[x + 1, y].bottomWall)
                        {
                            //Check to see if it will connect to another wall ignore if so
                            if (!map[x + 2, y].bottomWall && !map[x + 2, y].leftWall && !map[x + 2, y + 1].topWall && !map[x + 2, y + 1].leftWall)
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
                        if (!map[x - 1, y].bottomWall)
                        {
                            //Check to see if it will connect to another wall ignore if so
                            if (!map[x - 2, y].bottomWall && !map[x - 2, y].rightWall && !map[x - 2, y + 1].topWall && !map[x - 2, y + 1].rightWall)
                            {
                                boardPos.Add(new Vector2(x - 1, y));
                                wallDir.Add("D");
                                //Valid 3
                            }
                        }
                    }
                }

                //Check Valid start to Top Wall
                if (map[x, y].topWall && y < map.GetLength(1) - 1)
                {
                    //Check if valid Up on the Left and Right side are valid
                    if (!map[x, y].bottomWall)
                    {
                        //Check to place on right side
                        if (!map[x, y].rightWall)
                        {
                            //Check to see if it will connect to another wall ignore if so
                            if (!map[x + 1, y + 1].topWall && !map[x + 1, y + 1].leftWall)
                            {
                                boardPos.Add(new Vector2(x, y));
                                wallDir.Add("R");
                                //valid 1
                            }
                        }
                        //Check to place to left side
                        if (!map[x, y].leftWall)
                        {
                            //Check to see if it will connect to another wall ignore if so
                            if (!map[x - 1, y + 1].topWall && !map[x - 1, y + 1].rightWall)
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
                        if (!map[x + 1, y].topWall)
                        {
                            //Check to see if it will connect to another wall ignore if so
                            if (!map[x + 2, y].topWall && !map[x + 2, y].leftWall && !map[x + 2, y - 1].bottomWall && !map[x + 2, y - 1].leftWall)
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
                        if (!map[x - 1, y].topWall)
                        {
                            //Check to see if it will connect to another wall ignore if so
                            if (!map[x - 2, y].topWall && !map[x - 2, y].rightWall && !map[x - 2, y - 1].bottomWall && !map[x - 2, y - 1].rightWall)
                            {
                                boardPos.Add(new Vector2(x - 1, y));
                                wallDir.Add("U");
                                //Valid 3
                            }
                        }
                    }
                }

                if (map[x, y].rightWall && x > 0 && y > 0 && y < map.GetLength(1) - 1)
                {
                    if (!map[x, y].leftWall)
                    {
                        if (!map[x, y].topWall)
                        {
                            if (!map[x, y - 1].leftWall)
                            {
                                if (!map[x - 1, y - 1].bottomWall)
                                {
                                    boardPos.Add(new Vector2(x, y));
                                    wallDir.Add("U");
                                }
                            }
                        }
                        if (!map[x, y].bottomWall && y < map.GetLength(1) - 1)
                        {
                            if (!map[x, y + 1].leftWall)
                            {
                                if (!map[x - 1, y + 1].topWall)
                                {
                                    boardPos.Add(new Vector2(x, y));
                                    wallDir.Add("D");
                                }
                            }
                        }
                    }
                    if (!map[x, y - 1].rightWall && y < map.GetLength(1) - 2)
                    {
                        if (!map[x + 1, y - 1].topWall)
                        {
                            if (!map[x + 1, y - 2].leftWall)
                            {
                                if (!map[x, y - 2].bottomWall)
                                {
                                    boardPos.Add(new Vector2(x, y - 1));
                                    wallDir.Add("R");
                                }
                            }
                        }
                    }
                    if (!map[x, y + 1].rightWall && y < map.GetLength(1) - 2)
                    {
                        if (!map[x + 1, y + 1].bottomWall)
                        {
                            if (!map[x + 1, y + 2].leftWall)
                            {
                                if (!map[x, y + 2].topWall)
                                {
                                    boardPos.Add(new Vector2(x, y + 1));
                                    wallDir.Add("R");
                                }
                            }
                        }
                    }
                }

                if (map[x, y].leftWall && x > 0 && y > 0 && y < map.GetLength(1) - 1)
                {
                    if (!map[x, y].rightWall)
                    {
                        if (!map[x, y].topWall)
                        {
                            if (!map[x, y - 1].rightWall)
                            {
                                if (!map[x + 1, y - 1].bottomWall)
                                {
                                    boardPos.Add(new Vector2(x, y));
                                    wallDir.Add("U");
                                }
                            }
                        }
                        if (!map[x, y].bottomWall && y < map.GetLength(1) - 1)
                        {
                            if (!map[x, y + 1].rightWall)
                            {
                                if (!map[x + 1, y + 1].topWall)
                                {
                                    boardPos.Add(new Vector2(x, y));
                                    wallDir.Add("D");
                                }
                            }
                        }
                    }
                    if (!map[x, y - 1].leftWall && y < map.GetLength(1) - 2)
                    {
                        if (!map[x - 1, y - 1].topWall)
                        {
                            if (!map[x - 1, y - 2].rightWall)
                            {
                                if (!map[x, y - 2].bottomWall)
                                {
                                    boardPos.Add(new Vector2(x, y - 1));
                                    wallDir.Add("L");
                                }
                            }
                        }
                    }
                    if (!map[x, y + 1].leftWall && y < map.GetLength(1) - 2)
                    {
                        if (!map[x - 1, y + 1].bottomWall)
                        {
                            if (!map[x - 1, y + 2].rightWall)
                            {
                                if (!map[x, y + 2].topWall)
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
                int Rand = Random.Range(0, boardPos.Count);
                int Rand2 = 1;
                if (map[x, y].outerShell || map[x, y].connectedToOther && wallBreakPoints <= 0)
                    Rand2 = Random.Range(0, 10);
                else
                    wallBreakPoints--;
                if (wallDir[Rand] == "R" && map[x, y].getWallRightChance() && Rand2 == 1)
                {
                    map[(int)boardPos[Rand].x, (int)boardPos[Rand].y].rightWall = true;
                    map[(int)boardPos[Rand].x + 1, (int)boardPos[Rand].y].leftWall = true;
                }
                if (wallDir[Rand] == "L" && map[x, y].getWallLeftChance() && Rand2 == 1)
                {
                    map[(int)boardPos[Rand].x - 1, (int)boardPos[Rand].y].rightWall = true;
                    map[(int)boardPos[Rand].x, (int)boardPos[Rand].y].leftWall = true;
                }
                if (wallDir[Rand] == "U" && map[x, y].getWallTopChance() && Rand2 == 1)
                {
                    map[(int)boardPos[Rand].x, (int)boardPos[Rand].y].topWall = true;
                    map[(int)boardPos[Rand].x, (int)boardPos[Rand].y - 1].bottomWall = true;
                }
                if (wallDir[Rand] == "D" && map[x, y].getWallBotChance() && Rand2 == 1)
                {
                    map[(int)boardPos[Rand].x, (int)boardPos[Rand].y + 1].topWall = true;
                    map[(int)boardPos[Rand].x, (int)boardPos[Rand].y].bottomWall = true;
                }
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
            if (Rand == 0 && RandomWallCount <= max && !map[x + 1, y].outerShell && !map[x + 1, y].connectedToOther)
            {
                map[x, y].rightWall = true;
                map[x + 1, y].leftWall = true;
                RandomWallCount++;
            }
            if (Rand == 1 && RandomWallCount <= max && !map[x - 1, y].outerShell && !map[x - 1, y].connectedToOther)
            {
                map[x - 1, y].rightWall = true;
                map[x, y].leftWall = true;
                RandomWallCount++;
            }
            if (Rand == 2 && RandomWallCount <= max && !map[x, y - 1].outerShell && !map[x, y - 1].connectedToOther)
            {
                map[x, y].topWall = true;
                map[x, y - 1].bottomWall = true;
                RandomWallCount++;
            }
            if (Rand == 3 && RandomWallCount <= max && !map[x, y + 1].outerShell && !map[x, y + 1].connectedToOther)
            {
                map[x, y + 1].topWall = true;
                map[x, y].bottomWall = true;
                RandomWallCount++;
            }

            if (RandomWallCount >= max)
                noMore = true;
        }
    }
}