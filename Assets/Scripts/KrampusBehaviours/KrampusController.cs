using System;
using System.Collections;
using KrampUtils;
using LitMotion;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.VFX;
using static KrampusStats;

public class KrampusController : KrampusBehaviour {
	[ShowNativeProperty] public State CurrentState { get; private set; }
	public Vector3 VelocityVector => m_rigidbody.linearVelocity;
	public float RunSpeed => Kramp.Stats.GetFinalStat(Stat.Speed);
	public float Velocity => VelocityVector.magnitude;
	public float VelocitySqr => VelocityVector.sqrMagnitude;
	public float SneakSpeed => m_sneakSpeed;


	public UnityAction<KrampusController.State, KrampusController.State, KrampusController.StateChangeReason> onStateChanged;

	[SerializeField] private Rigidbody m_rigidbody;
	[BoxGroup("Speed Control")][SerializeField] private float m_weightedRunMultiplier = 0.8f;
	[BoxGroup("Speed Control")][SerializeField] private float m_sneakSpeed = 5f;
	[BoxGroup("Movement Assist")][SerializeField] private int m_assistValue = 45;
	[BoxGroup("Movement Assist")][SerializeField] private int m_assistCheckLength = 1;
	[BoxGroup("Movement Assist")][SerializeField] private LayerMask m_avoidableObjects;
	[BoxGroup("Acceleration")][SerializeField] private float m_accelerationTime = 0.2f;
	[BoxGroup("Acceleration")][SerializeField] private AnimationCurve m_accelerationCurve = AnimationCurve.Linear(0, 0, 1, 1);
	[BoxGroup("Acceleration")][SerializeField] private float m_movementThreshold = 0.5f;
	[BoxGroup("Acceleration")][SerializeField][FormerlySerializedAs("m_veloIdleThreshhold")] private float m_veloIdleThreshold = 120f;
	[BoxGroup("Acceleration")][SerializeField] private float m_timeIdleThreshold = 0.2f;
	[BoxGroup("Acceleration")][SerializeField] private float m_deltaIdleThreshold = 100f;
	[BoxGroup("Acceleration")][SerializeField] private float m_startRunSpeed = 2f;
	[BoxGroup("Intro")][SerializeField] private float m_getUpDuration = 1f;


	//Dashing
	private Vector4 m_inputWeights;
	private float m_timeout;
	private float m_timeHoldingInput = 0f;
	private float m_previousFrameVelocity = 0f;
	private float m_dashTime;
	[BoxGroup("Dash")][SerializeField] private float m_dashSpeedMultiplier = 4f;
	[BoxGroup("Dash")][SerializeField] private float m_dashCurveEvalSpeed;

	[BoxGroup("Dash")][SerializeField] private AnimationCurve m_dashCurve = AnimationCurve.Linear(0, 0, 1, 1);

	private bool m_canDash = false;

	public bool CanDash => m_canDash;
	[BoxGroup("Dash")][SerializeField] private float m_windUpDashCost;
	public float WindUpDashCost => m_windUpDashCost;
	private IInteractable m_dashTarget;

	private bool m_isLockedIn;
	public bool IsLockedIn => m_isLockedIn;

	private float m_lockInTimer = 0f;



	[BoxGroup("Lock In")][SerializeField] private float m_lockInThreshold;
	public float LockInThreshold => m_lockInThreshold;


	//Wind-up
	private float m_windUpPoints;
	private float WindUpBonus => Kramp.Stats.GetFinalStat(Stat.WindUpGain);

	public float WindUpPoints { get => m_windUpPoints; set => m_windUpPoints = value; }
	private float m_windUpGainLock = 0;



	private void Start() {
		Game.MainGameInfo.UI.SetWindUpCostBar(m_windUpDashCost);
		Kramp.KrampusEvents.onNaughtyChildEaten.AddListener(OnNaughtyChildEaten);
	}

	public enum State {
		Intro,
		GetUp,
		Idle,
		Run,
		Walk,
		Dash,
		Dead
	}

