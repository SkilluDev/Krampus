using Roomgen;
using UnityEngine;

public class Passage : MonoBehaviour {
    public enum Direction {
        Horizontal,
        Vertical
    }

    public Room A { get; private set; }
    public Room B { get; private set; }
    public Direction Orientation { get; private set; }
    public virtual bool Passable { get; protected set; }

    public void Initialize(Room a, Room b, Direction orientation) {
        A = a;
        B = b;
        Orientation = orientation;
    }

    public Room Other(Room that) {
        if (that == A) return B;
        if (that == B) return A;
        throw new System.Exception("Room is unconnected!");
    }
}
