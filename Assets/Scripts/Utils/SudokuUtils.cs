using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;

public class SudokuUtils
{
    
    private static int[,] mediumSudoku = {
	{5, 3, 0,  0, 7, 0,  0, 0, 0}, 
	{6, 0, 0,  1, 9, 5,  0, 0, 0}, 
	{0, 9, 8,  0, 0, 0,  0, 6, 0}, 

	{8, 0, 0,  0, 6, 0,  0, 0, 3}, 
	{4, 0, 0,  8, 0, 3,  0, 0, 1}, 
	{7, 0, 0,  0, 2, 0,  0, 0, 6}, 

	{0, 6, 0,  0, 0, 0,  2, 8, 0}, 
	{0, 0, 0,  4, 1, 9,  0, 0, 5}, 
	{0, 0, 0,  0, 8, 0,  0, 7, 9}
    };

    private static int[,] easySudoku = {
	{7, 0, 0,  4, 2, 0,  9, 0, 1},
	{2, 0, 0,  3, 1, 9,  0, 5, 7},
	{0, 9, 3,  7, 5, 6,  8, 0, 4},

	{9, 5, 8,  2, 0, 0,  7, 0, 0},
	{4, 0, 0,  0, 0, 0,  0, 0, 0},
	{0, 0, 0,  0, 0, 0,  2, 0, 8},

	{5, 4, 6,  1, 0, 7,  0, 9, 0},
	{3, 7, 0,  0, 9, 0,  0, 8, 5},
	{8, 0, 0,  5, 4, 0,  0, 0, 0}
    };
 

    private static int[,] hardSudoku = {
	{5, 0, 7,  2, 0, 0,  0, 9, 0},
	{0, 0, 6,  0, 3, 0,  7, 0, 1},
	{4, 0, 0,  0, 0, 0,  0, 6, 0},

	{1, 0, 0,  4, 9, 0,  0, 0, 7},
	{0, 0, 0,  5, 0, 8,  0, 0, 0},
	{8, 0, 0,  0, 2, 7,  0, 0, 5},

	{0, 7, 0,  0, 0, 0,  0, 0, 9},
	{2, 0, 9,  0, 8, 0,  6, 0, 0},
	{0, 4, 0,  0, 0, 9,  3, 0, 8}
    };

    private static int[,] debugSudoku = {
	{7, 6, 5, 4, 2, 8, 9, 3, 1},
	{2, 8, 4, 3, 1, 9, 6, 5, 7},
	{1, 9, 3, 7, 5, 6, 8, 2, 4},
	{9, 5, 8, 2, 6, 4, 7, 1, 3},
	{4, 3, 2, 8, 7, 1, 5, 6, 9},
	{6, 1, 7, 9, 3, 5, 2, 4, 8},
	{5, 4, 6, 1, 8, 7, 3, 9, 2},
	{3, 7, 1, 6, 9, 2, 4, 8, 5},
	{8, 2, 9, 5, 4, 3, 1, 0, 0},
    };

    public static List<SudokuLevel> allSudokuLevels = new List<SudokuLevel>();

    public static async Task<List<SudokuLevel>> GetAllLevels(FirebaseDatabase dbRef){
	List<PuzzleToUpload> allSudokus = new List<PuzzleToUpload>();

        if(dbRef == null) {
            // LoadPuzzlesOffline();
            Debug.LogError("Firebase database not initialized!");
        } else {
	    try {
		allSudokus = await LoadPuzzlesFromDatabase(dbRef);
	    } catch(Exception e){
                allSudokus.Add(new PuzzleToUpload("test", PuzzleToString(debugSudoku), 1000));
                Debug.LogError("Exception: " + e.Message);
            }
        }

        allSudokuLevels.Clear();

        for (int i = 0; i < allSudokus.Count; i++)
	{
            int[,] puzzle = StringToPuzzle(allSudokus[i].puzzle);
            int[,] solvedPuzzle = SudokuUtils.SudokuSolve(puzzle);
	    if (!isSudokuValid(solvedPuzzle))
	    {
		Debug.LogWarning("Suduku " + (i+1) + " Not Solvable!");
		// Console.WriteLine("Sudoku " + i + 1 + " Not Solvable!");
		continue;
	    }

	    int[,] inputArray = new int[9, 9];
	    int[,] puzzleArray = new int[9, 9];
	    Array.Copy(puzzle, inputArray, 9 * 9);
	    Array.Copy(puzzle, puzzleArray, 9 * 9);

	    if(i < allSudokuLevels.Count) {
                allSudokuLevels[i] = new SudokuLevel(allSudokus[i].id, inputArray, puzzleArray, solvedPuzzle, allSudokus[i].points);
            } else {
		allSudokuLevels.Add(new SudokuLevel(allSudokus[i].id, inputArray, puzzleArray, solvedPuzzle, allSudokus[i].points));
	    }
	}

        return allSudokuLevels;
    }
    
