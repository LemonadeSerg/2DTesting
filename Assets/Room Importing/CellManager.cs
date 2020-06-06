using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellManager : MonoBehaviour
{

    public bool save = false;
    public int ID;
    public string roomName;
    public string subFolder;

    public string[] images;

    public int[] layer;

    public bool[] collision;

    public Texture2D[] textures;

    public Vector2[] EntrancePos;

    public string[] EntranceDir;

    public CellDoor[] CellDoors;

    public CellMapManager cmm;

    private void Start()
    {
        cmm = FindObjectOfType<CellMapManager>();
    }

    private void Update()
    {
        if (save)
        {
            cmm.cellSaver.saveRoom(cmm.convertGameObjectToCollection(this));
            save = false;
        }
    }
}
