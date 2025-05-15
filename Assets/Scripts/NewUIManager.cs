using System;
using System.Linq;
using System.Net.Mime;
using LitMotion;
using LitMotion.Extensions;
using NaughtyAttributes;
using Roomgen;
using TMPro;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;


public class NewUIManager : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI m_remainingChildCount;
    [SerializeField] private TextMeshProUGUI m_currentSeed;
    [SerializeField] private GameObject m_pauseScreen;
    [SerializeField] private Transform m_timer;
    [SerializeField] private NumericDisplay m_timerDisplay;

    [SerializeField] private float m_timerShakeDuration;

    [SerializeField] private float m_timerShakeIntensity;
    [SerializeField] private float m_timerBounceDuration;

    [SerializeField] private float m_timerBounceIntensity;

    [SerializeField] private Image m_childIconImage;
    [SerializeField] private Image m_fillBar;
    private MainGameInfo.State m_currentGameState;

    private Color m_originalTimerColor;
    [SerializeField] private Color m_goodTimerColor;
    [SerializeField] private Color m_badTimerColor;




    [BoxGroup("Pause Screen")] [SerializeField] private Image m_resumePauseButton;
    [BoxGroup("Pause Screen")] [SerializeField] private Image m_restartPauseButton;
    [BoxGroup("Pause Screen")] [SerializeField] private Image m_rerollPauseButton;
    [BoxGroup("Pause Screen")] [SerializeField] private Image m_menuPauseButton;



    [BoxGroup("ButtonSets")]
    [SerializeField] private ButtonSet[] m_buttonSets;
    [BoxGroup("In Game Layout")][SerializeField] private Image m_pauseImage;
    [BoxGroup("In Game Layout")][SerializeField] private Image m_attackImage;
    [BoxGroup("In Game Layout")][SerializeField] private Image m_sneakImage;



    [BoxGroup("Score Board")][SerializeField] private GameObject m_scoreboard;


    [BoxGroup("Score Board")][SerializeField] private TextMeshProUGUI m_childPerMinuteText;
    [BoxGroup("Tutorial")][SerializeField] private TutorialHandler m_tutorial;

    [BoxGroup("End Screen")][SerializeField] private EndScreenHandler m_endScreenHandler;
    [BoxGroup("End Screen")] [SerializeField] private Image m_restartEndScreenButton;
    [BoxGroup("End Screen")] [SerializeField] private Image m_rerollEndScreenButton;
    [BoxGroup("End Screen")] [SerializeField] private Image m_menuEndScreenButton;


    [BoxGroup("Intro")][SerializeField] private Image m_topBarImage;
    [BoxGroup("Intro")][SerializeField] private Image m_bottomBarImage;
    [BoxGroup("Intro")] [SerializeField] private Image m_quitButtonImage;


    [BoxGroup("UI Blocks")] [SerializeField] private RectTransform m_uiBlockLeft;
    [BoxGroup("UI Blocks")] [SerializeField] private RectTransform m_uiBlockRight;

    [Button]
    private void EatGoodChild(){
        OnChildEaten(Game.MainGameInfo.GoodChildType);
    }

    [Button]
    private void EatBadChild(){
        OnChildEaten(Game.MainGameInfo.RandomBadChildType);
    }
    private void OnChildEaten(ChildType childType) {
        Color destinationColor;
        //Positive feedback
        if (childType != Game.MainGameInfo.GoodChildType) {
            destinationColor = m_goodTimerColor;
            ChangeChildCounter();
            Vector3 oldScale = m_timer.localScale;
            Vector3 newScale = oldScale*m_timerBounceIntensity;
            //Scrapped, Krampus always walking
            //(Game.MainGameInfo.Krampus.Kontroller.CurrentState is not KrampusController.State.Walk?1f:0.9f);
            LMotion.Create(oldScale, newScale, m_timerShakeDuration/2).WithEase(Ease.OutElastic).WithOnComplete(
                ()=>LMotion.Create(newScale, oldScale, m_timerShakeDuration/2).WithEase(Ease.OutBounce).BindToLocalScale(m_timer)
            ).BindToLocalScale(m_timer);

            LMotion.Shake.Create(m_timer.localPosition, Vector3.one*m_timerShakeIntensity/5, m_timerShakeDuration).WithEase(Ease.InOutQuad).BindToLocalPosition(m_timer);

        //Negative feedback
        } else {
            destinationColor = m_badTimerColor;
            LMotion.Shake.Create(m_timer.localPosition, Vector3.one*m_timerShakeIntensity, m_timerShakeDuration).WithEase(Ease.InOutQuad).BindToLocalPosition(m_timer);
        }

        LMotion.Create(m_timerDisplay.Color, destinationColor, 0.1f).WithEase(Ease.InOutCubic).WithOnComplete(
            () => LMotion.Create(destinationColor, m_originalTimerColor, 1.2f).WithEase(Ease.InOutCubic).Bind(m_timerDisplay.SetColor)
                ).Bind(m_timerDisplay.SetColor);

    }

    public void SetSeed(int seed) {
        m_currentSeed.text = $"Map seed: {seed:0000000}<br>";
    }

    public void ProcessEndGame(Ending ending) {
        switch (ending){
            case Ending.Win:
                Game.MainGameInfo.SetState(MainGameInfo.State.Won);
                break;
            case Ending.LoseNun:
                Game.MainGameInfo.SetState(MainGameInfo.State.Over);
                break;
            case Ending.LoseTime:
                Game.MainGameInfo.SetState(MainGameInfo.State.Over);
                break;
        }
        m_endScreenHandler.Activate(ending);
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


        //SetButtonSet();

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

        m_timerDisplay.Value = Game.MainGameInfo.Timer.GameTime;


        if (InputSubscribe.Raw.UI.Pause.triggered) {if(Game.MainGameInfo.CurrentState != MainGameInfo.State.Intro) SwitchPauseMenu();}
        
        if (!Game.MainGameInfo.BadChildren.Any() && !Game.IsLoading) {
            ProcessEndGame(Ending.Win);
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
        m_tutorial.gameObject.transform.parent.gameObject.SetActive(false);
        m_uiBlockLeft.gameObject.SetActive(false);
        m_uiBlockRight.gameObject.SetActive(false);

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

        m_sneakImage.sprite = bs.sneak_Button;
        m_attackImage.sprite = bs.attack_Button;
        m_quitButtonImage.sprite = bs.quit_Button;
		//==

		m_restartEndScreenButton.sprite = bs.restart_Button;
		m_rerollEndScreenButton.sprite = bs.reload_Button;
		m_menuEndScreenButton.sprite = bs.backToMenu_Button;

        //==
        m_rerollPauseButton.sprite = bs.reload_Button;
       m_restartPauseButton.sprite = bs.restart_Button;
       m_menuPauseButton.sprite = bs.backToMenu_Button;
       m_resumePauseButton.sprite = bs.pause_Button;
       //==
    }

    public void DisplayScoreBoard() {
	    m_scoreboard.SetActive(true);
	    float time = (Game.MainGameInfo.timeFromStart / 60);
	    float val = Mathf.Round(Game.MainGameInfo.Score / time * 100) / 100;
	    Debug.Log(val);
	    LMotion.Create(0, val , 2).WithEase(Ease.OutExpo)
        .Bind(x => m_childPerMinuteText.SetText(x.ToString("#.00")));
    }

    public void HideBlackBars(bool showTutorial) {
	    float basedYSize = m_bottomBarImage.rectTransform.sizeDelta.y;
	    LMotion.Create(basedYSize,0,0.75f).Bind(x=>m_bottomBarImage.rectTransform.sizeDelta=new Vector2(m_bottomBarImage.rectTransform.sizeDelta.x,x));
	    LMotion.Create(basedYSize,0,0.75f).Bind(x=>m_topBarImage.rectTransform.sizeDelta=new Vector2(m_topBarImage.rectTransform.sizeDelta.x,x));
	    if(!showTutorial) return;
	    if(Game.SetMan.GetValue<bool>("Show Tutorial")) m_tutorial.gameObject.transform.parent.gameObject.SetActive(true);
    }

    public void UIElementsEntryAnimation()
    {
	    m_uiBlockLeft.gameObject.SetActive(true);
	    LMotion.Create(-200, 50, 0.375f).Bind(x => m_uiBlockLeft.anchoredPosition =new Vector2( x,m_uiBlockLeft.anchoredPosition.y));
	    m_uiBlockRight.gameObject.SetActive(true);
	    LMotion.Create(300, -50, 0.375f).Bind(x => m_uiBlockRight.anchoredPosition =new Vector2( x,m_uiBlockRight.anchoredPosition.y));
    }
}
