using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
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

    public int heatGrowthChange()
    {
        int Rand = UnityEngine.Random.Range(1, 4);
        if (Rand == 1)
            return 1;
        if (Rand == 2)
            return -1;
        if (Rand == 3)
            return -2;
        return Rand;
    }

    public bool getWallRightChance()
    {
        if (biomeID == 1)
        {
            if (Random.Range(0, 10) == 1)
                return true;
        }
        else if (Random.Range(0, 2) == 1)
            return true;
        else
            return false;

        return false;
    }

    public bool getWallLeftChance()
    {
        if (biomeID == 1)
        {
            if (Random.Range(0, 10) == 1)
                return true;
        }
        else if (Random.Range(0, 2) == 1)
            return true;
        else
            return false;

        return false;
    }

    public bool getWallTopChance()
    {
        if (biomeID == 2)
        {
            if (Random.Range(0, 10) == 1)
                return true;
        }
        else if (Random.Range(0, 2) == 1)
            return true;
        else
            return false;

        return false;
    }

    public bool getWallBotChance()
    {
        if (biomeID == 2)
        {
            if (Random.Range(0, 10) == 1)
                return true;
        }
        else if (Random.Range(0, 2) == 1)
            return true;
        else
            return false;

        return false;
    }
}