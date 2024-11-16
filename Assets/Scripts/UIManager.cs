using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
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
        _timeText.text = "Score: " + time.ToString();
    }
}
