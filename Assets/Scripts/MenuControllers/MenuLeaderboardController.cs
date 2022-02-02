using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuLeaderboardController : MonoBehaviour
{

    public GameObject leaderboardEntryPrefab;
    public GameObject highscoreHolder;
    public Image loadingImage;

    private List<LeaderboardEntryUI> _entryTexts;


    void Awake(){
	_entryTexts = new List<LeaderboardEntryUI>();
    }

    public async void LoadLeaderboardEntry(){
        loadingImage.gameObject.SetActive(true);
        _entryTexts = new List<LeaderboardEntryUI>(highscoreHolder.GetComponentsInChildren<LeaderboardEntryUI>());
        _entryTexts.ForEach((t) => t.gameObject.SetActive(false));
        UpdateLeaderboardUI(await LeaderboardManager.Instance.GetLeaderboardEntries());
    }

    public void UpdateLeaderboardUI(List<LeaderboardEntry> entries){
        for (int i = 0; i < entries.Count; i++){
	    if(i >= _entryTexts.Count) {
                GameObject entryObj = Instantiate(leaderboardEntryPrefab, Vector3.zero, Quaternion.identity);
                entryObj.transform.parent = highscoreHolder.transform;
                _entryTexts.Add(entryObj.GetComponent<LeaderboardEntryUI>());
            }

            LeaderboardEntry entry = entries[i];
            LeaderboardEntryUI entryUI = _entryTexts[i];
            RectTransform rect = entryUI.GetComponent<RectTransform>();
	    rect.SetParent(highscoreHolder.transform);
            rect.anchorMin = new Vector2(0.5f, 1);
            rect.anchorMax = new Vector2(0.5f, 1);
            rect.anchoredPosition3D = new Vector3(0, -100 - (i * 300), 0);
            rect.localScale = Vector3.one;


            entryUI.rankText.text = (i + 1) + ". ";
            // ui.profileImage.sprite
            entryUI.usernameText.text = entry.username;
            entryUI.highscoreText.text = entry.highscore + "";

            // txt.text = i + 1 + ". " + entry.username + "  -  " + entry.highscore;
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
