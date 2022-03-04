using UnityEngine;

public class SudokuManager : MonoBehaviour
{
    private NumberBlock[,] _currentLayout;
    private NumberBlock _activeNumberBlock;

    private SudokuLevel _currentLevel;
    private float _timeElapsed = 0.0f;
    private bool _levelRunning = false;

    private bool _isInputDisabled = false; 

    private static SudokuManager _instance;
    public NumberBlock tutorialBlock;
    public InputBlock activeBlockAnswerInputBlock;

    void Awake(){
        _instance = this;
    }

    public static SudokuManager Instance(){
        return _instance;
    }

    public void Init(SudokuLevel sudokuLevel){
        _currentLayout = SudokuLayoutGenerator.Instance().GenerateSudokuLayout();
        InputManager.Instance.GenerateInputBlocks();
        InputManager.Instance.inputEvent.AddListener(OnInput);

        SudokuLevel loadedLevel = Player.Instance.GetPlayingSudokuLevel(sudokuLevel.id);
        if(loadedLevel != null){ // if there is previous save
	    LoadSudokuLevel(loadedLevel);
            _timeElapsed = loadedLevel.lastElapsedTime;
        } else {
	    LoadSudokuLevel(sudokuLevel);	    
	    _timeElapsed = 0.0f;
	}

        AdsManager.Instance.RequestInterstitial();
        _levelRunning = true;

        if (_currentLevel.id == "-MtvPvTVpipaRbX5lmlm" && Player.Instance.playerData.isFirstTime)
        {
            bool tutorialBlockSet = false;
            for (int i = 0; i < 9; i++)
            {
                if (tutorialBlockSet) break;
                for (int j = 0; j < 9; j++)
                {
                    NumberBlock block = _currentLayout[i, j];
                    if (block.isEditable())
                    {
                        tutorialBlock = block;
                        tutorialBlockSet = true;
                        tutorialBlock.location.x = j;
                        tutorialBlock.location.y = i;
                        // _activeX = j;
                        // _activeY = i;
                        break;
                    }
                }
            }
        }
    }

    public void Finish(){
        Debug.Log("Cleaning up the game...!");
        _levelRunning = false;
        foreach(NumberBlock block in _currentLayout) {
            Destroy(block.gameObject);
	}
        _currentLayout = null;
        InputManager.Instance.inputEvent.RemoveAllListeners();
        InputManager.Instance.DestroyInputBlocks();
    }
    
 
    void Update()
    {
	if(_levelRunning)
	    _timeElapsed += Time.deltaTime;

        if(_isInputDisabled) return;
        if(_currentLayout == null || _currentLayout.Length == 0) return;

        for (int i = 0; i < 9; i++){
            for (int j = 0; j < 9; j++){
                NumberBlock numberBlock = _currentLayout[i, j];
                if (numberBlock.isTouched()){
                    ProcessTouchBlock(numberBlock);
                }
            }
	}
    }

    public void ProcessTouchBlock(NumberBlock block){
	AudioManager.Instance().PlayAudio("click_basic");

	if(_activeNumberBlock != null) 
	    _activeNumberBlock.setActive(false);

	if(_activeNumberBlock == block) {
	    _activeNumberBlock.setActive(false);
	    _activeNumberBlock = null;
            return;
        }

	_activeNumberBlock = block;
	_activeNumberBlock.setActive(true);
    }

    public void ReloadLevel(){
        Init(_currentLevel);

        for (int i = 0; i < 9; i++){
            for (int j = 0; j < 9; j++){
        	if(_currentLevel.sudokuArray[i, j] == 0){
                    _currentLevel.inputSudokuArray[i, j] = 0;
        	    // NumberBlock block = _currentLayout[i, j].GetComponent<NumberBlock>();
                    // block.ChangeNumber(0);
                }
            }
        }	
    }

    public void SetInputActive(bool active){
        _isInputDisabled = !active;
    }

    public void OnInput(int inputValue){
	if(_isInputDisabled) return;

        if(_activeNumberBlock != null){
            Vector2Int loc = new Vector2Int(_activeNumberBlock.location.x, _activeNumberBlock.location.y); _activeNumberBlock.ChangeNumber(inputValue);
            _currentLevel.inputSudokuArray[loc.y, loc.x] = inputValue;

	    if(IsValidPlacement(inputValue, loc.y, loc.x)) {
                _activeNumberBlock.setValid(true);
		AudioManager.Instance().PlayAudio("click_wooden2");		
	    } else {
                _activeNumberBlock.setValid(false);
		AudioManager.Instance().PlayAudio("click_heavy");		
	    }
	    
            _activeNumberBlock.setActive(false);
            _activeNumberBlock = null;
        }

	if(SudokuUtils.isSudokuValid(_currentLevel.inputSudokuArray)) {
            OnWin();
        }
    }

    void OnWin(){
	AudioManager.Instance().PlayAudio("applause6");
	GameManager.Instance().SwitchState(GameState.YOUWIN);

	AdsManager.Instance.ShowInterstitial();

        Player.Instance.playerData.lastCompletedIndex = _currentLevel.index + 1;
        Player.Instance.SaveCurrentPlayerData();

        _levelRunning = false;
    }

    bool IsValidPlacement(int inputValue, int y, int x){
	return _currentLevel.validSolution[y, x] == inputValue;
    }

    public int GetValidPlacement(int y, int x){
        return _currentLevel.validSolution[y, x];
    }

    public InputBlock GetTutorialInputBlock(){
	if(tutorialBlock == null) {
            Debug.LogError("No Tutorial Block!");
            return null;
        }
        return InputManager.Instance.GetInputBlock(GetValidPlacement(tutorialBlock.location.y, tutorialBlock.location.x));
    } 

    void LoadSudokuLevel(SudokuLevel sudokuLevel){
	if(_currentLayout == null || _currentLayout.Length == 0) {
            Debug.LogWarning("Layout is not loaded yet!");
            return;
	}

        _currentLevel = sudokuLevel.Copy();

        for (int i = 0; i < 9; i++){
            for (int j = 0; j < 9; j++){
                NumberBlock block = _currentLayout[i, j].GetComponent<NumberBlock>();
		if(_currentLevel.sudokuArray[i,j] != 0){
		    block.ChangeNumber(_currentLevel.sudokuArray[i, j]);
                    block.setEditable(false);
                    continue;
                }
		if(_currentLevel.inputSudokuArray[i, j] != 0){
                    block.ChangeNumber(_currentLevel.inputSudokuArray[i, j]);
		    if(IsValidPlacement(_currentLevel.inputSudokuArray[i, j], i, j)) {
			block.setValid(true);
		    }
                }
            }
	}
    }

    public void PauseSudokuGame(){
        SetInputActive(false);
        _levelRunning = false;
    }

    public void ResumeSudokuGame(){
        SetInputActive(true);
        _levelRunning = true;	
    }

    public float GetTimeElapsed(){
        return _timeElapsed;
    }

    public int GetCurrentLevelPoint(){
        return _currentLevel.points;
    }

    public SudokuLevel GetCurrentSudokuLevel(){
        return _currentLevel;
    }

}
