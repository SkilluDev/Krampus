using UnityEngine;


[CreateAssetMenu(menuName = "Game/Items/ItemTest", fileName = "ItemTest")]
public class TestItem : Item {
    public override void RegisterEvents(KrampusEvents events) {
        events.onNaughtyChildEaten.AddListener(OnNaughtyChildEaten);
        Debug.Log("Event Registered");
    }

    public override void UnregisterEvents(KrampusEvents events) {
        events.onNaughtyChildEaten.RemoveListener(OnNaughtyChildEaten);
        Debug.Log("Event unregistered");
    }

    private void OnNaughtyChildEaten(Krampus krampus, Child child) {
        Debug.Log("Siema");
    }
}
