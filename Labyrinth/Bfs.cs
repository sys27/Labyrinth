using System;
using System.Collections.Generic;

namespace Labyrinth
{
    public class Bfs
    {
        public (IEnumerable<Cell>, bool) TryFind(Maze maze, Cell from, Cell to, bool exploring = false)
        {
            var queue = new Queue<PathNode>();
            var visited = new HashSet<Cell>(maze.Rows * maze.Columns);

            queue.Enqueue(new PathNode(from));

            while (queue.Count > 0)
            {
                var pathNode = queue.Dequeue();
                var cell = pathNode.Cell;

                if (cell.Equals(to))
                    return (pathNode.GetPath(), true);

                if (!visited.Contains(cell))
                {
                    var neighbors = maze.GetNeighbors(cell.Position);
                    foreach (var neighbor in neighbors)
                    {
                        // TODO:
                        if (exploring && neighbor.IsControlRoom())
                            continue;

                        var newPath = pathNode.Append(neighbor);
                        queue.Enqueue(newPath);
                    }
                }

                visited.Add(cell);
            }

            return (null, false);
        }

        public IEnumerable<Cell> Find(Maze maze, Cell from, Cell to, bool exploring = false)
        {
            var (path, ok) = TryFind(maze, from, to, exploring);
            if (ok)
                return path;

            throw new Exception("No solution.");
        }

        private class PathNode
        {
            public PathNode(Cell cell) : this(cell, null)
            {
            }

            private PathNode(Cell cell, PathNode parent)
            {
                Cell = cell;
                Parent = parent;
            }

            public PathNode Append(Cell cell)
                => new PathNode(cell, this);

            public IEnumerable<Cell> GetPath()
            {
                var path = new LinkedList<Cell>();

                var current = this;
                do
                {
                    path.AddFirst(current.Cell);

                    current = current.Parent;
                } while (current != null);

                return path;
            }

            public Cell Cell { get; }
            public PathNode Parent { get; }
        }
    }
}