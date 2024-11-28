using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChildContoller : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent navMeshAgent;
    private detection detection;
    private Vector3 initialPosition;
    private Animator animator;
    public float speed;
    public bool isDummy = false;

    private enum State{
        Panicing,
        Stopped,
        Walking,
        Running,
    }
    private State state;
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        //if(!isDummy) ResetChildDestination();
        initialPosition = transform.position;
        //navMeshAgent.stoppingDistance = 0.1f;
        detection = GetComponent<detection>();
        animator = GetComponentInChildren<Animator>();
        speed = navMeshAgent.speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!WinCondition.Instance.isGamePausedValue() && !isDummy)
        {
            if (detection.isAlerted) RunToParent();
            else
            {
                navMeshAgent.SetDestination(initialPosition);
            }
            UpdateState();
        }
    }

    void UpdateState(){
        State newState;
        bool moving = navMeshAgent.velocity.sqrMagnitude > 0;
        if(detection.isAlerted && moving) newState = State.Running;
        else if (detection.isAlerted) newState = State.Panicing;
        else if (moving) newState = State.Walking;
        else newState = State.Stopped;
        if(newState==state) return;
        state = newState;
        StateChanged();
    }
    void RunToParent(){
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
        //Debug.Log("previous navmesh dest: "+navMeshAgent.destination);
        //Debug.Log("this should be closest parents dest:" +closestParent);
        NavMesh.SamplePosition(closestParent, out NavMeshHit closestNavHit, dist, NavMesh.AllAreas);
        navMeshAgent.SetDestination(closestNavHit.position);
        //Debug.Log("new navmesh dest: "+navMeshAgent.destination);
    }

    public void Eat() 
    {

        animator.SetTrigger("Die");
    }

    void StateChanged(){
        
        switch (state) 
        {
            case State.Stopped:
                int i = Random.RandomRange(0,3);
                animator.SetTrigger("Stop");
                float value = i / 2;

                animator.SetFloat("Idle", i);
                break;

            case State.Running:
                animator.SetTrigger("Move");
                animator.SetBool("Running", true);
                navMeshAgent.speed = speed;
                
                break;
            case State.Walking:
                animator.SetTrigger("Move");
                animator.SetBool("Running", false);
                navMeshAgent.speed = speed/2;
                
                break;

        }
    }

    public void ResetChildDestination()
    {
        Vector3 pos = gameObject.transform.position;
        Vector3 destination = navMeshAgent.destination;
        Vector3 direction = pos - destination;
        Debug.Log(direction);
        destination += direction.normalized * 1.5f;
        navMeshAgent.SetDestination(destination);
    }
}
