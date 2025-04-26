using System.Collections.Generic;
using Roomgen;
using UnityEngine;

/// <summary>
/// De-facto game controller for the main gaame scene
/// </summary>
public class MainGameInfo : LevelInfo {
	public RoomGeneratorBase RoomGenerator => m_roomGenerator;
	[SerializeField] private RoomGeneratorBase m_roomGenerator;

	public IReadOnlyCollection<Child> Children => m_childRegistry;
	private List<Child> m_childRegistry;

	private Dictionary<Room, RoomData> m_roomdata;

	public RoomData GetRoomData(Room r){
		return m_roomdata[r];
	}

	public void RegisterChild(Child child) {
		m_childRegistry.Add(child);
	}

	public void UnregisterChild(Child child) {
		m_childRegistry.Remove(child);
	}
}
