using System;
using System.Collections;
using KrampUtils;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.VFX;

public class KrampusController : KrampusBehaviour {
	[ShowNativeProperty] public State CurrentState { get; private set; }
	public Vector3 VelocityVector => m_rigidbody.velocity;
	public float RunSpeed => m_runSpeed;
	public float Velocity => VelocityVector.magnitude;
	public float VelocitySqr => VelocityVector.sqrMagnitude;
	public float SneakSpeed => m_sneakSpeed;

	public UnityAction<KrampusController.State, KrampusController.State, KrampusController.StateChangeReason> onStateChanged;

	[SerializeField] private Rigidbody m_rigidbody;

	[BoxGroup("Speed Control")][SerializeField] private float m_sneakSpeed = 5f;
	[BoxGroup("Speed Control")][SerializeField] private float m_runSpeed = 10f;
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



	private Vector4 m_inputWeights;
	private float m_timeout;
	private float m_timeHoldingInput = 0f;
	private float m_previousFrameVelocity = 0f;


	public enum State {
		Intro,
		GetUp,
		Idle,
		Run,
		Walk,
		Dead
	}

	public enum StateChangeReason {
		Normal,
		Rapid
	}


	// Based on @SkilluDev's inputs
	private void Update() {
		if (!Game.Balling) {
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

		if (CurrentState == State.Dead) return;

		//		Debug.LogWarning(Game.MainGameInfo.Timer);
		if (Game.MainGameInfo.Timer.GameTime < 0) {
			Debug.Log("dead as hell");
			KrampTermination(Ending.LoseTime);
		}
		float acceleration = m_previousFrameVelocity - m_rigidbody.velocity.sqrMagnitude;

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
		if (inputs.sqrMagnitude == 0 && m_rigidbody.velocity.sqrMagnitude <= m_veloIdleThreshold) {
			if (m_timeHoldingInput >= m_timeIdleThreshold) { //if was accelarating for some time
				reason = StateChangeReason.Rapid;
			}
			state = State.Idle;
		} else if (acceleration >= m_deltaIdleThreshold) { //if sudden velocity loss

			reason = StateChangeReason.Rapid;
			state = State.Idle;

		} else if (CurrentState == State.Idle && m_rigidbody.velocity.sqrMagnitude <= m_startRunSpeed) { //if was already idle with no velocity
			state = State.Idle;
		} else {
			if (InputSubscribe.Sneaking || Kramp.Tongue.CurrentState != KrampusTongue.State.Idle) {
				state = State.Walk;
			} else {
				state = State.Run;
			}
		}

		ChangeState(state, reason);

		//Process what changed
		if (inputs.sqrMagnitude != 0) {
			m_timeHoldingInput += Time.deltaTime;
		}
		if (CurrentState == State.Idle) m_timeHoldingInput = 0f;
		m_previousFrameVelocity = m_rigidbody.velocity.sqrMagnitude;
	}

	private void ChangeState(State to, StateChangeReason reason) {
		if (to == CurrentState) return;
		onStateChanged?.Invoke(CurrentState, to, reason);
		CurrentState = to;
	}

	public void KrampTermination(Ending ending) {
		if (Game.MainGameInfo.CurrentState == MainGameInfo.State.Over) {
			return;
		}

		Game.MainGameInfo.SetState(MainGameInfo.State.Over);
		ChangeState(State.Dead, StateChangeReason.Rapid);
		Game.MainGameInfo.Krampus.Kamera.DefaultShake.GenerateImpulse();
		m_rigidbody.velocity = Vector3.zero;
		m_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		StartCoroutine(DeathTimer(ending));
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

		var computedVelocity = ComputeVelocity();
		computedVelocity = computedVelocity.normalized * Mathf.Max(Mathf.Abs(computedVelocity.x), Mathf.Abs(computedVelocity.z));
		var skewedInput = Kramp.Kamera.Matrix.MultiplyPoint3x4(computedVelocity);
		m_rigidbody.velocity = skewedInput * (CurrentState != State.Run ? m_sneakSpeed : m_runSpeed);
		if (Physics.Raycast(transform.position, VelocityVector, out var hit, m_assistCheckLength, m_avoidableObjects, QueryTriggerInteraction.Ignore)) {
			if (m_avoidableObjects == (m_avoidableObjects | 1 << hit.transform.gameObject.layer)) {
				if (!Physics.Raycast(transform.position, Quaternion.Euler(0, -m_assistValue, 0) * (VelocityVector), m_assistCheckLength)) {
					m_rigidbody.velocity = Quaternion.Euler(0, -m_assistValue, 0) * (VelocityVector);
				} else if (!Physics.Raycast(transform.position, Quaternion.Euler(0, m_assistValue, 0) * (VelocityVector), m_assistCheckLength)) {
					m_rigidbody.velocity = Quaternion.Euler(0, m_assistValue, 0) * (VelocityVector);
				}
			}
		}
	}

	private IEnumerator DeathTimer(Ending ending) {
		yield return new WaitForSeconds(1);
		Game.MainGameInfo.ProcessEndGame(ending);
	}

	public void MoveTo(Vector3 position) {
		m_rigidbody.position = position.NoY() + Vector3.up * m_rigidbody.position.y;
	}
}
