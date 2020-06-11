using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BoardCollection
{
    public int biomeID;
    public Color color;
    public bool outerShell;
    public bool connectedToOther;
    public bool doorWay;
    public bool topWall;
    public bool bottomWall;
    public bool rightWall;
    public bool leftWall;
    public int weight;

    public void Init()
    {
        biomeID = 0;
        color = Color.white;
        outerShell = false;
        connectedToOther = false;
    }

    public int getWallCount()
    {
        int count = 0;
        if (topWall)
            count++;
        if (bottomWall)
            count++;
        if (rightWall)
            count++;
        if (leftWall)
            count++;
        return count;
    }
}