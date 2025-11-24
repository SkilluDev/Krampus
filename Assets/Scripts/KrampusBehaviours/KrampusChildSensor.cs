using System.Linq;
using UnityEngine;
using NaughtyAttributes;

public class KrampusChildSensor : KrampusBehaviour {
	[ShowNativeProperty] public float Dist { get; set; } = float.MaxValue;
	private Child m_closestChild;

	public Child ClosestChild => m_closestChild;

	private void Ready() {
		if(!Game.IsInLobby){
		if (Game.roundInfo.NaughtyChildren.Any()) m_closestChild = Game.roundInfo.NaughtyChildren.First();
		}
	}

	private void Update() {
		if (Game.IsLoading) return;
		//Debug.Log($"NaughtyChildren");
		//foreach (Child c in Game.MainGameInfo.NiceChildren) {
		//	Debug.Log(c);
		//}
		if (Game.IsInLobby || !Game.roundInfo.NaughtyChildren.Any() ) return;
		if (!Game.roundInfo.NaughtyChildren.Contains(m_closestChild)) m_closestChild = Game.roundInfo.NaughtyChildren.First();
		var closestOffset = m_closestChild.transform.position - transform.position;
		Dist = closestOffset.sqrMagnitude;

		foreach (var child in Game.roundInfo.NaughtyChildren) {
			if (!child) continue;
			var offset = child.transform.position - transform.position;
			float sqrLen = offset.sqrMagnitude;

			if (sqrLen < Dist) {
				m_closestChild = child;
			}
		}
	}

}
