using KrampUtils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Nun : NPC {
    public enum State {
        Idle, ChasingKrampus
    }

    public UnityAction<Nun.State, Nun.State> onStateChanged;
    [SerializeField] private float m_interactionDistance = 8;


    public State CurrentState { get; private set; }



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
                if (m_currentPath.status == NavMeshPathStatus.PathInvalid || NearDestination(m_interactionDistance)) {
                    SelectNewWanderLocation();
                }

                SetVelocity(GetPathDirection());

                break;
        }
    }



    private void SwitchState(State previous) {
        if (previous == CurrentState) return;
        Debug.Log(onStateChanged);
        onStateChanged?.Invoke(CurrentState, previous);
        CurrentState = previous;
    }

}
