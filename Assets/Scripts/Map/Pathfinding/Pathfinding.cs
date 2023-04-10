using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private readonly GridController controller;

    public Pathfinding(GridController controller)
    {
        this.controller = controller;
    }

    public Queue<CustomTileData> FloodFill(CustomTileData start, CustomTileData goal, bool diagonal = false)
    {
        Dictionary<CustomTileData, CustomTileData> nextTileToGoal = new Dictionary<CustomTileData, CustomTileData>();
        Queue<CustomTileData> frontier = new Queue<CustomTileData>();
        List<CustomTileData> visited = new List<CustomTileData>();

        frontier.Enqueue(goal);

        while (frontier.Count > 0)
        {
            CustomTileData curTile = frontier.Dequeue();

            foreach (CustomTileData neighbor in controller.GetTileNeighboursData(curTile.GridPosition, diagonal))
            {
                if (visited.Contains(neighbor) == false && frontier.Contains(neighbor) == false)
                {
                    if (neighbor.IsOccupied == false)
                    {
                        frontier.Enqueue(neighbor);
                        nextTileToGoal[neighbor] = curTile;
                    }
                }
            }
            visited.Add(curTile);
        }

        if (visited.Contains(start) == false)
            return null;

        Queue<CustomTileData> path = new Queue<CustomTileData>();
        CustomTileData curPathTile = start;
        while (curPathTile != goal)
        {
            curPathTile = nextTileToGoal[curPathTile];
            path.Enqueue(curPathTile);
        }

        return path;
    }


    public Queue<CustomTileData> AStar(CustomTileData start, CustomTileData goal, bool diagonal = false)
    {
        Dictionary<CustomTileData, CustomTileData> NextTileToGoal = new Dictionary<CustomTileData, CustomTileData>();//Determines for each tile where you need to go to reach the goal. Key=Tile, Value=Direction to Goal
        Dictionary<CustomTileData, int> costToReachTile = new Dictionary<CustomTileData, int>();//Total Movement Cost to reach the tile

        PriorityQueue<CustomTileData> frontier = new PriorityQueue<CustomTileData>();
        frontier.Enqueue(goal, 0);
        costToReachTile[goal] = 0;

        while (frontier.Count > 0)
        {
            CustomTileData curTile = frontier.Dequeue();
            if (curTile == start)
                break;

            foreach (CustomTileData neighbor in controller.GetTileNeighboursData(curTile.GridPosition, diagonal))
            {
                int newCost = costToReachTile[curTile] + 0; //neighbor._Cost;
                if (costToReachTile.ContainsKey(neighbor) == false || newCost < costToReachTile[neighbor])
                {
                    if (neighbor.IsOccupied == false)
                    {
                        costToReachTile[neighbor] = newCost;
                        int priority = newCost + Distance(neighbor, start);
                        frontier.Enqueue(neighbor, priority);
                        NextTileToGoal[neighbor] = curTile;
                        // neighbor._Text = costToReachTile[neighbor].ToString();
                    }
                }
            }
        }

        //Get the Path

        //check if tile is reachable
        if (NextTileToGoal.ContainsKey(start) == false)
        {
            return null;
        }

        Queue<CustomTileData> path = new Queue<CustomTileData>();
        CustomTileData pathTile = start;
        while (goal != pathTile)
        {
            pathTile = NextTileToGoal[pathTile];
            path.Enqueue(pathTile);
        }
        return path;
    }

    //https://github.com/Squirrelius/Pathfinding_FloodfillToAstar/blob/main/Assets/Scripts
    /// <summary>
    /// Dijkstra algorithm for pathfinding
    /// </summary>
    /// <param name="start"></param>
    /// <param name="goal"></param>
    /// <returns></returns>
    public Queue<CustomTileData> Dijkstra(CustomTileData start, CustomTileData goal, bool diagonal = false)
    {
        Dictionary<CustomTileData, CustomTileData> NextTileToGoal = new Dictionary<CustomTileData, CustomTileData>();//Determines for each tile where you need to go to reach the goal. Key=Tile, Value=Direction to Goal
        Dictionary<CustomTileData, int> costToReachTile = new Dictionary<CustomTileData, int>();//Total Movement Cost to reach the tile


        PriorityQueue<CustomTileData> frontier = new PriorityQueue<CustomTileData>();
        frontier.Enqueue(goal, 0);
        costToReachTile[goal] = 0;

        while (frontier.Count > 0)
        {
            CustomTileData curTile = frontier.Dequeue();
            if (curTile == start)
                break;

            foreach (CustomTileData neighbor in controller.GetTileNeighboursData(curTile.GridPosition, diagonal))
            {
                int newCost = costToReachTile[curTile] + 0;  //neighbor._Cost;
                if (costToReachTile.ContainsKey(neighbor) == false || newCost < costToReachTile[neighbor])
                {
                    //if (neighbor._TileType != Tile.TileType.Wall)
                    if (neighbor.IsOccupied == false)
                    {
                        costToReachTile[neighbor] = newCost;
                        int priority = newCost;
                        frontier.Enqueue(neighbor, priority);
                        NextTileToGoal[neighbor] = curTile;
                        // neighbor._Text = costToReachTile[neighbor].ToString();
                    }
                }
            }
        }

        //Get the Path

        //check if tile is reachable
        if (NextTileToGoal.ContainsKey(start) == false)
        {
            return null;
        }

        Queue<CustomTileData> path = new Queue<CustomTileData>();
        CustomTileData pathTile = start;
        while (goal != pathTile)
        {
            pathTile = NextTileToGoal[pathTile];
            path.Enqueue(pathTile);
        }
        return path;
    }


    /// <summary>
    /// Determines the Manhatten Distance between two tiles. (=How many Tiles the player must move to reach it)
    /// </summary>
    /// <returns>Distance in amount of Tiles the player must move</returns>
    private int Distance(CustomTileData t1, CustomTileData t2)
    {
        return Mathf.Abs(t1.GridPosition.x - t2.GridPosition.x) + Mathf.Abs(t1.GridPosition.y - t2.GridPosition.y);
    }

}