using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class KrampusController : KrampusBehaviour {
	public State CurrentState { get; private set; }
	public Vector3 VelocityVector => m_rigidbody.velocity;
	public float RunSpeed => m_runSpeed;
	public float Velocity => VelocityVector.magnitude;
	public float VelocitySqr => VelocityVector.sqrMagnitude;
	public float SneakSpeed => m_sneakSpeed;

	public UnityAction<KrampusController.State, KrampusController.State> onStateChange;

	[SerializeField] private Rigidbody m_rigidbody;

	[BoxGroup("Speed Control")][SerializeField] private float m_sneakSpeed = 5f;
	[BoxGroup("Speed Control")][SerializeField] private float m_runSpeed = 10f;
	[BoxGroup("Movement Assist")][SerializeField] private int m_assistValue = 45;
	[BoxGroup("Movement Assist")][SerializeField] private int m_assistCheckLength = 1;
	[BoxGroup("Movement Assist")][SerializeField] private LayerMask m_avoidableObjects;
	[BoxGroup("Acceleration")][SerializeField] private float m_accelerationTime = 0.2f;
	[BoxGroup("Acceleration")][SerializeField] private AnimationCurve m_accelerationCurve = AnimationCurve.Linear(0, 0, 1, 1);
	[BoxGroup("Acceleration")][SerializeField] private float m_movementThreshold = 0.5f;
	[BoxGroup("Acceleration")][SerializeField] private float m_veloIdleThreshhold = 120f;
	[BoxGroup("Acceleration")][SerializeField] private float m_timeIdleThreshold = 0.2f;
	[BoxGroup("Acceleration")][SerializeField] private float m_deltaIdleThreshold = 100f;
	[BoxGroup("Acceleration")][SerializeField] private float m_startRunSpeed = 2f;
	private Vector4 m_inputWeights;
	private State m_temporaryState;
	private float m_timeHoldingInput = 0f;
	private float m_previousFrameVelocity = 0f;
	private bool m_isSuddenStop;
	private float m_veloDeltaFromLastFrame;


	public enum State {
		Idle,
		Run,
		Walk
	}


	// Based on @SkilluDev's inputs
	private void Update() {
		m_veloDeltaFromLastFrame = m_previousFrameVelocity - m_rigidbody.velocity.sqrMagnitude;

		var inputs = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		float adjustedTime = Time.deltaTime / m_accelerationTime;


		m_inputWeights.x += inputs.x > m_movementThreshold ? adjustedTime : -adjustedTime;
		m_inputWeights.z += inputs.x < -m_movementThreshold ? adjustedTime : -adjustedTime;

		m_inputWeights.y += inputs.y > m_movementThreshold ? adjustedTime : -adjustedTime;
		m_inputWeights.w += inputs.y < -m_movementThreshold ? adjustedTime : -adjustedTime;

		m_inputWeights.x = Mathf.Clamp01(m_inputWeights.x);
		m_inputWeights.y = Mathf.Clamp01(m_inputWeights.y);
		m_inputWeights.z = Mathf.Clamp01(m_inputWeights.z);
		m_inputWeights.w = Mathf.Clamp01(m_inputWeights.w);


		BeginStateChange();

		m_isSuddenStop = false;
		//if slow and no input
		if (inputs.sqrMagnitude == 0 && m_rigidbody.velocity.sqrMagnitude <= m_veloIdleThreshhold) {
			//if was accelarating for some time
			if (m_timeHoldingInput >= m_timeIdleThreshold) {
				m_isSuddenStop = true;
			}
			CurrentState = State.Idle;
			//if sudden velocity loss
		} else if (m_veloDeltaFromLastFrame >= m_deltaIdleThreshold) {
			m_isSuddenStop = true;
			CurrentState = State.Idle;
			//if was already idle with no velocity
		} else if (CurrentState == State.Idle && m_rigidbody.velocity.sqrMagnitude <= m_startRunSpeed) {
			CurrentState = State.Idle;
		} else {
			CurrentState = Input.GetKey(KeyCode.LeftShift) ? State.Walk : State.Run;
		}

		ApplyStateChange();

		//Process what changed
		if (inputs.sqrMagnitude != 0) {
			m_timeHoldingInput += Time.deltaTime;
		}
		if (CurrentState == State.Idle) m_timeHoldingInput = 0f;
		m_previousFrameVelocity = m_rigidbody.velocity.sqrMagnitude;
	}

	private void BeginStateChange() {
		m_temporaryState = CurrentState;
	}

	private void ApplyStateChange() {
		if (m_temporaryState == CurrentState) {
			return;
		}
		Kramp.Animator.MovementStateChanged(m_temporaryState, CurrentState, m_isSuddenStop);
		//Debug.Log("State changed to " + CurrentState+" isSuddenStop " + m_isSuddenStop);
	}

	private Vector3 ComputeVelocity() {
		return new Vector3(
			m_accelerationCurve.Evaluate(m_inputWeights.x) - m_accelerationCurve.Evaluate(m_inputWeights.z),
			0,
			m_accelerationCurve.Evaluate(m_inputWeights.y) - m_accelerationCurve.Evaluate(m_inputWeights.w)
		);
	}

	private void FixedUpdate() {
		RaycastHit m_movementAssist;
		var computedVelocity = ComputeVelocity();
		computedVelocity = computedVelocity.normalized * Mathf.Max(Mathf.Abs(computedVelocity.x), Mathf.Abs(computedVelocity.z));
		var skewedInput = Kramp.Kamera.Matrix.MultiplyPoint3x4(computedVelocity);
		m_rigidbody.velocity = skewedInput * (CurrentState != State.Run ? m_sneakSpeed : m_runSpeed);
		if (Physics.Raycast(transform.position, VelocityVector, out m_movementAssist, m_assistCheckLength)) {
			if (m_avoidableObjects == (m_avoidableObjects | 1 << m_movementAssist.transform.gameObject.layer)) {

				if (!Physics.Raycast(transform.position, Quaternion.Euler(0, -m_assistValue, 0) * (VelocityVector), m_assistCheckLength)) {
					m_rigidbody.velocity = Quaternion.Euler(0, -m_assistValue, 0) * (VelocityVector);
				} else if (!Physics.Raycast(transform.position, Quaternion.Euler(0, m_assistValue, 0) * (VelocityVector), m_assistCheckLength)) {
					m_rigidbody.velocity = Quaternion.Euler(0, m_assistValue, 0) * (VelocityVector);
				}

			}
		}
	}
}
