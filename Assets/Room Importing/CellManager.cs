using UnityEngine;

public class CellManager : MonoBehaviour
{
    public bool save = false;

    public GameObject[] Layers;
    public CellDoor[] CellDoors;
    private CellMapManager cmm;
    public CellData cellData;

    private void Start()
    {
        cmm = FindObjectOfType<CellMapManager>();
    }

    private void Update()
    {
        if (save)
        {
            save = false;
            cmm.cellSaver.saveRoom(cellData);
        }

        updateCollectionLayers();
        updateCollectionDoors();
    }


    void updateCollectionDoors()
    {
        cellData.EntranceDir = new string[CellDoors.Length];
        cellData.EntrancePos = new Vector2[CellDoors.Length];

        for (int i = 0; i < CellDoors.Length; i++)
        {
            cellData.EntranceDir[i] = CellDoors[i].Dir;
            cellData.EntrancePos[i] = CellDoors[i].transform.localPosition;
        }

    }
    void updateCollectionLayers()
    {
        string[] img = new string[Layers.Length];
        bool[] col = new bool[Layers.Length];
        int[] lay = new int[Layers.Length];
        Texture2D[] tex = new Texture2D[Layers.Length];
        for (int i = 0; i < Layers.Length; i++)
        {
            if (Layers[i].GetComponent<SpriteRenderer>().sprite.texture.name.Contains(".png"))
                img[i] = Layers[i].GetComponent<SpriteRenderer>().sprite.texture.name;
            else
                img[i] = Layers[i].GetComponent<SpriteRenderer>().sprite.texture.name + ".png";

            tex[i] = Layers[i].GetComponent<SpriteRenderer>().sprite.texture;
            if (i < cellData.images.Length)
            {
                col[i] = cellData.collision[i];
                lay[i] = cellData.layer[i];
            }
            else
            {
                col[i] = false;
                lay[i] = 1;
            }

            if (col[i] == true && Layers[i].GetComponent<PolygonCollider2D>() != null)
                cellData.points = Layers[i].GetComponent<PolygonCollider2D>().points;
        }
        cellData.images = img;
        cellData.collision = col;
        cellData.layer = lay;
        cellData.textures = tex;
    }
}