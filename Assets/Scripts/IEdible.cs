using UnityEngine;

public interface IEdible : IInteractable {
    public void Prepare(Krampus krampus);
    public void TongueHit(Krampus krampus);
    public void ReelIn(Krampus krampus, Vector3 position);
    public void Consume(Krampus krampus);

    IInteractor.Type IInteractable.InteractorMask => IInteractor.Type.Player;

    void IInteractable.Interact(IInteractor interactor) {
        TongueHit(interactor.AsPlayer());
    }

}
