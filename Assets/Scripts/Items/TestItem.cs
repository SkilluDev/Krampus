using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Items/ItemTest", fileName = "ItemTest")]
public class TestItem : Item {
    public override void RegisterItem(KrampEvents events) {


        events.onChildEaten.AddListener(OnChildEaten);
        Debug.Log("Event Registered");

    }

    private void OnChildEaten(Krampus krampus) {
        Debug.Log("Siema");
    }
}
