using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class CellLoader : MonoBehaviour
{
    string CellsPath;

    public List<CellData> cellCollection;

    void Start()
    {
        cellCollection = new List<CellData>();
        CellsPath = Application.dataPath + "/Rooms/";
        if (!Directory.Exists(CellsPath))
        {
            Directory.CreateDirectory(CellsPath);
        }
    }

    public void loadCells()
    {
        findRooms();

        for(int i = 0; i < cellCollection.Count; i++)
        {
            loadTexture(cellCollection[i]);
            cellCollection[i].ID = i;
        }
    }
    void findRooms()
    {
        foreach (string file in System.IO.Directory.GetFiles(CellsPath))
        {
            if (file.EndsWith(".json") && !file.Contains("Example"))
            {
                loadJson(file);
            }
        }
    }

    void loadJson(string fileName)
    {
        using (System.IO.StreamReader stream = new System.IO.StreamReader(fileName))
        {
            string json = stream.ReadToEnd();
            cellCollection.Add(JsonUtility.FromJson<CellData>(json));
        }
    }

    void loadTexture(CellData rc)
    {
        rc.textures = new Texture2D[rc.images.Length];
        for(int i = 0; i < rc.images.Length;i++)
        {
            rc.textures[i] = LoadPNG(CellsPath + rc.subFolder + "/" + rc.images[i]);
        }
    }

    public static Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(4, 4);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }
}
