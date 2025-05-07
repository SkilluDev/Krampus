using System.Linq;
using UnityEngine;
using NaughtyAttributes;

public class ChildSensor : KrampusBehaviour {
	[ShowNativeProperty] public float Dist { get; set; } = float.MaxValue;
	private Child m_closestChild;

	public Child ClosestChild => m_closestChild;

	private void Ready() {
		if (Game.MainGameInfo.BadChildren.Any()) m_closestChild = Game.MainGameInfo.BadChildren.First();
	}

	private void Update() {
		if (Game.IsLoading) return;
		//Debug.Log($"BadChildren");
		//foreach (Child c in Game.MainGameInfo.GoodChildren) {
		//	Debug.Log(c);
		//}
		if (!Game.MainGameInfo.BadChildren.Any()) return;
		if (!Game.MainGameInfo.BadChildren.Contains(m_closestChild)) m_closestChild = Game.MainGameInfo.BadChildren.First();
		var closestOffset = m_closestChild.transform.position - transform.position;
		Dist = closestOffset.sqrMagnitude;

		foreach (var child in Game.MainGameInfo.BadChildren) {
			if (!child) continue;
			var offset = child.transform.position - transform.position;
			float sqrLen = offset.sqrMagnitude;

			if (sqrLen < Dist) {
				m_closestChild = child;
			}
		}
	}

}
