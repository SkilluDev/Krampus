using UnityEngine;

public class PanelSource : MonoBehaviour , IInteractable
{
	public int Priority => 2;


    [SerializeField] private LobbyUIManager.Panel m_panel;

	public void Interact(IInteractor interactor) {
        if(Game.Balling)
            Game.Lobbyinfo.UI.OpenPanel(m_panel);
    }

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
