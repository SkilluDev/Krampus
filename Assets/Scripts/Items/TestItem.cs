using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Items/ItemTest", fileName = "ItemTest")]
public class TestItem : Item {
    public override void RegisterItem(Krampus krampus) {

        base.RegisterItem(krampus);
        krampus.KrampusEvents.onNaughtyChildEaten.AddListener(OnNaughtyChildEaten);
        Debug.Log("Event Registered");

    }

    private void OnNaughtyChildEaten(Child child) {
        Debug.Log("Siema");
    }
}
