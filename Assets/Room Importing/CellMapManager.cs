using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellMapManager : MonoBehaviour
{

    public CellSaver cellSaver;
    public CellLoader cellLoader;

    public CellManager cellManager;
    public CellData cellData;
    // Start is called before the first frame update
    void Start()
    {
        cellSaver = this.GetComponent<CellSaver>();
        cellLoader = this.GetComponent<CellLoader>();

        cellLoader.loadCells();

        convertCollectionToGameObject(cellLoader.cellCollection[0]);
    }

    public CellData convertGameObjectToCollection(CellManager cellManager)
    {
        CellData cd = cellLoader.cellCollection[cellManager.ID];

        cd.roomName = cellManager.roomName;
        cd.subFolder = cellManager.subFolder;

        cd.EntranceDir = new string[cellManager.CellDoors.Length];
        cd.EntrancePos = new Vector2[cellManager.CellDoors.Length];
        for(int i = 0; i < cellManager.CellDoors.Length;i++)
        {
            cd.EntranceDir[i] = cellManager.CellDoors[i].Dir;
            cd.EntrancePos[i] = cellManager.CellDoors[i].transform.localPosition;
        }

        return cd;
    }

    void convertCollectionToGameObject(CellData cellData)
    {
        GameObject go = new GameObject(cellData.roomName);
        CellManager cm = go.AddComponent<CellManager>();
        BoxCollider2D backCollider = new BoxCollider2D();
        List<CellDoor> doors = new List<CellDoor>();

        for(int i = 0; i < cellData.textures.Length; i++)
        {
            //Create Layer and center it with parent
            GameObject layerObject = new GameObject("Layer:" + i.ToString());
            layerObject.transform.parent = go.transform;
            layerObject.transform.position = Vector2.zero;

            //Add spriteRenderer Add sprite from texture
            SpriteRenderer spriteRenderer = layerObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Sprite.Create(cellData.textures[i], new Rect(0, 0, cellData.textures[i].width, cellData.textures[i].height), new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.Tight);

            //Sort DrawLayer
            if (cellData.layer[i] == 0)
            {
                spriteRenderer.sortingLayerName = "Background";
                layerObject.AddComponent<BoxCollider2D>();
                layerObject.tag = "BackWall";
                backCollider = layerObject.AddComponent<BoxCollider2D>();
                backCollider.isTrigger = true;
            }
            if (cellData.layer[i] == 1)
                spriteRenderer.sortingLayerName = "Midground";
            if (cellData.layer[i] == 2)
                spriteRenderer.sortingLayerName = "Foreground";
            //Add Collision
            if (cellData.collision[i])
                layerObject.AddComponent<PolygonCollider2D>();
        }

        for(int i = 0; i < cellData.EntranceDir.Length; i++)
        {
            GameObject cellDoor = new GameObject("Cell Door " + cellData.EntranceDir[i]);
            cellDoor.transform.parent = go.transform;
            cellDoor.transform.localPosition = cellData.EntrancePos[i];
            CellDoor cd = cellDoor.AddComponent<CellDoor>();
            cd.Dir = cellData.EntranceDir[i];
            cd.backWall = backCollider;
            doors.Add(cd);
        }


        //Add and fill CellManager

        cm.roomName = cellData.roomName;
        cm.subFolder = cellData.subFolder;

        cm.CellDoors = doors.ToArray();
    }
}
