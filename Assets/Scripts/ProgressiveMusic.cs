using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressiveMusic : MonoBehaviour
{
    public float minDistance;
    childDistance closestDist;

    float distanceToClosest;

    private void Start()
    {
        SoundManager.PlaySound("windup1");
        closestDist = GameObject.Find("Player").GetComponent<childDistance>();
        transform.GetChild(0).GetComponent<AudioSource>().volume = 0;
    }

    private void Update()
    {
        distanceToClosest = Mathf.Sqrt(closestDist.dist);
        if (distanceToClosest < minDistance)
        {
            transform.GetChild(0).GetComponent<AudioSource>().volume = (minDistance - distanceToClosest)/20;
        }
        else
        {
            transform.GetChild(0).GetComponent<AudioSource>().volume = 0;
        }
    }
}
