using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class HeatmapGen : MonoBehaviour
{
    public Vector2Int mapSize = new Vector2Int(100, 100);
    public float genSpeed;

    public int biomeCount;
    public Color[] biomeColors;
    private bool mapChange;

    public List<int[]> connections;

    public int textureWidth = 800, textureHeight = 800;
    public int debugOverlayW = 200, debugOverlayH = 200;
    public int TextureX = 0, TextureY = 0;

    public bool RenderMap = false;
    public bool RenderGrid = false;
    public bool RenderDebug = false;

    public GUIStyle debugStyle;

    private bool DataInitalised = false;
    private float lastGen;
    private BoardCollection[,] map;
    private Texture2D Colormap;
    private Texture2D grid;
    private int bio1;

    private void Start()
    {
        initMapData();
        initTexureData();
        DataInitalised = true;
    }

    private void Update()
    {
        if (DataInitalised)
        {
            if (cleanSpaceCount() > 0)
            {
                if ((lastGen + genSpeed < Time.time))
                {
                    growBiome();
                    if (cleanSpaceCount() <= 0)
                    {
                        wallOffBiomes();
                        GenAllHeatMap();
                        genAllInternallWalls();
                    }
                }
                lastGen = Time.time;
            }
            if (cleanSpaceCount() == 0 && mapChange)
            {
                setBaseBiomeColor();
                traitMarkBiome();
                applyMapToTexture();

                updateGrid();
                mapChange = false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                bio1 = map[getMouseToMapCoord().x, getMouseToMapCoord().y].biomeID;
            }
            if (Input.GetButtonDown("Jump"))
            {
                wallOffBiomes();
                mapChange = true;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                for (int i = 0; i < biomeCount; i++)
                {
                    if (i != bio1)
                        connecteBiome(bio1, i);
                    mapChange = true;
                }
            }
        }
    }

    private void fixPockets(int biomeID)
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
            }
        }
    }

    private void OnGUI()
    {
        if (RenderMap) GUI.DrawTexture(new Rect(TextureX, TextureY, textureWidth, textureHeight), Colormap);
        if (RenderGrid) GUI.DrawTexture(new Rect(TextureX, TextureY, textureWidth, textureHeight), grid);
        if (RenderDebug) drawDebug();
    }

    private void initTexureData()
    {
        Colormap = new Texture2D(mapSize.x, mapSize.y);
        Colormap.filterMode = FilterMode.Point;

        grid = new Texture2D(debugOverlayW, debugOverlayH);
        grid.filterMode = FilterMode.Point;

        for (int x = 0; x < debugOverlayW; x++)
        {
            for (int y = 0; y < debugOverlayH; y++)
            {
                grid.SetPixel(x, y, Color.clear);
            }
        }
    }

    private void initMapData()
    {
        connections = new List<int[]>();

        map = new BoardCollection[mapSize.x, mapSize.y];
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                map[x, y] = new BoardCollection();
                map[x, y].Init();
            }
        }

        biomeColors = new Color[biomeCount];
        for (int i = 0; i < biomeCount; i++)
        {
            biomeColors[i] = new Color(Random.Range(0F, 1F), Random.Range(0, 1F), Random.Range(0, 1F));
        }

        for (int i = 1; i < biomeCount; i++)
        {
            map[Random.Range(0, mapSize.x - 1), Random.Range(0, mapSize.y - 1)].biomeID = i;
        }
    }

    private void growBiome()
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
        mapChange = true;
    }

    private void setBaseBiomeColor()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                map[x, y].color = biomeColors[map[x, y].biomeID];
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

    private void wallOffBiomes()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (x == mapSize.x - 1)
                    map[x, y].rightWall = true;
                if (x == 0)
                    map[x, y].leftWall = true;
                if (y == mapSize.y - 1)
                    map[x, y].bottomWall = true;
                if (y == 0)
                    map[x, y].topWall = true;
                if (x > 0)
                    if (map[x - 1, y].biomeID != map[x, y].biomeID)
                    {
                        map[x, y].leftWall = true;
                    }
                if (y > 0)
                    if (map[x, y - 1].biomeID != map[x, y].biomeID)
                    {
                        map[x, y].topWall = true;
                    }
                if (x < mapSize.x - 1)
                    if (map[x + 1, y].biomeID != map[x, y].biomeID)
                    {
                        map[x, y].rightWall = true;
                    }
                if (y < mapSize.y - 1)
                    if (map[x, y + 1].biomeID != map[x, y].biomeID)
                    {
                        map[x, y].bottomWall = true;
                    }
            }
        }
    }

    private void GenAllHeatMap()
    {
        for (int i = 1; i < biomeCount; i++)
        {
            genBiomeHeatMap(i, 4);
        }
    }

    private void genAllInternallWalls()
    {
        for (int i = 0; i < biomeCount; i++)
        {
            genBoardWall(i);
        }
    }

    private void updateGrid()
    {
        for (int x = 0; x < debugOverlayW; x++)
        {
            for (int y = 0; y < debugOverlayH; y++)
            {
                grid.SetPixel(x, y, Color.clear);
            }
        }
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (map[x, y].topWall)
                {
                    for (int x2 = 0; x2 < debugOverlayW / mapSize.x; x2++)
                    {
                        grid.SetPixel((x * (debugOverlayW / mapSize.x)) + x2, debugOverlayH - 1 - (y * (debugOverlayH / mapSize.y)), Color.white);
                    }
                }
                if (map[x, y].bottomWall)
                {
                    for (int x2 = 0; x2 < debugOverlayW / mapSize.x; x2++)
                    {
                        grid.SetPixel((x * (debugOverlayW / mapSize.x)) + x2, debugOverlayH - ((y + 1) * (debugOverlayH / mapSize.y)), Color.white);
                    }
                }
                if (map[x, y].leftWall)
                {
                    for (int y2 = 0; y2 < debugOverlayH / mapSize.y; y2++)
                    {
                        grid.SetPixel(x * (debugOverlayW / mapSize.x), debugOverlayH - (y * (debugOverlayH / mapSize.y)) - y2 - 1, Color.white);
                    }
                }
                if (map[x, y].rightWall)
                {
                    for (int y2 = 0; y2 < debugOverlayH / mapSize.y; y2++)
                    {
                        grid.SetPixel(((x + 1) * (debugOverlayW / mapSize.x)) - 1, debugOverlayH - (y * (debugOverlayH / mapSize.y)) - y2 - 1, Color.white);
                    }
                }
            }
        }

        grid.Apply();
    }

    private void connecteBiome(int currentBiomeID, int connectingBiomeID)
    {
        List<Vector2> potentialSpotsF = new List<Vector2>();
        List<Vector2> potentialSpotsB = new List<Vector2>();
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (map[x, y].connectedToOther && map[x, y].biomeID == currentBiomeID && !areBiomesConnected(currentBiomeID, connectingBiomeID))
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
            if ((int)potentialSpotsF[rand].x > (int)potentialSpotsB[rand].x)
            {
                map[(int)potentialSpotsF[rand].x, (int)potentialSpotsF[rand].y].leftWall = false;
                map[(int)potentialSpotsB[rand].x, (int)potentialSpotsB[rand].y].rightWall = false;
            }
            if ((int)potentialSpotsF[rand].x < (int)potentialSpotsB[rand].x)
            {
                map[(int)potentialSpotsF[rand].x, (int)potentialSpotsF[rand].y].rightWall = false;
                map[(int)potentialSpotsB[rand].x, (int)potentialSpotsB[rand].y].leftWall = false;
            }
            if ((int)potentialSpotsF[rand].y > (int)potentialSpotsB[rand].y)
            {
                map[(int)potentialSpotsF[rand].x, (int)potentialSpotsF[rand].y].topWall = false;
                map[(int)potentialSpotsB[rand].x, (int)potentialSpotsB[rand].y].bottomWall = false;
            }
            if ((int)potentialSpotsF[rand].y < (int)potentialSpotsB[rand].y)
            {
                map[(int)potentialSpotsF[rand].x, (int)potentialSpotsF[rand].y].bottomWall = false;
                map[(int)potentialSpotsB[rand].x, (int)potentialSpotsB[rand].y].topWall = false;
            }
            int[] con1 = new int[2];
            int[] con2 = new int[2];
            con1[0] = map[(int)potentialSpotsF[rand].x, (int)potentialSpotsF[rand].y].biomeID;
            con1[1] = map[(int)potentialSpotsB[rand].x, (int)potentialSpotsB[rand].y].biomeID;
            connections.Add(con1);
            con2[0] = map[(int)potentialSpotsB[rand].x, (int)potentialSpotsB[rand].y].biomeID;
            con2[1] = map[(int)potentialSpotsF[rand].x, (int)potentialSpotsF[rand].y].biomeID;
            connections.Add(con2);
        }
    }

    private void genBiomeHeatMap(int biome, int maxWeight)
    {
        while (freeSpaceFromBiome(biome) > 0)
            generateHeatmapVein(biome, maxWeight);
    }

    private void generateHeatmapVein(int biomeID, int weight)
    {
        List<Vector2> freePoss = new List<Vector2>();
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (map[x, y].biomeID == biomeID && map[x, y].weight == 0)
                {
                    freePoss.Add(new Vector2Int(x, y));
                }
            }
        }

        if (freePoss.Count > 0)
        {
            int Rand = Random.Range(0, freePoss.Count);
            map[(int)freePoss[Rand].x, (int)freePoss[Rand].y].weight = weight;
            downThePath(new Vector2Int((int)freePoss[Rand].x, (int)freePoss[Rand].y));
        }
    }

    private void downThePath(Vector2Int pos)
    {
        if (pos.y >= 0 && pos.x >= 0 && pos.x < mapSize.x && pos.y < mapSize.y && map[pos.x, pos.y].weight > 2)
            if (getConnectedWeightedBiome(pos, map[pos.x, pos.y].biomeID, 0) > 0)
            {
                List<Vector2> freePoss = new List<Vector2>();
                if (pos.x > 0)
                    if (map[pos.x - 1, pos.y].biomeID == map[pos.x, pos.y].biomeID && map[pos.x - 1, pos.y].weight == 0)
                        freePoss.Add(new Vector2(-1, 0));
                if (pos.y > 0)
                    if (map[pos.x, pos.y - 1].biomeID == map[pos.x, pos.y].biomeID && map[pos.x, pos.y - 1].weight == 0)
                        freePoss.Add(new Vector2(0, -1));
                if (pos.x < mapSize.x - 1)
                    if (map[pos.x + 1, pos.y].biomeID == map[pos.x, pos.y].biomeID && map[pos.x + 1, pos.y].weight == 0)
                        freePoss.Add(new Vector2(+1, 0));
                if (pos.y < mapSize.y - 1)
                    if (map[pos.x, pos.y + 1].biomeID == map[pos.x, pos.y].biomeID && map[pos.x, pos.y + 1].weight == 0)
                        freePoss.Add(new Vector2(0, +1));
                int Rand = Random.Range(0, freePoss.Count);
                map[pos.x + (int)freePoss[Rand].x, pos.y + (int)freePoss[Rand].y].weight = Mathf.Clamp(map[pos.x, pos.y].weight + Random.Range(-1, 1), 1, 3);
                downThePath(new Vector2Int(pos.x + (int)freePoss[Rand].x, pos.y + (int)freePoss[Rand].y));
            }
    }

    private void genBoardWall(int biomeID)
    {
        Debug.ClearDeveloperConsole();
        print("3 weight space: " + getSpaceWithFreeWallsCount(biomeID, 3));
        print("2 weight space: " + getSpaceWithFreeWallsCount(biomeID, 2));
        print("1 weight space: " + getSpaceWithFreeWallsCount(biomeID, 1));
        if (getSpaceWithFreeWallsCount(biomeID, 3) > 0)
            setWalls(getSpaceWithFreeWalls(biomeID, 3));
        else if (getSpaceWithFreeWallsCount(biomeID, 2) > 0)
            setWalls(getSpaceWithFreeWalls(biomeID, 2));
        else if (getSpaceWithFreeWallsCount(biomeID, 1) > 0)
            setWalls(getSpaceWithFreeWalls(biomeID, 1));
        if (getSpaceWithFreeWallsCount(biomeID, 3) > 0 || getSpaceWithFreeWallsCount(biomeID, 2) > 0 || getSpaceWithFreeWallsCount(biomeID, 1) > 0)
            genBoardWall(biomeID);
    }

    private void setWalls(Vector2Int pos)
    {
        List<Vector2> freePos = new List<Vector2>();

        if (pos.x < mapSize.x - 1)
            if (map[pos.x + 1, pos.y].getWallCount() < 3 && map[pos.x, pos.y].weight >= 1)
                freePos.Add(new Vector2(+1, 0));
        if (pos.x > 0)
            if (map[pos.x - 1, pos.y].getWallCount() < 3 && map[pos.x, pos.y].weight >= 1)
                freePos.Add(new Vector2(-1, 0));
        if (pos.y < mapSize.y - 1)
            if (map[pos.x, pos.y + 1].getWallCount() < 3 && map[pos.x, pos.y].weight >= 1)
                freePos.Add(new Vector2(0, +1));
        if (pos.y > 0)
            if (map[pos.x, pos.y - 1].getWallCount() < 3 && map[pos.x, pos.y].weight >= 1)
                freePos.Add(new Vector2(0, -1));

        if (freePos.Count > 0)
        {
            int Rand = Random.Range(0, freePos.Count);
            if (pos.x + freePos[Rand].x > pos.x)
            {
                map[pos.x + (int)freePos[Rand].x, pos.y + (int)freePos[Rand].y].leftWall = true;
                map[pos.x, pos.y].rightWall = true;
            }
            if (pos.x + freePos[Rand].x < pos.x)
            {
                map[pos.x + (int)freePos[Rand].x, pos.y + (int)freePos[Rand].y].rightWall = true;
                map[pos.x, pos.y].leftWall = true;
            }
            if (pos.y + freePos[Rand].y > pos.y)
            {
                map[pos.x + (int)freePos[Rand].x, pos.y + (int)freePos[Rand].y].topWall = true;
                map[pos.x, pos.y].bottomWall = true;
            }
            if (pos.y + freePos[Rand].y < pos.y)
            {
                map[pos.x + (int)freePos[Rand].x, pos.y + (int)freePos[Rand].y].bottomWall = true;
                map[pos.x, pos.y].topWall = true;
            }
        }
        mapChange = true;
    }

    private Vector2Int getSpaceWithFreeWalls(int biomeID, int weight)
    {
        Vector2Int vec2 = new Vector2Int();
        List<Vector2> freePos = new List<Vector2>();
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (map[x, y].biomeID == biomeID && map[x, y].getWallCount() < 4 - map[x, y].weight && map[x, y].weight == weight)
                {
                    freePos.Add(new Vector2(x, y));
                }
            }
        }
        if (freePos.Count > 0)
        {
            int Rand = Random.Range(0, freePos.Count);
            return new Vector2Int((int)freePos[Rand].x, (int)freePos[Rand].y);
        }
        return vec2;
    }

    private int getSpaceWithFreeWallsCount(int biomeID, int weight)
    {
        int count = 0;
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (map[x, y].biomeID == biomeID && map[x, y].weight == weight && map[x, y].getWallCount() < 4 - map[x, y].weight)
                {
                    count++;
                }
            }
        }
        return count;
    }

    private int freeSpaceFromBiome(int biomeID)
    {
        int spaces = 0;
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (map[x, y].biomeID == biomeID && map[x, y].weight == 0)
                    spaces++;
            }
        }
        return spaces;
    }

    private int getConnectedWeightedBiome(Vector2Int pos, int biomeID, int weight)
    {
        int count = 0;
        if (pos.x > 0)
            if (map[pos.x - 1, pos.y].biomeID == map[pos.x, pos.y].biomeID && map[pos.x - 1, pos.y].weight == weight)
                count++;
        if (pos.y > 0)
            if (map[pos.x, pos.y - 1].biomeID == map[pos.x, pos.y].biomeID && map[pos.x, pos.y - 1].weight == weight)
                count++;
        if (pos.x < mapSize.x - 1)
            if (map[pos.x + 1, pos.y].biomeID == map[pos.x, pos.y].biomeID && map[pos.x + 1, pos.y].weight == weight)
                count++;
        if (pos.y < mapSize.y - 1)
            if (map[pos.x, pos.y + 1].biomeID == map[pos.x, pos.y].biomeID && map[pos.x, pos.y + 1].weight == weight)
                count++;
        return count;
    }

    private bool areBiomesConnected(int currentBiomeID, int connectingBiomeID)
    {
        foreach (int[] con in connections)
        {
            if (con[0] == currentBiomeID && con[1] == connectingBiomeID)
                return true;
        }
        return false;
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

    public Vector2Int getMouseToMapCoord()
    {
        Vector2 mousePos = Input.mousePosition;
        int XScale = textureWidth / mapSize.x;
        int YScale = textureHeight / mapSize.y;
        Vector2Int mousePosInt = new Vector2Int(((int)mousePos.x - TextureX) / XScale, (Screen.height - (int)mousePos.y - TextureY) / YScale);
        return mousePosInt;
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

    //////////////////////////////////////////////////////////////////////////////////
    ////////////////////Debug/////////////////////////////////////////////////////////
    private void drawDebug()
    {
        if (getMouseToMapCoord().x >= 0 && getMouseToMapCoord().x < mapSize.x && getMouseToMapCoord().y >= 0 && getMouseToMapCoord().y < mapSize.y)
        {
            GUI.Label(new Rect((int)Input.mousePosition.x, Screen.height - (int)Input.mousePosition.y, 200, 200), "Biome ID : " + map[getMouseToMapCoord().x, getMouseToMapCoord().y].biomeID.ToString(), debugStyle);
            GUI.Label(new Rect((int)Input.mousePosition.x, Screen.height - (int)Input.mousePosition.y + 20, 200, 200), "Connected To Other Biome : " + map[getMouseToMapCoord().x, getMouseToMapCoord().y].connectedToOther.ToString(), debugStyle);
            GUI.Label(new Rect((int)Input.mousePosition.x, Screen.height - (int)Input.mousePosition.y + 40, 200, 200), "Edge of the world : " + map[getMouseToMapCoord().x, getMouseToMapCoord().y].outerShell.ToString(), debugStyle);
            GUI.Label(new Rect((int)Input.mousePosition.x, Screen.height - (int)Input.mousePosition.y + 60, 200, 200), "Is Entrance : " + map[getMouseToMapCoord().x, getMouseToMapCoord().y].doorWay.ToString(), debugStyle);
            GUI.Label(new Rect((int)Input.mousePosition.x, Screen.height - (int)Input.mousePosition.y + 80, 200, 200), "Wall top : " + map[getMouseToMapCoord().x, getMouseToMapCoord().y].topWall.ToString(), debugStyle);
            GUI.Label(new Rect((int)Input.mousePosition.x, Screen.height - (int)Input.mousePosition.y + 100, 200, 200), "Wall bot: " + map[getMouseToMapCoord().x, getMouseToMapCoord().y].bottomWall.ToString(), debugStyle);
            GUI.Label(new Rect((int)Input.mousePosition.x, Screen.height - (int)Input.mousePosition.y + 120, 200, 200), "Wall left: " + map[getMouseToMapCoord().x, getMouseToMapCoord().y].leftWall.ToString(), debugStyle);
            GUI.Label(new Rect((int)Input.mousePosition.x, Screen.height - (int)Input.mousePosition.y + 140, 200, 200), "Wall right: " + map[getMouseToMapCoord().x, getMouseToMapCoord().y].rightWall.ToString(), debugStyle);
            GUI.Label(new Rect((int)Input.mousePosition.x, Screen.height - (int)Input.mousePosition.y + 160, 200, 200), "Weight: " + map[getMouseToMapCoord().x, getMouseToMapCoord().y].weight.ToString(), debugStyle);
        }
        GUI.Label(new Rect(0, 0, 1000, 20), "Connecting Biome 1 ID : " + bio1, debugStyle);
        for (int i = 0; i < connections.Count; i++)
        {
            int[] con = connections[i];
            GUI.Label(new Rect(0, 20 * (1 + i), 1000, 20), "Biome :" + con[0] + " connected to biome :" + con[1], debugStyle);
        }

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                GUI.Label(new Rect(x * (textureWidth / mapSize.x), y * (textureHeight / mapSize.y), (textureWidth / mapSize.x), (textureWidth / mapSize.y)), map[x, y].weight.ToString());
            }
        }
    }
}