	public enum StateChangeReason {
		Normal,
		Rapid
	}
	// Based on @SkilluDev's inputs
	private void Update() {


		if (!Game.Balling) {
			m_rigidbody.linearVelocity = Vector3.zero;
			return;
		}
		if (CurrentState == State.Intro) {
			m_timeout = m_getUpDuration;
			ChangeState(State.GetUp, StateChangeReason.Normal);
		}

		if (CurrentState == State.GetUp) {
			m_timeout -= Time.deltaTime;
			if (m_timeout <= 0) ChangeState(State.Idle, StateChangeReason.Normal);
			return;
		}

		if (m_windUpGainLock > 0) {
			m_windUpGainLock -= Time.deltaTime;
		}

		if (CurrentState is State.Dead or State.Dash) return;

		float acceleration = m_previousFrameVelocity - m_rigidbody.linearVelocity.sqrMagnitude;

		var inputs = InputSubscribe.Movement;
		float adjustedTime = Time.deltaTime / m_accelerationTime;


		m_inputWeights.x += inputs.x > m_movementThreshold ? adjustedTime : -adjustedTime;
		m_inputWeights.z += inputs.x < -m_movementThreshold ? adjustedTime : -adjustedTime;

		m_inputWeights.y += inputs.y > m_movementThreshold ? adjustedTime : -adjustedTime;
		m_inputWeights.w += inputs.y < -m_movementThreshold ? adjustedTime : -adjustedTime;

		m_inputWeights.x = Mathf.Clamp01(m_inputWeights.x);
		m_inputWeights.y = Mathf.Clamp01(m_inputWeights.y);
		m_inputWeights.z = Mathf.Clamp01(m_inputWeights.z);
		m_inputWeights.w = Mathf.Clamp01(m_inputWeights.w);





		var state = CurrentState;
		var reason = StateChangeReason.Normal;

		//if slow and no input
		if (inputs.sqrMagnitude == 0 && m_rigidbody.linearVelocity.sqrMagnitude <= m_veloIdleThreshold) {
			if (m_timeHoldingInput >= m_timeIdleThreshold) { //if was accelarating for some time
				reason = StateChangeReason.Rapid;
			}
			state = State.Idle;
		} else if (acceleration >= m_deltaIdleThreshold) { //if sudden velocity loss

			reason = StateChangeReason.Rapid;
			state = State.Idle;

		} else if (CurrentState == State.Idle && m_rigidbody.linearVelocity.sqrMagnitude <= m_startRunSpeed) { //if was already idle with no velocity
			state = State.Idle;
		} else {
			if (InputSubscribe.Sneaking /* || Kramp.Tongue.CurrentState is not (KrampusTongue.State.Idle or KrampusTongue.State.Carrying) */) {
				state = State.Walk;
			} else {
				state = State.Run;
			}
		}


		if (state == State.Walk || state == State.Idle) {
			if (!IsLockedIn) {

				m_lockInTimer += Time.deltaTime;
				if (m_lockInTimer > m_lockInThreshold) {
					LockIn();
				}
			}
		} else {
			LockOut();
		}

		ChangeState(state, reason);

		//Process what changed
		if (inputs.sqrMagnitude != 0) {
			m_timeHoldingInput += Time.deltaTime;
		}
		if (CurrentState == State.Idle) m_timeHoldingInput = 0f;
		m_previousFrameVelocity = m_rigidbody.linearVelocity.sqrMagnitude;

		if (CanDash && InputSubscribe.Special) {
			Dash();
		}
	}

	public void LockIn() {
		Kramp.KrampusEvents.onLockIn.Invoke(Kramp);
		m_isLockedIn = true;
	}

	public void LockOut() {
		if (IsLockedIn) {
			Kramp.KrampusEvents.onLockOut.Invoke(Kramp);
			m_isLockedIn = false;
		}
		m_lockInTimer = 0f;
	}
	public void Dash() {
		if (!CanDash) return;
		m_dashTime = 0;
		//m_dashDirection = Kramp.Tongue.TongueDirection;
		Kramp.KrampusEvents.onDashUsed.Invoke(Kramp);
		ChangeState(State.Dash, StateChangeReason.Rapid);
		//Debug.Log("Do dashing " + m_dashDirection);

		m_windUpGainLock = 1;
		SpendWindUpPoints(WindUpDashCost);
		SetCanDash(false);
	}

	private void ChangeState(State to, StateChangeReason reason) {
		if (to == CurrentState) return;
		onStateChanged?.Invoke(CurrentState, to, reason);
		CurrentState = to;
	}

	public void KrampTermination(Ending ending) {
		if (!Game.Balling) return;
		ChangeState(State.Dead, StateChangeReason.Rapid);
		Game.MainGameInfo.Krampus.Kamera.DefaultShake.GenerateImpulse();
		m_rigidbody.linearVelocity = Vector3.zero;
		m_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		Game.MainGameInfo.ProcessEndGame(ending);
	}

