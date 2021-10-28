using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Firebase.Database;



public class MenuLeaderboardController : MonoBehaviour
{

    public Text highscoreTextPrefab;
    public GameObject highscoreHolder;

    private FirebaseDatabase _dbRef;
    private List<LeaderboardEntry> entries;
    private List<Text> entryTexts;


    void Awake(){
	FirebaseManager.Instance().OnFirebaseInitialized.AddListener(OnLeaderboardFirebaseInitialize);
	entryTexts = new List<Text>();
    }

    void Start(){
    }

    public void OnLeaderboardFirebaseInitialize(){
	// FIXME: Why thisn't working
        _dbRef = FirebaseDatabase.DefaultInstance;
    }

    public async void LoadLeaderboardEntry(){
        Debug.Log("Loading leaderboard entries");

        entries = new List<LeaderboardEntry>();
        _dbRef = FirebaseDatabase.DefaultInstance;
        if(_dbRef == null) {
            entries.Add(new LeaderboardEntry("Couldn't Load Data!", 0));
            Debug.LogWarning("_dbRef == null");
            return;
        }

	DataSnapshot snapshot = await _dbRef.GetReference("Scores").OrderByChild("highscore").LimitToFirst(10).GetValueAsync();
	if(snapshot == null) {
	    Debug.LogWarning($"Loading leaderboard failed with");
            return;
        }	    
	    
	foreach(DataSnapshot childSnapshot in snapshot.Children){
            LeaderboardEntry entry = JsonUtility.FromJson<LeaderboardEntry>(childSnapshot.GetRawJsonValue());
            entries.Add(entry);
        }

        UpdateLeaderboardUI();
    }

    public void UpdateLeaderboardUI(){
        for (int i = 0; i < entries.Count; i++){
	    if(i >= entryTexts.Count) {
                Text txtObj = Instantiate(highscoreTextPrefab, Vector3.zero, Quaternion.identity);
                entryTexts.Add(txtObj);
            }

            LeaderboardEntry entry = entries[i];
            Text txt = entryTexts[i];
            RectTransform rect = txt.GetComponent<RectTransform>();
	    rect.SetParent(highscoreHolder.transform);
            rect.anchorMin = new Vector2(0.5f, 1);
            rect.anchorMax = new Vector2(0.5f, 1);
            rect.anchoredPosition3D = new Vector3(0, -1000 - (i * 300), 0);
            rect.localScale = Vector3.one;

            txt.text = i + 1 + ". " + entry.username + "  -  " + entry.highscore;
	    
        }
        for (int j = entries.Count; j < entryTexts.Count; j++) {
            Destroy(entryTexts[j].gameObject);
            entryTexts.Remove(entryTexts[j]);
        }
    }

    public void OnBackPressed(){
        GameManager.Instance().SwitchState(GameState.MAINMENU);
    }

    public void EnterNewLeaderboardEntry(string username, int highscore){
        LeaderboardEntry newEntry = new LeaderboardEntry(username, highscore);
        string json = JsonUtility.ToJson(newEntry);
        // Debug.Log(json);
        _dbRef.GetReference("Scores").Child(username).SetRawJsonValueAsync(json);
    }

    class LeaderboardEntry
    {
        public string username;
        public int highscore;
	public LeaderboardEntry(string __username, int __highscore){
            this.username = __username;
            this.highscore = __highscore;
        }
    };

}
