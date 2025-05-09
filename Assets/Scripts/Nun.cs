using System;
using System.Collections;
using KrampUtils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Nun : NPC {
    public enum State {
        Idle, Listening, ChasingKrampus, Stunned
    }

    public float RunSpeed => m_runSpeed;

    public UnityAction<Nun.State, Nun.State> onStateChanged;
    public UnityAction<Nun.State> onAttack;
    [SerializeField] private float m_interactionDistance = 8;
    [SerializeField] private float m_detectionRange = 4;
    public State CurrentState { get; private set; }

    [SerializeField] private float m_runSpeed = 8;

    private bool m_oldBehaviour = true;

    private float m_timeout;

    private void Ready() {
        Game.MainGameInfo.RegisterNun(this);
    }

    private void Unready() {
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
        if (m_oldBehaviour) {
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
                case State.Stunned:
                    m_timeout -= Time.deltaTime;
                    if (m_timeout < 0) SwitchState(State.Idle);
                    SetVelocity(Vector3.zero);
                    break;
            }
        } else {
            //
        }
    }



    private void SwitchState(State previous) {
        if (previous == CurrentState) return;
        onStateChanged?.Invoke(CurrentState, previous);
        CurrentState = previous;
    }

    public void ActivateTheBitch(float timeout) {
        if (m_oldBehaviour) {
            m_timeout = timeout;
            SwitchState(State.Listening);
        } else {
            //
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (m_oldBehaviour) {
            if (collision.gameObject.layer != LayerMask.NameToLayer("Player")) {
                return;
            }
            onAttack?.Invoke(CurrentState);
            SwitchState(State.Listening);
            Game.MainGameInfo.Krampus.Kontroller.KrampTermination();
        } else {
            //

        }
    }

    public void Stun(float duration) {
        if (m_oldBehaviour) {
            m_timeout = duration;
            SwitchState(State.Stunned);
        } else {
            //
        }
    }

}
