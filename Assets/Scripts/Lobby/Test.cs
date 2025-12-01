using UnityEngine;

public class Test : MonoBehaviour
{
    public Item[] items;


	void Ready() {
        foreach(Item i in items) {
            Game.PogMan.UnlockItem(i);
        }

        Game.Lobbyinfo.UI.UpdateEqu();
		
	}
}
    