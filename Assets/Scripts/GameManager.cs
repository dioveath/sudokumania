using System;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Auth;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // UI objects
    [Header("Menu Parent Holder Objects")]
    public GameObject loginHolder;
    public GameObject mainMenuHolder;
    public GameObject leaderboardHolder;
    public GameObject settingsHolder;
    public GameObject levelSelectHolder;
    public GameObject allLevelsHolder;
    public GameObject youwinHolder;
    public GameObject gameplayHolder;
    public GameObject infoHolder;
    public GameObject sudokuLinesHolder;

    // LevelSelect Menu
    [Header("LevelSelect Objects")]
    public GameObject levelButtonHolder;
    public GameObject levelButton;
    public GameObject loadingUI;
    public ScrollRect scrollRect;

    // MainMenu
    [Header("MainMenu Objects")]
    public Text welcomeText;

    private GameObject _lastMenu;
    private static GameManager _instance;
    private GameState _currentState;
    

    void Awake(){
        _instance = this;
    }

    public static GameManager Instance() {
	if(_instance == null) {
            Debug.LogWarning("Error: No GameManager instantiated as of now!");	    
	}
	return _instance;
    }
    
    void Start() {
	#if !(DEVELOPMENT_BUILD || UNITY_EDITOR)
	Debug.unityLogger.logEnabled = false;
	#endif

        FirebaseManager.Instance().OnFirebaseInitialized.AddListener(OnFirebaseInitialize);

        SwitchState(GameState.MAINMENU);
        AudioManager.Instance().PlayMusic("game_menu");
    }


    public void SwitchState(GameState state){
        Debug.Log(_currentState + " > " + state);

        // Exit the previous state
        switch(_currentState){
	    case GameState.LOGIN:
                loginHolder.SetActive(false);
                break;
            case GameState.MAINMENU:
                mainMenuHolder.SetActive(false);
                // sudokuLinesHolder.SetActive(false);
                break;
	    case GameState.LEVEL_SELECT:
                levelSelectHolder.SetActive(false);
                break;
	    case GameState.LEADERBOARD:
                leaderboardHolder.SetActive(false);
                break;
            case GameState.GAMEPLAY:
                sudokuLinesHolder.SetActive(false);
                gameplayHolder.SetActive(false);
                gameplayHolder.GetComponent<MenuGameplayController>().CleanUp();
                break;
            case GameState.YOUWIN:
                youwinHolder.SetActive(false);
                break;
	    case GameState.SETTINGS:
                settingsHolder.GetComponent<MenuSettingsController>().Cleanup();
                settingsHolder.SetActive(false);
		break;
	    case GameState.INFO:
                infoHolder.SetActive(false);
                break;
        }

	// Enter the given state
	switch(state){
	    case GameState.LOGIN:
                loginHolder.SetActive(true);
                break;
            case GameState.MAINMENU:
                mainMenuHolder.GetComponent<MenuMainController>().Init(Player.Instance.playerData);
                mainMenuHolder.SetActive(true);
                break;
	    case GameState.LEVEL_SELECT:
                HandleInitializeLevels();
                levelSelectHolder.GetComponent<MenuLevelSelectController>().Init();
                levelSelectHolder.SetActive(true);
                break;
	    case GameState.LEADERBOARD:
                leaderboardHolder.SetActive(true);
                leaderboardHolder.GetComponent<MenuLeaderboardController>().LoadLeaderboardEntry();
                break;
	    case GameState.SETTINGS:
                settingsHolder.GetComponent<MenuSettingsController>().Init(SaveManager.Instance.settingsData);
                settingsHolder.SetActive(true);
		break;
            case GameState.GAMEPLAY:
                sudokuLinesHolder.SetActive(true);
                gameplayHolder.SetActive(true);
                gameplayHolder.GetComponent<MenuGameplayController>().Init();		
                break;
	    case GameState.YOUWIN:
                youwinHolder.SetActive(true);
                youwinHolder.GetComponent<MenuYouwinController>().Init();
                break;
	    case GameState.INFO:
                infoHolder.SetActive(true);
                break;
        }

        _currentState = state;
    }

    void Update(){
	if(Input.GetKeyDown(KeyCode.Escape)){
	    switch(_currentState){
		case GameState.MAINMENU:
		    {
                        DialogData dialogData = new DialogData("Exit",
							       "Are you sure you want to exit?",
							       "Yes",
							       "No",
							       () => { Application.Quit(); },
							       false,
							       () => { DialogManager.Instance.HideDialog(); } );
                        DialogManager.Instance.ShowDialog(dialogData);
                        // Application.Quit();
                    }
		    break;
		case GameState.LEVEL_SELECT:
                    AudioManager.Instance().PlayAudio("click_heavy");
                    SwitchState(GameState.MAINMENU);
		    break;
		case GameState.LEADERBOARD:
                    AudioManager.Instance().PlayAudio("click_heavy");		    
		    SwitchState(GameState.MAINMENU);
		    break;
		case GameState.SETTINGS:
                    AudioManager.Instance().PlayAudio("click_heavy");
		    SwitchState(GameState.MAINMENU);		    
                    break;
		case GameState.INFO:
                    AudioManager.Instance().PlayAudio("click_heavy");		    
                    SwitchState(GameState.MAINMENU);
                    break;
            }
	}
    }

    // this should be moved to MenuLevelSelectcontroller
    async void HandleInitializeLevels(){
        loadingUI.SetActive(true);

        List<Button> currentButtons = levelButtonHolder.GetComponentsInChildren<Button>().ToList();
        int currentTotalButtons = currentButtons.Count;	
        for (int i = 0; i < currentTotalButtons; i++)
	    currentButtons[i].gameObject.SetActive(false);
	
	if(FirebaseManager.Instance().isInitialized) {
	    await SudokuUtils.GetAllLevels(FirebaseDatabase.DefaultInstance);
	} else {
            await SudokuUtils.GetAllLevels(null);
        }
        if(SudokuUtils.allSudokuLevels == null){
            Debug.LogWarning("NULL SudokuUtils.allSudokuLevels");
            return;
        }

        int levelsCount = SudokuUtils.allSudokuLevels.Count;
        RectTransform rt = allLevelsHolder.GetComponent<RectTransform>();
        for (int i = 0; i < levelsCount; i++){
            GameObject buttonObj;
            if(i >= currentTotalButtons) {
		buttonObj = Instantiate(levelButton, Vector3.zero, Quaternion.identity);
		currentButtons.Add(buttonObj.GetComponent<Button>());
	    } else {
                buttonObj = currentButtons[i].gameObject;
            }

            RectTransform rect = buttonObj.GetComponent<RectTransform>();
            rect.SetParent(levelButtonHolder.transform);
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
	    rect.anchoredPosition3D = new Vector3(0, 0, 0);
	    rect.localScale = Vector3.one;

            int levelIndex = i;

            buttonObj.GetComponentInChildren<Text>().text = "" + (levelIndex+1);
	    buttonObj.GetComponent<Image>().color = Color.white;

            for (int j = 0; j < Player.Instance.playerData.playingLevels.Count; j++){
                if (Player.Instance.playerData.playingLevels[j].id.Equals(SudokuUtils.allSudokuLevels[levelIndex].id)) {
                    buttonObj.GetComponent<Image>().color = Color.white;
		    if(Player.Instance.playerData.playingLevels[j].isCompleted){
			buttonObj.GetComponent<Image>().color = Color.yellow;
		    }
                    break;
                }
	    }

            if(Player.Instance.playerData.lastPlayedId == SudokuUtils.allSudokuLevels[levelIndex].id)
                buttonObj.GetComponent<Image>().color = Color.green;

            buttonObj.GetComponent<Button>().onClick.AddListener(() => {
		SwitchState(GameState.GAMEPLAY);
		SudokuManager.Instance().Init(SudokuUtils.allSudokuLevels[levelIndex]);
                AudioManager.Instance().PlayAudio("click_wooden1");
            });
        }

	if(currentTotalButtons > levelsCount){
            for (int i = levelsCount; i < currentTotalButtons; i++){
                currentButtons[i].gameObject.SetActive(false);
                DestroyImmediate(currentButtons[i]);
            }
	}

	for (int i = 0; i < currentButtons.Count; i++) {
            if(i > (Player.Instance.playerData.lastCompletedIndex)){
                currentButtons[i].interactable = false;
            } else {
                currentButtons[i].interactable = true;		
	    }	    
	    currentButtons[i].gameObject.SetActive(true);
	}

        loadingUI.SetActive(false);
        StartCoroutine(ScrollLevelsToTop());
    }

    IEnumerator ScrollLevelsToTop(){
	scrollRect.verticalNormalizedPosition = 0f;
        yield return new WaitForSeconds(0.2f);
        // scrollRect.verticalNormalizedPosition = 1f;
        float animationTime = 0.5f;
        while(animationTime >= 0f){
            animationTime -= Time.deltaTime;
	    scrollRect.verticalNormalizedPosition =  Mathf.Lerp(0f, 1f, 1f - (0.5f/1f * animationTime));
            yield return null;
        }
    }

    public void OnFirebaseInitialize(){
        AuthManager.Instance().authStateChangedUEvent.AddListener(OnAuthStateChange);
    }
    
    public void OnAuthStateChange(FirebaseUser user){
    }

    public GameState GetCurrenetGameState(){
        return _currentState;
    }
    
}



[Serializable]
public enum GameState
{
    LOGIN,
    MAINMENU,
    LEADERBOARD,
    SETTINGS,
    LEVEL_SELECT,
    INFO,
    YOUWIN,
    GAMEPLAY
};
