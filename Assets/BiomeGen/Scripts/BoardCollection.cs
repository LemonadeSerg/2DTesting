using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoardCollection
{
    private int biomeID;

    public int BiomeID
    {
        get { return biomeID; }
        set
        {
            updateBoardTexture();
            biomeID = value;
        }
    }

    private Color color;

    public Color Col
    {
        get { return color; }
        set
        {
            color = value;
            updateBoardTexture();
        }
    }

    private bool outerShell;

    public bool OuterShell
    {
        get { return outerShell; }
        set
        {
            outerShell = value;
            updateBoardTexture();
        }
    }

    private bool connectedToOther;

    public bool ConnectedToOther
    {
        get { return connectedToOther; }
        set
        {
            connectedToOther = value;
            updateBoardTexture();
        }
    }

    private bool doorWay;

    public bool DoorWay
    {
        get { return doorWay; }
        set
        {
            doorWay = value;
            updateBoardTexture();
        }
    }

    private bool topWall;

    public bool TopWall
    {
        get { return topWall; }
        set
        {
            topWall = value;
            updateBoardTexture();
        }
    }

    private bool bottomWall;

    public bool BottomWall
    {
        get { return bottomWall; }

        set
        {
            bottomWall = value;
            updateBoardTexture();
        }
    }

    private bool rightWall;

    public bool RightWall
    {
        get { return rightWall; }
        set
        {
            rightWall = value;
            updateBoardTexture();
        }
    }

    private bool leftWall;

    public bool LeftWall
    {
        get { return leftWall; }
        set
        {
            leftWall = value;
            updateBoardTexture();
        }
    }

    public RoomType RType
    {
        get { return rType; }
        set
        {
            rType = value;
            updateBoardTexture();
        }
    }

    private RoomType rType;

    public OrientationType OrType
    {
        get { return orType; }
        set
        {
            orType = value;
            updateBoardTexture();
        }
    }

    private OrientationType orType;

    public Texture2D tex;

    public enum RoomType
    {
        Normal,
        Boss,
        Spawn,
        Battle,
        Village,
        Puzzle,
    }

    public enum OrientationType
    {
        Clear,
        ClearT,
        ClearR,
        ClearB,
        ClearL,
        ClearTR,
        ClearTL,
        ClearTB,
        ClearRB,
        ClearRL,
        ClearBL,
        ClearTRB,
        ClearTRL,
        ClearRBL,
        ClearBLT,
    }

    public void Init(int width, int height)
    {
        tex = new Texture2D(width, height);
        biomeID = 0;
        color = Color.white;
        outerShell = false;
        connectedToOther = false;
    }

    public void updateBoardTexture()
    {
        if (tex != null)
        {
            for (int x = 0; x < tex.width; x++)
            {
                for (int y = 0; y < tex.height; y++)
                {
                    tex.SetPixel(x, y, color);
                }
            }
            if (bottomWall)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    tex.SetPixel(x, 0, Color.white);
                }
            }
            if (topWall)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    tex.SetPixel(x, tex.height - 1, Color.white);
                }
            }
            if (leftWall)
            {
                for (int y = 0; y < tex.height; y++)
                {
                    tex.SetPixel(0, y, Color.white);
                }
            }
            if (rightWall)
            {
                for (int y = 0; y < tex.height; y++)
                {
                    tex.SetPixel(tex.width - 1, y, Color.white);
                }
            }
            if (rType == RoomType.Boss)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    for (int y = 0; y < tex.height; y++)
                    {
                        if (x == y)
                        {
                            tex.SetPixel(x, y, Color.red);
                            tex.SetPixel(x, tex.height - y, Color.red);
                        }
                    }
                }
            }
            if (rType == RoomType.Village)
            {
                for (int x = 8; x < tex.width - 8; x++)
                {
                    for (int y = 8; y < tex.height - 8; y++)
                    {
                        if (x == y && y < tex.height / 2)
                        {
                            tex.SetPixel(x, y, Color.yellow);
                            tex.SetPixel(x, tex.height - y, Color.yellow);
                        }
                    }
                }
            }
            if (rType == RoomType.Normal)
            {
                for (int x = 8; x < tex.width - 8; x++)
                {
                    for (int y = 8; y < tex.height - 8; y++)
                    {
                        if (x == y || x == 8 || x == tex.width - 9)
                        {
                            tex.SetPixel(x, tex.height - y, Color.black);
                        }
                    }
                }
            }

            tex.Apply();
        }
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