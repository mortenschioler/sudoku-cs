using static SudokuCis.Puzzle;

namespace SudokuCis
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string pathToSudoku = args[0];
            int[] puzzle = LoadDotSDK(pathToSudoku);
            PuzzleInProgress pip = new PuzzleInProgress(puzzle);
            Console.WriteLine(pip.ToDotSS());
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            bool solved = pip.Solve();
            stopWatch.Stop();
            if (solved)
            {
                Console.WriteLine(pip.ToDotSS());
                Console.WriteLine("Solved.");
            } else
            {
                Console.WriteLine(pip.ToDotSdx());
                Console.WriteLine("The sudoku is unsolveable.");
            }
           
            Console.WriteLine("Elapsed time {0:D} ms. Guessed {1:D} times.", stopWatch.ElapsedMilliseconds, PuzzleInProgress.numGuesses);
        }
    }
}