using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{

    private SpriteRenderer _renderer;
    public Sprite[] bgSprites;

    void Start(){
        _renderer = GetComponent<SpriteRenderer>();
        Debug.Log(_renderer.bounds.min);
        Debug.Log(_renderer.bounds.max);	
    }


    void Update()
    {
	// _renderer.bounds
    }
}
