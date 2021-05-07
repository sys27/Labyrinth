using System;
using System.Collections.Generic;
using System.IO;

namespace Labyrinth
{
    public class Solution
    {
        private readonly TextReader input;
        private readonly TextWriter output;

        private Maze maze;
        private readonly Bfs pathFinder;

        private State state;

        public Solution(TextReader input, TextWriter output)
        {
            this.input = input;
            this.output = output;

            this.pathFinder = new Bfs();
        }

        public void Start()
        {
            state = State.Exploring;

            maze = Maze.FromStream(input);

            var route = new Queue<Cell>();

            while (state != State.Finished)
            {
                maze.Refresh(input);

                if (state == State.Exploring)
                {
                    Explore();
                }

                if (state == State.BuildControlRoomRoute)
                {
                    route = BuildRoute(maze.ControlRoom);
                    if (route == null)
                        throw new Exception("No solution.");

                    state = State.GoingToControlRoom;
                }

                if (state == State.GoingToControlRoom)
                {
                    if (route.TryDequeue(out var nextStep))
                        GoTo(nextStep);
                    else
                        state = State.BuildStartPositionRoute;
                }

                if (state == State.BuildStartPositionRoute)
                {
                    route = BuildRoute(maze.Start);
                    if (route == null)
                        throw new Exception("No solution.");

                    state = State.GoingToStartPosition;
                }

                if (state == State.GoingToStartPosition)
                {
                    if (route.TryDequeue(out var nextStep))
                        GoTo(nextStep);
                    else
                        state = State.Finished;
                }
            }
        }

        private void Explore()
        {
            while (true)
            {
                var cell = maze.GetCellToExplore();
                if (cell == null)
                {
                    state = State.BuildControlRoomRoute;
                    return;
                }

                var (route, ok) = TryBuildRoute(cell);
                if (!ok)
                    continue;

                GoTo(route.Dequeue());
                return;
            }
        }

        private (Queue<Cell>, bool) TryBuildRoute(Cell to)
        {
            var (path, ok) = pathFinder.TryFind(maze, maze.Player, to, true);
            if (ok)
            {
                var route = new Queue<Cell>(path);
                route.Dequeue();

                return (route, true);
            }

            maze.MarkAsUnreachable(to.Position);
            return (null, false);
        }

        private Queue<Cell> BuildRoute(Cell to)
        {
            var path = pathFinder.Find(maze, maze.Player, to);
            var route = new Queue<Cell>(path);
            route.Dequeue();

            return route;
        }

        private void GoTo(Cell nextStep)
        {
            var direction = GetDirection(maze.Player.Position, nextStep.Position);

            output.WriteLine(direction);
            output.Flush();
        }

        private static string GetDirection(Position playerPosition, Position nextStep)
        {
            var diffX = playerPosition.X - nextStep.X;
            var diffY = playerPosition.Y - nextStep.Y;

            return (diffX, diffY) switch
            {
                (-1, 0) => "RIGHT",
                (1, 0) => "LEFT",
                (0, -1) => "DOWN",
                (0, 1) => "UP",
                _ => throw new InvalidOperationException(),
            };
        }

        private enum State
        {
            Exploring,
            BuildControlRoomRoute,
            GoingToControlRoom,
            BuildStartPositionRoute,
            GoingToStartPosition,
            Finished,
        }
    }
}