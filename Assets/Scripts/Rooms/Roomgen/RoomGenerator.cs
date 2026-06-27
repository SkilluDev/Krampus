using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KrampUtils;
using SaintsField;
using SaintsField.Playa;
using Roomgen;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class RoomGenerator : RoomGeneratorBase {
	[SerializeField] private int m_width, m_length;
	[SerializeField] private RoomSet m_roomSet;
	[SerializeField] private int m_loopRectangles;
	[SerializeField] private Krampus m_krampus;
	private DoorFlags[,] m_doorGrid;
	private Room[,] m_generationGrid;
	private Vector2Int m_spawnPoint;
	private int m_spawnOrient;
	[SerializeField] private List<Room> m_placedRooms;
	//[SerializeField] private int m_minSpacesOnMap;

	/* [Layout("EntityGen", ELayout.FoldoutBox)][SerializeField] private int m_maxChildrenPerRoom;
	[Layout("EntityGen", ELayout.FoldoutBox)][SerializeField] private int m_minChildrenPerRoom;
	[Layout("EntityGen", ELayout.FoldoutBox)][SerializeField] private int m_maxNuns;
	[Layout("EntityGen", ELayout.FoldoutBox)][SerializeField] private int m_minNuns; */
	[Layout("Tags", ELayout.FoldoutBox)][SerializeField] private Tag m_kidProof;
	[Layout("Tags", ELayout.FoldoutBox)][SerializeField] private Tag m_nunProof;

	private LevelStats m_currentLevelStats;

	[SerializeField] private ChildType m_niceChildType;
	[SerializeField] private ChildType m_naughtyChildType;




	// TEMPORARY
	[SerializeField] private GameObject m_nunPrefab, m_childPrefab;

	public override IReadOnlyCollection<Room> Rooms => m_placedRooms;

	public override void Prepare() {

	}

	public override IEnumerator Generate() {
		yield return null;
		void Init() {

			//int mapSize = (int)Game.SetMan.GetValue<long>("Map Size");
			/* switch (mapSize) {
				case 0:
					m_width = 5; m_length = 5; m_loopRectangles = 4; m_minNuns = 1; m_maxNuns = 1;
					break;

				// 1: default
				case 2:
					m_width = 9; m_length = 9; m_loopRectangles = 8; m_minNuns = 1; m_maxNuns = 2;
					break;
				case 3:
					m_width = 11; m_length = 11; m_loopRectangles = 12; m_minNuns = 3; m_maxNuns = 4;
					break;
				case 4:
					m_width = 13; m_length = 13; m_loopRectangles = 16; m_minNuns = 3; m_maxNuns = 4;
					break;
				case 5:
					m_width = 15; m_length = 15; m_loopRectangles = 20; m_minNuns = 3; m_maxNuns = 4;
					break;
				default:
					m_width = 7; m_length = 7; m_loopRectangles = 4;
					break;
			} */
			m_currentLevelStats = Game.PogMan.GetCurrentLevelStats();
			m_width = m_currentLevelStats.GridWidth;
			m_length = m_currentLevelStats.GridLength;
			m_loopRectangles = 4;

			m_doorGrid = new DoorFlags[m_width, m_length];
			m_generationGrid = new Room[m_width, m_length];
			m_placedRooms = new List<Room>();
			for (int i = 0; i < m_width; i++) for (int j = 0; j < m_length; j++) m_doorGrid[i, j] = new DoorFlags();
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
				int ey = Random.Range(0, m_length - 1);
				if (ex >= m_spawnPoint.x) ex++;
				if (ey >= m_spawnPoint.y) ey++;
				CreateRectangle(m_spawnPoint.x, m_spawnPoint.y, ex, ey);

			}

			for (int i = 0; i < m_loopRectangles - 1; i++) {
				int sx = Random.Range(0, m_width - 1);
				int ex = Random.Range(sx + 1, m_width);
				int sy = Random.Range(0, m_length - 1);
				int ey = Random.Range(sy + 1, m_length);
				CreateRectangle(sx, sy, ex, ey);
			}
		}

		int RemoveDeadDoors() {
			bool[,] floodFill = new bool[m_width, m_length];

			void FillCell(int x, int y) {
				if (floodFill[x, y]) return;
				floodFill[x, y] = true;
				if (m_doorGrid[x, y].North && y > 0) FillCell(x, y - 1);
				if (m_doorGrid[x, y].South && y < m_length - 1) FillCell(x, y + 1);
				if (m_doorGrid[x, y].East && x < m_width - 1) FillCell(x + 1, y);
				if (m_doorGrid[x, y].West && x > 0) FillCell(x - 1, y);
			}

			FillCell(m_spawnPoint.x, m_spawnPoint.y);

			int filledSpaces = 0;

			for (int i = 0; i < m_width; i++) {
				for (int j = 0; j < m_length; j++) {
					if (!floodFill[i, j]) {
						m_doorGrid[i, j].Reset();
					} else {
						filledSpaces++;
					}
				}
			}
			return filledSpaces;
		}

		List<Vector2Int> FindPossiblePlacements(RoomType room) {
			var list = new List<Vector2Int>();
			for (int i = 0; i < m_width - room.Width + 1; i++) {
				for (int j = 0; j < m_length - room.Height + 1; j++) {
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
			int niceKidCount = m_currentLevelStats.NiceCount;
			for (int i = 0; i < niceKidCount; i++) {
				var room = m_placedRooms[Random.Range(0, m_placedRooms.Count)];
				if (room.HasTag(m_kidProof)) {
					Debug.LogWarning("[Roomgen]" + room.name + "is Kid-Proof - cannot spawn Child.");
					i--;
					continue;
				}

				if (NavMesh.SamplePosition(room.GetRandomPointOnFloor(), out var hit, 0.2f, NavMesh.AllAreas)) {
					var child = Instantiate(m_childPrefab, hit.position, Quaternion.identity);
					child.GetComponent<Child>().SetChildType(m_niceChildType);
				} else {
					Debug.LogWarning("[Roomgen] Could not spawn Child in " + room.name);
					i--;
					continue;
				}
			}
			int naughtyKidCount = m_currentLevelStats.NaughtyCount;
			for (int i = 0; i < naughtyKidCount; i++) {
				var room = m_placedRooms[Random.Range(0, m_placedRooms.Count)];
				if (room.HasTag(m_kidProof)) {
					Debug.LogWarning("[Roomgen]" + room.name + "is Kid-Proof - cannot spawn Child.");
					i--;
					continue;
				}

				if (NavMesh.SamplePosition(room.GetRandomPointOnFloor(), out var hit, 0.2f, NavMesh.AllAreas)) {
					var child = Instantiate(m_childPrefab, hit.position, Quaternion.identity);
					child.GetComponent<Child>().SetChildType(m_naughtyChildType);
				} else {
					Debug.LogWarning("[Roomgen] Could not spawn Child in " + room.name);
					i--;
					continue;
				}
			}
			int nunCount = m_currentLevelStats.NunCount;
			for (int i = 0; i < nunCount; i++) {
				var room = m_placedRooms[Random.Range(0, m_placedRooms.Count)];

				if (room.HasTag(m_nunProof)) {
					Debug.LogWarning("[Roomgen]" + room.name + "is Nun-Proof - cannot spawn Nun.");
					i--;
					continue;
				}

				if (NavMesh.SamplePosition(room.GetRandomPointOnFloor(), out var hit, 0.2f, NavMesh.AllAreas)) {
					Instantiate(m_nunPrefab, hit.position, Quaternion.identity);
				} else {
					Debug.LogWarning("[Roomgen] Could not spawn Nun in " + room.name);
					i--;
					continue;
				}
			}

		}

		void GenerateDoors() {
			for (int i = 0; i < m_width; i++) {
				for (int j = 0; j < m_length; j++) {
					if (j != m_length - 1 && m_doorGrid[i, j].South && m_generationGrid[i, j] != m_generationGrid[i, j + 1] && m_generationGrid[i, j] != null) {
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

		bool FindSpawnPoint() {
			for (int i = 0; i < m_width; i++) {
				for (int j = 0; j < m_length; j++) {
					if (m_doorGrid[i, j].Count == 0) {
						if (i - 1 >= 0 && m_doorGrid[i - 1, j].Count != 0) {
							m_spawnPoint = new Vector2Int(i, j);
							m_doorGrid[i - 1, j].East = true;
							m_doorGrid[i, j].West = true;
							m_spawnOrient = 2;
							return true;
						} else if (j - 1 >= 0 && m_doorGrid[i, j - 1].Count != 0) {
							m_spawnPoint = new Vector2Int(i, j);
							m_doorGrid[i, j - 1].South = true;
							m_doorGrid[i, j].North = true;
							m_spawnOrient = 3;
							return true;
						}
					}
				}
			}
			Debug.LogWarning("[Roomgen] DID NOT FIND SPAWNPOINT");
			return false;
		}

		int filledSpaces = 0;
		bool foundSpawn = false;
		int regenCounter = 1;
		while (filledSpaces <= (m_width + m_length) || !foundSpawn) {
			Status = "Creating layout attempt" + regenCounter;
			Init();
			CreateGrid();
			foundSpawn = FindSpawnPoint();
			filledSpaces = RemoveDeadDoors();
			yield return null;
			regenCounter++;
		}
		yield return null;

		PlaceSpawnRoom(m_spawnPoint, m_spawnOrient);



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
			Debug.Log($"[Roomgen] Found {gradeGroup.Count()} Room Variants with Tier {gradeGroup.Key}");
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
					Debug.Log("[Roomgen] No room could be placed");
					break;
				}
				var placements = FindPossiblePlacements(hardestToPlace);

				Debug.Log($"[Roomgen] Placing {hardestToPlace} in one of the {placements.Count} possible spots.");
				yield return new WaitForSecondsRealtime(0.01f);
				PlaceRoom(hardestToPlace, placements[Random.Range(0, placements.Count)]);
			}
		}

		void PlaceSpawnRoom(Vector2Int pos, int rot) {
			var spawnInstance = RoomVariantManager.CreateRotatedInstance(m_roomSet.spawn, rot); // TODO: This is to fix the tagging issue. Remove!
			var spawnRoom = PlaceRoom(spawnInstance, pos);
			RoomVariantManager.Release(spawnInstance);
		}


		m_navMesh.BuildNavMesh();
		GenerateNunsAndKids();
		GenerateDoors();
		RoomVariantManager.Release(types);


	}

	public override Room GetRoomAt(Vector3 position) {
		return m_generationGrid[Mathf.FloorToInt(position.x / Room.CELL_SIZE), Mathf.FloorToInt(-position.z / Room.CELL_SIZE)];
	}


	private void DebugLogDoorset() {
		var sb = new StringBuilder();
		for (int j = 0; j < m_length; j++) {
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

	public override void Cleanup() {
		foreach (var r in m_placedRooms) {
			Destroy(r.gameObject);
		}
		m_placedRooms.Clear();
		RoomVariantManager.ReleaseAll();
	}

	private void Update() {
		if ((NavmeshRebakeRequests <= 0 || Mathf.FloorToInt(Time.time) % 10 == 0) && NavmeshRebakeRequests <= 3) {
			return;
		}

		NavMeshSurface.BuildNavMesh();
		NavmeshRebakeRequests = 0;
	}


	#region Gizmos
	private void OnDrawGizmosSelected() {
		if (m_doorGrid == null) return;
		Gizmos.color = Color.gray;
		for (int i = 0; i < m_width; i++) {
			for (int j = 0; j < m_length; j++) {
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
