using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class InputBlock : MonoBehaviour
{

    [SerializeField]
    private int _inputValue;
    public Sprite[] sprites;

    private Collider2D _collider;
    private SpriteRenderer _renderer;

    public UnityEvent onClickEvent;

    void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    void Start(){
	_renderer.sprite = sprites[_inputValue];
        transform.DOScale(new Vector3(1.02f, 1.02f, 1.02f), 1f).SetLoops(-1, LoopType.Yoyo);
    }

    void Update()
    {
	if(isClicked()){
            onClickEvent?.Invoke();
        }
    }

    public int GetInputValue(){
        return _inputValue;
    }

    public void ChangeInputValue(int newValue){
        if(sprites[newValue] != null) {
            GetComponent<SpriteRenderer>().sprite = sprites[newValue];
            // _renderer.sprite = sprites[newValue];
            _inputValue = newValue;
        }
    }

    public bool isClicked(){
        if(Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began){
            Touch touch = Input.touches[0];
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(touch.position);

	    if(_collider == Physics2D.OverlapPoint(worldPoint)) {
                return true;
            }

            return false;
        }
        return false;	
    }

}
