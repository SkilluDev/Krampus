using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class KrampusController : MonoBehaviour {
	public Rigidbody rigidBody;
	new private GameObject camera;

	public Transform playerModel;

	[Header("Statistic")]
	public float sneakSpeed = 4;
	public float runSpeed = 12;



	public bool isRunning;

	public bool isDead = false;
	private Matrix4x4 matrix;

	private float xMovement;
	private float zMovement;
	public bool shouldKrampusMove = true;
	[SerializeField] private float lerpT;
	private float timeLerping;

	private readonly float stepRunSpeed = 0.416f;
	private readonly float stepSneakSpeed = 0.5f;
	private readonly float stepIdleSpeed = float.MaxValue;

	private readonly float windUpRunSpeed = 0.41f;
	private readonly float windUpSneakSpeed = 0.5f;
	private readonly float windUpIdleSpeed = 1.041f;

	private float timerWindUp1;
	private float timerWindUp2;

	private float timerStep1;
	private float timerStep2;

	private float windUpSpeed;
	private float stepSpeed;

	[SerializeField] private AnimationCurve accelerationCurve;
	[SerializeField] private float accelerationTime = 1f;
	private float timeGoingUp = 0f;
	private float timeGoingRight = 0f;
	private float timeGoingDown = 0f;
	private float timeGoingLeft = 0f;



	[SerializeField] private GameObject rocktParticle;

	private enum State {
		running, sneaking, idle
	}

	private State currentState = State.idle;
	private State previousState = State.idle;

	private Animator animator;

	private void Start() {
		timerWindUp1 = 0f;
		timerWindUp2 = -windUpIdleSpeed;

		timerStep1 = 0f;
		timerStep2 = 0f;

		rigidBody = GetComponent<Rigidbody>();

		animator = GetComponentInChildren<Animator>();

		camera = GameObject.FindWithTag("MainCamera");
		matrix = Matrix4x4.Rotate(Quaternion.Euler(0, camera.transform.localEulerAngles.y, 0));

		rocktParticle.SetActive(false);
	}

	private void Update() {

		previousState = currentState;
		if (!WinCondition.Instance.isGamePausedValue()) {
			xMovement = Input.GetAxisRaw("Horizontal"); //raw means the values are only -1, 0 or 1
			zMovement = Input.GetAxisRaw("Vertical");

			float adjustedTime = Time.deltaTime / accelerationTime;
			//incrementing time of accelerating
			if (zMovement > 0.5) {
				timeGoingUp += adjustedTime;
			} else {
				timeGoingUp -= adjustedTime;
			}

			if (zMovement < -0.5) {
				timeGoingDown += adjustedTime;
			} else {
				timeGoingDown -= adjustedTime;
			}

			if (xMovement > 0.5) {
				timeGoingRight += adjustedTime;
			} else {
				timeGoingRight -= adjustedTime;
			}

			if (xMovement < -0.5) {
				timeGoingLeft += adjustedTime;
			} else {
				timeGoingLeft -= adjustedTime;
			}
			timeGoingUp = Mathf.Clamp(timeGoingUp, 0, 1);
			timeGoingDown = Mathf.Clamp(timeGoingDown, 0, 1);
			timeGoingRight = Mathf.Clamp(timeGoingRight, 0, 1);
			timeGoingLeft = Mathf.Clamp(timeGoingLeft, 0, 1);

			//Debug.Log($"{timeGoingUp}+{timeGoingDown}+{timeGoingRight}+{timeGoingLeft}");

			timerWindUp1 += Time.deltaTime;
			timerWindUp2 += Time.deltaTime;
			timerStep1 += Time.deltaTime;
			timerStep2 += Time.deltaTime;

			if (rigidBody.velocity.x == 0 && rigidBody.velocity.z == 0) {
				if (currentState == State.running) {
					animator.SetTrigger("Stop");
					//Debug.Log("Lol");
				}

				currentState = State.idle;
				timeLerping = 0;
			} else {
				if (Input.GetKey(KeyCode.LeftShift)) {
					currentState = State.sneaking;
				} else {
					currentState = State.running;
				}
			}

			switch (currentState) {
				case State.running:
					isRunning = true;

					windUpSpeed = windUpRunSpeed;
					stepSpeed = stepRunSpeed;

					break;
				case State.sneaking:
					isRunning = false;

					windUpSpeed = windUpSneakSpeed;
					stepSpeed = stepSneakSpeed;

					break;
				case State.idle:
					isRunning = false;

					windUpSpeed = windUpIdleSpeed;
					stepSpeed = stepIdleSpeed;
					timerStep1 = 0;
					timerStep2 = 0;

					break;
			}

			if (previousState != currentState) {
				timerWindUp1 = 0;
				timerWindUp2 = -windUpSpeed;
				timerStep1 = 0;
				timerStep2 = -stepSpeed;
			}

			//winding up sounds playing
			if (timerWindUp1 >= windUpSpeed) {
				SoundManager.PlaySound("windup1");
				timerWindUp1 = -windUpSpeed;
			}
			if (timerWindUp2 >= windUpSpeed) {
				SoundManager.PlaySound("windup2");
				timerWindUp2 = -windUpSpeed;
			}
			//steps sounds playing
			if (timerStep1 >= stepSpeed) {
				SoundManager.PlaySound("step1");
				timerStep1 = -stepSpeed;
			}
			if (timerStep2 >= stepSpeed) {
				SoundManager.PlaySound("step2");
				timerStep2 = -stepSpeed;
			}


		}
	}

	private void FixedUpdate() {
		if (!WinCondition.Instance.isGamePausedValue()) {
			Vector3 movementDirection = new Vector3(xMovement, 0, zMovement).normalized;
			float horizontalForce = accelerationCurve.Evaluate(timeGoingRight) - accelerationCurve.Evaluate(timeGoingLeft);
			float verticalForce = accelerationCurve.Evaluate(timeGoingUp) - accelerationCurve.Evaluate(timeGoingDown);
			Vector3 velocity = new Vector3(
				horizontalForce,
				0,
				verticalForce
			);
			velocity = velocity.normalized * Mathf.Max(Mathf.Abs(horizontalForce), Mathf.Abs(verticalForce));
			//velocity = velocity.normalized;


			if (movementDirection.magnitude < 0.1f) {
				animator.SetFloat("Speed", 0, 0.1f, Time.deltaTime);
			} else if (!isRunning) {
				animator.SetFloat("Speed", 1, 0.1f, Time.deltaTime);
				timeLerping += Time.deltaTime;
			} else {
				animator.SetFloat("Speed", 2, 0.1f, Time.deltaTime);
				timeLerping += Time.deltaTime;
			}

			Vector3 skewedInput = matrix.MultiplyPoint3x4(velocity);
			//Vector3 skewedInput = velocity;
			skewedInput = skewedInput * (isRunning ? runSpeed : sneakSpeed);

			//rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, skewedInput * (isRunning ? runSpeed : sneakSpeed), lerpT * Mathf.Sqrt(timeLerping));
			rigidBody.velocity = skewedInput;

			//animator.SetFloat("Speed", (skewedInput.magnitude / (speedMultiplier * runningMultiplier)));
			//RotatePlayer(movementDirection );



		}
	}

	private void RotatePlayer(Vector3 movementDirection) {
		Ray ray;
		ray = camera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("MapCollider"))) {
			Vector3 direction = (hit.point - transform.position).normalized;
			direction.y = 0;

			playerModel.rotation = Quaternion.LookRotation(direction);

			Vector3 animDirection = playerModel.rotation * movementDirection;

			Debug.DrawLine(transform.position, transform.position + animDirection * 5f, Color.yellow);
			animator.SetFloat("X", animDirection.x, 0.3f, Time.deltaTime);
			animator.SetFloat("Y", animDirection.z, 0.3f, Time.deltaTime);

			Debug.Log(Math.Round(animDirection.x, 2) + " | " + Math.Round(animDirection.y, 2) + " | " + Math.Round(animDirection.z, 2));
		}
	}

	public void Die() {
		if (!isDead) {
			animator.SetTrigger("Death");
			isDead = true;
			rigidBody.velocity = Vector3.zero;
			shouldKrampusMove = false;
		}
	}

	public void Win() {
		if (!isDead) {
			animator.SetTrigger("Win");
			isDead = true;
			rigidBody.velocity = Vector3.zero;
			shouldKrampusMove = false;

			rocktParticle.SetActive(true);
		}
	}
}
