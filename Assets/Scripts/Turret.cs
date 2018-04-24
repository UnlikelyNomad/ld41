using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour {

    public float range = 10f;
    public float fireDelay = 2f;

    public GameObject projectilePrefab;

    public Mob currentTarget;

    public List<Mob> inRange = new List<Mob>();

    public Transform turret;

    public Transform emitter;

    public float projectileSpeed = 5f;
    public float projectileTime = 2f;
    public float damage = 1f;

    public AudioClip shoot;
    public AudioSource src;

	// Use this for initialization
	void Start () {
        StartCoroutine(Fire());

        src = GetComponent<AudioSource>();
	}

    IEnumerator Fire() {
        while (true) {
            yield return new WaitForSeconds(fireDelay);

            if (currentTarget != null) {
                Shoot();
            }
        }
    }

    void Shoot() {
        GameObject go = (GameObject)Instantiate(projectilePrefab, emitter.position, emitter.rotation);
        Projectile p = go.GetComponent<Projectile>();

        Vector3 v = currentTarget.transform.position - turret.transform.position;
        v.Normalize();
        v *= projectileSpeed;

        p.velocity = v;
        p.time = projectileTime;
        p.damage = damage;

        src.PlayOneShot(shoot);
    }

    void FixedUpdate() {
        if (inRange.Count == 0) {
            return;
        }

        currentTarget = inRange[0];

        if (currentTarget == null) {
            inRange.RemoveAt(0);
            return;
        }

        //aim at mob
        Vector3 position = currentTarget.transform.position;
        //Debug.Log(position);
        //Debug.Log(transform.position);
        Vector3 diff = position - turret.transform.position;
        //Debug.Log(diff);
        Quaternion rot = Quaternion.LookRotation(diff, Vector3.up);
        turret.rotation = rot;
    }

    void OnTriggerEnter(Collider other) {
        Mob m = other.GetComponentInParent<Mob>();
        inRange.Add(m);
    }

    void OnTriggerExit(Collider other) {
        Mob m = other.GetComponentInParent<Mob>();
        inRange.Remove(m);
    }
}
