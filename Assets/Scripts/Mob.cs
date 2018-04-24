using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : MonoBehaviour {

    public Vector3 velocity;
    public float health = 10f;
    public float damage = 2f;

    AudioSource src;

    public AudioClip damageClip;

    public Sprite dedSprite;

    public float walkInterval = 0.4f;

    public SpriteRenderer sr;

	// Use this for initialization
	void Start () {
        src = GetComponent<AudioSource>();
        sr = GetComponentInChildren<SpriteRenderer>();
        StartCoroutine(Walk());
	}

    void FixedUpdate() {
        transform.position += (velocity * Time.fixedDeltaTime);
    }

    public void TakeDamage(float amount) {
        health -= amount;

        float v = Random.Range(0.2f, 0.5f);
        float p = Random.Range(0.6f, 1.3f);

        src.pitch = p;
        src.PlayOneShot(damageClip, v);

        if (health <= 0) {
            StartCoroutine(Die());
            GameController.Instance.CollectCrap(1);
        }
    }

    IEnumerator Die() {
        //TODO: Fall backwards

        velocity = Vector3.zero;

        //disable collider
        transform.GetChild(0).GetComponent<Collider>().enabled = false;

        sr.sprite = dedSprite;

        for (int i = 0; i < 90; ++i) {
            transform.Rotate(-1f, 0f, 0f);
            yield return null;
        }

        Destroy(gameObject);
    }

    IEnumerator Walk() {
        while (true) {
            sr.flipX = !sr.flipX;

            if (health <= 0) break;

            yield return new WaitForSeconds(walkInterval);
        }
    }
}
