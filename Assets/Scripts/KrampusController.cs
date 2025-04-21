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
    [BoxGroup("Acceleration")][SerializeField] private float m_accelerationTime = 0.2f;
    [BoxGroup("Acceleration")][SerializeField] private AnimationCurve m_accelerationCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [BoxGroup("Acceleration")][SerializeField] private float m_movementThreshold = 0.5f;
    [BoxGroup("Acceleration")][SerializeField] private float m_idleThreshhold;
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
        if (m_rigidbody.velocity.sqrMagnitude >= m_idleThreshhold && inputs.sqrMagnitude > m_movementThreshold) {
            CurrentState = Input.GetKey(KeyCode.LeftShift) ? State.Walk : State.Run;
        } else if (
            inputs.sqrMagnitude < m_movementThreshold ||
            (inputs.sqrMagnitude >= m_movementThreshold && m_rigidbody.velocity.sqrMagnitude < m_idleThreshhold)
        ) {
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
        onStateChange.Invoke(m_temporaryState, CurrentState);
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
        computedVelocity = computedVelocity.normalized * Mathf.Max(Mathf.Abs(computedVelocity.x), Mathf.Abs(computedVelocity.z));
        var skewedInput = Kramp.Kamera.Matrix.MultiplyPoint3x4(computedVelocity);
        m_rigidbody.velocity = skewedInput * (CurrentState != State.Run ? m_sneakSpeed : m_runSpeed);
    }
}
