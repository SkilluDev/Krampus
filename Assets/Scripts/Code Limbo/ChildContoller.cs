using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChildContoller : MonoBehaviour {
    private UnityEngine.AI.NavMeshAgent navMeshAgent;
    private Detection detection;
    private Vector3 initialPosition;
    private Animator animator;
    public float speed;
    public bool isDummy = false;

    public bool logState = false;
    private Transform closestParent;
    public enum State {
        Stopped,
        Walking,
        Hiding,
        Running,
    }

    public State state;
    private State previousState;

    // Start is called before the first frame update
    private void Start() {
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        //if(!isDummy) ResetChildDestination();
        initialPosition = transform.position;
        navMeshAgent.stoppingDistance = 1f;
        detection = GetComponent<Detection>();
        animator = GetComponentInChildren<Animator>();
        speed = navMeshAgent.speed;
    }

    // Update is called once per frame
    private void Update() {

        if (logState && state != previousState) {
            previousState = state;
            Debug.Log(state);
        }
        if (!WinCondition.Instance.isGamePausedValue() && !isDummy) {
            if (detection.isAlerted) { UpdateState(State.Running); RunToParent(); } else if (state == State.Hiding) {
                if (destinationReached()) {
                    UpdateState(State.Stopped);
                }
            }
        }
    }

    private void UpdateState(State newState) {
        if (newState == state) return;
        state = newState;
        StateChanged();
    }


    
    private void RunToParent() {
        var parents = GameObject.FindGameObjectsWithTag("Parent");
        if (parents.Length == 0) {
            Debug.Log("No parents on map");
            return;
        }

        float dist = float.PositiveInfinity;
        Vector3 closestParent = new Vector3();

        foreach (GameObject parent in parents) {
            Vector3 parentPosition = parent.transform.position;
            Vector3 offset = parentPosition - transform.position;
            float sqrLen = offset.sqrMagnitude;

            if (sqrLen < dist) {
                dist = sqrLen;
                closestParent = parentPosition;
            }


        }

        SetDestination(closestParent);
    }

    private void RunToRandomPlace() {
        SetDestination(RandomPoint(transform.position, 100f));
    }

    public void Eat() {

        animator.SetTrigger("Die");
    }

    private void StateChanged() {

        switch (state) {
            case State.Stopped:
                int i = Random.Range(0, 3);
                animator.SetBool("Stop", true);
                navMeshAgent.velocity = Vector3.zero;
                animator.SetFloat("Idle", i);
                break;
            case State.Running:
                animator.SetTrigger("Move");
                animator.SetBool("Running", true);
                animator.SetBool("Stop", false);
                navMeshAgent.speed = speed;
                RunToParent();
                break;
            case State.Walking:
                animator.SetTrigger("Move");
                animator.SetBool("Running", false);
                animator.SetBool("Stop", false);
                navMeshAgent.speed = speed / 2;
                break;
            case State.Hiding:
                animator.SetTrigger("Move");
                animator.SetBool("Running", true);
                animator.SetBool("Stop", false);
                navMeshAgent.speed = speed;
                RunToRandomPlace();
                break;
        }
    }

    public void ResetChildDestination() {
        Vector3 pos = gameObject.transform.position;
        Vector3 destination = navMeshAgent.destination;
        Vector3 direction = pos - destination;
        Debug.Log(direction);
        destination += direction.normalized * 1.5f;
        navMeshAgent.SetDestination(destination);
    }




    private void SetDestination(Vector3 destination) {
        NavMesh.SamplePosition(destination, out NavMeshHit closestNavHit, 1000, NavMesh.AllAreas);
        navMeshAgent.SetDestination(closestNavHit.position);
    }


    public void Calming() {
        UpdateState(State.Hiding);
        detection.isAlerted = false;
    }


    private Vector3 RandomPoint(Vector3 center, float range) {
        Vector3 result;
        while (true) {
            Vector2 point = Random.insideUnitCircle;
            Vector3 randomPoint = center + new Vector3(point.x, 0, point.y) * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) {
                result = hit.position;
                return result;
            }
        }
    }


    private bool destinationReached() {
        if (!navMeshAgent.pathPending) {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f) {
                    return true;
                }
            }
        }
        return false;
    }

}
