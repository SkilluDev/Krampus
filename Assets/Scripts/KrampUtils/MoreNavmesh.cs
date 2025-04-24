using UnityEngine;
using UnityEngine.AI;

namespace KrampUtils {
	public static class MoreNavmesh {
		public static Vector3 RandomPoint(Vector3 center, float range) {
			Vector3 result;
			while (true) {
				Vector2 point = Random.insideUnitCircle;
				Vector3 randomPoint = center + new Vector3(point.x, 0, point.y) * range;
				NavMeshHit hit;
				if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) {
					result = hit.position;
					Debug.Log(result);
					return result;
				}
			}
		}
	}
}
