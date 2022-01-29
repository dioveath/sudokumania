using DG.Tweening;
using UnityEngine;

public class AnimateUIButton : MonoBehaviour
{

    public Vector3 scaleTo;
    public float time = 1.0f;

    public bool isLoop;
    public int loopCount;
    public LoopType loopType;

    void Start()
    {
        transform.DOScale(scaleTo, time).SetLoops(isLoop ? -1 : loopCount, loopType);        
    }


}
