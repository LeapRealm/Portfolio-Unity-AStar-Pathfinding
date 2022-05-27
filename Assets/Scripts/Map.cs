using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public enum TileType
{
    Grass,
    Wall,
    Dest
}

public class Map : MonoBehaviour
{
    public Vector2Int mapSize;
    public TileType[,] mapData;
    public TileBase[] tiles;
    public Tilemap tileMap;
    public Player player;
    public Vector2Int destPos;
    public Camera mainCamera;

    private void Start()
    {
        mapData = new TileType[mapSize.y, mapSize.x];
        tileMap = GetComponent<Tilemap>();
        destPos = new Vector2Int(1, 1);
        player = FindObjectOfType<Player>();
        mainCamera = Camera.main;

        GenerateMapData();
        RenderTiles();
    }

    private void GenerateMapData()
    {
        for (int y = 0; y < mapSize.y; y++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                if (x == 0 || x == mapSize.x - 1 || y == 0 ||  y == mapSize.y - 1)
                    mapData[y, x] = TileType.Wall;
                else
                    mapData[y, x] = TileType.Grass;
            }
        }
        mapData[destPos.y, destPos.x] = TileType.Dest;
    }
    
    private void RenderTiles()
    {
        Vector3Int position = new Vector3Int();
        
        for (int y = 0; y < mapSize.y; y++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                position.x = x;
                position.y = y;
                position.z = 0;
            
                tileMap.SetTile(position, tiles[(int)mapData[y, x]]);
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            if (mousePosition.x >= 1 && mousePosition.x < mapSize.x - 1 && 
                mousePosition.y >= 1 && mousePosition.y < mapSize.y - 1)
            {
                Vector3Int clickedPos = new Vector3Int((int)mousePosition.x, (int)mousePosition.y, 0);
                TileBase clickedTile = tileMap.GetTile<TileBase>(clickedPos);

                if (clickedTile == tiles[(int)TileType.Grass])
                    mapData[clickedPos.y, clickedPos.x] = TileType.Wall;
                else
                    mapData[clickedPos.y, clickedPos.x] = TileType.Grass;
                
                tileMap.SetTile(clickedPos, tiles[(int)mapData[clickedPos.y, clickedPos.x]]);
            }
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            if (mousePosition.x >= 1 && mousePosition.x < mapSize.x - 1 && 
                mousePosition.y >= 1 && mousePosition.y < mapSize.y - 1)
            {
                Vector3Int clickedPos = new Vector3Int((int)mousePosition.x, (int)mousePosition.y, 0);
                TileBase clickedTile = tileMap.GetTile<TileBase>(clickedPos);
                
                if (clickedTile != tiles[(int)TileType.Wall])
                {
                    mapData[destPos.y, destPos.x] = TileType.Grass;
                    tileMap.SetTile((Vector3Int)destPos, tiles[(int)mapData[destPos.y, destPos.x]]);
                    
                    destPos = new Vector2Int(clickedPos.x, clickedPos.y);
                    
                    mapData[destPos.y, destPos.x] = TileType.Dest;
                    tileMap.SetTile((Vector3Int)destPos, tiles[(int)mapData[destPos.y, destPos.x]]);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
}