using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KrampUtils;
using NaughtyAttributes;
using Roomgen;
using UnityEngine;
using UnityEngine.AI;

public class RoomGenerator : RoomGeneratorBase {
	[SerializeField] private int m_width, m_height;
	[SerializeField] private RoomSet m_roomSet;
	[SerializeField] private int m_loopRectangles;
	[SerializeField] private Krampus m_krampus;
	[SerializeField] private int m_seed = 20;
	private DoorFlags[,] m_doorGrid;
	private Room[,] m_generationGrid;
	private Vector2Int m_spawnPoint;
	[SerializeField] private List<Room> m_placedRooms;

	[BoxGroup("EntityGen")][SerializeField] private int m_maxChildrenPerRoom;
	[BoxGroup("EntityGen")][SerializeField] private int m_minChildrenPerRoom;
	[BoxGroup("EntityGen")][SerializeField] private int m_maxNuns;
	[BoxGroup("EntityGen")][SerializeField] private int m_minNuns;
	[BoxGroup("Tags")][SerializeField] private Tag m_kidProof;
	[BoxGroup("Tags")][SerializeField] private Tag m_nunProof;


	// TEMPORARY
	[SerializeField] private GameObject m_nunPrefab, m_childPrefab;

	public override IReadOnlyCollection<Room> Rooms => m_placedRooms;


	public override void Prepare() {
		if ((int)Game.SetMan.GetValue<long>("Custom seed") != -1) {
			m_seed = (int)Game.SetMan.GetValue<long>("Custom seed");
			Debug.Log($"Random seed overwrite: {m_seed}");
		} else m_seed = Random.Range(0, 99999);
	}

