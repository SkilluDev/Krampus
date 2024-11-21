using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressiveMusic : MonoBehaviour
{
    public float minDistance;
    childDistance closestDist;

    AudioSource childTrack;
    public float resetSpeed = 1;

    float distanceToClosest;

    private void Start()
    {
        closestDist = GameObject.Find("Player").GetComponent<childDistance>();
        childTrack = transform.GetChild(0).GetComponent<AudioSource>();
        childTrack.volume = 0;
    }

    private void Update()
    {
        distanceToClosest = Mathf.Sqrt(closestDist.dist);
        if (distanceToClosest < minDistance)
        {
            childTrack.volume = (minDistance - distanceToClosest)/20;
        }
        else
        {   if(childTrack.volume > 0){
                childTrack.volume -= Time.deltaTime*resetSpeed;
            }
        }
    }
}
