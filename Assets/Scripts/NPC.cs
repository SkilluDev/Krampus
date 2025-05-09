using Roomgen;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class NPC : MonoBehaviour, IInteractor, ICharacter {
    private const float NEAR_THRESHOLD = 0.2f;

    public Vector3 CurrentDestination { get; protected set; }
    public IInteractor.Type InteractorType => IInteractor.Type.NPC;
    public float BaseMovementSpeed => m_baseMovementSpeed;
    public Vector3 VelocityVector => m_rigidbody.velocity;
    public float Velocity => VelocityVector.magnitude;
    public float VelocitySqr => VelocityVector.sqrMagnitude;
    public Room CurrentRoom { get; set; }

    [SerializeField] protected float m_baseMovementSpeed = 4;
    [SerializeField] protected Rigidbody m_rigidbody;
    protected NavMeshPath m_currentPath;
    protected int m_currentPathPoint;


    protected void Awake() {
        m_currentPath = new NavMeshPath();
    }


    public virtual bool SetDestination(Vector3 destination) {
        if (m_currentPath == null) m_currentPath = new NavMeshPath();

        if (!NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, m_currentPath)) {
            Debug.LogError($"[NPC] {name} Could not form path");
            return false;
        }
        CurrentDestination = destination;
        m_currentPathPoint = 0;
        return true;
    }

    protected void SetVelocity(Vector3 velocity) {
        m_rigidbody.velocity = velocity;
    }

    protected Vector3 GetPathDirection() {
        if (m_currentPath == null || m_currentPath.status == NavMeshPathStatus.PathInvalid) {
            Debug.LogError("Path invalid");
            return Vector3.zero;
        }

        // absolutnie nie wiem co się tu dzieje
        var dest = CurrentDestination;
        if (m_currentPath.corners.Length > 0 && m_currentPathPoint < m_currentPath.corners.Length - 1) {
            if ((transform.position - m_currentPath.corners[m_currentPathPoint + 1]).sqrMagnitude < NEAR_THRESHOLD)
                m_currentPathPoint++;
            if (m_currentPathPoint < m_currentPath.corners.Length - 1)
                dest = m_currentPath.corners[m_currentPathPoint + 1];
        }

        return (dest - transform.position).normalized;
    }

    protected bool NearDestination(float distance) {
        return (CurrentDestination - transform.position).sqrMagnitude < distance * distance;
    }
}
