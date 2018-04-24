using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour {

    public int targetDir;
    public float targetStation;
    public Vector2 targetPos;
    public bool moving = false;
    public int currentDir = 0;
    public Vector2 velocity;

    public float speed = 6f;
    public float velLerp = 0.2f;

    public float altitude = 0.8f;

    public bool sameHallway = false;

    public float dist = 0;

    public bool home = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other) {
        if (!moving) {
            //dead!

            GameController.Instance.DestroyDrone(this);
            Destroy(gameObject);
        }
    }

    void FixedUpdate() {
        if (moving) {
            Move();
        }
    }

    void Move() {
        if (home) {
            if (MoveTo(Vector3.zero)) {
                moving = false;
                home = false;
            }
        } else {
            sameHallway = currentDir == targetDir;
            if (!sameHallway) {
                //move towards center
                if (MoveTo(Vector3.zero)) {
                    moving = true;
                }
            } else {
                //move to station in hallway
                MoveTo(new Vector3(targetPos.x, 0, targetPos.y));
            }
        }
    }

    bool MoveTo(Vector3 target) {
        Vector2 cur = new Vector2(transform.position.x, transform.position.z);
        Vector2 targ = new Vector2(target.x, target.z);

        Vector2 diff = targ - cur;
        dist = diff.magnitude;
        Vector2 v = diff;

        velocity = Vector2.Lerp(velocity, v.normalized * speed, velLerp);

        transform.position += new Vector3(velocity.x, 0, velocity.y) * Time.fixedDeltaTime;

        if (dist < 0.2f) {
            currentDir = targetDir;
            target.y = transform.position.y;
            transform.position = target;
            velocity = Vector2.zero;
            moving = false;
            return true;
        }

        return false;
    }

    public void Home() {
        home = true;
        moving = true;
    }

    public void Go(int dir, int station) {
        targetDir = dir;
        targetStation = GameController.StationDist(station);
        float x = 0, y = 0;

        switch (dir) {
        case 0: //north
            y = targetStation;
            break;

        case 2: //south
            y = -targetStation;
            break;

        case 1: //east
            x = targetStation;
            break;

        case 3: //west
            x = -targetStation;
            break;
        }
        targetPos.x = x;
        targetPos.y = y;

        home = false;
        moving = true;
    }
}
