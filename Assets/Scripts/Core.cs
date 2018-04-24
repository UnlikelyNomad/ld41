using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour {

    public float health = 100f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void TakeDamage(float damage) {
        health -= damage;
        if (health <= 0) {
            health = 0;

            Time.timeScale = 0;
        }
    }
}
