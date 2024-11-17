using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildContoller : MonoBehaviour
{
    private GameObject krampus;
    private Rigidbody rigidBody;
    private UnityEngine.AI.NavMeshAgent navMeshAgent;
    private detection detection;
    public float runDistance = 30;
    

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.PlaySound("windup1");
        krampus = GameObject.FindWithTag("Player");
        rigidBody = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        detection = GetComponent<detection>();
    }

    // Update is called once per frame
    void Update()
    {
        if(detection.isAlerted) runAway();
    }

    void runAway(){
        Vector3 offset = transform.position - krampus.transform.position;
        float distance = offset.magnitude;
        if(distance>runDistance) {runToParent(); return;}
        Vector3 runDestination = transform.position + offset;
        navMeshAgent.SetDestination(runDestination);
    }

    void runToParent(){
        var parents = GameObject.FindGameObjectsWithTag("Parent");
        if(parents.Length == 0) {
            Debug.Log("No parents on map");
            return;
        }

        float dist = float.PositiveInfinity;
        Vector3 closestParent = new Vector3();

        foreach (GameObject parent in parents)
        {
            Vector3 parentPosition = parent.transform.position;
            Vector3 offset = parentPosition - transform.position;
            float sqrLen = offset.sqrMagnitude;

            if (sqrLen < dist)
            {
                dist = sqrLen;
                closestParent = parentPosition;
            }
        }
        navMeshAgent.SetDestination(closestParent);
    }
}
