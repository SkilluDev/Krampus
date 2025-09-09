using KrampUtils;
using NaughtyAttributes;
using Roomgen;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class NPC : MonoBehaviour, IInteractor, ICharacter {
    private const float NEAR_THRESHOLD = 0.2f;

    public Vector3 CurrentDestination { get; protected set; }
    public IInteractor.Type InteractorType => IInteractor.Type.NPC;
    public float BaseMovementSpeed => m_baseMovementSpeed;
    public Vector3 VelocityVector => m_rigidbody.linearVelocity;
    public float Velocity => VelocityVector.magnitude;
    public float VelocitySqr => VelocityVector.sqrMagnitude;
    public Room CurrentRoom { get; set; }
    [ShowNativeProperty] public float FacingAngle { get; private set; }


    [SerializeField] protected float m_baseMovementSpeed = 4;
    [SerializeField] protected Rigidbody m_rigidbody;
    protected NavMeshPath m_currentPath;
    protected int m_currentPathPoint;


    protected void Awake() {
        m_currentPath = new NavMeshPath();
    }

    public virtual void OverridePathCosts() {

    }

    public void SetFacingToPoint(Vector3 towards) {
        var direction = new Vector3(towards.x - transform.position.x, 0, towards.z - transform.position.z).normalized;
        if (direction.sqrMagnitude > 0.001f) {
            FacingAngle = Quaternion.LookRotation(direction, Vector3.up).eulerAngles.y;
        }
    }

    public void SetFacingDirection(Vector3 direction) {
        if (direction.sqrMagnitude > 0.001f) {
            FacingAngle = Quaternion.LookRotation(direction.NoY(), Vector3.up).eulerAngles.y;
        }
    }

    public Vector3 GetFacingDirection() {
        return Quaternion.Euler(0, FacingAngle, 0) * Vector3.forward;
    }

    public void SetFacingAngle(float direction) {
        FacingAngle = direction;
    }


    public virtual bool SetDestination(Vector3 destination) {
        if (m_currentPath == null) m_currentPath = new NavMeshPath();

        OverridePathCosts();
        if (!NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, m_currentPath)) {
			if(Game.Balling) Debug.LogError($"[NPC] {name} Could not form path");
            return false;
        }
        CurrentDestination = destination;
        m_currentPathPoint = 0;
        return true;
    }

    protected void SetVelocity(Vector3 velocity) {
        m_rigidbody.linearVelocity = velocity;
        if (!Game.Balling) m_rigidbody.linearVelocity = Vector3.zero;
    }

    protected Vector3 GetPathDirection() {
        if (m_currentPath == null || m_currentPath.status == NavMeshPathStatus.PathInvalid) {
            Debug.LogWarning("Path invalid");
            return Vector3.zero;
        }

        if (m_currentPath.corners.Length == 0) {
            m_rigidbody.position = m_rigidbody.position.OnNavMesh(4);
            SetDestination(CurrentDestination);
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

    #region Gizmos

    private void OnDrawGizmosSelected() {
        if (m_currentPath != null && m_currentPath.corners.Length > 1) {
            for (int i = 0; i < m_currentPath.corners.Length - 1; i++) {
                Debug.DrawLine(m_currentPath.corners[i], m_currentPath.corners[i + 1], Color.green);
            }

            Gizmos.DrawSphere(m_currentPath.corners[m_currentPathPoint], 0.5f);
        }
    }
    #endregion
}
