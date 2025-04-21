using NaughtyAttributes;
using UnityEngine;

public class KrampusController : KrampusBehaviour {
    public State CurrentState { get; private set; }
    [SerializeField] private Rigidbody m_rigidbody;

    [BoxGroup("Speed Control")][SerializeField] private float m_sneakSpeed = 5f;
    [BoxGroup("Speed Control")][SerializeField] private float m_runSpeed = 10f;
    [BoxGroup("Acceleration")][SerializeField] private float m_accelerationTime = 0.2f;
    [BoxGroup("Acceleration")][SerializeField] private AnimationCurve m_accelerationCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [BoxGroup("Acceleration")][SerializeField] private float m_movementThreshold = 0.5f;
    private Vector4 m_inputWeights;
    private State m_temporaryState;

    public enum State {
        Idle,
        Run,
        Walk
    }


    // Based on @SkilluDev's inputs
    private void Update() {
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
        if (m_rigidbody.velocity.sqrMagnitude > 0.2f) {
            if (!Input.GetKey(KeyCode.LeftShift)) {
                CurrentState = State.Run;
            } else {
                CurrentState = State.Walk;
            }
        } else {
            CurrentState = State.Idle;
        }
        ApplyStateChange();
    }

    private void BeginStateChange() {
        m_temporaryState = CurrentState;
    }

    private void ApplyStateChange() {
        if (m_temporaryState == CurrentState) {
            return;
        }
        // Notify animator etc etc
        Debug.Log("State changed to " + CurrentState);
    }

    private Vector3 ComputeVelocity() {
        return new Vector3(
            m_accelerationCurve.Evaluate(m_inputWeights.x) - m_accelerationCurve.Evaluate(m_inputWeights.z),
            0,
            m_accelerationCurve.Evaluate(m_inputWeights.y) - m_accelerationCurve.Evaluate(m_inputWeights.w)
        );
    }

    private void FixedUpdate() {
        var computedVelocity = ComputeVelocity();
        computedVelocity = Vector3.ClampMagnitude(computedVelocity, 1);
        var skewedInput = Kramp.Kamera.Matrix.MultiplyPoint3x4(computedVelocity);
        m_rigidbody.velocity = skewedInput * (CurrentState != State.Run ? m_sneakSpeed : m_runSpeed);
    }
}
