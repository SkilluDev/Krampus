using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ParentController : MonoBehaviour
{
    public float distance = 30;
    private NavMeshAgent navMeshAgent;
    private detection[] parentDetections;



    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        parentDetections = GetComponents<detection>();
        animator = GetComponentInChildren<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        
        if (WinCondition.Instance.isGamePausedValue())  return;
        var children = GameObject.FindGameObjectsWithTag("Child");
        foreach(GameObject child in children){
            detection detection = child.GetComponent<detection>();
            if(!detection.isAlerted) continue;
            var vector = child.transform.position - transform.position;
            var sqrDistanceToChild = Vector3.SqrMagnitude(vector);
            if (sqrDistanceToChild <= distance && !Physics.Raycast(transform.position, vector.normalized, distance,1<<6)) { 
                detection.isAlerted = false;
                if (!parentDetections[1].isAlerted)
                {
                    navMeshAgent.SetDestination(detection.krampusEncounerPosition);
                }
                
            }
            
        }
        animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
    }
}
