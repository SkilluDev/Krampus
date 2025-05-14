using KrampUtils;
using UnityEngine;

public class ArrowController : MonoBehaviour {
	[SerializeField] private GameObject m_arrowModel;
	[SerializeField] private float m_minimalGameTime = 15;
	private Transform m_target;
	private Transform m_krampus;


	private void Start() {
		m_krampus = Game.MainGameInfo.Krampus.transform;
	}

	private void Update() {
		if (Game.MainGameInfo.Timer.GameTime <= m_minimalGameTime) {
			if (m_target == null) {
				m_target = Game.MainGameInfo.GoodChildren.UnityRandomElement().transform;
			}
		} else {
			m_target = null;
			m_arrowModel.SetActive(false);
		}

		if (m_target != null) {
			m_arrowModel.SetActive(true);
			transform.position = m_krampus.position;
			var direction = (m_target.position - transform.position).normalized;
			transform.rotation = Quaternion.LookRotation(direction);
		}
	}
}
