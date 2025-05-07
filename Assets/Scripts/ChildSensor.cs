using System.Linq;
using UnityEngine;

public class ChildSensor : MonoBehaviour {
	public float Dist { get; set; } = float.MaxValue;
	private Child m_closestChild;

	private void Ready() {
		if (Game.MainGameInfo.BadChildren.Count > 0) m_closestChild = Game.MainGameInfo.GoodChildren.First();
	}

	private void Update() {
		if (Game.IsLoading) return;
		Debug.Log($"Distance to closest naughty child: {Dist}");
		//Debug.Log($"BadChildren");
		//foreach (Child c in Game.MainGameInfo.GoodChildren) {
		//	Debug.Log(c);
		//}
		if (Game.MainGameInfo.BadChildren.Count == 0) return;
		if (!Game.MainGameInfo.BadChildren.Contains(m_closestChild)) m_closestChild = Game.MainGameInfo.GoodChildren.First();
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
