using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelTilt : MonoBehaviour {

    public float factor = 10f;
    public float lerp = 0.01f;

    Vector3 lastPosition;
    Vector3 lastVelocity;

    public Vector3 currentAcceleration;
    public Vector3 currentVelocity;
    public Vector3 currentAngle;
    public Vector3 lastAcceleration;
	
	// Update is called once per frame
	void FixedUpdate () {
        currentVelocity = (transform.position - lastPosition) / Time.fixedDeltaTime;
        currentAcceleration = currentVelocity - lastVelocity;

        lastPosition = transform.position;
        lastVelocity = currentVelocity;

        lastAcceleration = 0.5f * lastAcceleration + 0.5f * currentAcceleration;

        currentAngle = Vector3.Lerp(currentAngle, lastAcceleration * factor, lerp);

        transform.rotation = Quaternion.Euler(currentAngle.z, 0, -currentAngle.x);
	}
}
