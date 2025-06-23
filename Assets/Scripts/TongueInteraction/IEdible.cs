using UnityEngine;

public interface IEdible : IInteractable {
    public void Prepare(Krampus krampus);
    public void Hit(Krampus krampus);
    public void ReelIn(Krampus krampus, Vector3 position, Quaternion rotation, float progress);
    public void Consume(Krampus krampus);
    public bool CanBeConsumed => true;

    IInteractor.Type IInteractable.InteractorMask => IInteractor.Type.Player;

    void IInteractable.Interact(IInteractor interactor) {
        Hit(interactor.AsPlayer());
    }

}
