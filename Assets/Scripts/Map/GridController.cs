using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CustomTileData
{
    public bool IsOccupied = false;
    public Vector3Int GridPosition;
}

public class GridController : Singleton<GridController>
{

    //Map size
    public int MapSizeX, MapSizeY;

    //------------TILES------------//
    //TODO: All tiles should be scriptable objects - to switch textures 


    //Default testing tile
    public Tile DefaultTile;

    //------------DATA------------//

    // Ground tiles data
    public CustomTileData[,] TilesData;

    public Pathfinding Pathfinding;


    //------------TILEMAPS------------//

    // Unity grid of tilemap
    private Grid Grid;

    // Ground tilemap 
    private Tilemap TilemapGround;



    // Start is called before the first frame update
    void Start()
    {
        this.Grid = GameObject.Find("Grid").GetComponent<Grid>();
        this.TilemapGround = GameObject.Find("Grid/Ground").GetComponent<Tilemap>();
        this.Pathfinding = new Pathfinding(this);

        this.generateMap(MapSizeX, MapSizeY);
    }


    /// <summary>
    /// Generate isometric map
    /// </summary>
    /// <param name="sizeX"></param>
    /// <param name="sizeY"></param>
    private void generateMap(int sizeX, int sizeY)
    {
        TilesData = new CustomTileData[sizeX, sizeY];

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                var tiledata = new CustomTileData();
                var position = new Vector3Int(x, y, 0);
                tiledata.GridPosition = position;
                TilesData[x, y] = tiledata;
                TilemapGround.SetTile(position, DefaultTile);
            }
        }

    }

    /// <summary>
    /// Get neighbour data tiles
    /// </summary>
    /// <param name="tilePosition"></param>
    /// <returns></returns>
    public List<CustomTileData> GetTileNeighboursData(Vector3Int tilePosition, bool diagonal = false)
    {

        var dataList = new List<CustomTileData>(diagonal ? 8 : 4);
        var leftTile = this.GetTileData(tilePosition + Vector3Int.left);
        var rightTile = this.GetTileData(tilePosition + Vector3Int.right);
        var topTile = this.GetTileData(tilePosition + Vector3Int.up);
        var bottom = this.GetTileData(tilePosition + Vector3Int.down);

        if (leftTile != null)
            dataList.Add(leftTile);

        if (rightTile != null)
            dataList.Add(rightTile);

        if (topTile != null)
            dataList.Add(topTile);

        if (bottom != null)
            dataList.Add(bottom);

        if (diagonal)
        {
            var leftTop = this.GetTileData(tilePosition + new Vector3Int(-1, -1, 0));
            var rightTop = this.GetTileData(tilePosition + new Vector3Int(1, -1, 0));
            var leftBottom = this.GetTileData(tilePosition + new Vector3Int(-1, 1, 0));
            var rightBottom = this.GetTileData(tilePosition + new Vector3Int(1, 1, 0));

            if (leftTop != null)
                dataList.Add(leftTop);

            if (rightTop != null)
                dataList.Add(rightTop);

            if (leftBottom != null)
                dataList.Add(leftBottom);

            if (rightBottom != null)
                dataList.Add(rightBottom);
        }


        return dataList;

    }


    /// <summary>
    /// Returns tile data with bounds checking (null if out of bounds)
    /// </summary>
    /// <param name="tilePosition"></param>
    /// <returns></returns>
    public CustomTileData GetTileData(Vector3Int tilePosition)
    {
        if (tilePosition.x >= 0 && tilePosition.x < MapSizeX)
        {
            if (tilePosition.y >= 0 && tilePosition.y < MapSizeY)
            {
                return TilesData[tilePosition.x, tilePosition.y];
            }
        }

        return null;
    }

    /// <summary>
    /// Transforms screen coords to cell(tile) position
    /// </summary>
    /// <param name="screenPosition"></param>
    /// <returns></returns>
    public Vector3Int ScreenToTile(Vector2 screenPosition)
    {
        var worldPos = this.ScreenToWorld(screenPosition);
        return this.WorldToCell(worldPos);
    }

    /// <summary>
    /// Uses main camera to transform screen coords to world coords
    /// </summary>
    /// <param name="screenPosition"></param>
    /// <returns></returns>
    public Vector3 ScreenToWorld(Vector2 screenPosition)
    {
        var pos = Camera.main.ScreenToWorldPoint(screenPosition);
        pos.z = 0;
        return pos;
    }


    /// <summary>
    /// Transform world position to cell(tile) position
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns> 
    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        return Grid.WorldToCell(worldPosition);
    }


    /// <summary>
    /// cell(tile) position to world position 
    /// </summary>
    /// <param name="cellPosition"></param>
    /// <returns></returns>
    public Vector3 CellToWorld(Vector3Int cellPosition)
    {
        return Grid.CellToWorld(cellPosition);
    }

    public CustomTileData GetRandomWalkableTile()
    {
        var x = Random.Range(0, this.MapSizeX);
        var y = Random.Range(0, this.MapSizeY);

        var tileData = this.GetTileData(new Vector3Int(x, y, 0));
        if (tileData.IsOccupied == false)
        {
            return tileData;
        }
        else
        {
            return GetRandomWalkableTile();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
