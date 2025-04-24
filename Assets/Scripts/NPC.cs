using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class NPC : MonoBehaviour, IInteractor {
    [SerializeField] protected float m_baseMovementSpeed = 4;
    [SerializeField] protected Rigidbody m_rigidbody;

    public IInteractor.Type InteractorType => IInteractor.Type.NPC;
}
