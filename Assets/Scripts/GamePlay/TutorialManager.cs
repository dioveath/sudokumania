using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialObject;

    public Transform toClick;
    public GameObject handGesture;
    public GameObject pointPrompt;

    private Tween _handTween;
    private Tween _pointTween;

    private bool _isTutorialActive;

    private NumberBlock _promptBlock;
    private UnityAction _onPromptComplete;

    private InputBlock _inputBlock;
    private UnityAction _onInputComplete;

    private static TutorialManager _instance;
    public static TutorialManager Instance {
	get {
	    if(_instance == null)
                Debug.LogWarning("No Tutorial Manager!");
            return _instance;
        }
    }

    void Awake(){
	if(_instance != null){
            DestroyImmediate(this.gameObject);
        }
        _instance = this;
    }

    void Update(){
	if(_isTutorialActive){
	    if(_promptBlock != null && _promptBlock.isTouched()){
                _onPromptComplete?.Invoke();
                _isTutorialActive = false;
                tutorialObject.SetActive(false);
                _handTween.Kill();
                _pointTween.Kill();
                _promptBlock = null;
            }
	    if(_inputBlock != null && _inputBlock.isClicked()){
                _onInputComplete?.Invoke();
                _isTutorialActive = false;
                tutorialObject.SetActive(false);
                _handTween.Kill();
                _pointTween.Kill();
                _inputBlock = null;		
            }
	}
    }

    public void PromptNumberBlock(NumberBlock block, UnityAction onTouched){
        _isTutorialActive = true;
        _promptBlock = block;
        _onPromptComplete = onTouched;
        tutorialObject.SetActive(true);

        Vector3 blockPosition = block.GetComponent<SpriteRenderer>().bounds.center;
        Vector2 blockSize = block.GetComponent<SpriteRenderer>().bounds.size;
        pointPrompt.transform.position = new Vector3(blockPosition.x, blockPosition.y, -3f);

        Vector2 handSize = handGesture.GetComponent<SpriteRenderer>().size;
        handGesture.transform.position = new Vector3(blockPosition.x - blockSize.x/2, blockPosition.y - handSize.y/2 - 0.2f, -3f);

        _handTween = handGesture.transform.DOMoveY(blockPosition.y - handSize.y/2, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InSine);
        _pointTween = pointPrompt.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.OutSine);
    }

    public void PromptInputBlock(InputBlock inputBlock, UnityAction onTouched){
        _isTutorialActive = true;
        _inputBlock = inputBlock;
        _onInputComplete = onTouched;
        tutorialObject.SetActive(true);

        Vector3 blockPosition = inputBlock.GetComponent<SpriteRenderer>().bounds.center;
        Vector2 blockSize = inputBlock.GetComponent<SpriteRenderer>().bounds.size;
        pointPrompt.transform.position = new Vector3(blockPosition.x, blockPosition.y, -5f);

	Vector2 handSize = handGesture.GetComponent<SpriteRenderer>().size;
        handGesture.transform.position = new Vector3(blockPosition.x - blockSize.x/2, blockPosition.y - handSize.y/2 - 0.2f, -3f);

        _handTween = handGesture.transform.DOMoveY(blockPosition.y - handSize.y/2, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InSine);
        _pointTween = pointPrompt.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.OutSine);	
    }


}
