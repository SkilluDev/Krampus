using System.Linq;
using KrampUtils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Child : NPC, IEdible {
    public float RunSpeed => m_runSpeed;
    public State CurrentState { get; private set; }

    public UnityAction<Child.State, Child.State> onStateChanged;
    [SerializeField] private float m_interactionDistance = 8;
    [SerializeField] private float m_stunDuration = 0.4f;
    [SerializeField] private float m_reportingDuration = 0.4f;
    [SerializeField] private int m_detectionRange = 5;
    [SerializeField] private MeshRenderer m_shapeRenderer;
    [SerializeField][FormerlySerializedAs("m_KillSoundBite")] private SoundBite m_killSoundBite;
    [SerializeField] private ParticleSystem m_goreParticle;
    [SerializeField] private TrailRenderer m_trailRenderer;


    [SerializeField] private float m_runSpeed = 8;

    private Nun m_selectedNun;
    private float m_timeout = 0;

    private ChildType m_type;
    public ChildType Type => m_type;

    public enum State {
        Idle, // go to a random place
        Stunned, // do nothing.
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
    }

    private void Unready() {
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
        if (!CurrentRoom) return;


        switch (CurrentState) {
            case State.Idle:
                if (m_currentPath?.status == NavMeshPathStatus.PathInvalid)
                    SelectNewWanderLocation();

                if (NearDestination(m_interactionDistance) && m_timeout <= 0) {
                    // TODO: Magic
                    m_timeout = Random.Range(0.1f, 2f);
                    SelectNewWanderLocation();
                }



                if (CanGetKramped(transform.position)) {
                    m_timeout = m_stunDuration;
                    SwitchState(State.Stunned);
                }

                if (m_timeout > 0) {
                    m_timeout -= Time.deltaTime;
                    SetVelocity(Vector3.zero);
                } else {
                    SetVelocity(GetPathDirection() * m_baseMovementSpeed);
                }
                break;

            case State.Stunned:
                m_timeout -= Time.deltaTime;
                if (m_timeout <= 0) {
                    // select nearest nun or whatevs
                    if (Game.MainGameInfo.GetRoomData(CurrentRoom).Contains<Nun>()) {
                        m_selectedNun = (Nun)Game.MainGameInfo.GetRoomData(CurrentRoom).Characters.First(w => w is Nun);
                    } else {
                        m_selectedNun = Game.MainGameInfo.Nuns.UnityRandomElement();
                    }
                    SwitchState(State.Panic);
                }
                break;

            case State.Panic:
                SetDestination(m_selectedNun.transform.position);
                SetVelocity(GetPathDirection() * m_runSpeed);

                if (NearDestination(m_interactionDistance)) {
                    m_selectedNun.ActivateTheBitch(m_reportingDuration);
                    m_timeout = m_reportingDuration;
                    SwitchState(State.Reporting);
                }
                break;
            case State.Reporting:
                m_timeout -= Time.deltaTime;
                SetVelocity(Vector3.zero);
                if (m_timeout <= 0) {
                    SwitchState(State.Alerted);
                }
                break;

            case State.Alerted:
                // wander around and tell other kids

                break;
        }

    }

    private bool CanGetKramped(Vector3 position) {
        return (Game.MainGameInfo.Krampus.transform.position - position).sqrMagnitude < m_detectionRange * m_detectionRange;
    }

    public void Consume(Krampus krampus) {
        m_killSoundBite.Play(transform.position, 1, true);
        Instantiate(m_goreParticle);
        if (m_type != Game.MainGameInfo.GoodChildType) Game.MainGameInfo.ShaderManager.ProcessKill();
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


    public void SetChildType(ChildType type) {
        m_type = type;
        var skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var s in skinnedMeshRenderers) {
            s.material.SetColor("_Color", type.color);
        }

        m_shapeRenderer.material.mainTexture = type.shape;
        m_shapeRenderer.material.color = type.color;
        m_trailRenderer.colorGradient = new Gradient() {
            alphaKeys = new GradientAlphaKey[] { new(1, 0), new(1, 1) },
            colorKeys = new GradientColorKey[] { new(Type.color, 0.5f), new(Type.color + new Color(0.25f, 0.25f, 0.25f), 1) },
            colorSpace = ColorSpace.Linear,
            mode = GradientMode.Blend
        };
    }

    private void OnDestroy() {
        m_trailRenderer.transform.parent = null;
        m_trailRenderer.transform.position = transform.position;
        m_trailRenderer.autodestruct = true;
        m_trailRenderer = null;
    }

}
