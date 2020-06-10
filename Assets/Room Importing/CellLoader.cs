using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class CellLoader : MonoBehaviour
{
    private string CellsPath;

    public List<CellData> cellCollection;

    private void Start()
    {
        cellCollection = new List<CellData>();
        CellsPath = Application.dataPath + "/Rooms/";
        if (!Directory.Exists(CellsPath))
        {
            Directory.CreateDirectory(CellsPath);
        }
    }

    public void LoadCells()
    {
        FindRooms();

        for (int i = 0; i < cellCollection.Count; i++)
        {
            LoadTexture(cellCollection[i]);
            cellCollection[i].ID = i;
        }
    }

    private void FindRooms()
    {
        foreach (string file in System.IO.Directory.GetFiles(CellsPath))
        {
            if (file.EndsWith(".json") && !file.Contains("Example"))
            {
                LoadJson(file);
            }
        }
    }

    private void LoadJson(string fileName)
    {
        using (System.IO.StreamReader stream = new System.IO.StreamReader(fileName))
        {
            string json = stream.ReadToEnd();
            cellCollection.Add(JsonUtility.FromJson<CellData>(json));
        }
    }

    private void LoadTexture(CellData rc)
    {
        rc.textures = new Texture2D[rc.images.Length];
        for (int i = 0; i < rc.images.Length; i++)
        {
            rc.textures[i] = LoadPNG(CellsPath + rc.subFolder + "/" + rc.images[i], rc.images[i]);
        }
    }

    public static Texture2D LoadPNG(string filePath, string spriteName)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        tex.name = spriteName;
        return tex;
    }
}