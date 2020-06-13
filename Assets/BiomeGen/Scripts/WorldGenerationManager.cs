using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerationManager : MonoBehaviour
{
    public BiomeGen biomeGen;
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
    public bool DrawCellIndividually = true;

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
        if (biomeGen == null)
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
                else if (!ouWallsGenerated)
                {
                    newWallGen.wallOffBiomes(map);
                    ouWallsGenerated = true;
                }
                else if (!inWallsGenerated)
                {
                    newWallGen.addFloatingWall(map, 10);
                    if (newWallGen.noMore)
                    {
                        newWallGen.addPerimeterBreakoffs(map);
                        inWallsGenerated = true;
                    }
                }
                else if (!newWallGen.wallGened)
                {
                    newWallGen.buildWall(map, 60);
                }
                else if (!traitsMarked)
                {
                    traitGen.traitMarkBiome(map);
                    traitsMarked = true;
                }
            }
        }
    }

    private void OnGUI()
    {
        if (DataInitalised)
        {
            if (DrawCellIndividually)
            {
                for (int x = 0; x < mapSize.x; x++)
                {
                    for (int y = 0; y < mapSize.y; y++)
                    {
                        GUI.DrawTexture(new Rect(x * (textureWidth / mapSize.x), y * (textureHeight / mapSize.y), (textureWidth / mapSize.x), (textureHeight / mapSize.y)), map[x, y].tex);
                    }
                }
            }

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
                map[x, y].Init(textureWidth / mapSize.x, textureHeight / mapSize.y);
            }
        }
        biomeGen.init(biomeCount + 1);
        biomeActions.init();
        biomeGen.placeBiomeStarts(map);
        DataInitalised = true;

        if (!showGeneration)
        {
            biomeGen.growBiomeAll(map);
            biomeGen.setBaseBiomeColor(map);

            traitGen.traitMarkBiome(map);
            newWallGen.wallOffBiomes(map);
        }
    }

    //////////////////////////////////////////////////////////////////////////////////
    ////////////////////Debug/////////////////////////////////////////////////////////
    private void drawDebug()
    {
        List<string> debugInfo = new List<string>();
        if (getMouseToMapCoord().x >= 0 && getMouseToMapCoord().x < mapSize.x && getMouseToMapCoord().y >= 0 && getMouseToMapCoord().y < mapSize.y)
        {
            BoardCollection boardHoverover = map[getMouseToMapCoord().x, getMouseToMapCoord().y];
            debugInfo.Add("Mouse Rel Pos :" + Input.mousePosition.ToString());
            debugInfo.Add("Mouse Rel Pos :" + getMouseToMapCoord().ToString());
            debugInfo.Add("Biome ID : " + boardHoverover.BiomeID.ToString());
            debugInfo.Add("Connected To Other Biome : " + boardHoverover.ConnectedToOther.ToString());
            debugInfo.Add("Edge of the world : " + boardHoverover.OuterShell.ToString());
            debugInfo.Add("Is Entrance : " + boardHoverover.DoorWay.ToString());
            debugInfo.Add("Wall top : " + boardHoverover.TopWall.ToString());
            debugInfo.Add("Wall bot: " + boardHoverover.BottomWall.ToString());
            debugInfo.Add("Wall left: " + boardHoverover.LeftWall.ToString());
            debugInfo.Add("Wall right: " + boardHoverover.RightWall.ToString());
            debugInfo.Add("Type: " + boardHoverover.RType.ToString());
            debugInfo.Add("Orientation: " + boardHoverover.OrType.ToString());
            debugInfo.Add("Walls : " + boardHoverover.getWallCount().ToString());
        }
        for (int i = 0; i < debugInfo.Count; i++)
        {
            GUI.Label(new Rect((int)Input.mousePosition.x, Screen.height - (int)Input.mousePosition.y + 20 * i, 200, 200), "Mouse Rel Pos :" + debugInfo[i], debugStyle);
        }
        GUI.Label(new Rect(0, 0, 1000, 20), "Connecting Biome 1 ID : " + bio1, debugStyle);
        for (int i = 0; i < biomeActions.connections.Count; i++)
        {
            int[] con = biomeActions.connections[i];
            GUI.Label(new Rect(0, 20 * (1 + i), 1000, 20), "Biome :" + con[0] + " connected to biome :" + con[1], debugStyle);
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