	public override IEnumerator Generate() {
		Random.InitState(m_seed);
		Game.MainGameInfo.UI.SetSeed(m_seed);

		yield return null;
		void Init() {

			int mapSize = (int)Game.SetMan.GetValue<long>("Map Size");
			switch (mapSize) {
				case 0:
					m_width = 5; m_height = 5; m_loopRectangles = 4; m_minNuns = 1; m_maxNuns = 1;
					break;

				// 1: default
				case 2:
					m_width = 9; m_height = 9; m_loopRectangles = 8; m_minNuns = 1; m_maxNuns = 2;
					break;
				case 3:
					m_width = 11; m_height = 11; m_loopRectangles = 12; m_minNuns = 3; m_maxNuns = 4;
					break;
				case 4:
					m_width = 13; m_height = 13; m_loopRectangles = 16; m_minNuns = 3; m_maxNuns = 4;
					break;
				case 5:
					m_width = 15; m_height = 15; m_loopRectangles = 20; m_minNuns = 3; m_maxNuns = 4;
					break;
				default:
					m_width = 7; m_height = 7; m_loopRectangles = 4;
					break;
			}

			m_doorGrid = new DoorFlags[m_width, m_height];
			m_generationGrid = new Room[m_width, m_height];
			m_placedRooms = new List<Room>();
			for (int i = 0; i < m_width; i++) for (int j = 0; j < m_height; j++) m_doorGrid[i, j] = new DoorFlags();
			Game.MainGameInfo.ClearRoomData();
		}


		void CreateGrid() {
			void CreateRectangle(int sx, int sy, int ex, int ey) {
				if (sx > ex) (ex, sx) = (sx, ex);
				if (sy > ey) (ey, sy) = (sy, ey);

				for (int i = sx, j = sy; i <= ex; i++) {
					if (i != ex)
						m_doorGrid[i, j].East = true;
					if (i != sx)
						m_doorGrid[i, j].West = true;
				}

				for (int i = sx, j = ey; i <= ex; i++) {
					if (i != ex)
						m_doorGrid[i, j].East = true;
					if (i != sx)
						m_doorGrid[i, j].West = true;
				}

				for (int i = sx, j = sy; j <= ey; j++) {
					if (j != sy)
						m_doorGrid[i, j].North = true;
					if (j != ey)
						m_doorGrid[i, j].South = true;
				}

				for (int i = ex, j = sy; j <= ey; j++) {
					if (j != sy)
						m_doorGrid[i, j].North = true;
					if (j != ey)
						m_doorGrid[i, j].South = true;
				}
			}

			// Create a rect that goes through the spawn
			{ // yes this scope is useful, keeps the naming conventions and stuff
				int ex = Random.Range(0, m_width - 1);
				int ey = Random.Range(0, m_height - 1);
				if (ex >= m_spawnPoint.x) ex++;
				if (ey >= m_spawnPoint.y) ey++;
				CreateRectangle(m_spawnPoint.x, m_spawnPoint.y, ex, ey);

			}

			for (int i = 0; i < m_loopRectangles - 1; i++) {
				int sx = Random.Range(0, m_width - 1);
				int ex = Random.Range(sx + 1, m_width);
				int sy = Random.Range(0, m_height - 1);
				int ey = Random.Range(sy + 1, m_height);
				CreateRectangle(sx, sy, ex, ey);
			}
		}

		void RemoveDeadDoors() {
			bool[,] floodFill = new bool[m_width, m_height];

			void FillCell(int x, int y) {
				if (floodFill[x, y]) return;
				floodFill[x, y] = true;
				if (m_doorGrid[x, y].North && y > 0) FillCell(x, y - 1);
				if (m_doorGrid[x, y].South && y < m_height - 1) FillCell(x, y + 1);
				if (m_doorGrid[x, y].East && x < m_width - 1) FillCell(x + 1, y);
				if (m_doorGrid[x, y].West && x > 0) FillCell(x - 1, y);
			}

			FillCell(m_spawnPoint.x, m_spawnPoint.y);

			for (int i = 0; i < m_width; i++) {
				for (int j = 0; j < m_height; j++) {
					if (!floodFill[i, j]) {
						m_doorGrid[i, j].Reset();
					}
				}
			}
		}

		List<Vector2Int> FindPossiblePlacements(RoomType room) {
			var list = new List<Vector2Int>();
			for (int i = 0; i < m_width - room.Width + 1; i++) {
				for (int j = 0; j < m_height - room.Height + 1; j++) {
					if (room.CanPlace(i, j, m_doorGrid, m_generationGrid)) list.Add(new Vector2Int(i, j));
				}
			}
			return list;
		}

		Room PlaceRoom(RoomType room, Vector2Int placement) {
			var origin = Room.GetCellTopLeft(placement.x, placement.y);
			var prefab = Instantiate(room.prefab, origin, Quaternion.identity, transform).GetComponent<Room>();
			prefab.ConfigureDoors(placement.x, placement.y, m_doorGrid);
			for (int i = placement.x; i < placement.x + room.Width; i++) {
				for (int j = placement.y; j < placement.y + room.Height; j++) {
					var constraints = room.constraints[i - placement.x, j - placement.y];
					if (constraints == null || constraints.phantom) continue;
					m_generationGrid[i, j] = prefab;
				}
			}

			Game.MainGameInfo.CreateRoomData(prefab);
			m_placedRooms.Add(prefab);
			return prefab;
		}

		void GenerateNunsAndKids() {
			int kidCount = Random.Range(m_minChildrenPerRoom, m_maxChildrenPerRoom) * m_placedRooms.Count;
			for (int i = 0; i < kidCount; i++) {
				var room = m_placedRooms[Random.Range(0, m_placedRooms.Count)];
				if (room.HasTag(m_kidProof)) {
					Debug.LogWarning(room.name + "is Kid-Proof - cannot spawn Child.");
					i--;
					continue;
				}

				if (NavMesh.SamplePosition(room.GetRandomPointOnFloor(), out var hit, 0.2f, NavMesh.AllAreas)) {
					Instantiate(m_childPrefab, hit.position, Quaternion.identity);
				} else {
					Debug.LogWarning("Could not spawn Child in " + room.name);
					i--;
					continue;
				}
			}
			int nunCount = Random.Range(m_minNuns, m_maxNuns);
			for (int i = 0; i < nunCount; i++) {
				var room = m_placedRooms[Random.Range(0, m_placedRooms.Count)];

				if (room.HasTag(m_nunProof)) {
					Debug.LogWarning(room.name + "is Nun-Proof - cannot spawn Nun.");
					i--;
					continue;
				}

				if (NavMesh.SamplePosition(room.GetRandomPointOnFloor(), out var hit, 0.2f, NavMesh.AllAreas)) {
					Instantiate(m_nunPrefab, hit.position, Quaternion.identity);
				} else {
					Debug.LogWarning("Could not spawn Nun in " + room.name);
					i--;
					continue;
				}
			}

		}

		void GenerateDoors() {
			for (int i = 0; i < m_width; i++) {
				for (int j = 0; j < m_height; j++) {
					if (j != m_height - 1 && m_doorGrid[i, j].South && m_generationGrid[i, j] != m_generationGrid[i, j + 1] && m_generationGrid[i, j] != null) {
						var psg = Instantiate(
							m_roomSet.doorPrefabs.UnityRandomElement().gameObject,
							Room.GetCellCenter(i, j) - new Vector3(0, 0, Room.CELL_SIZE / 2f),
							Quaternion.Euler(0, 0, 0)
						).GetComponent<Passage>();

						Game.MainGameInfo.GetRoomData(m_generationGrid[i, j + 1]).AddPassage(psg);
						Game.MainGameInfo.GetRoomData(m_generationGrid[i, j]).AddPassage(psg);
						psg.Initialize(m_generationGrid[i, j], m_generationGrid[i, j + 1], Passage.Direction.Vertical);
					}
					if (i != m_width - 1 && m_doorGrid[i, j].East && m_generationGrid[i, j] != m_generationGrid[i + 1, j] && m_generationGrid[i, j] != null) {
						var psg = Instantiate(
							m_roomSet.doorPrefabs.UnityRandomElement().gameObject,
							Room.GetCellCenter(i, j) + new Vector3(Room.CELL_SIZE / 2f, 0, 0),
							Quaternion.Euler(0, 90, 0)
						).GetComponent<Passage>();

						Game.MainGameInfo.GetRoomData(m_generationGrid[i, j]).AddPassage(psg);
						Game.MainGameInfo.GetRoomData(m_generationGrid[i + 1, j]).AddPassage(psg);
						psg.Initialize(m_generationGrid[i, j], m_generationGrid[i + 1, j], Passage.Direction.Horizontal);
					}
				}
			}
		}

		Status = "Creating layout";
		Init();
		SelectSpawnPoint();
		CreateGrid();
		RemoveDeadDoors();
		yield return null;

		var spawnInstance = RoomVariantManager.CreateRotatedInstance(m_roomSet.spawn, 0); // TODO: This is to fix the tagging issue. Remove!
		var spawnRoom = PlaceRoom(spawnInstance, m_spawnPoint);
		m_krampus.transform.position = spawnRoom.GetMidPoint();
		m_krampus.GetComponent<Rigidbody>().position = spawnRoom.GetMidPoint();
		RoomVariantManager.Release(spawnInstance);


		Status = "Creating variants";
		var types = new List<RoomType>();
		foreach (var type in m_roomSet.types) {
			yield return null;
			for (int i = 0; i < 4; i++) {
				if (type.supportedRots[i]) types.Add(RoomVariantManager.CreateRotatedInstance(type, i));
			}
			Progress += 1 / m_roomSet.types.Count();
		}

		// Using LINQ is probably suboptimal here.
		// nah
		var roomsByGrade = types
			.GroupBy(x => x.Grade)
			.OrderByDescending((w) => w.Key);

		Status = "Spawning objects";

		foreach (var gradeGroup in roomsByGrade) {
			Debug.Log($"Found {gradeGroup.Count()} Room Variants with Tier {gradeGroup.Key}");
			Status = $"Spawning objects (Part {gradeGroup.Key})";
			Progress += 1f / roomsByGrade.Count();
			yield return null;
			while (true) {
				var hardestToPlace = gradeGroup
					.GroupBy(r => FindPossiblePlacements(r).Count)
					.Where(gr => gr.Key > 0)
					.NullIfEmpty()?
					.MinBy(w => w.Key)
					.UnityRandomElement();


				if (hardestToPlace == null) {
					Debug.Log("No room could be placed");
					break;
				}
				var placements = FindPossiblePlacements(hardestToPlace);

				Debug.Log($"Placing {hardestToPlace} in one of the {placements.Count} possible spots.");
				yield return new WaitForSecondsRealtime(0.01f);
				PlaceRoom(hardestToPlace, placements[Random.Range(0, placements.Count)]);
			}
		}

		m_navMesh.BuildNavMesh();
		GenerateNunsAndKids();
		GenerateDoors();
		RoomVariantManager.Release(types);
	}

