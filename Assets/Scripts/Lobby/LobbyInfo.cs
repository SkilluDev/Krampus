using UnityEngine;

public class LobbyInfo :  MainGameInfo
{

    public LobbyUIManager UI => (LobbyUIManager) m_ui;

    [SerializeField] int m_numberOfTasks = 3;
    [SerializeField] TaskPool m_taskPool;

    [SerializeField] private ShopScript m_shopScript;
    public ShopScript ShopScript => m_shopScript;

    public Task[] m_tasks {set;get;}

    public override bool Ballin => CurrentState == State.Game && (!UI.isPanelOpen);
    // Start is called once before the first execution of Update after the MonoBehaviour is created 
    void Start()
    {
            CurrentState = State.Game;
            m_tasks = m_taskPool.RandomTasksForKrampus(m_numberOfTasks);
            UI.PlacePins(m_numberOfTasks);
            

    }

    // Update is called once per frame
    void Update()
    {
           
    }
}
