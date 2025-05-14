using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using KrampUtils;
using Roomgen;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Nun : NPC {
    public enum State {
        Idle,
        Patrolling, // Follow a pre-determined path. Use view cone to notice krampus 
        Listening, // Wait and listen for the kid
        LookingForKrampus, // Go to the room that was reported by the kid. If krampus is in a room the nun is in, start chasing him. Otherwise back to patrolling.
        FoundKrampus, // shock state
        ChasingKrampus, // self-explanatory
        Stunned // self-explanatory. post stun, go to lookingforkrampus
    }

    public float RunSpeed => m_runSpeed;

    public UnityAction<Nun.State, Nun.State> onStateChanged;
    public UnityAction<Nun.State> onAttack;
    [SerializeField] private float m_interactionDistance = 8;
    [SerializeField] private float m_detectionRange = 4;
    public State CurrentState { get; private set; }

    [SerializeField] private float m_runSpeed = 8;



    [SerializeField] private float m_shortDetectionRange = 4, m_longDetectionRange = 10;
    [SerializeField] private float m_shockTimeout = 0.2f;

    [SerializeField] private ViewCone m_viewCone;
    private List<Vector3> m_patrolPath;
    private int m_currentControlPoint = 0;
    [SerializeField] private float m_patrolDetectTimeout = 1f;
    [SerializeField] private float m_patrolIgnoreTimeout = 2f;

    [SerializeField] private CinemachineImpulseSource m_shake;


    private Room m_reportedKrampusRoom;
    private float m_timeout;

    private void Ready() {
        Game.MainGameInfo.RegisterNun(this);

        CreatePatrolPath();
        m_viewCone.range = m_longDetectionRange;
        m_viewCone.trackedObject = Game.MainGameInfo.Krampus.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.transform;
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


    public override void OverridePathCosts() {
        NavMesh.SetAreaCost(NavMesh.GetAreaFromName("Kramped"), 1f);
    }

    private void CreatePatrolPath() {
        m_patrolPath = new List<Vector3>();
        for (int i = 0; i < 3; i++) {
            var room = Game.MainGameInfo.RoomGenerator.Rooms.UnityRandomElement();
            if (!NavMesh.SamplePosition(room.GetMidPoint(), out var navmeshPoint, 3f, NavMesh.AllAreas)) {
                i--;
                Debug.LogWarning($"Problem adding Nun patrol point in {room.name}. Retrying...");
                continue;
            }
            m_patrolPath.Add(navmeshPoint.position);
        }
    }



    private void Update() {

        switch (CurrentState) {
            case State.Idle:
                m_viewCone.SetFacing(Vector3.forward);
                m_timeout -= Time.deltaTime;
                if (m_timeout < 0) SwitchState(State.Patrolling);
                CurrentDestination = transform.position;
                m_viewCone.SetActive(true);
                Debug.Log("[Nun] Begin patrolling");
                break;
            case State.Patrolling:
                if (NearDestination(m_interactionDistance)) {
                    m_currentControlPoint++;
                    m_currentControlPoint %= m_patrolPath.Count;
                    SetDestination(m_patrolPath[m_currentControlPoint]);
                    Debug.Log("[Nun] Reached patrol point");
                }

                Debug.Log("[Nun] Patrolling");
                m_viewCone.SetActive(true);
                if (m_viewCone.Detect()) {
                    Debug.Log("[Nun] viewcone detected krampy");
                    m_timeout = m_shockTimeout;
                    SwitchState(State.FoundKrampus);
                }


                SetVelocity(GetPathDirection() * m_baseMovementSpeed);
                m_viewCone.SetFacing(GetPathDirection());
                break;
            case State.LookingForKrampus:
                m_viewCone.SetActive(false);
                if (Game.MainGameInfo.GetRoomData(CurrentRoom).Contains<Krampus>()) {
                    if (m_timeout > m_patrolDetectTimeout) {
                        Debug.Log("[Nun] Alerted & detected krampy");
                        m_timeout = m_shockTimeout;
                        SwitchState(State.FoundKrampus);
                    } else {
                        m_timeout += Time.deltaTime;
                    }
                } else {
                    if (NearDestination(m_interactionDistance)) {
                        m_timeout += Time.deltaTime;
                        if (m_timeout > m_patrolIgnoreTimeout) {
                            SwitchState(State.Idle);
                        }
                    }
                }



                SetVelocity(GetPathDirection() * m_runSpeed);
                break;
            case State.FoundKrampus:
                m_viewCone.SetActive(false);
                SetVelocity(Vector3.zero);
                m_timeout -= Time.deltaTime;
                if (m_timeout < 0) SwitchState(State.ChasingKrampus);
                break;
            case State.Listening:
                m_viewCone.SetActive(false);
                m_timeout -= Time.deltaTime;
                if (m_timeout < 0) {
                    SwitchState(State.LookingForKrampus);
                    SetDestination(m_reportedKrampusRoom.GetRandomPointOnFloor().OnNavMesh(5));
                }
                break;
            case State.ChasingKrampus:
                m_viewCone.SetActive(false);
                SetDestination(Game.MainGameInfo.Krampus.transform.position);
                SetVelocity(GetPathDirection() * m_runSpeed);
                break;
            case State.Stunned:
                m_timeout -= Time.deltaTime;

                if (m_timeout < 0) {
                    m_reportedKrampusRoom = Game.MainGameInfo.GetRoomData(CurrentRoom).Passages
                                                .OrderBy(w => Vector3.SqrMagnitude(w.transform.position - transform.position))
                                                .First()
                                                .Other(CurrentRoom);

                    SwitchState(State.LookingForKrampus);
                    SetDestination(m_reportedKrampusRoom.GetRandomPointOnFloor().OnNavMesh(5));
                }

                SetVelocity(Vector3.zero);
                m_viewCone.SetActive(false);
                break;
        }

    }



    private void SwitchState(State previous) {
        if (previous == CurrentState) return;
        onStateChanged?.Invoke(CurrentState, previous);
        CurrentState = previous;
        Debug.Log($"[Nun] Switch state to {CurrentState}");
    }

    public void ActivateTheBitch(float timeout, Room room) {
        foreach (var w in Game.MainGameInfo.RoomGenerator.Rooms) {
            Game.MainGameInfo.GetRoomData(w).MarkKramped(false);
        }
        Game.MainGameInfo.GetRoomData(room).MarkKramped(true);
        Game.MainGameInfo.RoomGenerator.NavMeshSurface.BuildNavMesh();

        m_reportedKrampusRoom = room;

        if (CurrentState != State.ChasingKrampus) {
            m_timeout = timeout;
            SwitchState(State.Listening);
        }

    }

    private void OnCollisionEnter(Collision collision) {

        if (CurrentState != State.ChasingKrampus) return;

        if (collision.gameObject.layer != LayerMask.NameToLayer("Player")) {
            return;
        }
        onAttack?.Invoke(CurrentState);
        SwitchState(State.Listening);
        Game.MainGameInfo.Krampus.Kontroller.KrampTermination();

    }

    public void Stun(float duration) {
        Game.MainGameInfo.Krampus.Kamera.DefaultShake.GenerateImpulse();
        m_timeout = duration;
        SwitchState(State.Stunned);

    }

}
