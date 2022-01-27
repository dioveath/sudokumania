using UnityEngine;

public class SudokuManager : MonoBehaviour
{
    private NumberBlock[,] _currentLayout;
    private NumberBlock _activeNumberBlock;
    private int _activeY;
    private int _activeX;

    private SudokuLevel _currentLevel;
    private float _timeElapsed = 0.0f;
    private bool _levelRunning = false;

    private bool _isInputDisabled = false; 

    private static SudokuManager _instance;
    // private FirebaseDatabase _dbRef;

    void Awake(){
        _instance = this;
    }

    public static SudokuManager Instance(){
        return _instance;
    }

    public void Init(SudokuLevel sudokuLevel){
        _currentLayout = SudokuLayoutGenerator.Instance().GenerateSudokuLayout();
        InputManager.Instance().GenerateInputBlocks();
        InputManager.Instance().inputEvent.AddListener(OnInput);

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
        SetInputActive(true);
    }

    public void Finish(){
        Debug.Log("Cleaning up the game...!");
        _levelRunning = false;
        foreach(NumberBlock block in _currentLayout) {
            Destroy(block.gameObject);
	}
        _currentLayout = null;
        InputManager.Instance().inputEvent.RemoveAllListeners();
        InputManager.Instance().DestroyInputBlocks();
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
		    AudioManager.Instance().PlayAudio("click_basic");

                    if(_activeNumberBlock != null) 
                        _activeNumberBlock.setActive(false);

		    if(_activeNumberBlock == numberBlock) {
                        _activeNumberBlock.setActive(false);
                        _activeNumberBlock = null;
                        continue;
                    }

                    _activeNumberBlock = numberBlock;
                    _activeNumberBlock.setActive(true);
                    _activeX = j;
                    _activeY = i;
                }

            }
	}
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

    void OnInput(int inputValue){
	if(_isInputDisabled) return;

        if(_activeNumberBlock != null){
            _activeNumberBlock.ChangeNumber(inputValue);
            _currentLevel.inputSudokuArray[_activeY, _activeX] = inputValue;

	    if(IsValidPlacement(inputValue, _activeY, _activeX)) {
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
            AudioManager.Instance().PlayAudio("applause6");
            GameManager.Instance().SwitchState(GameState.YOUWIN);

            AdsManager.Instance.ShowInterstitial();

            _levelRunning = false;
        }
    }

    bool IsValidPlacement(int inputValue, int y, int x){
	return _currentLevel.validSolution[y, x] == inputValue;
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


