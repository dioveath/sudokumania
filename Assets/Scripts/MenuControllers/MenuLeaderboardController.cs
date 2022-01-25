using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;

public class MenuLeaderboardController : MonoBehaviour
{

    public Text highscoreTextPrefab;
    public GameObject highscoreHolder;
    public Image loadingImage;

    private List<Text> _entryTexts;


    void Awake(){
	_entryTexts = new List<Text>();
    }

    public async void LoadLeaderboardEntry(){
        loadingImage.gameObject.SetActive(true);
	_entryTexts.ForEach((t) => t.gameObject.SetActive(false));
        UpdateLeaderboardUI(await LeaderboardManager.Instance.GetLeaderboardEntries());
    }

    public void UpdateLeaderboardUI(List<LeaderboardEntry> entries){
        for (int i = 0; i < entries.Count; i++){
	    if(i >= _entryTexts.Count) {
                Text txtObj = Instantiate(highscoreTextPrefab, Vector3.zero, Quaternion.identity);
                _entryTexts.Add(txtObj);
            }

            LeaderboardEntry entry = entries[i];
            Text txt = _entryTexts[i];
            RectTransform rect = txt.GetComponent<RectTransform>();
	    rect.SetParent(highscoreHolder.transform);
            rect.anchorMin = new Vector2(0.5f, 1);
            rect.anchorMax = new Vector2(0.5f, 1);
            rect.anchoredPosition3D = new Vector3(0, -1000 - (i * 300), 0);
            rect.localScale = Vector3.one;

            txt.text = i + 1 + ". " + entry.username + "  -  " + entry.highscore;
        }

        for (int j = entries.Count; j < _entryTexts.Count; j++) {
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

}
