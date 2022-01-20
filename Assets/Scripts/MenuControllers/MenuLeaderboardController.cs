using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;

public class MenuLeaderboardController : MonoBehaviour
{

    public Text highscoreTextPrefab;
    public GameObject highscoreHolder;
    public Image loadingImage;

    private FirebaseDatabase _dbRef;
    private List<LeaderboardEntry> _entries;
    private List<Text> _entryTexts;


    void Awake(){
	_entries = new List<LeaderboardEntry>();
	_entryTexts = new List<Text>();
	if(FirebaseManager.Instance().isInitialized) {
            _dbRef = FirebaseDatabase.DefaultInstance;
        }	
    }

    public void OnLeaderboardFirebaseInitialize(){
	// FIXME: Why thisn't working,
	// Reason: this script attached object is disabled and start is not called so not subscribed to event
        Debug.Log("Welcome to the good night!");
    }

    public async void LoadLeaderboardEntry(){
        Debug.Log("Loading leaderboard entries");
        _entries.Clear();

        if(_dbRef == null) {
            _entries.Add(new LeaderboardEntry("Couldn't Load Data!", 0));
            Debug.LogWarning("_dbRef == null");
            return;
        }

        loadingImage.gameObject.SetActive(true);
        _entryTexts.ForEach((t) => t.gameObject.SetActive(false));

        DataSnapshot snapshot = await _dbRef.GetReference("Scores").OrderByChild("highscore").LimitToFirst(10).GetValueAsync();
	if(snapshot == null) {
	    Debug.LogWarning($"Loading leaderboard failed with");
            return;
        }	    
	    
	foreach(DataSnapshot childSnapshot in snapshot.Children){
            LeaderboardEntry entry = JsonUtility.FromJson<LeaderboardEntry>(childSnapshot.GetRawJsonValue());
            _entries.Add(entry);
        }


        UpdateLeaderboardUI();
    }

    public void UpdateLeaderboardUI(){
        for (int i = 0; i < _entries.Count; i++){
	    if(i >= _entryTexts.Count) {
                Text txtObj = Instantiate(highscoreTextPrefab, Vector3.zero, Quaternion.identity);
                _entryTexts.Add(txtObj);
            }

            LeaderboardEntry entry = _entries[i];
            Text txt = _entryTexts[i];
            RectTransform rect = txt.GetComponent<RectTransform>();
	    rect.SetParent(highscoreHolder.transform);
            rect.anchorMin = new Vector2(0.5f, 1);
            rect.anchorMax = new Vector2(0.5f, 1);
            rect.anchoredPosition3D = new Vector3(0, -1000 - (i * 300), 0);
            rect.localScale = Vector3.one;

            txt.text = i + 1 + ". " + entry.username + "  -  " + entry.highscore;
        }

        for (int j = _entries.Count; j < _entryTexts.Count; j++) {
            _entryTexts[j].gameObject.SetActive(false);
            DestroyImmediate(_entryTexts[j].gameObject);
            _entryTexts.Remove(_entryTexts[j]);
        }

        loadingImage.gameObject.SetActive(false);
        _entryTexts.ForEach((t) => t.gameObject.SetActive(true));		

    }

    public void OnBackPressed(){
        GameManager.Instance().SwitchState(GameState.MAINMENU);
        AudioManager.Instance().PlayAudio("click_heavy");
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
