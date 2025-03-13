using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    [FormerlySerializedAs("naughtyChildrenList")][SerializeField] private GameObject m_naughtyChildrenList;
    [FormerlySerializedAs("naughtyChildrenListText")][SerializeField] private Text m_naughtyChildrenListText;
    [FormerlySerializedAs("naughtyChildrenLeftText")][SerializeField] private TextMeshProUGUI m_naughtyChildrenLeftText;
    [FormerlySerializedAs("scoreText")][SerializeField] private Text m_scoreText;
    [FormerlySerializedAs("timeText")][SerializeField] private TextMeshProUGUI m_timeText;
    [FormerlySerializedAs("gameOverTexture")][SerializeField] private GameObject m_gameOverTexture;
    [FormerlySerializedAs("gameOverText")][SerializeField] private Text m_gameOverText;
    [FormerlySerializedAs("gameWonTexture")][SerializeField] private GameObject m_gameWonTexture;
    [FormerlySerializedAs("gameWonText")][SerializeField] private Text m_gameWonText;
    //[SerializeField] private Text credits; //We show in the fist Menu
    //[SerializeField] private GameObject startMenu;
    [FormerlySerializedAs("settingsMenu")][SerializeField] private GameObject m_settingsMenu;
    [FormerlySerializedAs("sliderMaster")][SerializeField] private Slider m_sliderMaster;
    [FormerlySerializedAs("sliderMusic")][SerializeField] private Slider m_sliderMusic;
    [FormerlySerializedAs("sliderSFX")][SerializeField] private Slider m_sliderSFX;
    [FormerlySerializedAs("audioMixer")][SerializeField] private AudioMixer m_audioMixer;
    [FormerlySerializedAs("showTutorials")][SerializeField] private GameObject m_showTutorials;
    [FormerlySerializedAs("clockAnimator")][SerializeField] private Animator m_clockAnimator;
    [FormerlySerializedAs("loreNote")][SerializeField] private GameObject m_loreNote;
    private bool m_inTutorial;
    private int m_currentlyDisplayScore = 0;

    // Start is called before the first frame update
    private void Start() {
        m_scoreText.text = "Score: " + 0;
        m_timeText.text = "Time limit: --.--";

        //gameOverTexture.gameObject.SetActive(false);
        //gameWonTexture.gameObject.SetActive(false);


        m_loreNote.gameObject.SetActive(true);
        Time.timeScale = 0;
        m_inTutorial = true;
        m_showTutorials.GetComponent<Toggle>().isOn = TutorialManager.GetShowTutorials();
    }

    // Update is called once per frame
    private void Update() {
        UpdateScore();


        if (m_inTutorial) {
            if (Input.GetKeyUp(KeyCode.G)) {
                m_loreNote.gameObject.SetActive(false);
                Time.timeScale = 1;
                m_inTutorial = false;
            }
        }
    }

    public void UpdateNaughtlyCount(int count, bool playAnimation) {
        if (playAnimation) {
            m_naughtyChildrenLeftText.GetComponent<Animator>().SetTrigger("PositiveScore");


        }
        m_currentlyDisplayScore = count;

        m_naughtyChildrenLeftText.text = m_currentlyDisplayScore.ToString();
    }

    public void UpdateScore() {
        m_scoreText.text = "Score: " + WinCondition.Instance.GetScore();
    }

    public void UpdateTime(float time) {
        m_timeText.text = ((int)time).ToString();
    }


    public void StopClock() {
        m_clockAnimator.SetTrigger("Dead");

    }
    public void UpdateTime(float time, bool positiveImapct) {
        Debug.Log("W");
        if (positiveImapct) {
            m_timeText.GetComponent<Animator>().SetTrigger("PositiveScore");
        } else {
            m_timeText.GetComponent<Animator>().SetTrigger("NegativeScore");
        }

        UpdateTime(time);
    }

    public void ActivateGameOverScreen(WinCondition.LostGameCase lostGameCase) {
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
        m_gameOverTexture.gameObject.SetActive(true);
        Time.timeScale = 0f;
        m_gameOverText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Time elapsed: " + (int)WinCondition.Instance.totalTime + "\nNaughty children eaten: " + Interaction.badChildrenEatCount + "\nGood children eaten: " + Interaction.goodChildrenEatCount;
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

    public void ActivateGameWonScreen() {
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
        m_gameWonTexture.gameObject.SetActive(true);
        Time.timeScale = 0f;
        m_gameWonText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Time elapsed: " + (int)WinCondition.Instance.totalTime + "\nNaughty children eaten: " + Interaction.badChildrenEatCount + "\nGood children eaten: " + Interaction.goodChildrenEatCount;
    }

    public void ActivateNaughtyChildrenList() {
        m_naughtyChildrenList.gameObject.SetActive(true);
    }

    public void UpdateChildrenList(string listaDzieci) {
        m_naughtyChildrenListText.text = "These children were very naughty this year: " + listaDzieci;
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
    public void ActivateSettingsMenu() {
        m_settingsMenu.gameObject.SetActive(true);
        m_sliderMaster.value = Mathf.Pow(10, GetVolumeFromMixer("MasterVolume") / 20); //Reversed values from SetFloat in volumeUpdate()
        m_sliderMusic.value = Mathf.Pow(10, GetVolumeFromMixer("MusicVolume") / 20);
        m_sliderSFX.value = Mathf.Pow(10, GetVolumeFromMixer("SFXVolume") / 20);
    }
    public void DeactivateSettingsMenu() {
        m_settingsMenu.gameObject.SetActive(false);
    }

    /*public void ActivateCredits()
    {
        credits.gameObject.SetActive(true);
    }
    */
    public float GetVolumeFromMixer(string name) {
        float value;
        bool result = m_audioMixer.GetFloat(name, out value);
        if (result) {
            return value;
        } else {
            return 0f;
        }
    }
    public void MasterVolumeChanged() {
        m_audioMixer.SetFloat("MasterVolume", Mathf.Log10(m_sliderMaster.value) * 20);
    }
    public void MusicVolumeChanged() {
        m_audioMixer.SetFloat("MusicVolume", Mathf.Log10(m_sliderMusic.value) * 20);
    }
    public void SFXVolumeChanged() {
        m_audioMixer.SetFloat("SFXVolume", Mathf.Log10(m_sliderSFX.value) * 20);
    }
    public void volumeUpdate() {
        m_audioMixer.SetFloat("MasterVolume", Mathf.Log10(m_sliderMaster.value) * 20); //Changed to Log10 to adjust better than linear
        m_audioMixer.SetFloat("MusicVolume", Mathf.Log10(m_sliderMusic.value) * 20);
        m_audioMixer.SetFloat("SFXVolume", Mathf.Log10(m_sliderSFX.value) * 20);
    }

    public bool getShowTutorials() {
        return m_showTutorials.GetComponent<Toggle>().isOn;
    }

    public void saveShowTutorials() {
        TutorialManager.SetShowTutorials(m_showTutorials.GetComponent<Toggle>().isOn);
    }
}
