using UnityEngine;


public class NumberBlock : MonoBehaviour
{
    private Collider2D _collider;
    private SpriteRenderer _renderer;
    private bool _isActive;
    private bool _isEditable = true;
    private bool _isValid = false;
    private int _currentValue = 0;

    [Header("Number Block Colors")]
    public Color activeColor;
    public Color validColor;
    public Color invalidColor;
    public Color disabledColor;

    public Sprite[] sprites;

    void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _renderer = GetComponent<SpriteRenderer>();
        ChangeNumber(0);
    }

    void Update()
    {
        if (_isEditable)
        {
            if (_isActive)
            {
                _renderer.color = activeColor;
            } else {
		if(_isValid)
		{
                    _renderer.color = validColor;
                } else {
                    _renderer.color = invalidColor;
                }

		// If No Input
		if(_currentValue == 0)
                    _renderer.color = Color.white;
            } 
        } else {
            _renderer.color = disabledColor;
        }
    }

    public void ChangeNumber(int newValue){
	if(!_isEditable) return;
        _currentValue = newValue;
        GetComponent<SpriteRenderer>().sprite = sprites[newValue];
        // if(sprites[newValue] != null)
        //     _renderer.sprite = sprites[newValue];	
    }

    public void setEditable(bool editable){
        _isEditable = editable;
    }

    public void setActive(bool active){
	if(!_isEditable) return;
        _isActive = active;
    }

    public void setValid(bool valid){
	if(!_isEditable) return;
        _isValid = valid;
    }

    public bool isTouched(){
	if(!_isEditable) return false;
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
