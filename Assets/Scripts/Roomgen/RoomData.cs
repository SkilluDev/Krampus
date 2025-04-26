using System.Collections.Generic;
using System.Collections;
using Roomgen;
using UnityEngine;

public class RoomData : MonoBehaviour {
  public Room Room { get; private set;}
  public IReadOnlyCollection<NPC> NPCs => m_npcs;
  private List<NPC> m_npcs;
  
  public void Init(Room room) {
    Room = room;
    m_npcs = new List<NPC>();
  }
  
  public void AddNPC(NPC npc) {
    if(m_npcs.Contains(npc)){
      return;
    }
    m_npcs.Add(npc);
  }
  
  public void RemoveNPC(NPC npc) {
    m_npcs.Remove(npc);
  }
}
