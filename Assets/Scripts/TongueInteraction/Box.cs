using NaughtyAttributes;
using UnityEngine;
using UnityEngine.VFX;

public class Box : MonoBehaviour, IEdible {
    public int Priority => 10;

    public void Consume(Krampus krampus) => throw new System.NotImplementedException();
    public void Hit(Krampus krampus) => throw new System.NotImplementedException();
    public void Prepare(Krampus krampus) => throw new System.NotImplementedException();
    public void ReelIn(Krampus krampus, Vector3 position, float progress) => throw new System.NotImplementedException();
}
