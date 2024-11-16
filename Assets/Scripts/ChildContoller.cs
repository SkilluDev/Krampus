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
        if(distance>runDistance) return;
        Vector3 runDestination = transform.position + offset;
        navMeshAgent.SetDestination(runDestination);
    }
}
