using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildContoller : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent navMeshAgent;
    private detection detection;
    private Vector3 initialPosition;
    [SerializeField] private WinCondition winCondition;
    private Animator animator;

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
        detection = GetComponent<detection>();
        initialPosition = transform.position;
        winCondition = GameObject.Find("Win Condition").GetComponent<WinCondition>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!winCondition.isGamePausedValue())
        {
            if (detection.isAlerted) RunToParent();
            else navMeshAgent.SetDestination(initialPosition);
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
        navMeshAgent.SetDestination(closestParent);
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
                float value = i / 2;

                animator.SetFloat("Idle", i);
                break;

            case State.Running:
                animator.SetTrigger("Move");
                animator.SetBool("Running", true);
                break;
            case State.Walking:
                animator.SetTrigger("Move");
                animator.SetBool("Running", false);
                break;

        }
    }
}
