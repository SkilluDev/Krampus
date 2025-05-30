using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Items/ItemTest", fileName = "ItemTest")]
public class TestItem : Item {
    public override void RegisterItem(Krampus krampus) {

        base.RegisterItem(krampus);
        krampus.KrampEvents.onChildEaten.AddListener(OnChildEaten);
        Debug.Log("Event Registered");

    }

    private void OnChildEaten(Child child) {
        Debug.Log("Siema");
    }
}
