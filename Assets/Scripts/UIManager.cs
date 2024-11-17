using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text naughtyChildrenList;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text timeText;
    [SerializeField] private Text gameOverText;
    [SerializeField] private Text gameWonText;
    [SerializeField] private WinCondition winCondition;
    // Start is called before the first frame update
    void Start()
    {
        scoreText.text="Score: "+0;
        timeText.text = "Time limit: --.--";
        gameOverText.gameObject.SetActive(false);
        //GameFind to slow - changed to Serialized Field
        //winCondition = GameObject.Find("Win_Condition").GetComponent<WinCondition>();
        /*
        if (winCondition == null)
        {
            Debug.LogError("The Win Condition is NULL.");
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score.ToString();
    }
    public void UpdateTime(float time)
    {
        timeText.text = "Time: " + time.ToString();
    }
    public void ActivateGameOverScreen(WinCondition.LostGameCase lostGameCase)
    {
        gameOverText.gameObject.SetActive(true);
        switch (lostGameCase)
        {
            case WinCondition.LostGameCase.TimeRunOut:
                gameOverText.text = "You run out off time!";
                break;
            case WinCondition.LostGameCase.TooManyWrongChildren:
                gameOverText.text= "You ate the wrong children!";
                break;
            case WinCondition.LostGameCase.DetectedByParents:
                gameOverText.text = "You have been detected by human race!";
                break;
            default:
                gameOverText.text = "You have made an error!";
                break;
        }
    }
    public void ActivateGameWonScreen()
    {
        gameWonText.gameObject.SetActive(true);
    }
    public void ActivateNaughtyChildrenList()
    {
        naughtyChildrenList.gameObject.SetActive(true);
    }
}
