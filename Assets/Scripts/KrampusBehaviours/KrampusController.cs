using System;
using System.Collections;
using KrampUtils;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.VFX;
using static KrampusStats;

public class KrampusController : KrampusBehaviour {
	[ShowNativeProperty] public State CurrentState { get; private set; }
	public Vector3 VelocityVector => m_rigidbody.velocity;
	public float RunSpeed => Kramp.Stats.GetFinalStat(Stat.Speed);
	public float Velocity => VelocityVector.magnitude;
	public float VelocitySqr => VelocityVector.sqrMagnitude;
	public float SneakSpeed => m_sneakSpeed;

	public UnityAction<KrampusController.State, KrampusController.State, KrampusController.StateChangeReason> onStateChanged;

	[SerializeField] private Rigidbody m_rigidbody;

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
	[BoxGroup("Dash")][SerializeField] private float m_dashSpeed;
	[BoxGroup("Dash")][SerializeField] private float m_dashCurveEvalSpeed;

	[BoxGroup("Dash")][SerializeField] private AnimationCurve m_dashCurve = AnimationCurve.Linear(0, 0, 1, 1);

	public bool CanSting { get; set; } = false;
	private Vector3 m_dashDirection;
	public Vector3 NextStingDirection { get => m_dashDirection; set => m_dashDirection = value; }
	[SerializeField] private float m_comboStingCost;
	public float ComboStingCost => m_comboStingCost;
	private IInteractable m_stingerTarget;



	//Combo
	private float m_comboPoints;
	private float ComboBonus => Kramp.Stats.GetFinalStat(Stat.ComboGain);
	public float ComboPoints { get => m_comboPoints; set => m_comboPoints = value; }
	private float m_comboGainLock = 0;



	private void Start() {
		Game.MainGameInfo.UI.SetComboCostBar(m_comboStingCost);
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
			m_rigidbody.velocity = Vector3.zero;
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

		if (m_comboGainLock > 0) {
			m_comboGainLock -= Time.deltaTime;
		}

		if (CurrentState is State.Dead or State.Dash) return;

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

		if (CanSting && InputSubscribe.Special) {
			Dash();
		}
	}

	public void Dash() {
		if (!CanSting) return;
		m_dashTime = 0;
		//m_dashDirection = Kramp.Tongue.TongueDirection;
		Kramp.KrampusEvents.onStingerUsed.Invoke(Kramp);
		ChangeState(State.Dash, StateChangeReason.Rapid);
		//Debug.Log("Do dashing " + m_dashDirection);

		m_comboGainLock = 2;
		SpendComboPoints(ComboStingCost);
		SetCanSting(false);
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
		m_rigidbody.velocity = Vector3.zero;
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
			if (m_stingerTarget == null) {
				ChangeState(State.Run, StateChangeReason.Rapid);
				return;
			}
			m_dashTime += Time.fixedDeltaTime*m_dashCurveEvalSpeed;
			//Debug.Log("Dash Time: " + m_dashTime);
			Vector3 direction = m_stingerTarget.GameObject.transform.position - transform.position;
			float distance = direction.sqrMagnitude;
			m_rigidbody.velocity = direction.normalized * m_dashCurve.Evaluate(m_dashTime) * m_dashSpeed;

			if (distance < 10) {
				ChangeState(State.Run, StateChangeReason.Rapid);
			}
			return;
		}

		var computedVelocity = ComputeVelocity();
		computedVelocity = computedVelocity.normalized * Mathf.Max(Mathf.Abs(computedVelocity.x), Mathf.Abs(computedVelocity.z));
		var skewedInput = Kramp.Kamera.Matrix.MultiplyPoint3x4(computedVelocity);
		m_rigidbody.velocity = skewedInput * (CurrentState != State.Run ? m_sneakSpeed : RunSpeed);
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

	public void MoveTo(Vector3 position) {
		m_rigidbody.position = position.NoY() + Vector3.up * m_rigidbody.position.y;
	}


	public void AddComboPoints(float value) {

		if (m_comboGainLock > 0) { Debug.Log("Block"); return; }

		m_comboPoints += value * ComboBonus;
		if (m_comboPoints > Game.MainGameInfo.MaxComboPoints) {
			m_comboPoints = Game.MainGameInfo.MaxComboPoints;
		}
		Game.MainGameInfo.UI.ChangeComboValue(m_comboPoints);
		Kramp.KrampusEvents.onWindUpChanged.Invoke(Kramp, m_comboPoints);
	}

	public void SpendComboPoints(float value) {
		m_comboPoints -= value;
		Game.MainGameInfo.UI.ChangeComboValue(m_comboPoints, 0.1f);
		Kramp.KrampusEvents.onWindUpChanged.Invoke(Kramp, m_comboPoints);
	}

	public void SetCanSting(bool canSting) {
		if (ComboPoints < ComboStingCost) {
			CanSting = false;
		} else {
			CanSting = canSting;
		}

		Game.MainGameInfo.UI.ShowQuickActionIcon(CanSting);

	}

	public void OnNaughtyChildEaten(Krampus krampus, Child child) {
		AddComboPoints(Game.MainGameInfo.ComboGainFromChildren);
	}

	public void SetStingTarget(IInteractable interactable) {
		m_stingerTarget = interactable;
	}
}
