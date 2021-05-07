using System;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;

namespace Labyrinth.Cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var s1 = @"##############################
##############################
##############################
##############################
##############################
##############################
#####T.....C##################
##############################
##############################
##############################";
            
            var s2 = "####################.....#####\n" +
                     "#######........#.#############\n" +
                     "################.#......T#####\n" +
                     "###########...#..#.#######.###\n" +
                     "##########.#######...#####.###\n" +
                     "##########.#C..#####.#####.###\n" +
                     "##########.###.......#########\n" +
                     "##########.#.########........#\n" +
                     "##########...#################\n" +
                     "##############################";
            
            var s3 = @"##############################
#T...........................#
##...........................#
#............................#
#............................#
#............................#
#............................#
#............................#
#...........................C#
##############################";

            var playerToGame = new Pipe();
            var gameToPlayer = new Pipe();

            var playerToGameReader = new StreamReader(playerToGame.Reader.AsStream(), Encoding.ASCII);
            var playerToGameWriter = new StreamWriter(playerToGame.Writer.AsStream(), Encoding.ASCII);

            var gameToPlayerReader = new StreamReader(gameToPlayer.Reader.AsStream(), Encoding.ASCII);
            var gameToPlayerWrite = new StreamWriter(gameToPlayer.Writer.AsStream(), Encoding.ASCII);

            var gameTask = Task.Run(() =>
            {
                var game = new Game(playerToGameReader, gameToPlayerWrite);
                game.FromString(10, 30, s2);
                game.Start();
            });

            var playerTask = Task.Run(() =>
            {
                var solution = new Solution(gameToPlayerReader, playerToGameWriter);
                solution.Start();
            });

            await Task.WhenAll(gameTask, playerTask);
        }
    }
}