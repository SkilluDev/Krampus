using UnityEngine;

public class LevelInfo : MonoBehaviour {
    public Game.State State => m_state;
    [SerializeField] private Game.State m_state;

}
