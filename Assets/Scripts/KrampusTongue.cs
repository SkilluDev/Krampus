using UnityEngine;

public class KrampusTongue : KrampusBehaviour {
    [SerializeField] private float m_tongueExtendDuration = 0.1f;
    [SerializeField] private float m_tongueRetreatDuration = 0.5f;
    [SerializeField] private LineRenderer m_tongueRenderer;
    [SerializeField] private LayerMask m_layerMask = int.MaxValue;
    [SerializeField] private Transform m_tongueOrigin;

    [SerializeField] private AnimationCurve m_extendCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private AnimationCurve m_retreatCurve = AnimationCurve.Linear(0, 0, 1, 1);

    private Vector3 m_tongueDestination;
    private float m_tongueExtensionFactor = 0f;

    public State CurrentState { get; private set; }

    public enum State {
        Idle,
        Extending,
        Full,
        Retreating
    }

    private void Awake() {
        //Cursor.SetCursor(Cursor, new Vector2(cursor.width / 2, cursor.height / 2), CursorMode.ForceSoftware);
    }

    private void HandleInput() {
        if (Input.GetMouseButtonDown(0)) {
            var ray = Kramp.Kamera.Raw.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 100, m_layerMask)) {
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red, 20f);
                ShootOut(hit.point);
            } else {
                Debug.Log("Missed!");
            }
        }
    }

    private void Update() {
        HandleInput();

        switch (CurrentState) {
            case State.Extending:
                m_tongueExtensionFactor += Time.deltaTime / m_tongueExtendDuration;
                m_tongueRenderer.SetPosition(1, Vector3.Lerp(m_tongueOrigin.position, m_tongueDestination, m_extendCurve.Evaluate(m_tongueExtensionFactor)));
                if (m_tongueExtensionFactor >= 1) CurrentState = State.Retreating;
                break;
            case State.Retreating:
                m_tongueExtensionFactor -= Time.deltaTime / m_tongueRetreatDuration;
                m_tongueRenderer.SetPosition(1, Vector3.Lerp(m_tongueOrigin.position, m_tongueDestination, m_retreatCurve.Evaluate(m_tongueExtensionFactor)));
                if (m_tongueExtensionFactor <= 0) CurrentState = State.Idle;
                break;

            default:
                m_tongueRenderer.SetPosition(1, m_tongueOrigin.position);
                break;
        }

        m_tongueRenderer.SetPosition(0, m_tongueOrigin.position);
    }


    public void ShootOut(Vector3 destination) {
        m_tongueExtensionFactor = 0;
        m_tongueDestination = destination;
        CurrentState = State.Extending;
    }
}
