using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class PathFinding : MonoBehaviour
{
    int gridSizeX = 50;
    int gridSizeY = 30;

    float cellSize = 2;

    PathFindingNode[,] pathFindingNodes;

    PathFindingNode startNode;

    List<PathFindingNode> nodesToCheck = new List<PathFindingNode>();
    List<PathFindingNode> nodesChecked = new List<PathFindingNode>();

    List<Vector2> aiPath = new List<Vector2>();


    Vector3 startPosDebug = new Vector3(1000, 0, 0);
    Vector3 destPosDebug = new Vector3(1000, 0, 0);

    public bool isDebugActiveForCar = false;

    // Start is called before the first frame update
    void Start()
    {
        CreateGrid();
        FindPath(new Vector2(12, 5));
    }

    void CreateGrid()
    {
        pathFindingNodes = new PathFindingNode[gridSizeX, gridSizeY];

        for (int x = 0; x < gridSizeX; x++)
            for (int y = 0; y < gridSizeY; y++)
            {
                pathFindingNodes[x, y] = new PathFindingNode(new Vector2Int(x, y));

                Vector3 worldPos = ConvertGridPosToWorldPos(pathFindingNodes[x, y]);

                Collider2D collider2D = Physics2D.OverlapCircle(worldPos, cellSize / 2.0f);

                if (collider2D != null)
                {
                    if (collider2D.transform.root.CompareTag("AI"))
                        continue;
                    if (collider2D.transform.root.CompareTag("Player"))
                        continue;
                    if (collider2D.transform.root.CompareTag("Checkpoint"))
                        continue;

                    pathFindingNodes[x, y].isObstacle = true;
                }
            }

        for (int x = 0; x < gridSizeX; x++)
            for (int y = 0; y < gridSizeY; y++)
            {
                if (y - 1 >= 0)
                {
                    if (!pathFindingNodes[x, y - 1].isObstacle)
                        pathFindingNodes[x, y].neighbourCells.Add(pathFindingNodes[x, y - 1]);
                }

                if (y + 1 <= gridSizeY - 1)
                {
                    if (!pathFindingNodes[x, y + 1].isObstacle)
                        pathFindingNodes[x, y].neighbourCells.Add(pathFindingNodes[x, y + 1]);
                }

                if (x - 1 >= 0)
                {
                    if (!pathFindingNodes[x - 1, y].isObstacle)
                        pathFindingNodes[x, y].neighbourCells.Add(pathFindingNodes[x - 1, y]);
                }

                if (x + 1 <= gridSizeX - 1)
                {
                    if (!pathFindingNodes[x + 1, y].isObstacle)
                        pathFindingNodes[x, y].neighbourCells.Add(pathFindingNodes[x + 1, y]);
                }
            }
    }

    private void Reset()
    {
        nodesToCheck.Clear();
        nodesChecked.Clear();
        aiPath.Clear();

        for (int x = 0; x < gridSizeX; x++)
            for (int y = 0; y < gridSizeY; y++)
                pathFindingNodes[x, y].Reset();
    }

    public List<Vector2> FindPath(Vector2 destination)
    {
        if (pathFindingNodes == null)
            return null;

        Reset();

        Vector2Int destinationGridPoint = ConvertWorldPosToGridPos(destination);
        Vector2Int curPositionGridPoint = ConvertWorldPosToGridPos(transform.position);

        destPosDebug = destination;

        startNode = GetNodeFromPoint(curPositionGridPoint);

        startPosDebug = ConvertGridPosToWorldPos(startNode);

        PathFindingNode curNode = startNode;

        bool isDoneFindingPath = false;
        int pickOrder = 1;

        while (!isDoneFindingPath)
        {
            nodesToCheck.Remove(curNode);

            curNode.pickOrder = pickOrder;

            pickOrder++;

            nodesChecked.Add(curNode);

            if (curNode.gridPos == destinationGridPoint)
            {
                isDoneFindingPath = true;
                break;
            }

            CalculateCostsForNodeAndNeighbours(curNode, curPositionGridPoint, destinationGridPoint);

            foreach (PathFindingNode neighbourNode in curNode.neighbourCells)
            {
                if (nodesChecked.Contains(neighbourNode))
                    continue;

                if (nodesToCheck.Contains(neighbourNode))
                    continue;

                nodesToCheck.Add(neighbourNode);
            }

            nodesToCheck = nodesToCheck.OrderBy(x => x.fTotalCost).ThenBy(x => x.hDistanceFromGoalCost).ToList();

            if (nodesToCheck.Count == 0)
            {
                Debug.LogWarning($"No nodes left to check, there is no solution");
                return null;
            }
            else
            {
                curNode = nodesToCheck[0];
            }
        }

        aiPath = CreatePathForAI(curPositionGridPoint);

        return aiPath;
    }

    List<Vector2> CreatePathForAI(Vector2Int curPositionGridPoint)
    {
        List<Vector2> resultAIPath = new List<Vector2>();
        List<PathFindingNode> aiPath = new List<PathFindingNode>();

        nodesChecked.Reverse();

        bool isPathCreated = false;

        PathFindingNode curNode = nodesChecked[0];

        aiPath.Add(curNode);

        int attempts = 0;

        while (!isPathCreated)
        {
            curNode.neighbourCells = curNode.neighbourCells.OrderBy(x => x.pickOrder).ToList();

            foreach (PathFindingNode node in curNode.neighbourCells)
            {
                if (!aiPath.Contains(node) && nodesChecked.Contains(node))
                {
                    aiPath.Add(node);
                    curNode = node;

                    break;
                }
            }

            if (curNode == startNode)
                isPathCreated = true;

            if (attempts > 1000)
            {
                Debug.LogWarning("CreatePathForAI failed after too many attempts");
                break;
            }
            attempts++;
        }

        foreach (PathFindingNode node in aiPath)
        {
            resultAIPath.Add(ConvertGridPosToWorldPos(node));
        }

        resultAIPath.Reverse();

        return resultAIPath;
    }

    void CalculateCostsForNodeAndNeighbours(PathFindingNode node, Vector2Int aiPos, Vector2Int aiDest)
    {
        node.CalculateCostsForNode(aiPos, aiDest);

        foreach (PathFindingNode neighbourNode in node.neighbourCells)
        {
            neighbourNode.CalculateCostsForNode(aiPos, aiDest);
        }
    }

    PathFindingNode GetNodeFromPoint(Vector2Int gridPoint)
    {
        if (gridPoint.x < 0)
            return null;
        if (gridPoint.x > gridSizeX - 1)
            return null;
        if (gridPoint.y < 0)
            return null;
        if (gridPoint.y > gridSizeY - 1)
            return null;

        return pathFindingNodes[gridPoint.x, gridPoint.y];
    }

    Vector2Int ConvertWorldPosToGridPos(Vector2 pos)
    {
        Vector2Int gridPoint = new Vector2Int(Mathf.RoundToInt(pos.x / cellSize + gridSizeX / 2.0f), Mathf.RoundToInt(pos.y / cellSize + gridSizeY / 2.0f));

        return gridPoint;
    }

    Vector3 ConvertGridPosToWorldPos(PathFindingNode pathFindingNode)
    {
        return new Vector3(pathFindingNode.gridPos.x * cellSize - (gridSizeX * cellSize) / 2.0f, pathFindingNode.gridPos.y * cellSize - (gridSizeY * cellSize) / 2.0f, 0);
    }

    void OnDrawGizmos()
    {
        if (pathFindingNodes == null)
            return;

        if (!isDebugActiveForCar)
            return;

        for (int x = 0; x < gridSizeX; x++)
            for (int y = 0; y < gridSizeY; y++)
            {
                if (pathFindingNodes[x, y].isObstacle)
                    Gizmos.color = Color.red;
                else Gizmos.color = Color.green;

                Gizmos.DrawWireCube(ConvertGridPosToWorldPos(pathFindingNodes[x, y]), new Vector3(cellSize, cellSize, cellSize));
            }

        foreach (PathFindingNode checkedNode in nodesChecked)
        {
            Gizmos.color = Color.green;
            //Gizmos.DrawSphere(ConvertGridPosToWorldPos(checkedNode), 1f);

#if UNITY_EDITOR

            Vector3 labelPosition = ConvertGridPosToWorldPos(checkedNode);

            labelPosition.z = -1;

            GUIStyle style = new GUIStyle();

            style.normal.textColor = Color.green;
            Handles.Label(labelPosition + new Vector3(-0.6f, 1f, 0), $"{checkedNode.hDistanceFromGoalCost}", style);

            style.normal.textColor = Color.red;
            Handles.Label(labelPosition + new Vector3(0.5f, 1f, 0), $"{checkedNode.gDistanceFromStartCost}", style);

            style.normal.textColor = Color.yellow;
            Handles.Label(labelPosition + new Vector3(0.5f, -0.5f, 0), $"{checkedNode.pickOrder}", style);

            style.normal.textColor = Color.white;
            Handles.Label(labelPosition + new Vector3(0, 0.2f, 0), $"{checkedNode.fTotalCost}", style);
#endif
        }

        foreach (PathFindingNode nodeToCheck in nodesToCheck)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(ConvertGridPosToWorldPos(nodeToCheck), 1f);
        }

        Vector3 lastAIPoint = Vector3.zero;
        bool isFirstStep = true;

        Gizmos.color = Color.black;

        foreach (Vector2 point in aiPath)
        {
            if (!isFirstStep)
                Gizmos.DrawLine(lastAIPoint, point);

            lastAIPoint = point;

            isFirstStep = false;
        }

        Gizmos.color = Color.black;
        Gizmos.DrawSphere(startPosDebug, 1f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(destPosDebug, 1f);
    }
}
