using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinCondition : MonoBehaviour
{
    public enum LostGameCase
    {
        TimeRunOut,         //Is handleded by WinCondition
        DetectedByParents, //Parent Script ->
        TooManyWrongChildren //Child List ->
    }

    [SerializeField] private bool _isGameOver;
    [SerializeField] private float _TimeLimit; //Czas rundy
    [SerializeField] private int _score;
    private ChildList _ChildrenList; //Needs to be changed to ChildrenListManager appropiate class
    //private Detection _Detection; //Not sure where it will be relevant
    //private Player _PlayerScritpt; //Needs to be changed to appropiate class for handling Player

    private UIManager _uiManager; //Needs to be changed to appropiate class for handling Player

    // Start is called before the first frame update
    void Start()
    {
        _isGameOver = false;
        _ChildrenList = GameObject.Find("Child_List").GetComponent<ChildList>(); //find obj. get comp.
        if (_ChildrenList == null)
        {
            Debug.LogError("The ChildrenList (which is Children List) is NULL.");
        }
        /*
        _Detection = GameObject.Find("Player").GetComponent<Detection>(); //find obj. get comp.
        if (_Detection == null)
        {
            Debug.LogError("The Detection (which is PLayer) is NULL.");
        }

        _PlayerScritpt = GameObject.Find("Player").GetComponent<PlayerScript>(); //find obj. get comp.
        if (_PlayerScritpt == null)
        {
            Debug.LogError("The Detection (which is PLayer) is NULL.");
        }
        */
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager (which is canvas) is NULL.");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        _TimeLimit -= Time.deltaTime;
        _uiManager.UpdateTime(_TimeLimit);
        if (_TimeLimit <= 0.0f)
        {
            GameOver(LostGameCase.TimeRunOut);
        }

        if (Input.GetKeyDown(KeyCode.R) && _isGameOver) //if the game is over, you can reload game with R key
        {
                SceneManager.LoadScene(0); //current game scene
        }

        if (Input.GetKey(KeyCode.Escape)) //Exit Game
        {
                Application.Quit();
        }
        
    }

        public void GameOver(LostGameCase lostGameCase)
        {
            _isGameOver = true;
            //_uiManager.LosingScreen(lostGameCase);
            //Do other staff, like show the Score that you managed to get
        }
        
        public void GameWon()
        {
            _isGameOver = true;
            //_uiManager.WinningScreen();
            //Do other staff, that needs to happen , like Show Score
        }
        
        public void AddScore(int points)
        {
            _score += points;
            //_uiManager.UpdateScore(_score);
            //communicate with ui
        }
}
