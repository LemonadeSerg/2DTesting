using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CellSaver : MonoBehaviour
{
    string CellsPath;

    void Start()
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
    }
}
