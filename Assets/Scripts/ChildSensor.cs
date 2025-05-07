using System.Linq;
using UnityEngine;

public class ChildSensor : MonoBehaviour {
	public float Dist { get; set; } = float.MaxValue;
	private Transform m_closestChild;

	private void Ready() {
		if (Game.MainGameInfo.BadChildren.Count > 0) m_closestChild = Game.MainGameInfo.GoodChildren.First().transform;
	}

	private void Update() {
		//Debug.Log($"Distance to closest naughty child: {Dist}");
		if (Game.MainGameInfo.BadChildren.Count == 0) return;
		if (!m_closestChild) m_closestChild = Game.MainGameInfo.GoodChildren.First().transform;
		var closestOffset = m_closestChild.transform.position - transform.position;
		Dist = closestOffset.sqrMagnitude;

		foreach (var child in Game.MainGameInfo.BadChildren) {
			if (!child) continue;
			var offset = child.transform.position - transform.position;
			float sqrLen = offset.sqrMagnitude;

			if (sqrLen < Dist) {
				m_closestChild = child.transform;
			}
		}
	}

}
