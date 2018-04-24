using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public Vector3 velocity;
    public float time;
    public float damage;

    public AudioClip boom;

    bool explode = false;

	// Use this for initialization
	void Start () {
        StartCoroutine(Die());
        Billboard b = GetComponentInChildren<Billboard>();

        if (b != null) {
            b.velocity = velocity;
        }
	}

    IEnumerator Die() {
        yield return new WaitForSeconds(time);

        if (!explode) {
            Destroy(gameObject);
        }
    }

    void FixedUpdate() {
        transform.position += velocity * Time.fixedDeltaTime;
    }

    IEnumerator Explode() {
        explode = true;
        transform.GetChild(0).gameObject.SetActive(false);
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other) {
        Mob m = other.GetComponentInParent<Mob>();

        m.TakeDamage(damage);

        StartCoroutine(Explode());
    }
}
