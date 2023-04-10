using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cultist : MonoBehaviour
{

    public int WalkSpeed = 2;
    public CustomTileData CurrentTile;

    public Queue<CustomTileData> CurrentPath;

    public CustomTileData FinishTile;

    public CustomTileData NextTile;
    public Vector3 NextTileWorldPosition;

    public void CalculatePath(CustomTileData currentTile, CustomTileData finishTile)
    {
        CurrentPath = GridController.Instance.Pathfinding.AStar(currentTile, finishTile, true);
    }

    // Start is called before the first frame update
    void Start()
    {
        var gridController = GridController.Instance;

        var gridTilePosition = gridController.WorldToCell(this.transform.position);
        this.CurrentTile = gridController.GetTileData(gridTilePosition);

        this.FinishTile = gridController.GetRandomWalkableTile();

        this.CalculatePath(this.CurrentTile, this.FinishTile);

        this.NextTile = CurrentPath.Dequeue();
        this.NextTileWorldPosition = gridController.CellToWorld(this.NextTile.GridPosition);

    }

    // Update is called once per frame
    void Update()
    {
        if (this.NextTileWorldPosition != Vector3.zero)
        {
            if (this.transform.position == this.NextTileWorldPosition)
            {
                if (CurrentPath.Count > 0)
                {
                    this.CurrentTile = this.NextTile;
                    
                    this.NextTile = CurrentPath.Dequeue();
                    this.NextTileWorldPosition = GridController.Instance.CellToWorld(this.NextTile.GridPosition);
                }
                else
                {
                    this.CurrentTile = this.NextTile;
                    this.FinishTile = GridController.Instance.GetRandomWalkableTile();
                    this.CalculatePath(this.CurrentTile, this.FinishTile); 
                    Debug.Log("Making new path");
                    Debug.Log(CurrentPath.Count);               
                }

            }

            this.transform.position = Vector3.MoveTowards(this.transform.position, this.NextTileWorldPosition, Time.deltaTime * WalkSpeed);
        }
    }
}
