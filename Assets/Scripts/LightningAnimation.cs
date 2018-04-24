using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningAnimation : MonoBehaviour {

    public SpriteRenderer sprite;
    public Light flash;

    public float flipChance = 0.82f;
    public float flashChance = 0.7f;

	// Use this for initialization
	void Start () {
        sprite = GetComponentInChildren<SpriteRenderer>();
        flash = GetComponentInChildren<Light>();
	}
	
	// Update is called once per frame
	void Update () {
        float r = Random.Range(0f, 1f);
        if (r > flipChance) {
            sprite.flipY = !sprite.flipY;
        }

        if (r > flashChance) {
            flash.enabled = !flash.enabled;
        }
	}
}
