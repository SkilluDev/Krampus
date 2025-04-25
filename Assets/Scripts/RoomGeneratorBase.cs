using System.Collections;
using System.Collections.Generic;
using Roomgen;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshSurface))]
public abstract class RoomGeneratorBase : MonoBehaviour {
    [SerializeField] protected NavMeshSurface m_navMesh;
    public abstract IReadOnlyCollection<Room> Rooms { get; }
    public abstract void Generate();
    public abstract Room GetRoomAt(Vector3 position);
}
