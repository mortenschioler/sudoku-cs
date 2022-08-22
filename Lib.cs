using System;
using System.Collections.Generic;
using System.Text;
using static SudokuCis.Aspects;

namespace SudokuCis;

public class Puzzle
{
    public static int[] LoadDotSDK(string f)
    {
        int[] puzzle = new int[81];
        StreamReader r = File.OpenText(f);
        int i = 0;
        string? line;
        while((line = r.ReadLine()) is not null)
        {
            if (line[0] == '#') continue; // skip comment lines
            for (int j = 0; j < 9; j++)
            {
                if (line[j] != '.') puzzle[i] = line[j] - '0';
                i++;
            }
        }
        r.Close();
        return puzzle;   
    }

    public static string ToDotSDK(int[] puzzle)
    {
        int k = 0;
        StringBuilder sb = new StringBuilder(81);
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                int n = puzzle[k++];
                if (n == 0) sb.Append('.');
                else sb.Append(n);
            }
            sb.Append(Environment.NewLine);
        }
        return sb.ToString();
    }

    internal static string ToDtSS(int[] puzzle)
    {
        int k = 0;
        StringBuilder sb = new StringBuilder(81);
        for (int i = 0; i < 9; i++)
        {

            if (i == 3 || i == 6) sb.Append("-----------\n");
            for (int j = 0; j < 9; j++)
            {
                if (j == 3 || j == 6) sb.Append('|');
                int n = puzzle[k++];
                if (n == 0) sb.Append('.');
                else sb.Append(n);
            }
            sb.Append(Environment.NewLine);
        }
        return sb.ToString();
    }
}

public class PuzzleInProgress
{

    public static int numGuesses = 0;
    private readonly int[] originalPuzzle;
    private int[] puzzle = new int[81];
    private int[] candidates = new int[81];

    private PuzzleInProgress(PuzzleInProgress original)
    {
        this.originalPuzzle = original.originalPuzzle;
        Array.Copy(original.puzzle, this.puzzle, 81);
        Array.Copy(original.candidates, this.candidates, 81);
    }

    public PuzzleInProgress(int[] puzzle)
    {
        for (int i = 0; i < 81; i++)
        {
            if (puzzle[i] != 0) Fill(i, puzzle[i]);
        }
        this.originalPuzzle = puzzle;
    }

    private static int Mask(int n)
    {
        return 1 << (n - 1);
    }

    public void Fill(int i, int n)
    {
        puzzle[i] = n;
        foreach (int j in RowBoxCol(i))
        {
            EliminateCandidate(j, n);
        }
        candidates[i] = 0b111111111;
    }

    public bool IsFilled(int i)
    {
        return puzzle[i] > 0;
    }

    public bool IsCandidate(int i, int n)
    {
        return (candidates[i] & Mask(n)) == 0;
    }

    public void EliminateCandidate(int i, int n)
    {
        candidates[i] = candidates[i] | Mask(n);
    }

    public bool FillSingleCandidates()
    {
        bool didFill = false;
        for (int i = 0; i < 81; i++)
        {
            int n = candidates[i] switch
            {
                0b111111110 => 1,
                0b111111101 => 2,
                0b111111011 => 3,
                0b111110111 => 4,
                0b111101111 => 5,
                0b111011111 => 6,
                0b110111111 => 7,
                0b101111111 => 8,
                0b011111111 => 9,
                _ => 0
            };
            if (n > 0)
            {
                Fill(i, n);
                didFill = true;
            }
        }
        return didFill;
    }

    public string ToDotSdx()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < 81; i++)
        {
            if (IsFilled(i))
            {
                if (originalPuzzle[i] == 0) sb.Append('u');
                sb.Append(puzzle[i]);
            } else
            {
                int c = candidates[i];

                int n, mask;
                for (n = 1, mask = 1; n < 10; mask = mask * 2, n++)
                {
                    if ((mask & c) == 0) sb.Append(n);
                }
            }
            sb.Append(' ');
            if (i % 9 == 8) sb.Append(Environment.NewLine);
        }
        return sb.ToString();
    }

    public string ToDotSdk()
    {
        return Puzzle.ToDotSDK(puzzle); 
    }

    public string ToDotSS()
    {
        return Puzzle.ToDtSS(puzzle);
    }

    override public string ToString()
    {
        return ToDotSdx();
    }

    public int Status()
    {
        for (int i = 0; i < 81; i++)
        {
            if (puzzle[i] == 0)
            {
                if (candidates[i] == 0b111111111) return -2; // unsovleable
                else return i; // unsolved
            }
        }
        return -1; // solved
    }

    public bool Solve()
    {
        while (FillSingleCandidates());
        int statusOrIndexOfEmpty = Status();
        if (statusOrIndexOfEmpty == -1) return true; // solved
        if (statusOrIndexOfEmpty == -2) return false; // unsolveable
        for (int i = 1; i <= 9; i++)
        {
            if (IsCandidate(statusOrIndexOfEmpty, i))
            {
                PuzzleInProgress guess = new PuzzleInProgress(this);
                numGuesses++;
                guess.Fill(statusOrIndexOfEmpty, i);
                if (guess.Solve())
                {
                    this.puzzle = guess.puzzle;
                    this.candidates = guess.candidates;
                    return true;
                }
            }
        }
        return false;

    }
}