using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class CharacterController : MonoBehaviour {
    public Rigidbody rigidBody;
    private GameObject camera;

    public float speedMultiplier;
    public float runningMultiplier = 2;

    public bool isRunning;

    public bool isDead = false;
    private Matrix4x4 matrix;

    private float xMovement;
    private float zMovement;
    public bool shouldKrampusMove = true;

    private readonly float stepRunSpeed = 0.416f;
    private readonly float stepWalkSpeed = 0.5f;
    private readonly float stepIdleSpeed = float.MaxValue;

    private readonly float windUpRunSpeed = 0.41f;
    private readonly float windUpWalkSpeed = 0.5f;
    private readonly float windUpIdleSpeed = 1.041f;

    private float timerWindUp1;
    private float timerWindUp2;

    private float timerStep1;
    private float timerStep2;

    private float windUpSpeed;
    private float stepSpeed;



    [SerializeField] private GameObject rocktParticle;

    private enum State {
        running, walking, idle
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
            xMovement = Input.GetAxis("Horizontal");
            zMovement = Input.GetAxis("Vertical");

            timerWindUp1 += Time.deltaTime;
            timerWindUp2 += Time.deltaTime;

            timerStep1 += Time.deltaTime;
            timerStep2 += Time.deltaTime;

            if (rigidBody.velocity.x == 0 && rigidBody.velocity.z == 0) {
                currentState = State.idle;
            } else {
                if (Input.GetKey(KeyCode.LeftShift)) {
                    currentState = State.running;
                } else {
                    currentState = State.walking;
                }
            }

            switch (currentState) {
                case State.running:
                    isRunning = true;

                    windUpSpeed = windUpRunSpeed;
                    stepSpeed = stepRunSpeed;

                    break;
                case State.walking:
                    isRunning = false;

                    windUpSpeed = windUpWalkSpeed;
                    stepSpeed = stepWalkSpeed;

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
            Vector3 rawinput = new Vector3(xMovement, 0, zMovement).normalized * speedMultiplier *
                               (isRunning ? runningMultiplier : 1) * (shouldKrampusMove ? 1 : 0);
            Vector3 skewedInput = matrix.MultiplyPoint3x4(rawinput);

            rigidBody.velocity = skewedInput;
            animator.SetFloat("Speed", (skewedInput.magnitude / (speedMultiplier * runningMultiplier)));
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
