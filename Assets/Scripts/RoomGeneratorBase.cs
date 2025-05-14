using System.Collections;
using System.Collections.Generic;
using Roomgen;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshSurface))]
public abstract class RoomGeneratorBase : MonoBehaviour, IGameLoadable {
    public NavMeshSurface NavMeshSurface => m_navMesh;
    [SerializeField] protected NavMeshSurface m_navMesh;
    public abstract IReadOnlyCollection<Room> Rooms { get; }

    public string Status { get; protected set; }

    public float Progress { get; protected set; }

    public abstract void Prepare();
    public abstract IEnumerator Generate();
    public abstract Room GetRoomAt(Vector3 position);
    public abstract void Cleanup();
    public IEnumerator Load() {
        Status = "Initializing";
        Prepare();
        yield return null;
        var w = Generate();
        while (w.MoveNext()) {
            yield return w.Current;
        }
        Status = "Done";
        Progress = 1;
    }
}