	public override Room GetRoomAt(Vector3 position) {
		return m_generationGrid[Mathf.FloorToInt(position.x / Room.CELL_SIZE), Mathf.FloorToInt(-position.z / Room.CELL_SIZE)];
	}

	private void SelectSpawnPoint() {
		m_spawnPoint = new Vector2Int(m_width / 2, m_height / 2);
		//m_generationGrid[m_width / 2, m_height / 2] = true;
	}

	private void DebugLogDoorset() {
		var sb = new StringBuilder();
		for (int j = 0; j < m_height; j++) {
			for (int i = 0; i < m_width; i++) {
				var directions = m_doorGrid[i, j];
				char c = (directions.North, directions.East, directions.South, directions.West) switch {
					(true, false, false, false) => '╀',
					(true, false, false, true) => '╃',
					(true, true, false, false) => '╄',
					(true, true, false, true) => '╇',
					(true, false, true, false) => '╂',
					(true, false, true, true) => '╉',
					(true, true, true, false) => '╊',
					(true, true, true, true) => '╋',
					(false, false, true, false) => '╁',
					(false, false, true, true) => '╅',
					(false, true, false, false) => '┽',
					(false, true, false, true) => '┿',
					(false, true, true, false) => '╆',
					(false, true, true, true) => '╈',
					(false, false, false, true) => '┽',
					(false, false, false, false) => '┼', // Default empty space
				};
				sb.Append(c);
			}
			sb.Append('\n');
		}
		Debug.Log(sb.ToString());
	}

