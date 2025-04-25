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


    public State CurrentState { get; private set; }


    private void OnEnable() {
        Game.MainGameInfo.RegisterChild(this);
    }

    private void OnDisable() {
        Game.MainGameInfo.UnregisterChild(this);
    }

    private void SelectNewWanderLocation() {
        if (NavMesh.SamplePosition(Game.MainGameInfo.RoomGenerator.Rooms.UnityRandomElement().GetMidPoint(), out var hit, 10, NavMesh.AllAreas)) {
            SetDestination(hit.position);
        } else {
            Debug.Log("ever considered ending your life");
        }
    }

    private void Update() {
        switch (CurrentState) {
            case State.Idle:
                if (m_currentPath?.status == NavMeshPathStatus.PathInvalid || NearDestination(m_interactionDistance)) {
                    SelectNewWanderLocation();
                }

                SetVelocity(GetPathDirection());

                break;
        }
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
        Game.MainGameInfo.UnregisterChild(this);
    }

    public void ReelIn(Krampus krampus, Vector3 position, float progress) {
        transform.position = position;
    }

}
