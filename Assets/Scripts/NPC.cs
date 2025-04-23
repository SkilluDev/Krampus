using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour {
    [SerializeField] private NavMeshAgent m_agent;
    [SerializeField] private Krampus m_krampus;
    private void Update() {
        m_agent.destination = m_krampus.transform.position;
    }
}
