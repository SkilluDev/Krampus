using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ParentController : MonoBehaviour
{
    public float distance = 30;
    private NavMeshAgent navMeshAgent;
    private Detection[] parentDetections;

    private float waitTime = 1f;

    private Animator animator;

    // Start is called before the first frame update
    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        parentDetections = GetComponents<Detection>();
        animator = GetComponentInChildren<Animator>();




    }

    // Update is called once per frame
    private void Update()
    {

        if (WinCondition.Instance.isGamePausedValue()) return;
        var children = GameObject.FindGameObjectsWithTag("Child");
        foreach (GameObject child in children)
        {
            Detection detection = child.GetComponent<Detection>();
            if (!detection.isAlerted) continue;
            var vector = child.transform.position - transform.position;
            var sqrDistanceToChild = Vector3.SqrMagnitude(vector);
            if (sqrDistanceToChild <= distance && !Physics.Raycast(transform.position, vector.normalized, distance, 1 << 6))
            {
                child.GetComponent<ChildContoller>().Calming();
                if (!parentDetections[1].isAlerted)
                {
                    navMeshAgent.SetDestination(detection.krampusEncounerPosition);
                }

            }



        }
        animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);

        ChoosingTarget();
    }

    private void ChoosingTarget()
    {
        if (destinationReached() || navMeshAgent.destination == null)
        {

            waitTime -= Time.deltaTime;

            if (waitTime < 0)
            {

                GameObject[] children = GameObject.FindGameObjectsWithTag("Child");
                Vector3 descination = children[Random.Range(0, children.Length)].gameObject.transform.position;


                SetDestination(descination);

                waitTime = 10f;
            }

        }
        else
        {
            waitTime = 10f;
        }

    }
    private bool destinationReached()
    {
        if (!navMeshAgent.pathPending)
        {
            if (navMeshAgent.remainingDistance <= 15)
            {


                return true;

            }
        }
        return false;
    }

    private void SetDestination(Vector3 destination)
    {

        NavMesh.SamplePosition(destination, out NavMeshHit closestNavHit, 1000, NavMesh.AllAreas);
        navMeshAgent.SetDestination(closestNavHit.position);
    }

}
