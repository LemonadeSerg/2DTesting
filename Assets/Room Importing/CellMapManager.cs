using System.Collections.Generic;
using UnityEngine;

public class CellMapManager : MonoBehaviour
{
    public CellSaver cellSaver;
    public CellLoader cellLoader;

    public CellManager cellManager;
    public CellData cellData;

    // Start is called before the first frame update
    private void Start()
    {
        cellSaver = this.GetComponent<CellSaver>();
        cellLoader = this.GetComponent<CellLoader>();

        cellLoader.LoadCells();

        ConvertCollectionToGameObject(cellLoader.cellCollection[0]);
    }

    private void ConvertCollectionToGameObject(CellData cellData)
    {
        GameObject go = new GameObject(cellData.roomName);
        CellManager cm = go.AddComponent<CellManager>();
        BoxCollider2D backCollider = new BoxCollider2D();
        List<CellDoor> doors = new List<CellDoor>();
        List<GameObject> layers = new List<GameObject>();
        for (int i = 0; i < cellData.textures.Length; i++)
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
                layerObject.tag = "BackWall";
                layerObject.AddComponent<Rigidbody2D>().gravityScale = 0;
                backCollider = layerObject.AddComponent<BoxCollider2D>();
                backCollider.isTrigger = true;
            }
            if (cellData.layer[i] == 1)
                spriteRenderer.sortingLayerName = "Midground";
            if (cellData.layer[i] == 2)
                spriteRenderer.sortingLayerName = "Foreground";
            //Add Collision
            if (cellData.collision[i])
            {
                PolygonCollider2D pc2d = layerObject.AddComponent<PolygonCollider2D>();
                if (cellData.points != null)
                {
                    pc2d.points = cellData.points;
                }
            }

            layers.Add(layerObject);
        }

        for (int i = 0; i < cellData.EntranceDir.Length; i++)
        {
            GameObject cellDoor = new GameObject("Cell Door " + cellData.EntranceDir[i]);
            CellDoor cd = cellDoor.AddComponent<CellDoor>();
            cellDoor.transform.parent = go.transform;
            cellDoor.transform.localPosition = cellData.EntrancePos[i];
            cellDoor.AddComponent<BoxCollider2D>();
            cd.Dir = cellData.EntranceDir[i];
            cd.backWall = backCollider;
            doors.Add(cd);
        }

        //Add and fill CellManager

        cm.cellData = cellData;
        cm.Layers = layers.ToArray();
        cm.CellDoors = doors.ToArray();
    }
}