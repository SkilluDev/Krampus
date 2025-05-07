using System.Collections.Generic;
using System.Collections;
using Roomgen;
using UnityEngine;

public class RoomData : MonoBehaviour {
	public Room Room { get; private set; }
	public IReadOnlyCollection<ICharacter> Characters => m_characters;
	private List<ICharacter> m_characters;

	public void Init(Room room) {
		Room = room;
		m_characters = new List<ICharacter>();
	}

	public void AddCharacter(ICharacter character) {
		if (m_characters.Contains(character)) {
			return;
		}
		m_characters.Add(character);
	}

	public void RemoveCharacter(ICharacter character) {
		if (character == null) return;
		m_characters.Remove(character);
	}

	public bool Contains<T>() {
		return m_characters.Exists(w => w is T);
	}

	public bool Contains(ICharacter character) {
		return m_characters.Contains(character);
	}
}
