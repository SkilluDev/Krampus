using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ParentController : MonoBehaviour
{
    public float distance = 30;
    private NavMeshAgent navMeshAgent;
    [SerializeField] private WinCondition winCondition;
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (winCondition.isGamePausedValue())  return;
        var children = GameObject.FindGameObjectsWithTag("Child");
        foreach(GameObject child in children){
            detection detection = child.GetComponent<detection>();
            if(!detection.isAlerted) continue;
            var vector = child.transform.position - transform.position;
            var sqrDistanceToChild = Vector3.SqrMagnitude(vector);
            if (sqrDistanceToChild <= distance && !Physics.Raycast(transform.position, vector.normalized, distance*30,1<<6)) { 
                detection.isAlerted = false;
                navMeshAgent.SetDestination(detection.krampusEncounerPosition);
            }
        }
    }
}
