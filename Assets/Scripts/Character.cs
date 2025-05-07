using Roomgen;
using UnityEngine;
public interface ICharacter {
    public Room CurrentRoom { get; set; }
}

public class Character : MonoBehaviour {
    [SerializeField] private ICharacter m_character;

    private void Awake() {
        m_character = GetComponent<ICharacter>();
    }

    private void Update() {
        if (Game.IsLoading) return;

        var newRoom = Game.MainGameInfo.RoomGenerator.GetRoomAt(transform.position);
        if (m_character.CurrentRoom != newRoom) {
            var currentRoomData = Game.MainGameInfo.GetRoomData(m_character.CurrentRoom);
            if (currentRoomData != null)
                currentRoomData.RemoveCharacter(m_character);
            m_character.CurrentRoom = newRoom;

            var newRoomData = Game.MainGameInfo.GetRoomData(newRoom);

            if (newRoomData != null)
                newRoomData.AddCharacter(m_character);
        }
    }
}