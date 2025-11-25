using UnityEngine;

public class LobbyUIManager : NewUIManager
{

	void Start() {
        HideBlackBars();
       
		
	}

	protected override void Ready() {
        base.Ready();
         UIElementsEntryAnimation();
    }
}
