using System;
using System.IO;
using System.Linq;

namespace Labyrinth
{
    public class Game
    {
        private readonly TextReader input;
        private readonly TextWriter output;

        private int rows;
        private int columns;
        private (Cell Cell, bool IsVisible)[][] cells;

        private Position player;

        private bool controlRoomVisited;
        private bool startPositionVisited;

        public Game(TextReader input, TextWriter output)
        {
            this.input = input;
            this.output = output;
        }

        public void FromString(int rows, int columns, string maze)
        {
            this.rows = rows;
            this.columns = columns;

            var strings = maze.Split('\n');

            cells = new (Cell, bool)[rows][];
            for (var row = 0; row < rows; row++)
            {
                cells[row] = new (Cell, bool)[columns];

                for (var column = 0; column < columns; column++)
                {
                    var cellKind = (CellKind)strings[row][column];

                    cells[row][column] = (new Cell(cellKind, column, row), false);

                    if (cellKind == CellKind.StartPosition)
                        player = new Position(column, row);
                }
            }

            UpdateVisibility();

            controlRoomVisited = false;
            startPositionVisited = false;
        }

        public void Start()
        {
            output.WriteLine($"{rows} {columns} {1000}");
            output.Flush();

            while (!controlRoomVisited || !startPositionVisited)
            {
                PrintMaze();

                PlayerTurn();

                UpdateVisibility();
            }
        }

        private void PrintMaze()
        {
            output.WriteLine($"{player.Y} {player.X}");

            for (var row = 0; row < rows; row++)
            {
                for (var column = 0; column < columns; column++)
                {
                    var tuple = cells[row][column];

                    output.Write(tuple.IsVisible ? (char)tuple.Cell.Type : '?');
                }

                output.WriteLine();
            }

            output.Flush();
        }

        private void PlayerTurn()
        {
            var direction = input.ReadLine();
            var position = direction switch
            {
                "UP" => player.Top(),
                "DOWN" => player.Down(),
                "LEFT" => player.Left(),
                "RIGHT" => player.Right(),
                _ => throw new InvalidOperationException(),
            };
            var cell = cells[position.Y][position.X].Cell;

            if (cell.IsWall())
                throw new InvalidOperationException("Wall.");

            if (cell.IsControlRoom())
                controlRoomVisited = true;

            if (controlRoomVisited && cell.IsStartPosition())
                startPositionVisited = true;

            player = cell.Position;
        }

        private void UpdateVisibility()
        {
            for (var row = 0; row < rows; row++)
            for (var column = 0; column < columns; column++)
                if (IsVisible(player, new Position(column, row)))
                    cells[row][column].IsVisible = true;
        }

        private static bool IsVisible(Position player, Position cell)
            => Math.Abs(player.X - cell.X) <= 2 && Math.Abs(player.Y - cell.Y) <= 2;
    }
}