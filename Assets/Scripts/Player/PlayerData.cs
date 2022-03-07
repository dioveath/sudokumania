using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData {

    public List<SudokuLevel> playingLevels;
    public int points = 0;
    public string lastPlayedId;
    public int lastCompletedIndex;
    public string lastPlayedSeason;
    public string profileLink;
    public bool isLinked;
    public bool isFirstTime;
    public bool isTutorialComplete;

    public string authProvider;
    public string email;
    public string fullName;
    public string userId;

    public PlayerData() {
        this.playingLevels = new List<SudokuLevel>();
        this.lastPlayedId = "";
        this.profileLink = "";
        this.isLinked = false;
        this.isFirstTime = true;
        this.isTutorialComplete = false;
        this.lastCompletedIndex = 0;
        this.authProvider = "offline";
        this.email = "null";
        this.fullName = "Fellow Traveller";
        this.userId = "Charicha ID";
        this.lastPlayedSeason = SudokuUtils.season;
    }

}
