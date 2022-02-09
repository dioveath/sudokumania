using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public GameObject dialogPanel;
    public Button yesButton;
    public Button noButton;
    public Text headerText;
    public Text bodyText;
    public Image _panelImage;

    private bool _isActive = false;
    private bool _isAnimating = false;
    private Vector2 _defaultSizeDelta;
    private Vector2 _defaultAnchoredPosition;

    [SerializeField]
    public Dictionary<string, DialogData> dialogDict;

    private static DialogManager _instance;
    public static DialogManager Instance { get
        {
            if (_instance != null)
            {
                return _instance;
            }
            else
            {
                Debug.LogError("No Instance of DialogManager!!!");
                return null;
            }
        }
        private set { }
    }

    void Awake(){
	if(_instance != null) {
            DestroyImmediate(this.gameObject);
        }
        _instance = this;

        dialogPanel.transform.localScale = Vector3.zero;
    }

    void Start(){
        _defaultSizeDelta = yesButton.GetComponent<RectTransform>().sizeDelta;
        _defaultAnchoredPosition = yesButton.GetComponent<RectTransform>().anchoredPosition;	
        dialogPanel.SetActive(false);
    }

    public void ShowDialog(string dialogName){
	if(dialogDict.TryGetValue(dialogName, out DialogData data)){
            ShowDialog(data);
        } else {
            Debug.LogWarning("Not dialog with name '" + dialogName + "'!!!");
        }
    }

    public void ShowDialog(DialogData dialogData){
	if(_isActive || _isAnimating) return;

        yesButton.GetComponentInChildren<Text>().text = dialogData.yesText;
        noButton.GetComponentInChildren<Text>().text = dialogData.noText;
        headerText.text = dialogData.headerText;
        bodyText.text = dialogData.bodyText;
        yesButton.onClick.AddListener(dialogData.Yes == null ? () => {  } : dialogData.Yes);

        RectTransform parentRect = yesButton.transform.parent.GetComponent<RectTransform>();
        RectTransform yesRect = yesButton.GetComponent<RectTransform>();

        if(dialogData.isOneButton) {
            float prevDeltaX = yesRect.sizeDelta.x;
            yesRect.sizeDelta = new Vector2(parentRect.sizeDelta.x, yesRect.sizeDelta.y);
            yesRect.anchoredPosition = new Vector2(yesRect.anchoredPosition.x + prevDeltaX/2, yesRect.anchoredPosition.y);
            noButton.gameObject.SetActive(false);
        } else {
            // yesRect.anchoredPosition = _defaultAnchoredPosition;
            // yesRect.sizeDelta = _defaultSizeDelta;
            noButton.onClick.AddListener(dialogData.No);
            noButton.gameObject.SetActive(true);	    
	}

        dialogPanel.SetActive(true);
        _panelImage.DOColor(new Color(0, 0, 0, 1/255f * 87), 0.7f).OnComplete(() => _panelImage.enabled = true);

        _isAnimating = true;
        dialogPanel.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.7f).SetEase(Ease.InOutElastic).OnComplete(() => {
	    _isActive = true;
            _isAnimating = false;
        });;
    }

    public void HideDialog(){
	if(!_isActive || _isAnimating) return;
        _isAnimating = true;

        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        _panelImage.DOColor(new Color(0, 0, 0, 0), 0.7f).OnComplete(() => _panelImage.enabled = false);	
        dialogPanel.transform.DOScale(new Vector3(0f, 0f, 0f), 0.7f).SetEase(Ease.InOutElastic).OnComplete(() => {
	    _isActive = false;
            _isAnimating = false;
            dialogPanel.SetActive(false);

            RectTransform parentRect = yesButton.transform.parent.GetComponent<RectTransform>();
	    RectTransform yesRect = yesButton.GetComponent<RectTransform>();

            yesRect.anchoredPosition = _defaultAnchoredPosition;
            yesRect.sizeDelta = _defaultSizeDelta;	    
	});
    }

    public void StartDialogSequence(DialogSequence sequence, UnityAction onComplete = null) {
        Queue<string> sentenceQueue = new Queue<string>(sequence.sentences);
        DialogData sequenceData = new DialogData(sequence.title, sentenceQueue.Dequeue(), "Continue", "", null, true, null);
        sequenceData.Yes = (() =>
        {
	    if(sentenceQueue.Count != 0) {
		string text = sentenceQueue.Dequeue();
                bodyText.DOText(text, 0.5f);
            } else {
		if(onComplete == null)
		    HideDialog();
		else
                    onComplete?.Invoke();
            }
        });
        ShowDialog(sequenceData);
    }

}

[Serializable]
public class DialogData {
    public string headerText;
    public string bodyText;
    public string yesText;
    public string noText;
    public UnityAction Yes;
    public UnityAction No;
    public bool isOneButton;

    public DialogData(string __headerText,
                      string __bodyText,
                      string __yesText,
                      string __noText,
                      UnityAction __yes = null,
		      bool __isOneButton = false,
		      UnityAction __no = null) {
        this.headerText = __headerText;
        this.bodyText = __bodyText;
        this.yesText = __yesText;
        this.noText = __noText;
        this.Yes = __yes;
        this.isOneButton = __isOneButton;
        this.No = __no;
    }
}

[Serializable]
public class DialogSequence {
    public string title;
    [TextArea(3, 10)]
    public string[] sentences;
}
