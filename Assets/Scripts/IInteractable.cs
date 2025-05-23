using UnityEngine;

public interface IInteractable {
    public GameObject GameObject => ((MonoBehaviour)this).gameObject;

    public IInteractor.Type InteractorMask => IInteractor.Type.Player | IInteractor.Type.NPC;
    public Vector3 InteractionPoint => GameObject.transform.position;

    public int Priority { get; }

    public bool CanInteract(IInteractor interactor) =>
        InteractorMask.HasFlag(interactor.InteractorType);

    public void Interact(IInteractor interactor);
}
