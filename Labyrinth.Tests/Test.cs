using System.IO;
using Xunit;

namespace Labyrinth.Tests
{
    public class Test
    {
        [Fact]
        public void Test1()
        {
            var str = @"10 30 1000
3 6
??????????????????????????????
#..............???????????????
#.#############???????????????
#.....T........???????????????
#.......................#.#..#
#.#######################.#..#
#.....##......##......#....###
#...####..##..##..##..#..#...#
#.........##......##.....#.C.#
##############################";

            var input = new StringReader(str);
            var output = new StringWriter();

            var solution = new Solution(input, output);
            solution.Start();
            
            Assert.Equal("", output.ToString());
        }
        
        [Fact]
        public void Test2()
        {
            var str = @"10 30 1000
3 6
??????????????????????????????
????.....?????????????????????
????#####?????????????????????
????..T..?????????????????????
????.....?????????????????????
????#####?????????????????????
??????????????????????????????
??????????????????????????????
??????????????????????????????
??????????????????????????????";

            var input = new StringReader(str);
            var output = new StringWriter();

            var solution = new Solution(input, output);
            solution.Start();
            
            Assert.Equal("", output.ToString());
        }
        
        [Fact]
        public void Test3()
        {
            var str = @"10 30 1000
1 1
##############################
#T...........................#
##...........................#
#............................#
#............................#
#............................#
#............................#
#............................#
#...........................C#
##############################";

            var input = new StringReader(str);
            var output = new StringWriter();

            var solution = new Solution(input, output);
            solution.Start();
            
            Assert.Equal("", output.ToString());
        }
    }
}