using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData {

    public List<SudokuLevel> playingLevels;
    public int points = 0;
    public string lastPlayedId;
    public string playerName;
    public string profileLink;
    public bool isLinked;
    public bool isFirstTime;

    public PlayerData() {
        this.playingLevels = new List<SudokuLevel>();
        this.lastPlayedId = "";
        this.profileLink = "";
        this.isLinked = false;
        this.isFirstTime = true;
    }

}
