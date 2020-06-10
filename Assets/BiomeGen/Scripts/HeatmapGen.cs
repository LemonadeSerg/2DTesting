using System.Collections.Generic;
using UnityEngine;

public class HeatmapGen : MonoBehaviour
{
    public Vector2Int mapSize = new Vector2Int(100, 100);
    public bool doUpdate = false;
    public float genSpeed;
    public bool darkenBiomeWall = false;
    public bool darkenOuterWall = false;

    private float lastGen;
    public BoardCollection[,] map;

    private int initGen = 5;

    private int genLevel = 5;
    private int minChange = -2;
    private int maxChange = 0;

    public int biomeCount;

    public Color[] biomeColors;

    public Texture2D Colormap;

    private int bio1, bio2;

    private void Start()
    {
        map = new BoardCollection[mapSize.x, mapSize.y];

        biomeColors = new Color[biomeCount];
        Colormap = new Texture2D(mapSize.x, mapSize.y);
        Colormap.filterMode = FilterMode.Point;

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                map[x, y] = new BoardCollection();
                map[x, y].Init();
            }
        }
        for (int i = 0; i < biomeCount; i++)
        {
            biomeColors[i] = new Color(Random.Range(0F, 1F), Random.Range(0, 1F), Random.Range(0, 1F));
        }
        for (int i = 1; i < biomeCount; i++)
        {
            map[Random.Range(0, mapSize.x - 1), Random.Range(0, mapSize.y - 1)].biomeID = i;
        }
        updateMapColors();
    }

    private void Update()
    {
        if (doUpdate && (lastGen + genSpeed < Time.time))
        {
            doThing();
            updateMapColors();
            traitMarkBiome();
            applyBiomeColorChange();
            applyMapToTexture();
            lastGen = Time.time;
        }

        if (Input.GetMouseButtonDown(0))
        {
            bio1 = map[getMouseToMapCoord().x, getMouseToMapCoord().y].biomeID;
        }
        if (Input.GetMouseButtonDown(1))
        {
            bio2 = map[getMouseToMapCoord().x, getMouseToMapCoord().y].biomeID;
        }
        if (Input.GetButtonDown("Jump"))
        {
            connecteBiome(bio1, bio2);
        }
    }

    private void applyMapToTexture()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Colormap.SetPixel(x, Colormap.height - y - 1, map[x, y].color);
            }
        }
        Colormap.Apply();
    }

    private void updateMapColors()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                map[x, y].color = biomeColors[map[x, y].biomeID];
            }
        }
    }

    public GUIStyle debugStyle;
    public int textureWidth = 800, textureHeight = 800;
    public int TextureX = 0, TextureY = 0;

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(TextureX, TextureY, textureWidth, textureHeight), Colormap);
        if (getMouseToMapCoord().x > 0 && getMouseToMapCoord().x < mapSize.x && getMouseToMapCoord().y > 0 && getMouseToMapCoord().y < mapSize.y)
        {
            GUI.Label(new Rect((int)Input.mousePosition.x, Screen.height - (int)Input.mousePosition.y, 200, 200), "Biome ID : " + map[getMouseToMapCoord().x, getMouseToMapCoord().y].biomeID.ToString(), debugStyle);
            GUI.Label(new Rect((int)Input.mousePosition.x, Screen.height - (int)Input.mousePosition.y + 20, 200, 200), "Connected To Other Biome : " + map[getMouseToMapCoord().x, getMouseToMapCoord().y].connectedToOther.ToString(), debugStyle);
            GUI.Label(new Rect((int)Input.mousePosition.x, Screen.height - (int)Input.mousePosition.y + 40, 200, 200), "Edge of the world : " + map[getMouseToMapCoord().x, getMouseToMapCoord().y].outerShell.ToString(), debugStyle);
            GUI.Label(new Rect((int)Input.mousePosition.x, Screen.height - (int)Input.mousePosition.y + 60, 200, 200), "Is Entrance : " + map[getMouseToMapCoord().x, getMouseToMapCoord().y].doorWay.ToString(), debugStyle);
        }
        GUI.Label(new Rect(0, 0, 1000, 20), "Connecting Biome 1 ID : " + bio1, debugStyle);
        GUI.Label(new Rect(0, 20, 1000, 20), "Connecting Biome 2 ID : " + bio2, debugStyle);
    }

    public Vector2Int getMouseToMapCoord()
    {
        Vector2 mousePos = Input.mousePosition;
        int XScale = textureWidth / mapSize.x;
        int YScale = textureHeight / mapSize.y;
        Vector2Int mousePosInt = new Vector2Int(((int)mousePos.x - TextureX) / XScale, (Screen.height - (int)mousePos.y - TextureY) / YScale);
        return mousePosInt;
    }

    private void connecteBiome(int currentBiomeID, int connectingBiomeID)
    {
        List<Vector2> potentialSpotsF = new List<Vector2>();
        List<Vector2> potentialSpotsB = new List<Vector2>();
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (map[x, y].connectedToOther && map[x, y].biomeID == currentBiomeID)
                {
                    if (x > 0)
                        if (map[x - 1, y].biomeID == connectingBiomeID)
                        {
                            potentialSpotsF.Add(new Vector2(x, y));
                            potentialSpotsB.Add(new Vector2(x - 1, y));
                        }
                    if (y > 0)
                        if (map[x, y - 1].biomeID == connectingBiomeID)
                        {
                            potentialSpotsF.Add(new Vector2(x, y));
                            potentialSpotsB.Add(new Vector2(x, y - 1));
                        }
                    if (x < mapSize.x - 1)
                        if (map[x + 1, y].biomeID == connectingBiomeID)
                        {
                            potentialSpotsF.Add(new Vector2(x, y));
                            potentialSpotsB.Add(new Vector2(x + 1, y));
                        }
                    if (y < mapSize.y - 1)
                        if (map[x, y + 1].biomeID == connectingBiomeID)
                        {
                            potentialSpotsF.Add(new Vector2(x, y));
                            potentialSpotsB.Add(new Vector2(x, y + 1));
                        }
                }
            }
        }

        if (potentialSpotsB.Count > 0)
        {
            int rand = Random.Range(0, potentialSpotsB.Count);
            map[(int)potentialSpotsF[rand].x, (int)potentialSpotsF[rand].y].doorWay = true;
            map[(int)potentialSpotsB[rand].x, (int)potentialSpotsB[rand].y].doorWay = true;
        }
    }

    private void doThing()
    {
        BoardCollection[,] tempMap = new BoardCollection[mapSize.x, mapSize.y];

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                tempMap[x, y] = new BoardCollection();
                tempMap[x, y].biomeID = map[x, y].biomeID;
            }
        }

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (tempMap[x, y].biomeID != 0)
                {
                    if (x > 0)
                        if (tempMap[x - 1, y].biomeID == 0)
                            map[x - 1, y].biomeID = expandChance(tempMap[x, y].biomeID, 1);
                    if (y > 0)
                        if (tempMap[x, y - 1].biomeID == 0)
                            map[x, y - 1].biomeID = expandChance(tempMap[x, y].biomeID, 2);
                    if (x < mapSize.x - 1)
                        if (tempMap[x + 1, y].biomeID == 0)
                            map[x + 1, y].biomeID = expandChance(tempMap[x, y].biomeID, 3);
                    if (y < mapSize.y - 1)
                        if (tempMap[x, y + 1].biomeID == 0)
                            map[x, y + 1].biomeID = expandChance(tempMap[x, y].biomeID, 4);
                }
            }
        }
    }

    private void traitMarkBiome()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                map[x, y].connectedToOther = false;
                map[x, y].outerShell = false;

                if (x == 0 || x == mapSize.x - 1 || y == 0 || y == mapSize.x - 1)
                    map[x, y].outerShell = true;
                if (x > 0)
                    if (map[x - 1, y].biomeID != map[x, y].biomeID)
                        map[x, y].connectedToOther = true;
                if (y > 0)
                    if (map[x, y - 1].biomeID != map[x, y].biomeID)
                        map[x, y].connectedToOther = true;
                if (x < mapSize.x - 1)
                    if (map[x + 1, y].biomeID != map[x, y].biomeID)
                        map[x, y].connectedToOther = true;
                if (y < mapSize.y - 1)
                    if (map[x, y + 1].biomeID != map[x, y].biomeID)
                        map[x, y].connectedToOther = true;
            }
        }
    }

    private void applyBiomeColorChange()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (map[x, y].outerShell)
                    map[x, y].color = new Color(Mathf.Clamp(map[x, y].color.r - 0.1f, 0, 1), Mathf.Clamp(map[x, y].color.g - 0.1f, 0, 1), Mathf.Clamp(map[x, y].color.b - 0.1f, 0, 1));
                if (map[x, y].connectedToOther)
                    map[x, y].color = new Color(Mathf.Clamp(map[x, y].color.r - 0.1f, 0, 1), Mathf.Clamp(map[x, y].color.g - 0.1f, 0, 1), Mathf.Clamp(map[x, y].color.b - 0.1f, 0, 1));
                if (map[x, y].doorWay)
                    map[x, y].color = new Color(Mathf.Clamp(map[x, y].color.r + 0.2f, 0, 1), Mathf.Clamp(map[x, y].color.g + 0.2f, 0, 1), Mathf.Clamp(map[x, y].color.b + 0.2f, 0, 1));
            }
        }
    }

    private int expandChance(int biomeID, int dir)
    {
        switch (biomeID)
        {
            case 1:
                return dir == 1 ? Random.Range(0, 4) == 1 ? biomeID : 0 :
                    dir == 2 ? Random.Range(0, 4) == 1 ? biomeID : 0 :
                    dir == 3 ? Random.Range(0, 2) == 1 ? biomeID : 0 :
                    dir == 4 ? Random.Range(0, 2) == 1 ? biomeID : 0 : 0;

            case 2:
                return dir == 1 ? Random.Range(0, 2) == 1 ? biomeID : 0 :
                    dir == 2 ? Random.Range(0, 2) == 1 ? biomeID : 0 :
                    dir == 3 ? Random.Range(0, 4) == 1 ? biomeID : 0 :
                    dir == 4 ? Random.Range(0, 4) == 1 ? biomeID : 0 : 0;

            case 3:
                return dir == 1 ? Random.Range(0, 2) == 1 ? biomeID : 0 :
                    dir == 2 ? Random.Range(0, 4) == 1 ? biomeID : 0 :
                    dir == 3 ? Random.Range(0, 2) == 1 ? biomeID : 0 :
                    dir == 4 ? Random.Range(0, 4) == 1 ? biomeID : 0 : 0;

            case 4:
                return dir == 1 ? Random.Range(0, 4) == 1 ? biomeID : 0 :
                    dir == 2 ? Random.Range(0, 2) == 1 ? biomeID : 0 :
                    dir == 3 ? Random.Range(0, 4) == 1 ? biomeID : 0 :
                    dir == 4 ? Random.Range(0, 2) == 1 ? biomeID : 0 : 0;

            default:
                return Random.Range(0, 5) == 1 ? biomeID : 0;
        }
    }

    private int cleanSpaceCount()
    {
        int count = 0;
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (map[x, y].biomeID == 0)
                    count++;
            }
        }
        return count;
    }
}