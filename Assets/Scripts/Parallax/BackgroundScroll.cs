using System;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    [SerializeField]
    public ParallaxLayer[] layers;

    [SerializeField]
    private Vector3[] worldCameraBounds;
    [SerializeField]
    private Vector3 _rendererBounds;

    void Start(){
        worldCameraBounds = new Vector3[2];
        worldCameraBounds[1] = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        worldCameraBounds[0] = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
    }


    void Update()
    {

        for (int i = 0; i < layers.Length; i++){
            ParallaxLayer layer = layers[i];

            for (int j = 0; j < layer.renderers.Length; j++){
                SpriteRenderer renderer = layer.renderers[j];
                Transform t = renderer.transform;

		t.Translate(new Vector3(layer.scrollSpeed * Time.fixedDeltaTime, 0, 0), Space.World);
		if(renderer.bounds.min.x > worldCameraBounds[1].x) {
		    t.position = new Vector3(layer.renderers[(j+1) % layer.renderers.Length].bounds.min.x - renderer.bounds.size.x / 2 + layer.scrollSpeed * Time.fixedDeltaTime, t.position.y, t.position.z);
		}

            }
        }

    }

}
