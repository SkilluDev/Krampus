using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SusDebug : MonoBehaviour {
    [SerializeField] private RoomGenerator m_roomGenerator;
    private void Update() {
        Debug.DrawLine(transform.position, m_roomGenerator.GetRoomAt(transform.position).GetMidPoint(), Color.red, Time.deltaTime);
    }
}
