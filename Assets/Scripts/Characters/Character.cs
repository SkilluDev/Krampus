using Roomgen;
using UnityEngine;
public interface ICharacter {
    public Room CurrentRoom { get; set; }
    public Vector3 VelocityVector { get; }
    public float Velocity { get; }
    public float VelocitySqr { get; }
    public GameObject GameObject => ((MonoBehaviour)this).gameObject;
}

public class Character : MonoBehaviour {
    [SerializeField] private ICharacter m_character;

    private void Awake() {
        m_character = GetComponent<ICharacter>();
    }

    private void Update() {
        if (Game.IsLoading) return;
        if(Game.IsInLobby) return;
        var newRoom = Game.roundInfo.RoomGenerator.GetRoomAt(transform.position);
        if (m_character.CurrentRoom != newRoom) {
            var currentRoomData = Game.roundInfo.GetRoomData(m_character.CurrentRoom);
            if (currentRoomData != null)
                currentRoomData.RemoveCharacter(m_character);
            m_character.CurrentRoom = newRoom;

            var newRoomData = Game.roundInfo.GetRoomData(newRoom);

            if (newRoomData != null)
                newRoomData.AddCharacter(m_character);
        }
    }
}