    // public static SudokuLevel GenerateSudokuLevel(){
    //     int[,] inputArray = new int[9, 9];
    //     int[,] puzzleArray = new int[9, 9];	
    //     Array.Copy(debugSudoku, inputArray, 9*9);
    //     Array.Copy(debugSudoku, puzzleArray, 9*9);	
    //     return new SudokuLevel(puzzleArray, inputArray);
    // }    


    public static bool isSudokuValid(int[,] puzzle)
    {
        int sum = 45;
        for (int i = 0; i < 9; i++)
        {
            int sumx = 0, sumy = 0, sumd = 0;
            for (int j = 0; j < 9; j++)
            {
                sumy += puzzle[j, i];
                sumx += puzzle[i, j];
                sumd += puzzle[j / 3, j % 3];
            }
            if (sumy != sum || sumx != sum || sumd != sum) return false;
        }
        return true;
    }


    public static int[,] SudokuSolve(int[,] puzzle){
        List<int>[,] solvingPuzzle;
        solvingPuzzle = new List<int>[9, 9];
        for (int i = 0; i < 9; i++){
            for (int j = 0; j < 9; j++){
		if(puzzle[i,j] == 0)
		    solvingPuzzle[i, j] = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9};
		else
                    solvingPuzzle[i, j] = new List<int> { puzzle[i, j] };
            }

        }

	int iteration = 10;
        for(int iter = 0; iter < iteration; iter++){
            PrintSolvingPuzzle(solvingPuzzle);
            for(int i = 0; i < 9; i++){
		for(int j = 0; j < 9; j++){
                    if(solvingPuzzle[i, j].Count > 1) {
			List<int> newPValues = getPossibleValues(solvingPuzzle, j, i);
                        solvingPuzzle[i, j] = newPValues;
                    }
		}
	    }      
	}


        int[,] solvedPuzzle = new int[9, 9];
        for (int i = 0; i < 9; i++){
            for (int j = 0; j < 9; j++){
		// if(solvingPuzzle[i, j].Count != 1) {
                //     return false;
		// }
		solvedPuzzle[i, j] = solvingPuzzle[i, j][0];
            }
	}

