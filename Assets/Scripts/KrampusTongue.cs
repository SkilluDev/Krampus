using System;
using KrampUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms;

public class KrampusTongue : KrampusBehaviour {

    public UnityAction<KrampusTongue.State, KrampusTongue.State> onStateChanged;
    [SerializeField] private LineRenderer m_tongueRenderer;
    [SerializeField] private LayerMask m_layerMask = int.MaxValue;
    [SerializeField] private Transform m_tongueOrigin;

    [Serializable]
    private class Timings : TimedSequence<Timings> {
        [SeqDuration] public float windup = 0.4f;
        [SeqDuration] public float extend = 0.2f;
        public AnimationCurve extendCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SeqDuration] public float preRetreat = 0.1f;
        [SeqDuration] public float retreat = 0.3f;
        public AnimationCurve retreatCurve = AnimationCurve.Linear(0, 0, 1, 1);
    }

    [SerializeField] private Timings m_tng;


    private Vector3 m_tongueDestination;
    private float m_tongueTime = 0f;
    private float m_tongueExtensionFactor = 0f;

    public State CurrentState { get; private set; }

    public enum State {
        Idle,
        Windup,
        Extending,
        Full,
        PreRetreat,
        Retreating,
        Eating
    }

    private void Awake() {
        m_tng.Init();
        //Cursor.SetCursor(Cursor, new Vector2(cursor.width / 2, cursor.height / 2), CursorMode.ForceSoftware);
    }

    private void HandleInput() {
        if (Input.GetMouseButtonDown(0)) {
            var ray = Kramp.Kamera.Raw.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 100, m_layerMask)) {
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red, 20f);
                ShootOut(hit.point);
            } else {
                Debug.Log("Missed!");
            }
        }
    }
    private void AdvanceState() {
        var previous = CurrentState;
        if (CurrentState == State.Eating) CurrentState = State.Idle;
        else CurrentState++;
        onStateChanged.Invoke(previous, CurrentState);
    }

    private bool AdvanceStateIfTime(string nameof) {
        if (m_tongueTime >= m_tng.End(nameof)) {
            AdvanceState();
            return true;
        } else return false;
    }


    private void Update() {
        if (CurrentState == State.Idle)
            HandleInput();
        // first - wait for the krampus wind up
        // after that, check what we hit, the object is reachable by krampus. if hittable activate pre-hit methods
        // extend tongue to reach what we hit. as we extend trigger minor interactable methods
        // actually execute hit methods. if edible, handle the objects animation
        // begin retreat
        // if edible, execute eat method. play eat animation. Otherwise play regular retreat animation

        switch (CurrentState) {
            case State.Windup:
                AdvanceStateIfTime(nameof(Timings.windup));
                break;
            case State.Extending:
                m_tongueExtensionFactor = m_tng.extendCurve.Evaluate(m_tng.InverseLerp(nameof(Timings.extend), m_tongueTime));
                AdvanceStateIfTime(nameof(Timings.extend));
                break;
            case State.PreRetreat:
                AdvanceStateIfTime(nameof(Timings.preRetreat));
                break;
            case State.Full:
                AdvanceState();
                break;
            case State.Retreating:
                m_tongueExtensionFactor = m_tng.retreatCurve.Evaluate(1 - m_tng.InverseLerp(nameof(Timings.retreat), m_tongueTime));
                AdvanceStateIfTime(nameof(Timings.retreat));
                break;
            case State.Eating:
                AdvanceState();
                break;
            default:
                break;
        }

        m_tongueRenderer.SetPosition(0, m_tongueOrigin.position);
        m_tongueRenderer.SetPosition(1, Vector3.Lerp(m_tongueOrigin.position, m_tongueDestination, m_tongueExtensionFactor));

        m_tongueTime += CurrentState != State.Idle ? Time.deltaTime : 0;
    }


    public void ShootOut(Vector3 destination) {
        CurrentState = State.Windup;
        onStateChanged.Invoke(State.Idle, State.Windup);
        m_tongueDestination = destination;
        m_tongueExtensionFactor = 0;
        m_tongueTime = 0;
    }
}
