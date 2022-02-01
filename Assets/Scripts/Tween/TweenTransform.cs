using DG.Tweening;
using UnityEngine;

public class TweenTransform : MonoBehaviour
{

    public Vector3 scaleRatio = Vector3.one;
    public float time = 1.0f;

    public float randomRange = 1.0f;

    public bool isLoop;
    public int loopCount;
    public Ease easeType;
    public LoopType loopType;

    void Start()
    {
        Vector3 newScale = new Vector3(transform.localScale.x * scaleRatio.x, transform.localScale.y * scaleRatio.y, transform.localScale.z * scaleRatio.z);
        transform.DOScale(newScale, time).SetLoops(isLoop ? -1 : loopCount, loopType).SetEase(easeType).SetDelay(Random.Range(0f, randomRange));        
    }


}