        return solvedPuzzle;
    }

    
    public static bool isSudokuSolvable(int[,] puzzle){
        int[,] solvedPuzzle = SudokuSolve(puzzle);
        return isSudokuValid(solvedPuzzle);
    }

    public static List<int> getPossibleValues(List<int>[,] solvingPuzzle, int x, int y){
        if (solvingPuzzle[y, x].Count == 1)
            return solvingPuzzle[y, x];

        List<int> possibleValues = new List<int>(solvingPuzzle[y, x]);

        for (var j = 0; j < 9; j++)
        {

            // horizontal check
            if (solvingPuzzle[y, j].Count == 1 && possibleValues.Contains(solvingPuzzle[y, j][0]))
            {
		if(x == 5 && y == 0){
                    Console.WriteLine("solvingPuzzle[" + y + ", " + j +"][0] : " + solvingPuzzle[y, j][0]);
                    Console.WriteLine("Removing Horizontal: " + solvingPuzzle[y, j][0]);
                }
                possibleValues.Remove(solvingPuzzle[y, j][0]);
            }

            // vertical check
            if (solvingPuzzle[j, x].Count == 1 && possibleValues.Contains(solvingPuzzle[j, x][0]))
            {
		if(x == 5 && y == 0){
                    Console.WriteLine("Removing Vertical: " + solvingPuzzle[j, x][0]);
                }		
                possibleValues.Remove(solvingPuzzle[j, x][0]);
            }

            // 3x3 box check

            int offx = (x / 3);
            int offy = (y / 3);
            if(solvingPuzzle[offy * 3 + (j / 3), offx * 3 + j % 3].Count != 1) continue;
	    
            int value = solvingPuzzle[offy * 3 + (j / 3), offx * 3 + j % 3][0];


            if (possibleValues.Contains(value))
            {
                possibleValues.Remove(value);
            }
            if (x == 5 && y == 0)
            {
                Console.WriteLine("Removing : " + value);

                Console.Write("[ ");
                foreach (int k in possibleValues)
                {
                    Console.Write(k + ", ");
                }
                Console.WriteLine(" ]");
		Console.WriteLine("==============================");
            }
        }

        return possibleValues;
    }

    // public static List<int LoadPuzzlesOffline(){
    //     Debug.Log("Loading puzzles offline..!");
    //     List<int[,]> allSudokus = new List<int[,]>();
    //     allSudokus.Add(debugSudoku);
    //     allSudokus.Add(easySudoku);
    //     allSudokus.Add(mediumSudoku);
    //     allSudokus.Add(hardSudoku);
    //     return allSudokus;
    // }

    public static async Task<List<PuzzleToUpload>> LoadPuzzlesFromDatabase(FirebaseDatabase dbRef){
        Debug.Log("Loading puzzles online....");
	List<PuzzleToUpload> allPuzzles = new List<PuzzleToUpload>();

        DataSnapshot snapshot = await dbRef.GetReference("Puzzles").LimitToFirst(10).GetValueAsync();

        foreach(DataSnapshot childSnapshot in snapshot.Children){
            string json = childSnapshot.GetRawJsonValue();
            PuzzleToUpload pzu = JsonUtility.FromJson<PuzzleToUpload>(json);
            allPuzzles.Add(pzu);
        }
	
        return allPuzzles;
    }
    

    public static void WritePuzzlesToDatabase(FirebaseDatabase dbRef, string puzzle, int points)
    {
        DatabaseReference pushedRef = dbRef.GetReference("").Child("Puzzles").Push();
        PuzzleToUpload newPzu = new PuzzleToUpload(pushedRef.Key, puzzle, points);
        string pzuString = JsonUtility.ToJson(newPzu);
        pushedRef.SetRawJsonValueAsync(pzuString);
    }


    public static void Main(){
        // if(isSudokuSolvable(easySudoku)){
        //     Console.WriteLine("Solvable");
        // } else {
        //     Console.WriteLine("Not Solvable");	    
        // }

        string puzzleString = PuzzleToString(mediumSudoku);
        Console.WriteLine(puzzleString);
        int[,] puzzle = StringToPuzzle(puzzleString);
        PrintPuzzle(puzzle);

    }

    public static string PuzzleToString(int[,] puzzle){
        string result = "";
        for (int i = 0; i < 9; i++){
            for (int j = 0; j < 9; j++){
                result += puzzle[i, j];
		// if(j != 8 || i != 8  )
		//     result += ", ";
            }
        }
	// Debug.Log("")
        return result;
    }

    public static int[,] StringToPuzzle(string puzzleString){
        int[,] puzzle = new int[9, 9];
        for (int i = 0; i < 9; i++){
            for (int j = 0; j < 9; j++){

		if(i * 9 + j > puzzleString.Length - 1) {
                    // safeguarding null if puzzleString badly formatted
                    Debug.LogWarning("Puzzle String Badly Formatted! string:" + puzzleString);
                    return puzzle;
		}

                char c = puzzleString[i * 9 + j];
                int value = c - '0';
                puzzle[i, j] = value;
            }
	}
        return puzzle;
    }

    public static void PrintSolvingPuzzle(List<int>[,] solvingPuzzle){
        for (int i = 0; i < 9; i++){
            for (int j = 0; j < 9; j++){
                Console.Write("[");
                foreach(int k in solvingPuzzle[i,j]){
                    Console.Write(k + ",");
                }
                Console.Write("]");		
	    }
            Console.WriteLine("--");	    
	}
	Console.WriteLine("============================================================");
    }

    public static void PrintPuzzle(int[,] puzzle){
        for (int i = 0; i < 9; i++){
	    Console.Write("[ ");
            for (int j = 0; j < 9; j++){
                Console.Write(puzzle[i, j]);
                if(j != 8)
                    Console.Write(", ");
            }
            Console.WriteLine(" ]");
        }
    }


}

[Serializable]
public class SudokuLevel {
    public string id;
    public int[,] sudokuArray;
    public int[,] inputSudokuArray;
    public int[,] validSolution;
    public int points;
    public bool isCompleted;
    public float lastElapsedTime;

    public SudokuLevel(string __id, int[,] __sudokuArray, int[,] __inputSudokuArray, int[,] __validSolution, int points){
        this.id = __id;
        this.sudokuArray = __sudokuArray;
        this.inputSudokuArray = __inputSudokuArray;
        this.validSolution = __validSolution;
        this.points = points;
        this.isCompleted = false;
        this.lastElapsedTime = 0.0f;
    }

    public SudokuLevel Copy(){
        int[,] newSudokuArray = new int[9,9];
        int[,] newInputSudokuArray = new int[9,9];
        int[,] newValidSolution = new int[9, 9];

        Array.Copy(sudokuArray, newSudokuArray, newSudokuArray.Length);
        Array.Copy(inputSudokuArray, newInputSudokuArray, newSudokuArray.Length);
        Array.Copy(validSolution, newValidSolution, newSudokuArray.Length);	

        return new SudokuLevel(id, newSudokuArray, newInputSudokuArray, newValidSolution, points);
    }

    public void Reset(){
        Array.Copy(sudokuArray, inputSudokuArray, sudokuArray.Length);
    }
}
