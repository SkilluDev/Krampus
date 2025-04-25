using System;
using KrampUtils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Child : NPC, IEdible {
    public enum State {
        Idle, // go to a random place
        Stunned, // do nothing.
        Panic, // go to the nearest nun
        Alerted, // go interact with stuff
        Dead
    }

    public UnityAction<Child.State, Child.State> onStateChanged;
    [SerializeField] private float m_interactionDistance = 8;
    [SerializeField] private Transform m_dest;
    [SerializeField] private RoomGenerator m_gen;


    public State CurrentState { get; private set; }

    private void Start() {
        MoveChildToRandomPlace();
        Debug.Log(transform.position);
        SelectNewWanderLocation();
    }

    private void SelectNewWanderLocation() {
        if (NavMesh.SamplePosition(m_gen.Rooms.UnityRandomElement().GetMidPoint(), out var hit, 10, NavMesh.AllAreas)) {
            SetDestination(hit.position);
            Debug.Log("Moving to new room");
        } else {
            Debug.Log("ever considered ending your life");
        }

    }

    private void Update() {
        switch (CurrentState) {
            case State.Idle:
                if ((CurrentDestination - transform.position).sqrMagnitude < m_interactionDistance) {
                    SelectNewWanderLocation();
                }
                MoveAlongPath();
                break;
        }
    }

    private void MoveChildToRandomPlace() {
        var temp = MoreNavmesh.RandomPoint(Vector3.zero, 200);
        GetComponent<Rigidbody>().position = new Vector3(temp.x, 0, temp.z);
    }

    public void Consume(Krampus krampus) {
        Destroy(gameObject);
    }
    public void Hit(Krampus krampus) {
        SwitchState(State.Dead);
    }

    private void SwitchState(State previous) {
        if (previous == CurrentState) return;
        Debug.Log(onStateChanged);
        onStateChanged?.Invoke(CurrentState, previous);
        CurrentState = previous;
    }

    public void Prepare(Krampus krampus) {

    }

    public void ReelIn(Krampus krampus, Vector3 position, float progress) {
        transform.position = position;
    }

}
