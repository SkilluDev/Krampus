using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressiveMusic : MonoBehaviour {
    public float minDistance;
    private ChildDistance closestDist;

    private AudioSource childTrack;
    public float resetSpeed = 1;

    private float distanceToClosest;

    private void Start() {
        closestDist = GameObject.Find("Player").GetComponent<ChildDistance>();
        childTrack = transform.GetChild(0).GetComponent<AudioSource>();
        childTrack.volume = 0;
    }

    private void Update() {
        distanceToClosest = Mathf.Sqrt(closestDist.dist);
        if (distanceToClosest < minDistance) {
            childTrack.volume = (minDistance - distanceToClosest) / 20;
        } else {
            if (childTrack.volume > 0) {
                childTrack.volume -= Time.deltaTime * resetSpeed;
            }
        }
    }
}
