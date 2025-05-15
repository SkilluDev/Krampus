using System.Linq;
using KrampUtils;
using NaughtyAttributes;
using Roomgen;
using Sound;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.VFX;

public class Child : NPC, IEdible, INoiseReactor {
    public float RunSpeed => m_runSpeed;
    [ShowNativeProperty] public State CurrentState { get; private set; }

    public Vector3 InteractionPoint => m_pinTarget.transform.position;

    public UnityAction<Child.State, Child.State> onStateChanged;
    [SerializeField] private float m_interactionDistance = 8;
    [SerializeField] private float m_stunDuration = 0.4f;
    [SerializeField] private float m_reportingDuration = 0.4f;
    [SerializeField] private SpriteRenderer m_shapeSprite;
    [SerializeField] private Sex m_killSoundBite;
    [SerializeField] private VisualEffect m_goreParticle;
    [SerializeField] private TrailRenderer m_trailRenderer;
    [SerializeField] private ViewCone m_viewCone;
    [SerializeField] private Transform m_pinTarget;


    [SerializeField] private float m_runSpeed = 8;

    private Nun m_selectedNun;
    private Room m_selectedRoom;
    private Room m_lastKrampusSpotted;

    private float m_timeout = 0;

    private ChildType m_type;
    private Transform m_modelTransform;

    public ChildType Type => m_type;

    public enum State {
        Idle, // go to a random place
        Stunned, // do nothing.
        InitialPanic, // go to the nearest nun
        Panic, // go to the nearest nun
        Reporting, // talking to another
        Alerted, // go interact with stuff
        Dead
    }


    public void Start() {
        SetChildType(Game.MainGameInfo.Types.UnityRandomElement());
    }

    private void Ready() {
        Game.MainGameInfo.RegisterChild(this);

        m_viewCone.trackedObject = Game.MainGameInfo.Krampus.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.Kramp.transform;
        m_modelTransform = transform.GetComponentInChildren<Animator>().transform;
    }

    private void Unready() {
        Game.MainGameInfo.UnregisterChild(this);
    }



    public override void OverridePathCosts() {
        NavMesh.SetAreaCost(NavMesh.GetAreaFromName("Kramped"), 99f);
    }

