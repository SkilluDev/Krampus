using System;
using System.Linq;
using LitMotion;
using LitMotion.Extensions;
using NaughtyAttributes;
using Roomgen;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class NewUIManager : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI m_remainingChildCount;
    [SerializeField] private TextMeshProUGUI m_goodChild;
    [SerializeField] private TextMeshProUGUI m_currentSeed;
    [SerializeField] private GameObject m_gameOverScreen;
    [SerializeField] private GameObject m_gameWinScreen;
    [SerializeField] private GameObject m_pauseScreen;
    [SerializeField] private TextMeshProUGUI m_timerText;
    [SerializeField] private NumericDisplay m_timerDisplay;

    [SerializeField] private Image m_childIconImage;
    [SerializeField] private Image m_fillBar;
    private MainGameInfo.State m_currentGameState;

    private Color m_originalTimerColor;
    [SerializeField] private Color m_goodTimerColor;
    [SerializeField] private Color m_badTimerColor;



    [BoxGroup("ButtonSets")]
    [SerializeField] private ButtonSet[] m_buttonSets;
    [BoxGroup("In Game Layout")][SerializeField] private Image m_pauseImage;
    [BoxGroup("In Game Layout")][SerializeField] private Image m_attackImage;
    [BoxGroup("In Game Layout")][SerializeField] private Image m_sneakImage;


    [BoxGroup("GameOver Screen")][SerializeField] private Image m_menuImage;
    [BoxGroup("GameOver Screen")][SerializeField] private Image m_resetImage;

    [BoxGroup("Score Board")][SerializeField] private GameObject m_scoreboard;


    [BoxGroup("Score Board")][SerializeField] private TextMeshProUGUI m_childPerMinuteText;
    [BoxGroup("Tutorial")][SerializeField] private TutorialHandler m_tutorial;


    private void OnChildEaten(ChildType childType) {
        Color destinationColor;
        if (childType != Game.MainGameInfo.GoodChildType) {
            destinationColor = m_goodTimerColor;
            ChangeChildCounter();
        } else {
            destinationColor = m_badTimerColor;

        }

        LMotion.Create(m_timerDisplay.Color, destinationColor, 0.1f).WithEase(Ease.InOutCubic).WithOnComplete(
            () => LMotion.Create(destinationColor, m_originalTimerColor, 1.2f).WithEase(Ease.InOutCubic).Bind(m_timerDisplay.SetColor)
                ).Bind(m_timerDisplay.SetColor);

    }

    public void SetSeed(int seed) {
        m_currentSeed.text = $"Map seed: {seed:0000000}<br>";
    }

    public void ShowGameOverScreen() {
        m_gameOverScreen.SetActive(true);
        m_timerText.gameObject.SetActive(false);
        DisplayScoreBoard();
    }

    public void SwitchPauseMenu() {
        if (m_currentGameState != MainGameInfo.State.Over) {
            if (m_currentGameState == MainGameInfo.State.Paused) {
                Game.MainGameInfo.SetState(MainGameInfo.State.Game);
                m_pauseScreen.SetActive(false);
            } else {
                Game.MainGameInfo.SetState(MainGameInfo.State.Paused);
                m_pauseScreen.SetActive(true);
                Debug.Log("relrrellrelrlelrel");
            }
            Time.timeScale = m_currentGameState == MainGameInfo.State.Paused ? 1 : 0;
        }
    }


    private void Update() {


        SetButtonSet();

        m_currentGameState = Game.MainGameInfo.CurrentState;
        var col = Game.MainGameInfo.GoodChildType;
        m_remainingChildCount.text = $"<color=#{ColorUtility.ToHtmlStringRGB(col.color)}>Do not eat: {col.shape.name}</color><br>Remaining children: {Game.MainGameInfo.Children.Count}";
        if (Game.MainGameInfo.BadChildrenCountOnStart > 0) {

        }

        if (!Game.Balling) {
            m_timerDisplay.gameObject.SetActive(Mathf.RoundToInt(Time.unscaledTime * 2) % 2 == 0);
        } else {
            m_timerDisplay.gameObject.SetActive(true);
        }

        m_timerText.text = Game.MainGameInfo.Timer.GameTime.ToString("00");
        m_timerDisplay.Value = Game.MainGameInfo.Timer.GameTime;


        if (InputSubscribe.Raw.UI.Pause.triggered) SwitchPauseMenu();

        if (!Game.MainGameInfo.BadChildren.Any() && !Game.IsLoading) {
            Debug.Log("won");
            m_gameWinScreen.SetActive(true);
            Game.MainGameInfo.SetState(MainGameInfo.State.Won);
            Time.timeScale = 0;
            DisplayScoreBoard();

        }
        if (m_currentGameState == MainGameInfo.State.Over || m_currentGameState == MainGameInfo.State.Paused || m_currentGameState == MainGameInfo.State.Won) {//if the game is over, won, or paused, you can
            if (InputSubscribe.Raw.UI.MenuReturn.triggered) { //go back to menu with M
                Game.MainGameInfo.SetState(MainGameInfo.State.Game);
                Game.LoadState(Game.State.MainMenu);
            }
            if (InputSubscribe.Raw.UI.Restart.triggered) { //restart with R
                Game.RoomGenInfo.Regenerate = RoomGenerationType.Old;
                Game.MainGameInfo.SetState(MainGameInfo.State.Game);
                Game.LoadState(Game.State.MainGame);
            }

            if (InputSubscribe.Raw.UI.RestartAndRegen.triggered) { //generate and restart with G
                Game.RoomGenInfo.Regenerate = RoomGenerationType.New;
                Game.MainGameInfo.SetState(MainGameInfo.State.Game);
                Game.LoadState(Game.State.MainGame);
            }
        }
    }

    public void SetChildrenIcon(Sprite icon) {
        m_childIconImage.sprite = icon;
    }

    private void Ready() {
        Game.MainGameInfo.GlobalEvents.onChildEaten += OnChildEaten;
        m_originalTimerColor = m_timerDisplay.Color;
        if(Game.SetMan.GetValue<bool>("Show Tutorial")) m_tutorial.gameObject.transform.parent.gameObject.SetActive(true);
    }

    public void ChangeChildCounter() {
        float oldValue = m_fillBar.fillAmount;
        LMotion.Create(oldValue, 0.1f + (float)(Game.MainGameInfo.BadChildrenCountOnStart - Game.MainGameInfo.BadChildren.Count()) /
                                 Game.MainGameInfo.BadChildrenCountOnStart * 0.9f, 0.1f).BindToFillAmount(m_fillBar);
    }

    public void SetButtonSet() {

        ButtonSet bs = null;
        foreach (var b in m_buttonSets) {
            if (b.method == InputSubscribe.InputMethod) {
                bs = b;
                break;
            }
        }
        if (bs == null) return;

        m_pauseImage.sprite = bs.pause_Button;
        m_resetImage.sprite = bs.restart_Button;
        m_menuImage.sprite = bs.backToMenu_Button;
        m_sneakImage.sprite = bs.sneak_Button;
        m_attackImage.sprite = bs.attack_Button;
    }

    public void DisplayScoreBoard() {
	    m_scoreboard.SetActive(true);
	    float time = (Game.MainGameInfo.timeFromStart / 60);
	    float val = Mathf.Round(Game.MainGameInfo.Score / time * 100) / 100;
	    Debug.Log(val);
	    LMotion.Create(0, val , 2).WithEase(Ease.OutExpo)
        .Bind(x => m_childPerMinuteText.SetText(x.ToString("#.00")));
    }
}