	private Vector3 ComputeVelocity() {
		return new Vector3(
			m_accelerationCurve.Evaluate(m_inputWeights.x) - m_accelerationCurve.Evaluate(m_inputWeights.z),
			0,
			m_accelerationCurve.Evaluate(m_inputWeights.y) - m_accelerationCurve.Evaluate(m_inputWeights.w)
		);
	}

	private void FixedUpdate() {
		if (!Game.Balling) return;
		if (CurrentState == State.Dead) return;

		if (CurrentState == State.Dash) {
			if (m_dashTarget == null) {
				ChangeState(State.Run, StateChangeReason.Rapid);
				return;
			}
			m_dashTime += Time.fixedDeltaTime * m_dashCurveEvalSpeed;
			//Debug.Log("Dash Time: " + m_dashTime);
			Vector3 direction = m_dashTarget.GameObject.transform.position - transform.position;
			float distance = direction.sqrMagnitude;
			m_rigidbody.linearVelocity = direction.normalized * m_dashCurve.Evaluate(m_dashTime) * m_dashSpeedMultiplier * RunSpeed;

			if (distance < 1) {
				ChangeState(State.Run, StateChangeReason.Rapid);
			}
			return;
		}

		var computedVelocity = ComputeVelocity();
		computedVelocity = computedVelocity.normalized * Mathf.Max(Mathf.Abs(computedVelocity.x), Mathf.Abs(computedVelocity.z));
		var skewedInput = Kramp.Kamera.Matrix.MultiplyPoint3x4(computedVelocity);

		if (CurrentState != State.Run) {
			m_rigidbody.linearVelocity = skewedInput * SneakSpeed;
		} else {
			m_rigidbody.linearVelocity = skewedInput * RunSpeed;
			/*
			if (Kramp.Tongue.InMouth != null) {
				m_rigidbody.velocity *= m_weightedRunMultiplier;
			}*/
		}

		if (Physics.Raycast(transform.position, VelocityVector, out var hit, m_assistCheckLength, m_avoidableObjects, QueryTriggerInteraction.Ignore)) {
			if (m_avoidableObjects == (m_avoidableObjects | 1 << hit.transform.gameObject.layer)) {
				if (!Physics.Raycast(transform.position, Quaternion.Euler(0, -m_assistValue, 0) * (VelocityVector), m_assistCheckLength)) {
					m_rigidbody.linearVelocity = Quaternion.Euler(0, -m_assistValue, 0) * (VelocityVector);
				} else if (!Physics.Raycast(transform.position, Quaternion.Euler(0, m_assistValue, 0) * (VelocityVector), m_assistCheckLength)) {
					m_rigidbody.linearVelocity = Quaternion.Euler(0, m_assistValue, 0) * (VelocityVector);
				}
			}
		}

	}

	public void MoveTo(Vector3 position) {
		m_rigidbody.position = position.NoY() + Vector3.up * m_rigidbody.position.y;
	}


	public void AddWindUpPoints(float value) {

		if (m_windUpGainLock > 0) {
			//Debug.Log("Block");
			return;
		}


		//Debug.Log("Windup bonus: " + WindUpBonus);

		m_windUpPoints += value * WindUpBonus;
		if (m_windUpPoints > Game.MainGameInfo.MaxWindUpPoints) {
			m_windUpPoints = Game.MainGameInfo.MaxWindUpPoints;
		}
		Game.MainGameInfo.UI.ChangeWindUpValue(m_windUpPoints);
		Kramp.KrampusEvents.onWindUpChanged.Invoke(Kramp, m_windUpPoints);
	}

	public void SpendWindUpPoints(float value) {
		m_windUpPoints -= value;
		Game.MainGameInfo.UI.ChangeWindUpValue(m_windUpPoints, 0.1f);
		Kramp.KrampusEvents.onWindUpChanged.Invoke(Kramp, m_windUpPoints);
	}

	// TODO: why is this public and not a single method with set dash target
	public void SetCanDash(bool canDash) {
		if (Game.PogMan.GetCurrentLevelStats().LockWindUpUse || WindUpPoints < WindUpDashCost) {
			m_canDash = false;
		} else {
			m_canDash = canDash;
		}
		Game.MainGameInfo.UI.WorldSpaceUI.SetDashIcon(m_canDash);
	}

	public void OnNaughtyChildEaten(Krampus krampus, Child child) {
		AddWindUpPoints(Game.MainGameInfo.WindUpGainFromChildren);
	}

	public void SetDashTarget(IInteractable interactable) {
		m_dashTarget = interactable;
		if (m_dashTarget == null) return;
		Game.MainGameInfo.UI.WorldSpaceUI.SetDashIconPosition(m_dashTarget);
	}
}
