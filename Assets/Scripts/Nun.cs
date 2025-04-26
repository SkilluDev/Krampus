using System;
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
    [SerializeField] private float m_detectionRange = 4;
    public State CurrentState { get; private set; }


    private void OnEnable() {
        Game.MainGameInfo.RegisterNun(this);
    }

    private void OnDisable() {
        Game.MainGameInfo.UnregisterNun(this);
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
                if (m_currentPath.status == NavMeshPathStatus.PathInvalid || NearDestination(m_interactionDistance)) {
                    SelectNewWanderLocation();
                }

                if ((Game.MainGameInfo.Krampus.transform.position - transform.position).sqrMagnitude < m_detectionRange * m_detectionRange) {
                    SwitchState(State.ChasingKrampus);
                }

                SetVelocity(GetPathDirection());
                break;
            case State.ChasingKrampus:
                SetDestination(Game.MainGameInfo.Krampus.transform.position);
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

    public void ActivateTheBitch() {
        SwitchState(State.ChasingKrampus);
    }
}
