using NUnit.Framework;

public class SudokuSolveTest
{

    [Test]
    public void SudokuSolveTestSimplePasses()
    {
        int[,] debugSudoku = {
	    {0, 0, 5, 4, 2, 8, 9, 3, 1},
	    {2, 8, 4, 3, 0, 9, 0, 5, 7},
	    {1, 9, 0, 0, 5, 6, 8, 2, 4},
	    {9, 5, 8, 0, 0, 4, 7, 0, 3},
	    {0, 3, 2, 0, 7, 1, 0, 6, 9},
	    {6, 0, 0, 9, 3, 5, 0, 4, 0},
	    {5, 4, 6, 0, 8, 0, 0, 9, 2},
	    {3, 0, 1, 6, 9, 0, 4, 8, 5},
	    {0, 2, 9, 0, 4, 0, 1, 0, 0},
	};

        int[,] solvedPuzzle = {
	    {7, 6, 5, 4, 2, 8, 9, 3, 1},
	    {2, 8, 4, 3, 1, 9, 6, 5, 7},
	    {1, 9, 3, 7, 5, 6, 8, 2, 4},
	    {9, 5, 8, 2, 6, 4, 7, 1, 3},
	    {4, 3, 2, 8, 7, 1, 5, 6, 9},
	    {6, 1, 7, 9, 3, 5, 2, 4, 8},
	    {5, 4, 6, 1, 8, 7, 3, 9, 2},
	    {3, 7, 1, 6, 9, 2, 4, 8, 5},
	    {8, 2, 9, 5, 4, 3, 1, 7, 6},
	};

        Assert.AreEqual(SudokuUtils.SudokuSolve(debugSudoku), solvedPuzzle);
    }


}