	private void MoveKrampusToRandomPlace() {
		m_krampus.GetComponent<Rigidbody>().position = MoreNavmesh.RandomPoint(Vector3.zero, 200);
	}

	public override void Cleanup() {
		foreach (var r in m_placedRooms) {
			Destroy(r.gameObject);
		}
		m_placedRooms.Clear();
		RoomVariantManager.ReleaseAll();
	}


	#region Gizmos
	private void OnDrawGizmosSelected() {
		if (m_doorGrid == null) return;
		Gizmos.color = Color.gray;
		for (int i = 0; i < m_width; i++) {
			for (int j = 0; j < m_height; j++) {
				if (m_doorGrid[i, j] == null) continue;
				var rd = m_doorGrid[i, j];

				if (rd.North)
					Gizmos.DrawLine(Room.GetCellCenter(i, j), Room.GetCellCenter(i, j - 1));

				if (rd.South)
					Gizmos.DrawLine(Room.GetCellCenter(i, j), Room.GetCellCenter(i, j + 1));

				if (rd.East)
					Gizmos.DrawLine(Room.GetCellCenter(i, j), Room.GetCellCenter(i + 1, j));

				if (rd.West)
					Gizmos.DrawLine(Room.GetCellCenter(i, j), Room.GetCellCenter(i - 1, j));
			}
		}
	}


	#endregion
}