    private void Update() {
        if (!CurrentRoom) return;

        void SelectNewWanderLocation() {
            if (NavMesh.SamplePosition(MoreMath.RandomInBounds(CurrentRoom.GetBounds()), out var hit, 10, NavMesh.AllAreas)) {
                SetDestination(hit.position);
            } else {
                Debug.Log("ever considered ending your life");
            }
        }

        void SelectRandomNun() {
            m_selectedNun = (Nun)Game.MainGameInfo.GetRoomData(CurrentRoom).Characters.FirstOrDefault(w => w is Nun);
            if (m_selectedNun == null) m_selectedNun = Game.MainGameInfo.Nuns.UnityRandomElement();
        }

        void SelectRoomAwayFromKrampy() {
            var passages = Game.MainGameInfo.GetRoomData(CurrentRoom).Passages.OrderBy(w => Vector3.Dot(Game.MainGameInfo.Krampus.transform.position - transform.position, w.transform.position - transform.position));
            m_selectedRoom = passages.First().Other(CurrentRoom);
            SetDestination(m_selectedRoom.GetRandomPointOnFloor().OnNavMesh(5));
        }

        switch (CurrentState) {
            case State.Idle: // Child wanders around at random
                if (m_currentPath?.status == NavMeshPathStatus.PathInvalid)
                    SelectNewWanderLocation();

                if (NearDestination(m_interactionDistance) && m_timeout <= 0) {
                    // TODO: Magic
                    m_timeout = Random.Range(0.1f, 2f);
                    SelectNewWanderLocation();
                }

                if (m_viewCone.Detect()) {
                    m_timeout = m_stunDuration;
                    SwitchState(State.Stunned);
                }

                if (m_timeout > 0) {
                    m_timeout -= Time.deltaTime;
                    SetVelocity(Vector3.zero);
                } else {
                    SetVelocity(GetPathDirection() * m_baseMovementSpeed);
                    SetFacingDirection(GetPathDirection());
                }

                m_viewCone.SetActive(true);
                break;
            case State.Stunned:
                m_viewCone.SetActive(false);
                m_timeout -= Time.deltaTime;
                if (m_timeout <= 0) {
                    m_lastKrampusSpotted = CurrentRoom;
                    Game.MainGameInfo.GetRoomData(m_lastKrampusSpotted).MarkKramped(true);
                    SelectRoomAwayFromKrampy();
                    SwitchState(State.InitialPanic);
                }
                break;

            case State.InitialPanic: // basically run away franticlly from krampus, however, given the opportunity to go to a nun without turning around, use it
                if (!Game.MainGameInfo.GetRoomData(CurrentRoom).Contains<Krampus>()) {
                    SelectRandomNun();
                    SetDestination(m_selectedNun.transform.position);
                    SwitchState(State.Panic);
                } else if (NearDestination(m_interactionDistance)) {
                    SelectRandomNun();
                    SetDestination(m_selectedNun.transform.position);
                    if (Vector3.Dot(GetPathDirection(), Game.MainGameInfo.Krampus.transform.position - transform.position) > 0f) {
                        m_lastKrampusSpotted = CurrentRoom;
                        Game.MainGameInfo.GetRoomData(m_lastKrampusSpotted).MarkKramped(true);
                        Game.MainGameInfo.RoomGenerator.NavMeshSurface.BuildNavMesh();
                        SelectRoomAwayFromKrampy();
                    } else {
                        SwitchState(State.Panic);
                    }
                } else if (Game.MainGameInfo.GetRoomData(CurrentRoom).Contains<Nun>()) {
                    SelectRandomNun();
                    SetDestination(m_selectedNun.transform.position);
                    SwitchState(State.Panic);
                }

                SetVelocity(GetPathDirection() * m_runSpeed);
                SetFacingDirection(GetPathDirection());
                break;
            case State.Panic: // regular panic. just go to the nun and report
                if (Game.MainGameInfo.GetRoomData(CurrentRoom).Contains<Nun>()) {
                    SelectRandomNun();
                }

                SetVelocity(GetPathDirection() * m_runSpeed);
                SetFacingDirection(GetPathDirection());

                SetDestination(m_selectedNun.transform.position);

                if (m_selectedNun.CurrentState == Nun.State.ChasingKrampus) {
                    SelectNewWanderLocation();
                    m_selectedNun = null;
                    SwitchState(State.Idle);
                }

                if (NearDestination(m_interactionDistance)) {
                    if (m_selectedNun.CurrentState is Nun.State.Idle or Nun.State.Patrolling or Nun.State.LookingForKrampus) {
                        m_selectedNun.ActivateTheBitch(this, m_reportingDuration, m_lastKrampusSpotted);
                        m_timeout = m_reportingDuration;
                        SwitchState(State.Reporting);
                    } else {
                        SwitchState(State.Idle);
                    }
                }
                break;
            case State.Reporting:
                m_timeout -= Time.deltaTime;
                SetVelocity(Vector3.zero);
                if (m_timeout <= 0) {
                    SwitchState(State.Idle);
                }
                break;
        }

        m_viewCone.SetFacing(m_modelTransform.forward); // TODO Smart rotation
    }


    public void Consume(Krampus krampus) {
        m_killSoundBite.Play(transform.position, 1);

        //m_trailRenderer.transform.parent = null;
        //m_trailRenderer.transform.position = Game.MainGameInfo.Krampus.Tongue.transform.position;
        //m_trailRenderer.autodestruct = true;
        //m_trailRenderer = null;

        var particle = Instantiate(m_goreParticle);
        //        particle.SetVector4("Particle Color", m_type == Game.MainGameInfo.GoodChildType ? Game.MainGameInfo.GoodChildrenColor : Game.MainGameInfo.BadChildrenColor);
        particle.transform.position = Game.MainGameInfo.Krampus.Tongue.transform.position;
        Game.MainGameInfo.UnregisterChild(this);
        Game.MainGameInfo.GlobalEvents.onChildEaten?.Invoke(Type);
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
        //  Game.MainGameInfo.UnregisterChild(this);
    }

    public void ReelIn(Krampus krampus, Vector3 position, float progress) {
        transform.position = position - transform.InverseTransformPoint(m_pinTarget.position);
    }


    public void SetChildType(ChildType type) {
        m_type = type;
        var skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var s in skinnedMeshRenderers) {
            s.material.SetColor("_Color", type.color);
        }

        m_shapeSprite.sprite = type.shape;
        var c = new Color(type.color.r, type.color.g, type.color.b, 0.25f);
        m_shapeSprite.color = c;
    }

    public void Alert(RoomData roomData, Vector3 place, ICharacter actor) {
        if (actor is not Krampus) return;
        Debug.Log("[Child] Child alerted");
        m_timeout = m_stunDuration;
        SwitchState(State.Stunned);
    }
}
