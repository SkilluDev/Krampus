using UnityEngine;

public interface IKrampable : IInteractable {
    public enum krampableType {
        Instant,
        DelayedSimple,
        DelayedAiming
    }

    public void Prepare(Krampus krampus);
    public void Hit(Krampus krampus);
    public void AttachToTongue(Krampus krampus, Vector3 position, Quaternion rotation, float progress);
    public void Consume(Krampus krampus, Vector3 position, Quaternion rotation);
    public krampableType Type => krampableType.Instant;

    IInteractor.Type IInteractable.InteractorMask => IInteractor.Type.Player;

    void IInteractable.Interact(IInteractor interactor) {
        Hit(interactor.AsPlayer());
    }

}
