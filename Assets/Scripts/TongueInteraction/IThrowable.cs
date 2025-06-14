using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IThrowable : IInteractable
{

    
    public void Prepare(Krampus krampus);
    public void Hit(Krampus krampus);
    public void ReelIn(Krampus krampus, Vector3 position, float progress);

    public void Throw(Vector3 vector3, Krampus krampus);

	public int Priority => throw new System.NotImplementedException();

	public void Interact(IInteractor interactor) => throw new System.NotImplementedException();

	
    
}
