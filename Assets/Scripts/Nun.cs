using System;
using KrampUtils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Nun : NPC {
    public enum State {
        Idle, Listening, ChasingKrampus
    }

    public float RunSpeed => m_runSpeed;

    public UnityAction<Nun.State, Nun.State> onStateChanged;
    [SerializeField] private float m_interactionDistance = 8;
    [SerializeField] private float m_detectionRange = 4;
    public State CurrentState { get; private set; }

    [SerializeField] private float m_runSpeed = 8;

    private float m_timeout;

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

                SetVelocity(GetPathDirection() * m_baseMovementSpeed);
                break;
            case State.Listening:
                m_timeout -= Time.deltaTime;
                if (m_timeout < 0) SwitchState(State.ChasingKrampus);
                break;
            case State.ChasingKrampus:
                SetDestination(Game.MainGameInfo.Krampus.transform.position);
                SetVelocity(GetPathDirection() * m_runSpeed);
                break;
        }
    }



    private void SwitchState(State previous) {
        if (previous == CurrentState) return;
        Debug.Log(onStateChanged);
        onStateChanged?.Invoke(CurrentState, previous);
        CurrentState = previous;
    }

    public void ActivateTheBitch(float timeout) {
        m_timeout = timeout;
        SwitchState(State.Listening);
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Player")) {
            return;
        }
        ///Tu powinno być timer
        Game.MainGameInfo.Krampus.Kramp.Kontroller.KrampTermination();
        Game.MainGameInfo.UI.ShowGameOverScreen();
    }

}
