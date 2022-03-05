using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData {

    public List<SudokuLevel> playingLevels;
    public int points = 0;
    public string lastPlayedId;
    public int lastCompletedIndex;
    public string playerName;
    public string profileLink;
    public bool isLinked;
    public bool isFirstTime;
    public bool isTutorialComplete;

    public PlayerData() {
        this.playingLevels = new List<SudokuLevel>();
        this.lastPlayedId = "";
        this.profileLink = "";
        this.isLinked = false;
        this.isFirstTime = true;
        this.isTutorialComplete = false;
        this.lastCompletedIndex = 0;
    }

}

[Serializable]
public class OnlinePlayerData {
    public string authProvider;
    public string email;
    public string fullName;
    public string userId;
    public int score;
    public int lastCompletedIndex;
}
