using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _naugthyChildrenList;
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Text _timeText;
    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _gameWonText;
    [SerializeField]
    private WinCondition _WinCondition;
    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text="Score: "+0;
        _timeText.text = "Time limit: --.--";
        _gameOverText.gameObject.SetActive(false);
        _WinCondition = GameObject.Find("Win_Condition").GetComponent<WinCondition>();
        if (_WinCondition == null)
        {
            Debug.LogError("The Win Condition is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateScore(int score)
    {
        _scoreText.text = "Score: " + score.ToString();
    }
    public void UpdateTime(float time)
    {
        _timeText.text = "Time: " + time.ToString();
    }
    public void ActivateGameOverScreen(WinCondition.LostGameCase lostGameCase)
    {
        _gameOverText.gameObject.SetActive(true);
        switch (lostGameCase)
        {
            case WinCondition.LostGameCase.TimeRunOut:
                _gameOverText.text = "You run out off time!";
                break;
            case WinCondition.LostGameCase.TooManyWrongChildren:
                _gameOverText.text= "You ate the wrong children!";
                break;
            case WinCondition.LostGameCase.DetectedByParents:
                _gameOverText.text = "You have been detected by human race!";
                break;
            default:
                _gameOverText.text = "You have made an error!";
                break;
        }
    }
    public void ActivateGameWonScreen()
    {
        _gameWonText.gameObject.SetActive(true);
    }
    public void ActivateNaughtyChildrenList()
    {
        _naugthyChildrenList.gameObject.SetActive(true);
    }
}
