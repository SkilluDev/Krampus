using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewUIManager : MonoBehaviour
{
    public static NewUIManager Instance {get; private set;}

    [SerializeField] private static int m_badChildrenOnStart;
    private int m_badChildAmount = m_badChildrenOnStart;
    [SerializeField] private bool m_isGamePaused;
    [SerializeField] private float m_currentTime;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        Instance = this;
    }

    public void decreaseChildAmount(){m_badChildAmount -= 1;}
    public void setCurrentTime(float newTime){m_currentTime = newTime;}
    public void switchGamePauseState(){m_isGamePaused = m_isGamePaused?!m_isGamePaused:m_isGamePaused;}

}
