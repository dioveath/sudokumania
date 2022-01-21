using System;

[Serializable]
public class PlayerData {

    public SudokuLevel playingLevel;
    public int points = 0;

    public PlayerData(SudokuLevel sudokuLevel){
        this.playingLevel = sudokuLevel;
    }

}
