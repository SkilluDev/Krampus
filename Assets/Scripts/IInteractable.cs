public interface IInteractable {
    public IInteractor.Type InteractorMask { get; }
    public bool CanInteract(IInteractor interactor) =>
        InteractorMask.HasFlag(interactor.InteractorType);

    public void Interact(IInteractor interactor);
}
