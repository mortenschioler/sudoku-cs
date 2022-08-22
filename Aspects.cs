using System.Collections;

namespace SudokuCis;

public class Aspects
{

    public static IEnumerable<int> Row(int rowi)
    {
        int i = rowi * 9;
        int target = i + 9;
        for (; i < target; i++)
        {
            yield return i;
        }
    }

    public static IEnumerable<int> Col(int coli)
    {
        for (int i = coli; i < 81; i += 9)
        {
            yield return i;
        }
    }

    public static IEnumerable<int> Box(int boxi)
    {
        int anchor = (boxi*3) % 9 + (boxi / 3) * 27;
        yield return anchor++;
        yield return anchor++;
        yield return anchor;
        anchor += 7;
        yield return anchor++;
        yield return anchor++;
        yield return anchor;
        anchor += 7;
        yield return anchor++;
        yield return anchor++;
        yield return anchor;
    }

    // TODO iterator for intersection of row, col, box without overlap
    public static IEnumerable<int> RowBoxCol(int i)
    {
        return Enumerable.Concat(Enumerable.Concat(Row(RowOf(i)), Col(ColOf(i))), Box(BoxOf(i)));
    }

    public static int RowOf(int i) => i / 9;

    public static int ColOf(int i) => i % 9;

    public static int BoxOf(int i) => (i % 9) / 3 + 3 * (i / 27);
}
