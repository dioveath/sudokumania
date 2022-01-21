using System;

[Serializable]
public class PlayerData {

    public SudokuLevel playingLevel;
    public int points;

    public PlayerData(SudokuLevel sudokuLevel){
        this.playingLevel = sudokuLevel;
    }

}
