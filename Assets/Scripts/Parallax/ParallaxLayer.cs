using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{

    public SpriteRenderer[] renderers;
    public float scrollSpeed = 1.0f;

    void Start(){
        renderers = GetComponentsInChildren<SpriteRenderer>();
    }

}
