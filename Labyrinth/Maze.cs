using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Labyrinth
{
    public class Maze
    {
        private Cell[][] cells;
        private Position player;
        private Position start;
        private Position? controlRoom;
        private HashSet<Position> unreachablePositions;

        private static Position[] offsets =
        {
            new Position(1, 0),
            new Position(0, 1),
            new Position(-1, 0),
            new Position(0, -1),
        };

        private Maze(Cell[][] cells, int rows, int columns)
        {
            this.cells = cells;
            this.Rows = rows;
            this.Columns = columns;
            this.unreachablePositions = new HashSet<Position>();
        }

        public static Maze FromStream(TextReader input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            var inputs = input.ReadLine().Split(' ');
            var rows = int.Parse(inputs[0]);
            var columns = int.Parse(inputs[1]);
            var rounds = int.Parse(inputs[2]);

            var cells = new Cell[rows][];
            for (var row = 0; row < rows; row++)
                cells[row] = new Cell[columns];

            return new Maze(cells, rows, columns);
        }

        public void Refresh(TextReader input)
        {
            var inputs = input.ReadLine().Split(' ');
            var playerRow = int.Parse(inputs[0]);
            var playerColumn = int.Parse(inputs[1]);
            player = new Position(playerColumn, playerRow);

            for (var row = 0; row < Rows; row++)
            {
                for (var column = 0; column < Columns; column++)
                {
                    var cellKind = (CellKind)input.Read();
                    var cell = new Cell(cellKind, column, row);

                    cells[row][column] = cell;

                    if (cellKind == CellKind.ControlRoom)
                        controlRoom = new Position(column, row);
                    else if (cellKind == CellKind.StartPosition)
                        start = new Position(column, row);
                }

                // skip new line
                input.Read();
            }
            
            unreachablePositions.Clear();
        }

        public Cell GetCellToExplore()
        {
            for (var row = 0; row < Rows; row++)
            {
                for (var column = 0; column < Columns; column++)
                {
                    var cell = cells[row][column];
                    if (!cell.IsPass() && !cell.IsStartPosition())
                        continue;

                    if (unreachablePositions.Contains(cell.Position))
                        continue;

                    var position = cell.Position;

                    var cellsToExplore = new List<Cell>(4);

                    foreach (var offset in offsets)
                    {
                        var nextPosition = new Position(position.X + offset.X, position.Y + offset.Y);
                        if (!IsPositionValid(nextPosition))
                            continue;

                        cellsToExplore.Add(this[nextPosition]);
                    }

                    var hasUnknown = cellsToExplore.Any(x => x.IsUnknown());
                    if (hasUnknown)
                        return cell;
                }
            }

            return null;
        }

        public IEnumerable<Cell> GetNeighbors(Position position)
        {
            if (position.X < 0 || position.X >= Columns)
                throw new ArgumentOutOfRangeException(nameof(position.X));
            if (position.Y < 0 || position.Y >= Rows)
                throw new ArgumentOutOfRangeException(nameof(position.Y));

            var neighbors = new List<Cell>(4);

            foreach (var offset in offsets)
            {
                var nextPosition = new Position(position.X + offset.X, position.Y + offset.Y);
                var nextCell = GetNeighbor(nextPosition);
                if (nextCell != null)
                    neighbors.Add(nextCell);
            }

            return neighbors;
        }

        private Cell GetNeighbor(Position position)
        {
            if (IsPositionValid(position))
            {
                var cell = this[position];
                if (cell.IsPass() ||
                    cell.IsControlRoom() ||
                    cell.IsStartPosition())
                    return cell;
            }

            return null;
        }

        private bool IsPositionValid(Position position)
            => position.X >= 0 &&
               position.X < Columns &&
               position.Y >= 0 &&
               position.Y < Rows;

        public void MarkAsUnreachable(Position position)
            => unreachablePositions.Add(position);

        public Cell this[Position position] => cells[position.Y][position.X];

        public int Columns { get; }
        public int Rows { get; }

        public Cell Player => this[player];
        public Cell Start => this[start];
        public Cell ControlRoom => this[controlRoom.GetValueOrDefault()];
    }
}