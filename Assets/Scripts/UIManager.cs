using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject naughtyChildrenList;
    [SerializeField] private Text naughtyChildrenListText;
    [SerializeField] private TextMeshProUGUI naughtyChildrenLeftText;
    [SerializeField] private Text scoreText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private GameObject gameOverTexture;
    [SerializeField] private Text gameOverText;
    [SerializeField] private GameObject gameWonTexture;
    //[SerializeField] private Text credits; //We show in the fist Menu
    //[SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private Slider sliderMaster;
    [SerializeField] private Slider sliderMusic;
    [SerializeField] private Slider sliderSFX;
    [SerializeField] private AudioMixer audioMixer;



    [SerializeField] private GameObject tutrialInfo;
    bool inTutorial;


    int currentlyDisplayScore = 0;

    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = "Score: " + 0;
        timeText.text = "Time limit: --.--";
       
        //gameOverTexture.gameObject.SetActive(false);
        //gameWonTexture.gameObject.SetActive(false);


        tutrialInfo.gameObject.SetActive(true);
        Time.timeScale = 0;
        inTutorial = true;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScore();
        

        if (inTutorial) 
        {
            if(Input.GetKeyUp(KeyCode.G)) 
            {
                tutrialInfo.gameObject.SetActive(false);
                Time.timeScale = 1;
                inTutorial = false;
            }
        }
    }

    public void UpdateNaughtlyCount(int count, bool playAnimation)
    {
        if (playAnimation)
        {
            naughtyChildrenLeftText.GetComponent<Animator>().SetTrigger("PositiveScore");


        }
        currentlyDisplayScore = count;
       
        naughtyChildrenLeftText.text = currentlyDisplayScore.ToString() ;
    }

    public void UpdateScore()
    {
        scoreText.text = "Score: " + WinCondition.Instance.GetScore();
    }

    public void UpdateTime(float time)
    {
        timeText.text =   ((int)time).ToString();

            
    }

    public void UpdateTime(float time, bool positiveImapct) 
    {
        Debug.Log("W");
        if (positiveImapct)
        {
            timeText.GetComponent<Animator>().SetTrigger("PositiveScore");
        }
        else 
        {
            timeText.GetComponent<Animator>().SetTrigger("NegativeScore");
        }

        UpdateTime(time);
    }

    public void ActivateGameOverScreen(WinCondition.LostGameCase lostGameCase)
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
        gameOverTexture.gameObject.SetActive(true);
        Time.timeScale = 0f;
        gameOverText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Time elapsed: "+ (int)WinCondition.Instance.totalTime+"\nNaughty children eaten: " + interaction.badChildrenEatCount + "\nGood children eaten: " + interaction.goodChildrenEatCount;
        /*switch (lostGameCase)
        {
            case WinCondition.LostGameCase.TimeRunOut:
                gameOverText.text = "You ran out of time!";
                break;
            case WinCondition.LostGameCase.TooManyWrongChildren:
                gameOverText.text = "You ate the wrong children!";
                break;
            case WinCondition.LostGameCase.DetectedByParents:
                gameOverText.text = "You have been caught by nuns!";
                break;
            default:
                gameOverText.text = "You have made an error!";
                break;
        }*/
    }

    public void ActivateGameWonScreen()
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
        gameWonTexture.gameObject.SetActive(true);
        Time.timeScale = 0f;
        gameOverText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Time elapsed: "+ (int)WinCondition.Instance.totalTime+"\nNaughty children eaten: " + interaction.badChildrenEatCount + "\nGood children eaten: " + interaction.goodChildrenEatCount;
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
        sliderMaster.value = Mathf.Pow(10,GetVolumeFromMixer("MasterVolume")/20); //Reversed values from SetFloat in volumeUpdate()
        sliderMusic.value = Mathf.Pow(10, GetVolumeFromMixer("MusicVolume")/20);
        sliderSFX.value = Mathf.Pow(10, GetVolumeFromMixer("SFXVolume")/20);
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
    public void MasterVolumeChanged()
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(sliderMaster.value) * 20);
    }
    public void MusicVolumeChanged()
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(sliderMusic.value) * 20);
    }
    public void SFXVolumeChanged()
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(sliderSFX.value) * 20);
    }
    public void volumeUpdate()
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(sliderMaster.value)*20); //Changed to Log10 to adjust better than linear
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(sliderMusic.value)*20);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(sliderSFX.value)*20);
    }
}
