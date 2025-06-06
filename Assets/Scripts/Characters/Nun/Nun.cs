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

    [SerializeField] private float m_shockTimeout = 0.2f;

    [SerializeField] private ViewCone m_viewCone;
    private List<Vector3> m_patrolPath;
    private int m_currentControlPoint = 0;
    private Transform m_modelTransform;
    [SerializeField] private float m_krampusDetectTime = 1f;
    [SerializeField] private float m_patrolIdleDuration = 2f;

    [SerializeField] private Tag m_dontPatrolTag;



    private Room m_reportedKrampusRoom;
    private float m_timeout;

    private void Ready() {
        Game.MainGameInfo.RegisterNun(this);

        m_runSpeed = (float)Game.SetMan.GetValue<long>("Nun run speed");

        CreatePatrolPath();
        m_viewCone.trackedObject = Game.MainGameInfo.Krampus.Kramp.transform;
        m_modelTransform = transform.GetComponentInChildren<Animator>().transform;
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
            if (room.HasTag(m_dontPatrolTag)) {
                i--;
                Debug.LogWarning($"Problem adding Nun patrol point in {room.name}. Retrying...");
                continue;
            }
            m_patrolPath.Add(navmeshPoint.position);
        }
    }


    void SeeKrampus() {
        Game.MainGameInfo.Krampus.KrampusEvents.onKrampusFoundByNun.Invoke(Game.MainGameInfo.Krampus, this);
          SwitchState(State.FoundKrampus);
     }

    private void Update() {
        if (!Game.Balling) return;

        switch (CurrentState) {
            case State.Idle:
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

                m_viewCone.SetActive(true);
                if (m_viewCone.Detect()) {
                    Debug.Log("[Nun] viewcone detected krampy");
                    m_timeout = m_shockTimeout;
                    //Change in multi

                    SeeKrampus();

                   
                }


                SetVelocity(GetPathDirection() * m_baseMovementSpeed);
                SetFacingDirection(GetPathDirection());
                break;
            case State.LookingForKrampus:
                m_viewCone.SetActive(false);
                if (Game.MainGameInfo.GetRoomData(CurrentRoom).Contains<Krampus>()) {
                    if (m_timeout > m_krampusDetectTime) {
                        Debug.Log("[Nun] Alerted & detected krampy");
                        m_timeout = m_shockTimeout;
                        SeeKrampus();
                    } else {
                        m_timeout += Time.deltaTime;
                    }

                } else if (NearDestination(m_interactionDistance)) {
                    m_timeout += Time.deltaTime;
                    if (m_timeout > m_patrolIdleDuration) {
                        SwitchState(State.Idle);
                    }
                    SetVelocity(Vector3.zero);
                    SetFacingDirection(Vector3.zero);

                } else {
                    SetVelocity(GetPathDirection() * m_runSpeed);
                    SetFacingDirection(GetPathDirection());
                }

                break;
            case State.FoundKrampus:
                m_viewCone.SetActive(false);
                SetVelocity(Vector3.zero);
                m_timeout -= Time.deltaTime;
                SetFacingToPoint(Game.MainGameInfo.Krampus.transform.position);



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
                SetFacingDirection(GetPathDirection());
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

        // m_viewCone.SetFacing(Quaternion.Euler(0, FacingAngle, 0) * Vector3.forward);
        m_viewCone.SetFacing(m_modelTransform.forward); // TODO: SMART ROTATION
    }


    private void SwitchState(State previous) {
        if (previous == CurrentState) return;
        onStateChanged?.Invoke(CurrentState, previous);
        CurrentState = previous;
        Debug.Log($"[Nun] Switch state to {CurrentState}");
    }

    public void ActivateTheBitch(Child who, float timeout, Room room) {
        foreach (var w in Game.MainGameInfo.RoomGenerator.Rooms) {
            Game.MainGameInfo.GetRoomData(w).MarkKramped(false);
        }
        Game.MainGameInfo.GetRoomData(room).MarkKramped(true);

        m_reportedKrampusRoom = room;

        if (CurrentState is not State.ChasingKrampus or State.LookingForKrampus) {
            m_timeout = timeout;
            SetFacingToPoint(who.transform.position);
            SwitchState(State.Listening);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (!Game.Balling) return;
        if (CurrentState != State.ChasingKrampus) return;
        if (collision.gameObject.layer != LayerMask.NameToLayer("Player")) {
            return;
        }
        if (Game.MainGameInfo.Krampus.Kontroller.CurrentState == KrampusController.State.Dash) return;
        onAttack?.Invoke(CurrentState);
        SwitchState(State.Idle);
        Game.MainGameInfo.Krampus.Kontroller.KrampTermination(Ending.LoseNun);

    }

    public void Stun(float duration) {
        Game.MainGameInfo.Krampus.Kamera.DefaultShake.GenerateImpulse();
        m_timeout = duration;
        SwitchState(State.Stunned);
    }

}
