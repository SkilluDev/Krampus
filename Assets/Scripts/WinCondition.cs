using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using static WinCondition;

public class WinCondition : MonoBehaviour
{
    public static WinCondition Instance { get; private set; }


    public int badChildrenOnStart;
    public int badChildrenCount;

    [SerializeField] UIManager manager;
    

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
    }
    public enum LostGameCase
    {
        TimeRunOut,         //Is handleded by WinCondition
        DetectedByParents, //Parent Script ->
        TooManyWrongChildren //Child List ->
    }
    [SerializeField] private bool isGamePaused; //Flag to check if Game is Paused
    [SerializeField] private bool isGameOver; //Game is Over - either Won or Lost
    [SerializeField] private float timeLimit; //Czas rundy
    private int score;

    public float totalTime;
    //private ChildList _ChildrenList; //Needs to be changed to ChildrenListManager appropiate class
    //private Detection _Detection; //Not sure where it will be relevant
    //private Player _PlayerScritpt; //Needs to be changed to appropiate class for handling Player
    [SerializeField] private UIManager uiManager; //UI controller attached to Canvas -> where our UI lurks

    // Start is called before the first frame update
    void Start()
    {
        isGameOver = false;
        isGamePaused = false;
       
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGamePaused)
        {
            timeLimit -= Time.deltaTime;
            totalTime += Time.deltaTime;
            uiManager.UpdateTime(timeLimit);
            
            if (timeLimit <= 0.0f)
            {
                GameOver(LostGameCase.TimeRunOut);
            }
        }

        if ((Input.GetKeyDown(KeyCode.R)||Input.GetKeyDown(KeyCode.G) )&& (isGameOver || isGamePaused)) //if the game is over or game is paused, you can reload game with R key
        {
            Time.timeScale = 1; //Revert Speed to 1, so everything reverts to normal time if the game was paused
            SceneManager.LoadScene("UITest"); //Goes Back to First Scene
        }

        if (Input.GetKeyDown(KeyCode.Escape)||Input.GetKeyDown(KeyCode.P)) //Pause Game
        {
            GamePauseToggle();
        }
        
    }

        public void GameOver(LostGameCase lostGameCase)
        {

        StartCoroutine(GameOverAnimation( lostGameCase));
            //Do other staff, like show the Score that you managed to get
        }
        IEnumerator GameOverAnimation(LostGameCase lostGameCase) 
        {
            isGameOver = true;
            characterController characterController = GameObject.FindGameObjectWithTag("Player").gameObject.GetComponent<characterController>();

            if (characterController != null)
            {
                characterController.Die();
            }
        yield return new WaitForSeconds(2);
            uiManager.ActivateGameOverScreen(lostGameCase);
            //StartCoroutine(AutoQuit());
    }
        
        public void GameWon()
        {
            isGameOver = true;
            uiManager.ActivateGameWonScreen();
            //StartCoroutine(AutoQuit());
            //Do other staff, that needs to happen , like Show Score
        }
        
        public void AddScore(int points)
        {
            score += points;
        }
        
        public void SubtractScore(int points)
        {
            score -= points;
        }

        public void SubtractTime(float seconds)
        {
            timeLimit = timeLimit - seconds;
            //Debug.Log("Subtracted time: "+ seconds);
        }

        public int GetScore()
        {
            return score;
        }
        public void GamePauseToggle()
        {
            if (!isGamePaused)  //Instead of Time Scale we are just deactivating Movement Scritps
            {
                Time.timeScale = 0;
                AudioListener.pause = true;
                isGamePaused = true;
                uiManager.ActivateSettingsMenu();
            }
            else
            {
                Time.timeScale = 1;
                AudioListener.pause = false;
                isGamePaused = false;
                uiManager.DeactivateSettingsMenu();
            }
        }

        public bool isGamePausedValue()
        {
            return isGamePaused;
        }
        
        IEnumerator AutoQuit()
        {
            yield return new WaitForSeconds(4);
            SceneManager.LoadScene("UITest");
        }


    //FunctionAboutEatingChildren


    public int getBadChildrenCount() 
    {
        return badChildrenCount;
    }

    public void SetChildCount(int count) {
        badChildrenOnStart = count;
        badChildrenCount = badChildrenOnStart;

        uiManager.UpdateNaughtlyCount(badChildrenCount);
    }

   public void badChildEaten() 
    {
        badChildrenCount--;

       uiManager.UpdateNaughtlyCount(badChildrenCount);
        if (badChildrenCount <= 0) 
        {
            GameWon();
        }
    }

    public float getTimer() 
    {
        return timeLimit;
    }
}
