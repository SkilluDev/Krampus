using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdibleProp : MonoBehaviour, IEdible {
    public void Consume(Krampus krampus) => throw new System.NotImplementedException();
    public void Hit(Krampus krampus) => throw new System.NotImplementedException();
    public void Prepare(Krampus krampus) => throw new System.NotImplementedException();
    public void ReelIn(Krampus krampus, Vector3 position) => throw new System.NotImplementedException();

}
