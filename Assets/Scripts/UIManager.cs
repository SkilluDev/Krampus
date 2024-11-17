using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject naughtyChildrenList;
    [SerializeField] private Text naughtyChildrenListText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text timeText;
    [SerializeField] private GameObject gameOverTexture;
    [SerializeField] private Text gameOverText;
    [SerializeField] private GameObject gameWonTexture;
    //[SerializeField] private Text credits; //We show in the fist Menu
    //[SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private Slider sliderMaster;
    [SerializeField] private Slider sliderMusic;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private WinCondition winCondition;

    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = "Score: " + 0;
        timeText.text = "Time limit: --.--";
        gameOverTexture.gameObject.SetActive(false);
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
        gameOverTexture.gameObject.SetActive(true);
        switch (lostGameCase)
        {
            case WinCondition.LostGameCase.TimeRunOut:
                gameOverText.text = "You run out off time!";
                break;
            case WinCondition.LostGameCase.TooManyWrongChildren:
                gameOverText.text = "You ate the wrong children!";
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
        gameWonTexture.gameObject.SetActive(true);
    }

    public void ActivateNaughtyChildrenList()
    {
        naughtyChildrenList.gameObject.SetActive(true);
    }

    public void UpdateChildrenList(string listaDzieci)
    {
        naughtyChildrenListText.text = "These children were very naughty this year: "+ listaDzieci;
    }
    /*
    public void ActivateStartMenu()
    {
        startMenu.gameObject.SetActive(true);
    }

    public void DeactivateStartMenu()
    {
        startMenu.gameObject.SetActive(false);
    }
    */
    public void ActivateSettingsMenu()
    {
        settingsMenu.gameObject.SetActive(true);
        sliderMaster.value = GetVolumeFromMixer("Master");
        sliderMusic.value = GetVolumeFromMixer("Music");
    }
    public void DeactivateSettingsMenu()
    {
        settingsMenu.gameObject.SetActive(false);
    }

    /*public void ActivateCredits()
    {
        credits.gameObject.SetActive(true);
    }
    */
    public float GetVolumeFromMixer(string name){
        float value;
        bool result =  audioMixer.GetFloat(name, out value);
        if(result){
            return value;
        }else{
            return 0f;
        }
    }
    public void volumeUpdate()
    {
        audioMixer.SetFloat("Master", sliderMaster.value);
        audioMixer.SetFloat("Music", sliderMusic.value);
    }
}
