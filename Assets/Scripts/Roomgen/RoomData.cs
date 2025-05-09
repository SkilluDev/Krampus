using System.Collections.Generic;
using System.Linq;
using Roomgen;
using UnityEngine;

public class RoomData : MonoBehaviour {
	public Room Room { get; private set; }
	public IReadOnlyCollection<ICharacter> Characters => m_characters;
	private List<ICharacter> m_characters;
	public IReadOnlyCollection<Passage> Passages => m_passages;
	private List<Passage> m_passages;

	public void Init(Room room) {
		Room = room;
		m_characters = new List<ICharacter>();
		m_passages = new List<Passage>();
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

	public void AddPassage(Passage psg) {
		m_passages.Add(psg);
	}

	public void MakeNoise(Vector3 place, float alertDistance) {
		var colliders = Physics.OverlapSphere(place, alertDistance)
			.Where(w => Room.IsPointInRoom(w.transform.position))
			.Select(w => w.GetComponent<INoiseReactor>())
			.Where(w => w != null);

		foreach (var reactor in colliders)
			reactor.Alert(this, place);
	}
}
