using System.Linq;
using UnityEngine;
using SaintsField;
using SaintsField.Playa;

public class KrampusChildSensor : KrampusBehaviour {
	[ShowInInspector] public float Dist { get; set; } = float.MaxValue;
	private Child m_closestChild;

	public Child ClosestChild => m_closestChild;

	private void Ready() {
		if (Game.MainGameInfo.NaughtyChildren.Any()) m_closestChild = Game.MainGameInfo.NaughtyChildren.First();
	}

	private void Update() {
		if (Game.IsLoading) return;
		//Debug.Log($"NaughtyChildren");
		//foreach (Child c in Game.MainGameInfo.NiceChildren) {
		//	Debug.Log(c);
		//}
		if (!Game.MainGameInfo.NaughtyChildren.Any()) return;
		if (!Game.MainGameInfo.NaughtyChildren.Contains(m_closestChild)) m_closestChild = Game.MainGameInfo.NaughtyChildren.First();
		var closestOffset = m_closestChild.transform.position - transform.position;
		Dist = closestOffset.sqrMagnitude;

		foreach (var child in Game.MainGameInfo.NaughtyChildren) {
			if (!child) continue;
			var offset = child.transform.position - transform.position;
			float sqrLen = offset.sqrMagnitude;

			if (sqrLen < Dist) {
				m_closestChild = child;
			}
		}
	}

}
