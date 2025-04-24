using System;
using System.Collections.Generic;
using System.Linq;
using KrampUtils;
using NaughtyAttributes;
using Roomgen;
using UnityEngine;
using UnityEngine.Events;

public class KrampusTongue : KrampusBehaviour {

    public UnityAction<KrampusTongue.State, KrampusTongue.State> onStateChanged;
    [BoxGroup("Visual")][SerializeField] private LineRenderer m_tongueRenderer;
    [BoxGroup("Visual")][SerializeField] private Transform m_tongueVisualOrigin;
    [BoxGroup("Visual")][SerializeField] private Texture2D m_cursor; // TODO: cursor should not be set here!

    [BoxGroup("Physics")][SerializeField] private LayerMask m_layerMask = int.MaxValue;
    [BoxGroup("Physics")][SerializeField] private Transform m_tongueOrigin;
    [BoxGroup("Physics")][SerializeField] private float m_tongueLength = 8;
    [BoxGroup("Physics")][SerializeField] private float m_tongueHitRadius = 0.5f;

    [Serializable]
    private class Timings : TimedSequence<Timings> {
        [SeqDuration] public float windup = 0.4f;
        [SeqDuration] public float extend = 0.2f;
        public AnimationCurve extendCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SeqDuration] public float preRetreat = 0.1f;
        [SeqDuration] public float retreat = 0.3f;
        public AnimationCurve retreatCurve = AnimationCurve.Linear(0, 0, 1, 1);
    }
    [SerializeField] private Timings m_sequence;


    private Vector3 m_tongueDestination;
    private float m_tongueTime = 0f;
    private Vector3 m_tongueDirection;
    private Vector3 m_tongueSpecificPoint;
    private float m_tongueExtensionFactor = 0f;
    private IInteractable m_hitInteractable;
    private ITongueable m_hitTonguable;
    private IEdible m_hitEdible;
    private List<(float dst, ITongueable component)> m_midwayToungables;

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
    public State CurrentState { get; private set; }

    private void Awake() {
        m_sequence.Init();
        Cursor.SetCursor(m_cursor, new Vector2(m_cursor.width / 2, m_cursor.height / 2), CursorMode.ForceSoftware);
    }

    private void HandleInput() {
        if (Input.GetMouseButtonDown(0)) {
	        ShootOut();

        }
    }

    public void ShootOut() {
        CurrentState = State.Windup;
        onStateChanged.Invoke(State.Idle, State.Windup);
        m_tongueTime = 0;
        m_tongueExtensionFactor = 0;
        m_hitEdible = null;
        m_hitInteractable = null;
        m_hitTonguable = null;
        m_midwayToungables = null;
    }


    private void Update() {
        if (CurrentState == State.Idle)
            HandleInput();


        switch (CurrentState) {
            case State.Windup: // Pre-shoot phase. Wait for the windup
                AdvanceStateIfTime(nameof(Timings.windup));
                break;

            case State.TargetFetch: // Actually calculate what gets caught

	            var ray = Kramp.Kamera.Raw.ScreenPointToRay(Input.mousePosition);
	            if (Physics.Raycast(ray, out var mouseHit, 1000, m_layerMask)) {
		            m_tongueSpecificPoint = mouseHit.point;
	            } else {
		            //reset the state machine?
		            CurrentState = State.Idle;
		            Debug.Log("Missed!");
		            break;
	            }

                m_tongueDirection = m_tongueSpecificPoint - m_tongueOrigin.position;
                m_tongueDirection.y = 0;
                m_tongueDirection.Normalize();

                // Actually raycast from Krampus towards where the tongue is supposed to be shot.
                var checkingPoint = Physics.Raycast(transform.position, m_tongueDirection, out var hit, m_tongueLength, m_layerMask) ?
                    hit.point : m_tongueOrigin.position + (m_tongueDirection * m_tongueLength);

                var hitObjects = Physics.OverlapCapsule(new Vector3(hit.point.x, Room.STANDARD_FLOOR_Y, hit.point.z), new Vector3(hit.point.x, Room.STANDARD_CEILING_Y, hit.point.z), m_tongueHitRadius);

                m_hitInteractable = hitObjects.Select(w => w.GetComponent<IInteractable>()).Where(w => w != null && w.CanInteract(Kramp)).NullIfEmpty()?.MinBy(w => (w.GameObject.transform.position - m_tongueSpecificPoint).sqrMagnitude);

                if (m_hitInteractable is IEdible edible) {
                    m_hitEdible = edible;
                    try {
                        m_hitEdible.Prepare(Kramp);
                    } catch (Exception e) {
                        LogException(e, m_hitEdible);
                        m_hitEdible = null;
                        m_hitInteractable = null;
                    }
                }

                // Determine the primary Tongueable - if we have already hit something, we give priority to the previously hit Interactable
                if (m_hitInteractable == null || !m_hitInteractable.GameObject.TryGetComponent<ITongueable>(out m_hitTonguable)) {
                    m_hitTonguable = hitObjects.Select(w => w.GetComponent<ITongueable>()).FirstOrDefault(w => w != null);
                }

                m_tongueDestination = m_hitInteractable == null ? checkingPoint : m_hitInteractable.InteractionPoint;

                // Find all Tonguables to update with the extending tongue. We want to not include the main hit one, as its 'Hit' will get called separately
                var possibleTongueables = Physics.OverlapCapsule(transform.position, m_tongueDestination, m_tongueHitRadius);
                float fullLength = (m_tongueDestination - transform.position).sqrMagnitude;
                m_midwayToungables = possibleTongueables
                    .Select(w => (dst: (w.transform.position - transform.position).sqrMagnitude / fullLength, component: w.GetComponent<ITongueable>()))
                    .Where(w => w.component != null && (m_hitTonguable == null || w.component.GameObject != m_hitTonguable.GameObject))
                    .OrderBy(w => w.dst).ToList();

                AdvanceState();
                break;

            case State.Extending: // Tongue goes from visual origin to target, activating the tonguables along the way
                if (m_hitInteractable != null) m_tongueDestination = m_hitInteractable.InteractionPoint;

                m_tongueExtensionFactor = m_sequence.extendCurve.Evaluate(m_sequence.InverseLerp(nameof(Timings.extend), m_tongueTime));
                if (m_midwayToungables.Count > 0 && m_tongueExtensionFactor >= m_midwayToungables[0].dst) {
                    try {
                        m_midwayToungables[0].component.TonguePassBy(Kramp, Vector3.Lerp(m_tongueVisualOrigin.position, m_tongueDestination, m_tongueExtensionFactor), m_tongueExtensionFactor);
                    } catch (Exception e) {
                        LogException(e, m_midwayToungables[0].component);
                    }
                    m_midwayToungables.RemoveAt(0);
                }
                AdvanceStateIfTime(nameof(Timings.extend));
                break;

            case State.Full:
                if (m_hitInteractable != null) {
                    try {
                        m_hitInteractable.Interact(Kramp);
                    } catch (Exception e) {
                        LogException(e, m_hitInteractable);
                        m_hitInteractable = null;
                        m_hitEdible = null;
                    }
                    m_tongueDestination = m_hitInteractable.InteractionPoint;
                }
                if (m_hitTonguable != null) {
                    try {
                        m_hitTonguable.TongueHit(Kramp, m_tongueDestination);
                    } catch (Exception e) {
                        LogException(e, m_hitTonguable);
                        m_hitTonguable = null;
                    }
                }
                AdvanceState();
                break;

            case State.PreRetreat: // Tongue still attached to the hit object
                if (m_hitInteractable != null) m_tongueDestination = m_hitInteractable.InteractionPoint;
                if (m_hitEdible != null) {
                    try {
                        m_hitEdible.ReelIn(Kramp, GetTonguePositions().end, 0);
                    } catch (Exception e) {
                        LogException(e, m_hitTonguable);
                        m_hitEdible = null;
                        m_hitInteractable = null;
                    }
                }
                AdvanceStateIfTime(nameof(Timings.preRetreat));
                break;

            case State.Retreating: // Tongue goes from the target to visual origin, potentially carrying an Edible
                if (m_hitEdible != null) {
                    try {
                        m_hitEdible.ReelIn(Kramp, GetTonguePositions().end, m_sequence.InverseLerp(nameof(Timings.retreat), m_tongueTime));
                    } catch (Exception e) {
                        LogException(e, m_hitTonguable);
                        m_hitEdible = null;
                        m_hitInteractable = null;
                    }
                }
                m_tongueExtensionFactor = m_sequence.retreatCurve.Evaluate(1 - m_sequence.InverseLerp(nameof(Timings.retreat), m_tongueTime));
                AdvanceStateIfTime(nameof(Timings.retreat));
                break;

            case State.Eating: // Tongue is back at origin, if caught something - eat it
                if (m_hitEdible != null) {
                    try {
                        m_hitEdible.Consume(Kramp);
                    } catch (Exception e) {
                        LogException(e, m_hitEdible);
                    }
                }

                m_hitEdible = null;
                m_hitInteractable = null;
                m_hitTonguable = null;

                AdvanceState();
                break;
        }

        m_tongueRenderer.SetPosition(0, GetTonguePositions().begin);
        m_tongueRenderer.SetPosition(1, GetTonguePositions().end);

        m_tongueTime += CurrentState != State.Idle ? Time.deltaTime : 0;
    }

    /// <summary>
    /// Moves the state forward and notifies the change
    /// </summary>
    private void AdvanceState() {
        var previous = CurrentState;
        if (CurrentState == State.Eating) CurrentState = State.Idle;
        else CurrentState++;
        onStateChanged.Invoke(previous, CurrentState);
    }

    /// <summary>
    /// Advance the state if a certain time was reached
    /// </summary>
    /// <param name="nameof">Name of the time param from m_tng</param>
    /// <returns>Whether the state has been advanced</returns>
    private bool AdvanceStateIfTime(string nameof) {
        if (m_tongueTime >= m_sequence.End(nameof)) {
            AdvanceState();
            return true;
        } else return false;
    }

    private void LogException(Exception e, object context) {
        switch (context) {
            case IEdible edible:
                Debug.LogError($"Interaction error caught on Edible: {e.GetType().Name}\n{e}", edible.GameObject);
                break;
            case IInteractable interactable:
                Debug.LogError($"Interaction error caught on Interactable: {e.GetType().Name}\n{e}", interactable.GameObject);
                break;
            case ITongueable tongueable:
                Debug.LogError($"Interaction error caught on Tongueable: {e.GetType().Name}\n{e}", tongueable.GameObject);
                break;
            case UnityEngine.Object uobj:
                Debug.LogError($"Interaction error caught on Other ({e.GetType().Name}): {e.GetType().Name}\n{e}", uobj);
                break;
            default:
                Debug.LogError($"Interaction error caught on Other ({e.GetType().Name}): {e.GetType().Name}\n{e}");
                break;
        }
    }

    private (Vector3 begin, Vector3 end) GetTonguePositions() {
        if (CurrentState == State.Idle) return (Vector3.one * -10_000_000, Vector3.one * -10_000_000);
        return (m_tongueVisualOrigin.position, Vector3.Lerp(m_tongueVisualOrigin.position, m_tongueDestination, m_tongueExtensionFactor));
    }

    public Vector3 GetTongueDirection() {
	    return m_tongueDirection;
    }

}
