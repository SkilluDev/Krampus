using UnityEngine;

public class NunNightHitbox : MonoBehaviour, IDayNightCycleReactor
{

	[SerializeField] private Nun m_nun;

	[SerializeField] private CapsuleCollider m_collider;

	public void React(DayNightCycle.CyclePhase oldPhase, DayNightCycle.CyclePhase newPhase) {
		if (newPhase == DayNightCycle.CyclePhase.Night) {
			m_collider.enabled = true;

		} else if (newPhase == DayNightCycle.CyclePhase.Day) {
			m_collider.enabled = false;
		}
    }

	private void OnTriggerEnter(Collider other) {
        if (!Game.Balling) return;
        //if (CurrentState != State.ChasingKrampus) return;
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) {
            return;
        }
        if (Game.MainGameInfo.Krampus.Kontroller.CurrentState == KrampusController.State.Dash) return;
        m_nun.AttackKrampus();
	   }
}
