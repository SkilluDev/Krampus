using System;
using System.Collections.Generic;
using System.Linq;
using KrampUtils;
using UnityEngine;
using UnityEngine.Events;

public class KrampusTongue : KrampusBehaviour {

    public UnityAction<KrampusTongue.State, KrampusTongue.State> onStateChanged;
    [SerializeField] private LineRenderer m_tongueRenderer;
    [SerializeField] private LayerMask m_layerMask = int.MaxValue;
    [SerializeField] private Transform m_tongueOrigin;
    [SerializeField] private float m_tongueLength = 8;
    [SerializeField] private float m_tongueHitRadius = 0.5f;
    [SerializeField] private Texture2D m_cursor;

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
    private Vector3 m_tongueDirection;
    private float m_tongueExtensionFactor = 0f;
    private IInteractable m_hitInteractable;
    private ITongueable m_hitTonguable;
    private List<(float dst, ITongueable component)> m_midwayToungables;

    public State CurrentState { get; private set; }

    public enum State {
        Idle,
        Windup,
        TargetFetch,
        Extending,
        Full,
        PreRetreat,
        Retreating,
        Eating
    }

    private void Awake() {
        m_tng.Init();
        Cursor.SetCursor(m_cursor, new Vector2(m_cursor.width / 2, m_cursor.height / 2), CursorMode.ForceSoftware);
    }

    private void HandleInput() {
        if (Input.GetMouseButtonDown(0)) {
            var ray = Kramp.Kamera.Raw.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, 1000, m_layerMask)) {
                ShootOut(hit.point - transform.position);
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
            case State.TargetFetch:
                var checkingPoint = Physics.Raycast(transform.position, m_tongueDirection, out var hit, m_tongueLength, m_layerMask) ?
                    hit.point : transform.position + (m_tongueDirection * m_tongueLength);

                // magic numbers - from ground to max wall height.
                var possibleInteractors = Physics.OverlapCapsule(new Vector3(hit.point.x, 0, hit.point.z), new Vector3(hit.point.x, 6, hit.point.z), m_tongueHitRadius);

                m_hitInteractable = possibleInteractors.Select(w => w.GetComponent<IInteractable>()).FirstOrDefault(w => w != null && w.CanInteract(Kramp));

                if (m_hitInteractable == null || !m_hitInteractable.GameObject.TryGetComponent<ITongueable>(out m_hitTonguable))
                    m_hitTonguable = possibleInteractors.Select(w => w.GetComponent<ITongueable>()).FirstOrDefault(w => w != null);


                m_tongueDestination = m_hitInteractable == null ? checkingPoint : m_hitInteractable.InteractionPoint;

                // find all tonguables to update with the extending tongue. We want to not include
                var possibleTongueables = Physics.OverlapCapsule(transform.position, m_tongueDestination, m_tongueHitRadius);
                float fullLength = (m_tongueDestination - transform.position).sqrMagnitude;
                m_midwayToungables = possibleTongueables
                    .Select(w => (dst: (w.transform.position - transform.position).sqrMagnitude / fullLength, component: w.GetComponent<ITongueable>()))
                    .Where(w => w.component != null && (m_hitTonguable == null || w.component.GameObject != m_hitTonguable.GameObject))
                    .OrderBy(w => w.dst).ToList();

                AdvanceState();
                break;
            case State.Extending:
                m_tongueExtensionFactor = m_tng.extendCurve.Evaluate(m_tng.InverseLerp(nameof(Timings.extend), m_tongueTime));
                if (m_midwayToungables.Count > 0 && m_tongueExtensionFactor >= m_midwayToungables[0].dst) {
                    m_midwayToungables[0].component.Passby(Kramp, Vector3.Lerp(m_tongueOrigin.position, m_tongueDestination, m_tongueExtensionFactor), m_tongueExtensionFactor);
                    m_midwayToungables.RemoveAt(0);
                }
                AdvanceStateIfTime(nameof(Timings.extend));
                break;
            case State.PreRetreat:
                AdvanceStateIfTime(nameof(Timings.preRetreat));
                break;
            case State.Full:
                if (m_hitInteractable != null) m_hitInteractable.Interact(Kramp);
                if (m_hitTonguable != null) m_hitTonguable.DirectHit(Kramp, m_tongueDestination);
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


    public void ShootOut(Vector3 direction) {
        CurrentState = State.Windup;
        onStateChanged.Invoke(State.Idle, State.Windup);
        m_tongueDirection = new Vector3(direction.x, 0, direction.z).normalized;
        m_tongueExtensionFactor = 0;
        m_tongueTime = 0;
    }
}
