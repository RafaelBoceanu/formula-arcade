using System;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingNode
{
    public Vector2Int gridPos;

    public List<PathFindingNode> neighbourCells = new List<PathFindingNode>();

    public bool isObstacle = false;

    public int gDistanceFromStartCost = 0;
    public int hDistanceFromGoalCost = 0;
    public int fTotalCost = 0;
    public int pickOrder = 0;

    bool isCostCalculated = false;

    public PathFindingNode(Vector2Int gridPos_)
    {
        gridPos = gridPos_;
    }

    public void CalculateCostsForNode(Vector2Int aiPos, Vector2Int aiDest)
    {
        if (isCostCalculated)
            return;

        gDistanceFromStartCost = Mathf.Abs(gridPos.x - aiPos.x) + Mathf.Abs(gridPos.y - aiPos.y);

        hDistanceFromGoalCost = Mathf.Abs(gridPos.x - aiDest.x) + Mathf.Abs(gridPos.y - aiDest.y);

        fTotalCost = gDistanceFromStartCost + hDistanceFromGoalCost;
    }

    public void Reset()
    {
        isCostCalculated = false;
        pickOrder = 0;
        gDistanceFromStartCost = 0;
        hDistanceFromGoalCost = 0;
        fTotalCost = 0;
    }
}
