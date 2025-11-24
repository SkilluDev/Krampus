using NaughtyAttributes;
using Roomgen;
using UnityEngine;

namespace KrampTests {

    public class DebugCurrentRoom : MonoBehaviour {
        [ReadOnly][SerializeField] private Room m_room;
        [SerializeField] private bool m_log = false;
        private void Update() {
            if (Game.IsLoading) return;

            var room = Game.roundInfo.RoomGenerator.GetRoomAt(transform.position);
            if (room != m_room) {
                m_room = room;
            }
        }

        private void OnDrawGizmosSelected() {
            if (m_room == null) {
                return;
            }
            var bounds = m_room.GetBounds();
            Gizmos.DrawCube(bounds.center, bounds.size);
        }
    }

}
