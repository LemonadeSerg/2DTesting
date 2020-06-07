using System.IO;
using UnityEngine;

public class CellSaver : MonoBehaviour
{
    private string CellsPath;

    private void Start()
    {
        CellsPath = Application.dataPath + "/Rooms/";
    }

    public void saveRoom(CellData roomTemp)
    {
        if (!Directory.Exists(CellsPath))
            Directory.CreateDirectory(CellsPath);
        if (!Directory.Exists(CellsPath + roomTemp.subFolder))
            Directory.CreateDirectory(CellsPath + roomTemp.subFolder);

        using (StreamWriter stream = new StreamWriter(CellsPath + roomTemp.roomName + ".json"))
        {
            string json = JsonUtility.ToJson(roomTemp);
            stream.Write(json);
        }

        for (int i = 0; i < roomTemp.layer.Length; i++)
        {
            saveTexture2D(roomTemp.textures[i], CellsPath + roomTemp.subFolder + "/" + roomTemp.images[i]);
        }
    }

    public void saveTexture2D(Texture2D texture,string filePath)
    {
        var pixels = texture.GetPixels32();
        Texture2D tex = new Texture2D(texture.width,texture.height,TextureFormat.ARGB32,false);
        tex.SetPixels32(pixels);
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);
    }
}