using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
    public class BoardCollection
    {
    public int biomeID;
    public Color color;
    public bool outerShell;
    public bool connectedToOther;
    public bool doorWay;
    public void Init()
    {
        biomeID = 0;
        color = Color.white;
        outerShell = false;
        connectedToOther = false;
    }
}
