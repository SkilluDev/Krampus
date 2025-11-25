using UnityEngine;

public class LobbyInfo :  MainGameInfo
{

    public LobbyUIManager UI => (LobbyUIManager) m_ui;

    public override bool Ballin => CurrentState == State.Game && (!UI.isPanelOpen);
    // Start is called once before the first execution of Update after the MonoBehaviour is created 
    void Start()
    {
            CurrentState = State.Game;
    }

    // Update is called once per frame
    void Update()
    {
           
    }
}
