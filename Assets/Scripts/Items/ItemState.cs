public abstract class ItemState<T> where T : Item {
    public abstract T Item { get; }
}