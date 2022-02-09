using UnityEngine;
using UnityEngine.Events;

public class InputEvent : UnityEvent<int> { }

public class InputManager : MonoBehaviour
{

    public float blockSize = 1f;
    public float blockGap = 0.2f;
    public GameObject inputBlockPrefab;
    public Vector2 offsetPosition;

    [SerializeField]
    private string holderName = "InputHolder";

    private InputBlock[] _inputBlocks;
    public InputEvent inputEvent;

    private static InputManager _instance;
    public static InputManager Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("Error: No Input Manager!");
            return _instance;
        } private set {}
    }

    void Awake(){
	if(_instance != null)
            DestroyImmediate(this.gameObject);
        _instance = this;
    }

    void Start()
    {
        inputEvent = new InputEvent();
    }

    public void DestroyInputBlocks(){
	if(transform.Find(holderName)) {
            DestroyImmediate(transform.Find(holderName).gameObject);	    
	}
    }

    public void GenerateInputBlocks(){
        DestroyInputBlocks();

        Transform inputHolder = new GameObject(holderName).transform;
        inputHolder.parent = transform;

        _inputBlocks = new InputBlock[10];

        for(int i = 0; i < 2; i++){
            for (int j = 0; j < 5; j++){
                Vector3 newPos = new Vector3(offsetPosition.x + (j - 2) * (blockSize/2f + blockGap),
					     offsetPosition.y + (i - 2) * (blockSize/2f + blockGap),
					     -3);
                InputBlock inputBlock = Instantiate(inputBlockPrefab, newPos, Quaternion.identity).GetComponent<InputBlock>();
                inputBlock.transform.localScale = new Vector3(blockSize, blockSize, 1);
                inputBlock.ChangeInputValue(i * 5 + j);
                inputBlock.transform.parent = inputHolder;
                _inputBlocks[i * 5 + j] = inputBlock;
            }
	}	
    }

    void Update()
    {
        if(_inputBlocks == null) return;

        for (int i = 0; i < 10; i++){
            var inputBlock = _inputBlocks[i];
	    if(inputBlock.isClicked()) {
                inputEvent?.Invoke(inputBlock.GetInputValue());
            }
        }
    }

    public InputBlock GetInputBlock(int number){
	return _inputBlocks[number];
    }

}
