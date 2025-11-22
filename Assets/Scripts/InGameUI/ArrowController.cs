using System.Linq;
using KrampUtils;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

public class ArrowController : MonoBehaviour {
	[SerializeField] private GameObject m_arrowModel;
	[SerializeField] private float m_minimalGameTime = 15;
	private Transform m_target;

	private Transform m_previousTarget;
	private Transform m_krampus;

	private MotionHandle m_rotationHandle;


	private void Start() {
		m_krampus = Game.MainGameInfo.Krampus.transform;
	}

	private void Update() {
		if (!Game.Balling) return;
		//if (Game.MainGameInfo.Timer.GameTime <= m_minimalGameTime || Game.MainGameInfo.NaughtyChildren.Count() <= 1) {
			if (Game.MainGameInfo.Krampus.ChildSensor.ClosestChild != null) m_target = Game.MainGameInfo.Krampus.ChildSensor.ClosestChild.transform;
			else m_target = null;
/* 		} else {
			m_target = null;
			m_arrowModel.SetActive(false);
		} */

		if (m_target == null) return;

		m_arrowModel.SetActive(true);
		transform.position = m_krampus.position;
		var direction = (m_target.position - transform.position).normalized;
		if (m_previousTarget != m_target) {
			m_rotationHandle.TryCancel();
			m_rotationHandle = LMotion.Create(transform.rotation, Quaternion.LookRotation(direction), 0.2f).WithEase(Ease.InOutQuad).BindToRotation(transform);
		} else {
			if (!m_rotationHandle.IsPlaying()) transform.rotation = Quaternion.LookRotation(direction);
		}
		m_previousTarget = m_target;
	}
}
