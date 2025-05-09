using UnityEngine;

public interface INoiseReactor {
    public GameObject GameObject => ((MonoBehaviour)this).gameObject;
    public void Alert(RoomData roomData, Vector3 place);
}
