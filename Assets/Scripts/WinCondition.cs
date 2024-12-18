using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class WinCondition : MonoBehaviour
{
    public enum LostGameCase
    {
        TimeRunOut,         //Is handleded by WinCondition
        DetectedByParents, //Parent Script ->
        TooManyWrongChildren //Child List ->
    }
    [SerializeField] private bool isGamePaused; //Flag to check if Game is Paused
    [SerializeField] private bool isGameOver; //Game is Over - either Won or Lost
    [SerializeField] private float timeLimit; //Czas rundy
    [SerializeField] private int score;
    //private ChildList _ChildrenList; //Needs to be changed to ChildrenListManager appropiate class
    //private Detection _Detection; //Not sure where it will be relevant
    //private Player _PlayerScritpt; //Needs to be changed to appropiate class for handling Player
    [SerializeField] private UIManager uiManager; //UI controller attached to Canvas -> where our UI lurks

    // Start is called before the first frame update
    void Start()
    {
        isGameOver = false;
        isGamePaused = false;
        //Not using GameFind -> Serialized Field through Editor will be faster
        /*
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager (which is canvas) is NULL.");
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGamePaused)
        {
            timeLimit -= Time.deltaTime;
            uiManager.UpdateTime(timeLimit);
            if (timeLimit <= 0.0f)
            {
                GameOver(LostGameCase.TimeRunOut);
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && isGameOver) //if the game is over, you can reload game with R key
        {
                SceneManager.LoadScene(0); //current game scene
        }

        if (Input.GetKeyDown(KeyCode.Escape)||Input.GetKeyDown(KeyCode.P)) //Pause Game
        {
            GamePauseToggle();
        }
        
    }

        public void GameOver(LostGameCase lostGameCase)
        {
            isGameOver = true;
            uiManager.ActivateGameOverScreen(lostGameCase);
            //Do other staff, like show the Score that you managed to get
        }
        
        public void GameWon()
        {
            isGameOver = true;
            uiManager.ActivateGameWonScreen();
            //Do other staff, that needs to happen , like Show Score
        }
        
        public void AddScore(int points)
        {
            score += points;
            uiManager.UpdateScore(score);
            //communicate with ui
        }
        public void GamePauseToggle()
        {
            if (!isGamePaused)  //Instead of Time Scale we are just deactivating Movement Scritps
            {
                //Time.timeScale = 0;
                AudioListener.pause = true;
                isGamePaused = true;
                uiManager.ActivateSettingsMenu();
            }
            else
            {
                //Time.timeScale = 1;
                AudioListener.pause = false;
                isGamePaused = false;
                uiManager.DeactivateSettingsMenu();
            }
        }

        public bool isGamePausedValue()
        {
            return isGamePaused;
        }
}
