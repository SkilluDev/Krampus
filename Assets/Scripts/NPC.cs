using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class NPC : MonoBehaviour, IInteractor {
    private const float NEAR_THRESHOLD = 0.2f;
    [SerializeField] protected float m_baseMovementSpeed = 4;
    [SerializeField] protected Rigidbody m_rigidbody;

    public Vector3 CurrentDestination { get; protected set; }
    protected NavMeshPath CurrentPath { get; set; }
    protected int CurrentPathPoint { get; set; }

    public IInteractor.Type InteractorType => IInteractor.Type.NPC;

    public virtual bool SetDestination(Vector3 destination) {
        if (CurrentPath == null) CurrentPath = new NavMeshPath();

        if (!NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, CurrentPath)) {
            Debug.LogError("Could not form path");
            return false;
        }
        CurrentDestination = destination;
        CurrentPathPoint = 0;
        return true;
    }

    public virtual void MoveAlongPath() {
        if (CurrentPath.status == NavMeshPathStatus.PathInvalid) {
            Debug.LogError("Path invalid");
            return;
        }

        var dest = CurrentDestination;
        if (CurrentPath.corners.Length > 0 && CurrentPathPoint < CurrentPath.corners.Length - 1) {
            if ((transform.position - CurrentPath.corners[CurrentPathPoint + 1]).sqrMagnitude < NEAR_THRESHOLD)
                CurrentPathPoint++;
            dest = CurrentPath.corners[CurrentPathPoint + 1];
        }

        m_rigidbody.velocity = (dest - transform.position).normalized * m_baseMovementSpeed;
    }
}
