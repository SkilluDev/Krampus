using UnityEngine;
using UnityEngine.AI;

public class Child : NPC, IEdible {
    public enum State {
        Idle,
        Stunned,
        Panic,
        Alerted,
        Dead
    }

    public Child.State CurrentState { get; private set; }

    private void Update() {

    }

    public void Consume(Krampus krampus) => throw new System.NotImplementedException();
    public void Hit(Krampus krampus) => throw new System.NotImplementedException();
    public void Prepare(Krampus krampus) => throw new System.NotImplementedException();
    public void ReelIn(Krampus krampus, Vector3 position, float progress) => throw new System.NotImplementedException();
}
