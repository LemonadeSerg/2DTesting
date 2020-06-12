using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerationManager : MonoBehaviour
{
    public WallGeneration wallGen;
    public BiomeGen biomeGen;
    public HeatmapGen heatGen;
    public VisualGen visGen;
    public BoardTraitGen traitGen;
    public BiomeActions biomeActions;
    public NewWallGeneration newWallGen;

    public int seed;

    public Vector2Int mapSize = new Vector2Int(100, 100);
    public BoardCollection[,] map;

    public bool DataInitalised;
    public bool showGeneration = true;

    public int textureWidth = 800, textureHeight = 800;
    public int TextureX = 0, TextureY = 0;

    public bool RenderMap = false;
    public bool RenderGrid = false;
    public bool RenderDebug = false;

    public GUIStyle debugStyle;

    public int biomeCount = 6;

    private int bio1;

    private bool biomeGrown = false;
    private bool heatmapGenerated = false;
    private bool traitsMarked = false;
    private bool ouWallsGenerated = false;
    private bool inWallsGenerated = false;
    public int cycleSize = 10;
    public float lastTime;

    public float wallingSpeed = 0.5f;

    // Start is called before the first frame update
    private void Start()
    {
        Random.InitState(seed);
        textureWidth = Screen.height;
        textureHeight = Screen.height;
        biomeGen = new BiomeGen();
        heatGen = new HeatmapGen();
        visGen = new VisualGen();
        wallGen = new WallGeneration();
        traitGen = new BoardTraitGen();
        biomeActions = new BiomeActions();
        newWallGen = new NewWallGeneration();
        biomeGrown = false;
        heatmapGenerated = false;
        traitsMarked = false;
        ouWallsGenerated = false;
        inWallsGenerated = false;

        init();
    }

    private void restart()
    {
        Start();
    }

    // Update is called once per frame
    private void Update()
    {
        if (visGen == null)
            Start();
        if (showGeneration && DataInitalised)
        {
            for (int cycle = 0; cycle < cycleSize; cycle++)
            {
                if (!biomeGrown)
                {
                    biomeGen.growBiome(map);
                    biomeGen.setBaseBiomeColor(map);
                    if (biomeGen.cleanSpaceCount(map) == 0)
                        biomeGrown = true;
                }
                /*else if (!heatmapGenerated)
                {
                    heatmapGenerated = true;
                    for (int i = 1; i < biomeCount; i++)
                    {
                        if (heatGen.freeSpaceFromBiome(i, map) > 0)
                        {
                            heatmapGenerated = false;
                            heatGen.generateHeatmapVein(i, 3, map);
                        }
                    }
                    if (heatmapGenerated)
                        heatGen.applyReduction(map);
                }*/
                else if (!traitsMarked)
                {
                    traitGen.traitMarkBiome(map);
                    traitsMarked = true;
                }
                else if (!ouWallsGenerated)
                {
                    wallGen.wallOffBiomes(map);
                    ouWallsGenerated = true;
                }
                else if (!inWallsGenerated)
                {
                    newWallGen.addFloatingWall(map, 100);
                    if (newWallGen.noMore)
                        inWallsGenerated = true;
                }
                else
                {
                    if (Time.time > lastTime + wallingSpeed)
                    {
                        newWallGen.buildWall(map, 1);
                        lastTime = Time.time;
                    }
                }
                /*else if (!inWallsGenerated)

                {
                    inWallsGenerated = true;
                    for (int i = 0; i < biomeCount; i++)
                    {
                        if (wallGen.getSpaceWithFreeWallsCount(i, 1, map) > 0)
                        {
                            wallGen.setWalls(wallGen.getSpaceWithFreeWalls(i, 1, map), map);
                            inWallsGenerated = false;
                        }
                        if (wallGen.getSpaceWithFreeWallsCount(i, 2, map) > 0)
                        {
                            wallGen.setWalls(wallGen.getSpaceWithFreeWalls(i, 2, map), map);
                            inWallsGenerated = false;
                        }
                        if (wallGen.getSpaceWithFreeWallsCount(i, 3, map) > 0)
                        {  s
                            wallGen.setWalls(wallGen.getSpaceWithFreeWalls(i, 3, map), map);
                            inWallsGenerated = false;
                        }
                    }
                }*/
            }

            visGen.updateGraphics(map);
        }
    }

    private void OnGUI()
    {
        if (DataInitalised)
        {
            if (RenderMap) GUI.DrawTexture(new Rect(TextureX, TextureY, textureWidth, textureHeight), visGen.Colormap);
            if (RenderGrid) GUI.DrawTexture(new Rect(TextureX, TextureY, textureWidth, textureHeight), visGen.grid);
            if (RenderDebug) drawDebug();
        }
    }

    private void init()
    {
        map = new BoardCollection[mapSize.x, mapSize.y];
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                map[x, y] = new BoardCollection();
                map[x, y].Init();
            }
        }
        biomeGen.init(biomeCount);
        visGen.init(map);
        biomeActions.init();
        biomeGen.placeBiomeStarts(map);
        DataInitalised = true;

        if (!showGeneration)
        {
            biomeGen.growBiomeAll(map);
            biomeGen.setBaseBiomeColor(map);
            heatGen.GenAllHeatMap(biomeCount, map);

            traitGen.traitMarkBiome(map);
            wallGen.wallOffBiomes(map);
            wallGen.genAllInternallWalls(biomeCount, map);
        }
        visGen.updateGraphics(map);
    }

    //////////////////////////////////////////////////////////////////////////////////
    ////////////////////Debug/////////////////////////////////////////////////////////
    private void drawDebug()
    {
        if (getMouseToMapCoord().x >= 0 && getMouseToMapCoord().x < mapSize.x && getMouseToMapCoord().y >= 0 && getMouseToMapCoord().y < mapSize.y)
        {
            GUI.Label(new Rect((int)Input.mousePosition.x, Screen.height - (int)Input.mousePosition.y - 40, 200, 200), "Mouse Rel Pos :" + Input.mousePosition.ToString(), debugStyle);
            GUI.Label(new Rect((int)Input.mousePosition.x, Screen.height - (int)Input.mousePosition.y - 20, 200, 200), "Mouse Rel Pos :" + getMouseToMapCoord().ToString(), debugStyle);
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
        for (int i = 0; i < biomeActions.connections.Count; i++)
        {
            int[] con = biomeActions.connections[i];
            GUI.Label(new Rect(0, 20 * (1 + i), 1000, 20), "Biome :" + con[0] + " connected to biome :" + con[1], debugStyle);
        }

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                //GUI.Label(new Rect((int)((float)x * (float)textureWidth / (float)(mapSize.x)), (int)((float)y * (float)textureHeight / (float)(mapSize.y)), (textureWidth / mapSize.x), (textureWidth / mapSize.y)), map[x, y].weight.ToString(), debugStyle);
            }
        }
    }

    public Vector2Int getMouseToMapCoord()
    {
        Vector2 mousePos = Input.mousePosition;
        float XScale = (float)textureWidth / (float)mapSize.x;
        float YScale = (float)textureHeight / (float)mapSize.y;
        Vector2Int mousePosInt = new Vector2Int((int)(mousePos.x / XScale) + TextureX, (int)((Screen.height - mousePos.y) / YScale) + TextureY);
        return mousePosInt;
    }
}