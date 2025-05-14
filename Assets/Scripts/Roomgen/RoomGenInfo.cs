using UnityEngine;

namespace Roomgen {
	public class RoomGenInfo : MonoBehaviour {
		public RoomGenerationType Regenerate { get; set; } = RoomGenerationType.First;
		public int Seed { get; private set; }


		public void SetInitialSeed() {
			if ((int)Game.SetMan.GetValue<long>("Custom Seed") != -1) {
				Seed = (int)Game.SetMan.GetValue<long>("Custom Seed");
				Debug.Log($"Random seed overwrite: {Seed}");
			} else Seed = Random.Range(0, 99999);
			Debug.Log($"First seed overwrite: {Seed}");

		}

		public void SetNewSeed() {
			Seed = Random.Range(0, 99999);
			Debug.Log($"New seed overwrite: {Seed}");

		}
	}

	public enum RoomGenerationType {
		First,
		Old,
		New
	}
}
