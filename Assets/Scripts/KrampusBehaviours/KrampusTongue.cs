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
	[BoxGroup("Visual")][SerializeField] private Transform m_inMouthOrigin;
	[BoxGroup("Visual")][SerializeField] private SkinnedMeshRenderer m_tongueAimIndicator;

	[BoxGroup("Physics")][SerializeField] private LayerMask m_interactionSearchMask = int.MaxValue;
	[BoxGroup("Physics")][SerializeField] private LayerMask m_interactionBlockerMask = int.MaxValue;
	[BoxGroup("Physics")][SerializeField] private Transform m_tongueOrigin;
	private float TongueLength => Kramp.Stats.GetFinalStat(KrampusStats.Stat.TongueRange);
	private float m_previousTongueLength;
	[BoxGroup("Physics")][SerializeField] private float m_tongueHitRadius = 0.5f;
	[BoxGroup("Controls")][SerializeField] private float m_inputMinimumDrag = 0.4f;
	[BoxGroup("Controls")][SerializeField] private float m_inputMinimumMouseDistance = 1f;
	//[BoxGroup("Controls")][SerializeField] private float m_inputMaximumMouseDistance = 4f;
	[BoxGroup("Controls")][SerializeField] private float m_inputDragSmoothing = 12f;
	[BoxGroup("Controls")][SerializeField] private float m_inputMousePlaneY = 1f;

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


	public Vector3 HitPoint => m_tongueDestination;
	private Vector3 m_tongueDestination;
	private float m_tongueTime = 0f;
	public Vector3 TongueDirection => m_tongueDirection;
	private Vector3 m_tongueDirection;
	private float m_tongueExtensionFactor = 0f;
	public IInteractable HitInteractable => m_hitInteractable;
	private IInteractable m_hitInteractable;
	private ITongueable m_hitTonguable;
	private IKrampable m_hitKrampable;

	public bool HoldsThrowable => m_hitKrampable != null && m_hitKrampable.Type == IKrampable.krampableType.DelayedAiming;

	private List<(float dst, ITongueable component)> m_midwayToungables;

	public enum State {
		Idle,
		Windup,
		TargetFetch,
		Extending,
		Full,
		PreRetreat,
		Retreating,
		Done,
		Carrying,
		Consume
	}

	public State CurrentState { get; private set; }

	private void Awake() {
		m_sequence.Init();
	}


	#region Input handling

	private Vector3 GetDirectionToMouse() {
		Vector3 point;
		if (!Physics.Raycast(Game.MainGameInfo.Krampus.Kamera.Rendering.ScreenPointToRay(InputSubscribe.Aim), out var info, 999, m_interactionSearchMask, QueryTriggerInteraction.Ignore)) {
			if (!MoreMath.LinePlaneIntersection(out point, Kramp.Kamera.Rendering.ScreenPointToRay(InputSubscribe.Aim), Vector3.up, Vector3.up * m_inputMousePlaneY)) {
				Debug.LogError("Something went horrendously wrong with aiming!");
				return Vector3.zero;
			}
		} else {
			point = info.point;
		}
		return point.NoY() + (Vector3.up * m_tongueOrigin.position.y) - m_tongueOrigin.position;
	}

	private Vector3 InputTongueDirection() {
		return InputSubscribe.InputMethod switch {
			InputSubscribe.Method.PC => Vector3.Lerp(m_tongueDirection, GetDirectionToMouse(), Time.deltaTime * m_inputDragSmoothing),
			_ => Vector3.Lerp(m_tongueDirection,
			//(Game.SetMan.GetValue<bool>("Controller Aim Flip") ? 1 : -1) *
			Kramp.Kamera.Matrix.MultiplyVector(new Vector3(InputSubscribe.Aim.x, 0, InputSubscribe.Aim.y)), Time.deltaTime * m_inputDragSmoothing),
		};
	}

	private bool InputWantsStartAiming() {
		return InputSubscribe.InputMethod switch {
			InputSubscribe.Method.PC => InputSubscribe.Raw.Player.BeginAiming.IsPressed() && !InputWantsCancelAiming(),
			_ => InputSubscribe.Raw.Player.BeginAiming.IsPressed()
		};
	}

	private bool InputWantsCancelAiming() {
		return InputSubscribe.InputMethod switch {
			InputSubscribe.Method.PC => GetDirectionToMouse().sqrMagnitude <= m_inputMinimumMouseDistance * m_inputMinimumMouseDistance,
			_ => InputSubscribe.Aim.magnitude <= m_inputMinimumDrag,
		};
	}

	private float InputGetShootFactor() {
		return InputSubscribe.InputMethod switch {
			InputSubscribe.Method.PC => Mathf.InverseLerp(m_inputMinimumMouseDistance, TongueLength, GetDirectionToMouse().magnitude),
			_ => Mathf.InverseLerp(m_inputMinimumDrag, 1f, InputSubscribe.Aim.magnitude),
		};
	}

	private bool InputWantsShoot() {
		return InputSubscribe.InputMethod switch {
			InputSubscribe.Method.PC => !InputSubscribe.Raw.Player.Shoot.IsPressed(),
			_ => InputSubscribe.Raw.Player.Shoot.WasPerformedThisFrame(),
		};
	}
	#endregion

	private void Ready() {
		m_previousTongueLength = TongueLength;
	}

	private void Start() {
		Kramp.Kontroller.onStateChanged += MovementStateChanged;
	}

	private void MovementStateChanged(KrampusController.State previous, KrampusController.State next, KrampusController.StateChangeReason reason) {
		if (next is KrampusController.State.Dead && m_hitKrampable != null)
			m_hitKrampable.Consume(Kramp, transform.position, transform.rotation);
	}

	private void Update() {
		if (!Game.Balling) {
			m_tongueAimIndicator.gameObject.SetActive(false);
			return;
		}
		if (TongueLength != m_previousTongueLength) {
			Kramp.KrampusEvents.onTongueLengthChanged.Invoke(Kramp, TongueLength - m_previousTongueLength);
		}
		m_previousTongueLength = TongueLength;

		switch (CurrentState) {
			case State.Carrying:
				if (m_hitKrampable != null) {
					m_hitKrampable.AttachToTongue(Kramp, m_inMouthOrigin.position, m_inMouthOrigin.rotation, 1);
				}
				goto case State.Idle;

			case State.Idle:
				m_tongueTime = 0;
				m_tongueAimIndicator.gameObject.SetActive(false);
				if (InputWantsStartAiming()) {
					if (m_hitKrampable != null && m_hitKrampable.Type == IKrampable.krampableType.DelayedSimple) {
						SwitchState(State.Consume);
					} else {
						SwitchState(State.Windup);
					}
				}
				break;

			case State.Windup: // Pre-shoot phase. Wait for the windup
				if (m_hitKrampable != null) {
					m_hitKrampable.AttachToTongue(Kramp, m_inMouthOrigin.position, m_inMouthOrigin.rotation, 1);
				}

				if (IsTime(nameof(Timings.windup))) {
					m_tongueTime = m_sequence.End(nameof(Timings.windup));

					m_tongueAimIndicator.gameObject.SetActive(true); // TODO: this should be done by the animator;
					m_tongueAimIndicator.transform.rotation = Quaternion.LookRotation(m_tongueDirection, Vector3.up);
					//m_tongueAimIndicator.SetBlendShapeWeight(0, InputGetShootFactor() * 100f);
					m_tongueAimIndicator.transform.localScale = new Vector3(100f, 100f, (100f / 7f) * TongueLength);

					if (InputWantsShoot()) {
						if (m_hitKrampable != null) {
							SwitchState(State.Consume);
						} else {
							SwitchState(State.TargetFetch);
							m_tongueExtensionFactor = 0;
							m_hitKrampable = null;
							m_hitInteractable = null;
							m_hitTonguable = null;
							m_midwayToungables = null;
							m_tongueAimIndicator.gameObject.SetActive(false);
							break;
						}
					}
				}

				if (InputWantsCancelAiming()) {
					if (m_hitKrampable != null) {
						SwitchState(State.Carrying);
					} else {
						SwitchState(State.Idle);
					}
					m_tongueTime = 0;
				}

				m_tongueDirection = InputTongueDirection();
				m_tongueDirection.y = 0;
				m_tongueDirection.Normalize();
				break;

			case State.Consume:
				try {
					m_hitKrampable.Consume(Kramp, m_tongueVisualOrigin.position, Quaternion.LookRotation(m_tongueDirection));
				} catch (Exception e) {
					LogException(e, m_hitKrampable);
				}
				m_tongueTime = 0;
				m_hitKrampable = null;
				m_hitInteractable = null;
				m_hitTonguable = null;
				m_midwayToungables = null;
				m_tongueAimIndicator.gameObject.SetActive(false);
				SwitchState(State.Idle);
				break;

			case State.TargetFetch: // Actually calculate what gets caught
									// Actually raycast from Krampus towards where the tongue is supposed to be shot.
				var hitObjects = Physics.CapsuleCastAll(
					new Vector3(m_tongueOrigin.position.x, Room.STANDARD_FLOOR_Y, m_tongueOrigin.position.z),
					new Vector3(m_tongueOrigin.position.x, Room.STANDARD_CEILING_Y, m_tongueOrigin.position.z), m_tongueHitRadius,
					m_tongueDirection, TongueLength, m_interactionSearchMask
				);

				var interactables = hitObjects.Select(w => (hit: w, interactable: w.collider.GetComponentInParent<IInteractable>()))
									.Where(w => w.interactable != null && w.interactable.CanInteract(Kramp))
									.NullIfEmpty()?
									.OrderBy(w => w.interactable is not IKrampable) // IKrampables first
									.ThenByDescending(w => w.interactable.Priority) // Priority (highest first)
									.ThenBy(w => Vector3.SqrMagnitude(m_tongueOrigin.position - w.hit.point)) // Distance (closest first)
									.Where(w => !Physics.Linecast(m_tongueOrigin.position, w.hit.point, m_interactionBlockerMask) || w.hit.distance == 0)
									.NullIfEmpty()?
									.Select(w => w.interactable);

				m_hitInteractable = interactables?.FirstOrDefault();

				if (m_hitInteractable is IKrampable krampable) {
					m_hitKrampable = krampable;
					try {
						m_hitKrampable.Prepare(Kramp);
					} catch (Exception e) {
						LogException(e, m_hitKrampable);
						m_hitKrampable = null;
						m_hitInteractable = null;
					}
				}
				if (m_hitInteractable == null) {
					if (Physics.Raycast(m_tongueOrigin.position, m_tongueDirection, out var hit, TongueLength, m_interactionBlockerMask)) {
						m_tongueDestination = hit.point;
					} else {
						m_tongueDestination = m_tongueVisualOrigin.position + (m_tongueDirection * TongueLength);
					}
				} else {
					m_tongueDestination = m_hitInteractable.InteractionPoint;
				}

				float tongueSqrMag = Vector3.SqrMagnitude(m_tongueDestination - m_tongueOrigin.position);

				var tongueables = hitObjects.Select(w => (hit: w, tongueable: w.collider.GetComponentInParent<ITongueable>()))
								   .Where(w => w.tongueable != null)
								   .NullIfEmpty()?
								   .Where(w => !Physics.Linecast(m_tongueOrigin.position, w.hit.point, m_interactionBlockerMask))
								   .NullIfEmpty()?
								   .Select(w => (dst: Vector3.SqrMagnitude(m_tongueOrigin.position - w.hit.point) / tongueSqrMag, tongueable: w.tongueable))
								   .OrderBy(w => w.dst);

				// Determine the primary Tongueable - if we have already hit something, we give priority to the previously hit Interactable
				if (m_hitInteractable == null || !m_hitInteractable.GameObject.TryGetComponent<ITongueable>(out m_hitTonguable)) {
					m_hitTonguable = Physics.OverlapSphere(m_tongueDestination, m_tongueHitRadius).Select(w => w.GetComponent<ITongueable>()).FirstOrDefault();
					DebugHelper.DrawSphere(m_tongueDestination, m_tongueHitRadius, Color.red, 5f);
				}

				// Find all Tonguables to update with the extending tongue. We want to not include the main hit one, as its 'Hit' will get called separately
				float fullLength = (m_tongueDestination - m_tongueOrigin.position).sqrMagnitude;

				m_midwayToungables = (tongueables?.Where(w => w.tongueable != m_hitTonguable)).EmptyIfNull().ToList();
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
						//if (m_hitInteractable is Child) {
						Kramp.Kontroller.SetCanDash(true);
						Kramp.Kontroller.SetDashTarget(m_hitInteractable);
						//}
					} catch (Exception e) {
						LogException(e, m_hitInteractable);
						m_hitInteractable = null;
						m_hitKrampable = null;
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
				if (m_hitKrampable != null) {
					try {
						m_hitKrampable.AttachToTongue(Kramp, GetTonguePositions().end, transform.rotation, 0);
					} catch (Exception e) {
						LogException(e, m_hitTonguable);
						m_hitKrampable = null;
						m_hitInteractable = null;
					}
				}
				AdvanceStateIfTime(nameof(Timings.preRetreat));
				break;

			case State.Retreating: // Tongue goes from the target to visual origin, potentially carrying an krampable
				if (m_hitKrampable != null) {
					try {
						m_hitKrampable.AttachToTongue(Kramp, GetTonguePositions().end, transform.rotation, m_sequence.InverseLerp(nameof(Timings.retreat), m_tongueTime));
					} catch (Exception e) {
						LogException(e, m_hitTonguable);
						m_hitKrampable = null;
						m_hitInteractable = null;
					}
				}
				m_tongueExtensionFactor = m_sequence.retreatCurve.Evaluate(1 - m_sequence.InverseLerp(nameof(Timings.retreat), m_tongueTime));
				AdvanceStateIfTime(nameof(Timings.retreat));
				break;

			case State.Done: // Tongue is back at origin, if caught something - eat it unless it is not krampable
				if (m_hitKrampable != null) {
					if (m_hitKrampable.Type == IKrampable.krampableType.Instant) {
						SwitchState(State.Consume);
					} else {
						SwitchState(State.Carrying);
					}
				} else {
					m_tongueTime = 0;
					m_hitKrampable = null;
					m_hitInteractable = null;
					m_hitTonguable = null;
					m_midwayToungables = null;
					m_tongueAimIndicator.gameObject.SetActive(false);
					SwitchState(State.Idle);
				}
				Kramp.Kontroller.LockOut();
				Kramp.Kontroller.SetCanDash(false);
				Kramp.Kontroller.SetDashTarget(null);
				break;
		}

		m_tongueRenderer.SetPosition(0, GetTonguePositions().begin);
		m_tongueRenderer.SetPosition(1, GetTonguePositions().end);

		m_tongueTime += Time.deltaTime;
	}

	/// <summary>
	/// Moves the state forward and notifies the change
	/// </summary>
	private void AdvanceState() {
		var previous = CurrentState;
		if (CurrentState == State.Done) CurrentState = State.Idle;
		else CurrentState++;
		onStateChanged?.Invoke(previous, CurrentState);
	}

	private void SwitchState(State state) {
		var previous = CurrentState;
		if (state == previous) return;
		CurrentState = state;
		onStateChanged?.Invoke(previous, CurrentState);
	}

	/// <summary>
	/// Advance the state if a certain time was reached
	/// </summary>
	/// <param name="nameof">Name of the time param from m_tng</param>
	/// <returns>Whether the state has been advanced</returns>
	private bool AdvanceStateIfTime(string nameof) {
		if (IsTime(nameof)) {
			AdvanceState();
			return true;
		} else return false;
	}

	private bool IsTime(string nameof) {
		return m_tongueTime >= m_sequence.End(nameof);
	}

	private void LogException(Exception e, object context) {
		switch (context) {
			case IKrampable krampable:
				Debug.LogError($"Interaction error caught on krampable: {e.GetType().Name}\n{e}", krampable.GameObject);
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
		if (CurrentState == State.Idle) return (m_tongueVisualOrigin.position, m_tongueVisualOrigin.position);
		return (m_tongueVisualOrigin.position, Vector3.Lerp(m_tongueVisualOrigin.position, m_tongueDestination, m_tongueExtensionFactor));
	}

	public void SetTongueLineRenderer(bool b) {
		m_tongueRenderer.enabled = b;
	}
}
