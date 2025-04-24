using KrampUtils;
using UnityEngine;
using UnityEngine.AI;

public class Child : NPC, IEdible {
    public enum State {
        Idle, // go to a random place
        Stunned, // do nothing.
        Panic, // go to the nearest nun
        Alerted, // go interact with stuff
        Dead
    }

    [SerializeField] private float m_interactionDistance = 8;
    [SerializeField] private Transform m_dest;
    [SerializeField] private RoomGenerator m_gen;

    public Child.State CurrentState { get; private set; }

    private void Start() {
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

    public void Consume(Krampus krampus) => throw new System.NotImplementedException();
    public void Hit(Krampus krampus) => throw new System.NotImplementedException();
    public void Prepare(Krampus krampus) => throw new System.NotImplementedException();
    public void ReelIn(Krampus krampus, Vector3 position, float progress) => throw new System.NotImplementedException();
}
