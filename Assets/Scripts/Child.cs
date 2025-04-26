using System.Linq;
using KrampUtils;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

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
    [SerializeField] private float m_maxStunTimer = 0.4f;
    [SerializeField] private int m_detectionRange = 5;
    [SerializeField] private SpriteRenderer m_shapeRenderer;

    private Nun m_selectedNun;
    private float m_stunTimer = 0;


    public void Start() {
        SetChildColor();
    }

    public State CurrentState { get; private set; }


    private void OnEnable() {
        Game.MainGameInfo.RegisterChild(this);
    }

    private void OnDisable() {
        Game.MainGameInfo.UnregisterChild(this);
    }

    private void SelectNewWanderLocation() {
        if (NavMesh.SamplePosition(MoreMath.RandomInBounds(CurrentRoom.GetBounds()), out var hit, 10, NavMesh.AllAreas)) {
            SetDestination(hit.position);
        } else {
            Debug.Log("ever considered ending your life");
        }
    }

    private void Update() {
        HandleRoomRegistration();

        switch (CurrentState) {
            case State.Idle:
                if (m_currentPath?.status == NavMeshPathStatus.PathInvalid || NearDestination(m_interactionDistance)) {
                    SelectNewWanderLocation();
                }


                if ((Game.MainGameInfo.Krampus.transform.position - transform.position).sqrMagnitude < m_detectionRange * m_detectionRange) {
                    m_stunTimer = m_maxStunTimer;
                    SwitchState(State.Stunned);
                }

                SetVelocity(GetPathDirection());
                break;

            case State.Stunned:
                m_stunTimer -= Time.deltaTime;
                if (m_stunTimer <= 0) {
                    // select nearest nun or whatevs
                    if (Game.MainGameInfo.GetRoomData(CurrentRoom).Contains<Nun>()) {
                        m_selectedNun = (Nun)Game.MainGameInfo.GetRoomData(CurrentRoom).NPCs.First(w => w is Nun);
                    } else {
                        m_selectedNun = Game.MainGameInfo.Nuns.UnityRandomElement();
                    }
                    SwitchState(State.Panic);
                }
                break;

            case State.Panic:
                SetDestination(m_selectedNun.transform.position);
                SetVelocity(GetPathDirection());

                if (NearDestination(m_interactionDistance)) {
                    m_selectedNun.ActivateTheBitch();
                    SwitchState(State.Alerted);
                }
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
        onStateChanged?.Invoke(CurrentState, previous);
        CurrentState = previous;
    }

    public void Prepare(Krampus krampus) {
        Game.MainGameInfo.UnregisterChild(this);
    }

    public void ReelIn(Krampus krampus, Vector3 position, float progress) {
        transform.position = position;
    }


    public void SetChildType(MainGameInfo.ChildType type) {
        var skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var s in skinnedMeshRenderers) {
            s.material.SetColor("_Color", type.color);
        }

        m_shapeRenderer.sprite = type.shape;
        m_shapeRenderer.color = type.color;
    }

}
