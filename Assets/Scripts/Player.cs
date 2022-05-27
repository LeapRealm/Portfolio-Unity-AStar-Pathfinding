using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector2Int nextPos;
    public Vector2Int destPos;
    public Map map;
    public float speed = 5f;
    public LineRenderer lineRenderer;
    public GameObject noWay;
    
    private void Start()
    { 
        map = FindObjectOfType<Map>();
        lineRenderer = GetComponent<LineRenderer>();
        transform.position = new Vector3(1, 1, 0);
        nextPos = new Vector2Int(1, 1);
    }

    private void Update()
    {
        UpdateMoving();
    }

    private void UpdateMoving()
    {
        Vector3 moveDir = (Vector3Int)nextPos - transform.position;
    
        float distance = moveDir.magnitude;
        if (distance < speed * Time.deltaTime)
        {
            transform.position = (Vector3Int)nextPos;
            FindNextPos();
        }
        else
        {
            transform.position += moveDir.normalized * speed * Time.deltaTime;
        }
    }
    
    private void FindNextPos()
    {
        List<Vector2Int> paths = FindPath(nextPos, map.destPos);

        if (paths.Count < 2)
        {
            lineRenderer.enabled = false;
            return;
        }
        
        if (paths[0].x == 0 && paths[0].y == 0 && paths[1].x == map.destPos.x && paths[1].y == map.destPos.y)
        {
            lineRenderer.enabled = false;
            noWay.SetActive(true);
            return;
        }

        lineRenderer.enabled = true;
        noWay.SetActive(false);
        lineRenderer.positionCount = paths.Count - 1;
        for (int i = 0, j = 1; i < paths.Count - 1; i++, j++)
            lineRenderer.SetPosition(i, new Vector3(paths[j].x + 0.5f, paths[j].y + 0.5f, -5));

        nextPos = paths[1];
    }
    
    public struct Node : IComparable<Node>
    {
        public int f;
        public int g;
        public int y;
        public int x;

        public int CompareTo(Node other)
        {
            if (f == other.f)
                return 0;
            return f < other.f ? 1 : -1;
        }
    }

    #region A* PathFinding

    // Up Left Down Right
    private int[] deltaY = new int[] { 1, 0, -1, 0 };
    private int[] deltaX = new int[] { 0, -1, 0, 1 };
    private int[] cost = new int[] { 10, 10, 10, 10 };

    public List<Vector2Int> FindPath(Vector2Int currPos, Vector2Int destPos)
    {
        bool[,] closed = new bool[map.mapSize.y, map.mapSize.x];

        int[,] open = new int[map.mapSize.y, map.mapSize.x];
        for (int y = 0; y < map.mapSize.y; y++)
            for (int x = 0; x < map.mapSize.x; x++)
                open[y, x] = int.MaxValue;

        Vector2Int[,] parent = new Vector2Int[map.mapSize.y, map.mapSize.x];

        PriorityQueue<Node> queue = new PriorityQueue<Node>();

        open[currPos.y, currPos.x] = 10 * (Math.Abs(destPos.y - currPos.y) + Math.Abs(destPos.x - currPos.x));
        queue.Push(new Node() { f = 10 * (Math.Abs(destPos.y - currPos.y) + Math.Abs(destPos.x - currPos.x)), g = 0, y = currPos.y, x = currPos.x });
        parent[currPos.y, currPos.x] = new Vector2Int(currPos.x, currPos.y);

        while (queue.Count > 0)
        {
            Node node = queue.Pop();
            if (closed[node.y, node.x])
                continue;

            closed[node.y, node.x] = true;
            if (node.y == destPos.y && node.x == destPos.x)
                break;

            for (int i = 0; i < deltaY.Length; i++)
            {
                Vector2Int next = new Vector2Int(node.x + deltaX[i], node.y + deltaY[i]);

                if (next.x < 0 || next.y < 0 || next.x >= map.mapSize.x || next.y >= map.mapSize.y)
                    continue;

                if (map.mapData[next.y, next.x] == TileType.Wall)
                    continue;
                
                if (closed[next.y, next.x])
                    continue;

                int g = node.g + cost[i];
                int h = 10 * ((destPos.y - next.y) * (destPos.y - next.y) + (destPos.x - next.x) * (destPos.x - next.x));
                if (open[next.y, next.x] < g + h)
                    continue;

                open[next.y, next.x] = g + h;
                queue.Push(new Node() { f = g + h, g = g, y = next.y, x = next.x });
                parent[next.y, next.x] = new Vector2Int(node.x, node.y);
            }
        }

        return CalcPathFromParent(parent, destPos);
    }

    private List<Vector2Int> CalcPathFromParent(Vector2Int[,] parent, Vector2Int dest)
    {
        List<Vector2Int> positions = new List<Vector2Int>();

        int y = dest.y;
        int x = dest.x;
        while (parent[y, x].y != y || parent[y, x].x != x)
        {
            positions.Add(new Vector2Int(x, y));
            Vector2Int pos = parent[y, x];
            y = pos.y;
            x = pos.x;
        }

        positions.Add(new Vector2Int(x, y));
        positions.Reverse();

        return positions;
    }

    #endregion
}