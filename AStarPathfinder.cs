using Microsoft.Xna.Framework;
using System.Collections.Generic;

public class AStarPathfinder
{
    private int width, height;
    private float cellSize;
    private System.Func<Vector2, float, bool> isCollision;

    public AStarPathfinder(int width, int height, float cellSize, System.Func<Vector2, float, bool> isCollision)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.isCollision = isCollision;
    }

    private class Node
    {
        public Vector2 Pos;
        public float G, H;
        public Node Parent;
        public Node(Vector2 pos, float g, float h, Node parent)
        {
            Pos = pos; G = g; H = h; Parent = parent;
        }
        public float F => G + H;
    }

    public List<Vector2> FindPath(Vector2 start, Vector2 goal, float radius)
    {
        var open = new SortedSet<(float, int, Node)>();
        var closed = new HashSet<(int, int)>();
        var startCell = ToCell(start);
        var goalCell = ToCell(goal);
        int id = 0;
        open.Add((0, id++, new Node(start, 0, Heuristic(start, goal), null)));
        int maxSteps = 150; // ограничение на длину пути
        int steps = 0;
        var watch = System.Diagnostics.Stopwatch.StartNew();
        long maxMs = 20; // максимум 20 мс на поиск
        while (open.Count > 0)
        {
            if (++steps > maxSteps || watch.ElapsedMilliseconds > maxMs)
                return null;
            var current = open.Min;
            open.Remove(current);
            var node = current.Item3;
            var cell = ToCell(node.Pos);
            if (cell == goalCell)
                return ReconstructPath(node);
            closed.Add(cell);
            foreach (var neighbor in GetNeighbors(node.Pos))
            {
                var nCell = ToCell(neighbor);
                if (closed.Contains(nCell) || isCollision(neighbor, radius)) continue;
                float g = node.G + Vector2.Distance(node.Pos, neighbor);
                float h = Heuristic(neighbor, goal);
                open.Add((g + h, id++, new Node(neighbor, g, h, node)));
            }
        }
        return null;
    }

    private List<Vector2> ReconstructPath(Node node)
    {
        var path = new List<Vector2>();
        while (node.Parent != null)
        {
            path.Add(node.Pos);
            node = node.Parent;
        }
        path.Reverse();
        return path;
    }

    private float Heuristic(Vector2 a, Vector2 b) => Vector2.Distance(a, b);

    private (int, int) ToCell(Vector2 pos) => ((int)(pos.X / cellSize), (int)(pos.Y / cellSize));

    private IEnumerable<Vector2> GetNeighbors(Vector2 pos)
    {
        float[] dx = { -cellSize, 0, cellSize };
        float[] dy = { -cellSize, 0, cellSize };
        foreach (var x in dx)
        foreach (var y in dy)
        {
            if (x == 0 && y == 0) continue;
            yield return new Vector2(pos.X + x, pos.Y + y);
        }
    }
}
