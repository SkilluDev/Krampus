using UnityEngine;
using UnityEngine.SocialPlatforms;

public class KrampusTongue : KrampusBehaviour {
    [SerializeField] private LineRenderer m_tongueRenderer;
    [SerializeField] private LayerMask m_layerMask = int.MaxValue;
    [SerializeField] private Transform m_tongueOrigin;

    [SerializeField] private float m_tongueWindupDuration = 0.4f;
    [SerializeField] private float m_tongueExtendDuration = 0.1f;
    [SerializeField] private AnimationCurve m_extendCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private float m_tongueRetreatDuration = 0.5f;
    [SerializeField] private AnimationCurve m_retreatCurve = AnimationCurve.Linear(0, 0, 1, 1);

    private Vector3 m_tongueDestination;
    private float m_tongueTime = 0f;
    private float m_tongueExtensionFactor = 0f;

    public State CurrentState { get; private set; }

    public enum State {
        Idle,
        Windup,
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


        // first - wait for the krampus wind up
        // after that, check what we hit, the object is reachable by krampus. if hittable activate pre-hit methods
        // extend tongue to reach what we hit. as we extend trigger minor interactable methods
        // actually execute hit methods. if edible, handle the objects animation
        // begin retreat
        // if edible, execute eat method. play eat animation. Otherwise play regular retreat animation

        switch (CurrentState) {
            case State.Windup:

                break;
            case State.Extending:

                break;
            case State.Retreating:

                break;

            default:
                m_tongueRenderer.SetPosition(1, m_tongueOrigin.position);
                break;
        }

        m_tongueRenderer.SetPosition(0, m_tongueOrigin.position);

        m_tongueTime += CurrentState != State.Idle ? Time.deltaTime : 0;
    }


    public void ShootOut(Vector3 destination) {
        CurrentState = State.Windup;
        m_tongueDestination = destination;
        m_tongueExtensionFactor = 0;
        m_tongueTime = 0;
    }
}
