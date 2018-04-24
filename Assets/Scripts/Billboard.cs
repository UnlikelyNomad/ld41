using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {
	
    public Vector3 velocity;

    public Vector3 up;

    void Start() {
    }

	// Update is called once per frame
	void Update () {
        Vector3 viewer = Camera.main.transform.position;
        Vector3 diff = viewer - transform.position;

        //Vector3 f = Camera.main.transform.forward;
        //up = velocity - ((Vector3.Dot(velocity, f) / Vector3.Dot(f, f)) * f);

        transform.rotation = Quaternion.LookRotation(diff, -velocity);
	}
}
