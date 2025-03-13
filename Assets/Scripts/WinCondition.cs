using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using static WinCondition;

public class WinCondition : MonoBehaviour {
    public static WinCondition Instance { get; private set; }


    public int badChildrenOnStart;
    public int badChildrenCount;

    [SerializeField] private UIManager manager;


    private void Awake() {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }
    public enum LostGameCase {
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
    private void Start() {
        isGameOver = false;
        isGamePaused = false;


    }

    // Update is called once per frame
    private void Update() {
        if (!isGamePaused) {
            if (timeLimit <= 0.0f) {
                GameOver(LostGameCase.TimeRunOut);
            } else {
                timeLimit -= Time.deltaTime;
                totalTime += Time.deltaTime;
                uiManager.UpdateTime(timeLimit);
            }
        }

        if ((Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.G)) && (isGameOver || isGamePaused)) //if the game is over or game is paused, you can reload game with R key
        {
            Time.timeScale = 1; //Revert Speed to 1, so everything reverts to normal time if the game was paused
            if (!isGameOver) GamePauseToggle();
            SceneManager.LoadScene("UITest"); //Goes Back to First Scene
        }

        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) && ((Time.timeScale != 0f) || isGamePausedValue())) //Pause Game
        {
            GamePauseToggle();
        }

    }

    public void GameOver(LostGameCase lostGameCase) {


        uiManager.StopClock();

        StartCoroutine(GameOverAnimation(lostGameCase));
        //Do other staff, like show the Score that you managed to get
    }
    private IEnumerator GameOverAnimation(LostGameCase lostGameCase) {
        isGameOver = true;
        CharacterController characterController = GameObject.FindGameObjectWithTag("Player").gameObject.GetComponent<CharacterController>();

        if (characterController != null) {
            characterController.Die();
        }
        yield return new WaitForSeconds(2);
        uiManager.ActivateGameOverScreen(lostGameCase);
        //StartCoroutine(AutoQuit());
    }

    public void GameWon() {
        isGameOver = true;

        StartCoroutine(GameWinAnimation());
    }

    private IEnumerator GameWinAnimation() {

        CharacterController characterController = GameObject.FindGameObjectWithTag("Player").gameObject.GetComponent<CharacterController>();

        if (characterController != null) {
            characterController.Win();
        }
        yield return new WaitForSeconds(3);

        uiManager.ActivateGameWonScreen();

    }


    public void AddScore(int points) {
        score += points;
    }

    public void SubtractScore(int points) {
        score -= points;
    }

    public void SubtractTime(float seconds) {
        timeLimit = timeLimit - seconds;

        uiManager.UpdateTime(timeLimit, seconds > 0 ? false : true);
        //Debug.Log("Subtracted time: "+ seconds);
    }

    public int GetScore() {
        return score;
    }
    public void GamePauseToggle() {
        if (!isGamePaused)  //Instead of Time Scale we are just deactivating Movement Scritps
        {
            Time.timeScale = 0;
            AudioListener.pause = true;
            isGamePaused = true;
            uiManager.ActivateSettingsMenu();
        } else {
            Time.timeScale = 1;
            AudioListener.pause = false;
            isGamePaused = false;
            uiManager.DeactivateSettingsMenu();
        }
    }

    public bool isGamePausedValue() {
        return isGamePaused;
    }

    private IEnumerator AutoQuit() {
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene("UITest");
    }


    //FunctionAboutEatingChildren


    public int getBadChildrenCount() {
        return badChildrenCount;
    }

    public void SetChildCount(int count, bool playAnimation) {
        badChildrenOnStart = count;
        badChildrenCount = badChildrenOnStart;

        uiManager.UpdateNaughtlyCount(badChildrenCount, playAnimation);
    }

    public void badChildEaten() {
        badChildrenCount--;

        uiManager.UpdateNaughtlyCount(badChildrenCount, true);
        if (badChildrenCount <= 0) {
            GameWon();
        }
    }

    public float getTimer() {
        return timeLimit;
    }
}
