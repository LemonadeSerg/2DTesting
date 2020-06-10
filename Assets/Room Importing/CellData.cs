using System;
using UnityEngine;

[Serializable]
public class CellData
{
    public int ID = 0;
    public string roomName = "Example";
    public string subFolder = "Example";

    public string[] images;
    public int[] layer;
    public bool[] collision;

    public Vector2[] EntrancePos;
    public string[] EntranceDir;

    //Json File name for EntityData
    public string[] Entities;

    //Loaded at run time
    public Texture2D[] textures;

    public Vector2[] points;

    public override string ToString()
    {
        return roomName;
    }
}