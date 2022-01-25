using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Database;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    private FirebaseDatabase _dbRef;

    public static LeaderboardManager _instance;
    public static LeaderboardManager Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogWarning("LeaderboardManager null!");
            return _instance;
        }
    }

    void Awake(){
	if(_instance != null){
            Debug.LogWarning("There is already LeaderboardManager script!");
            DestroyImmediate(this.gameObject);
        }
        _instance = this;


    }

    public async Task<List<LeaderboardEntry>> GetLeaderboardEntries(){
        List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

	_dbRef = FirebaseDatabase.DefaultInstance;
        if(_dbRef == null) {
            entries.Add(new LeaderboardEntry("Couldn't Load Data!", 0));
            Debug.LogWarning("_dbRef == null");
            return entries;
        }

        DataSnapshot snapshot = await _dbRef.GetReference("Scores").OrderByChild("highscore").LimitToFirst(10).GetValueAsync();
	if(snapshot == null) {
            entries.Add(new LeaderboardEntry("Couldn't Load Data!", 0));	    
	    Debug.LogWarning($"Loading leaderboard failed with");
            return entries;
        }	    
	    
	foreach(DataSnapshot childSnapshot in snapshot.Children){
            LeaderboardEntry entry = JsonUtility.FromJson<LeaderboardEntry>(childSnapshot.GetRawJsonValue());
            entries.Add(entry);
        }

        entries.Reverse();
        return entries;
    }

    public async void EnterNewLeaderboardEntry(string username, int highscore){
        LeaderboardEntry newEntry = new LeaderboardEntry(username, highscore);

        List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
        entries = await GetLeaderboardEntries();

        entries.Add(newEntry);
        entries.Sort((a, b) => { return a.highscore - b.highscore; });
        entries.RemoveAt(0);

        for (int i = 0; i < entries.Count; i++){
	    string json = JsonUtility.ToJson(entries[i]);
            await _dbRef.GetReference("Scores").Child(entries[i].username).SetRawJsonValueAsync(json);	    
	}

    }

}


[Serializable]
public class LeaderboardEntry
{
    public string username;
    public int highscore;
    public LeaderboardEntry(string __username, int __highscore){
	this.username = __username;
	this.highscore = __highscore;
    }
};
