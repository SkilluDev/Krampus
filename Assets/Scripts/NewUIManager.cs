using System;
using System.Linq;
using System.Net.Mime;
using LitMotion;
using LitMotion.Extensions;
using NaughtyAttributes;
using Roomgen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class NewUIManager : MonoBehaviour {

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

    private Color m_originalTimerColor;
    private Vector3 m_originalTimerLocalScale;
    [SerializeField] private Color m_goodTimerColor;
    [SerializeField] private Color m_badTimerColor;

    private MotionHandle m_currentBounce;








    [BoxGroup("Pause Screen")][SerializeField] private Image m_resumePauseButton;
    [BoxGroup("Pause Screen")][SerializeField] private Image m_restartPauseButton;
    [BoxGroup("Pause Screen")][SerializeField] private Image m_rerollPauseButton;
    [BoxGroup("Pause Screen")][SerializeField] private Image m_menuPauseButton;




    [BoxGroup("Score Board")][SerializeField] private GameObject m_scoreboard;
    [BoxGroup("Score Board")][SerializeField] private TextMeshProUGUI m_childPerMinuteText;




    [BoxGroup("Tutorial")][SerializeField] private TutorialHandler m_tutorial;

    [BoxGroup("End Screen")][SerializeField] private EndScreenHandler m_endScreenHandler;
    [BoxGroup("End Screen")][SerializeField] private Image m_restartEndScreenButton;
    [BoxGroup("End Screen")][SerializeField] private Image m_rerollEndScreenButton;
    [BoxGroup("End Screen")][SerializeField] private Image m_menuEndScreenButton;


    [BoxGroup("Intro")][SerializeField] private Image m_topBarImage;
    [BoxGroup("Intro")][SerializeField] private Image m_bottomBarImage;
    [BoxGroup("Intro")][SerializeField] private Image m_quitButtonImage;


    [BoxGroup("UI Blocks")][SerializeField] private RectTransform m_uiBlockLeft;
    [BoxGroup("UI Blocks")][SerializeField] private RectTransform m_uiBlockRight;

    [Button]
    private void EatGoodChild() {
        OnChildEaten(Game.MainGameInfo.GoodChildType);
    }

    [Button]
    private void EatBadChild() {
        OnChildEaten(Game.MainGameInfo.RandomBadChildType);
    }

    private void Awake() {
        m_originalTimerLocalScale = m_timer.localScale;

    }
    private void OnChildEaten(ChildType childType) {
        Color destinationColor;
        //Positive feedback
        if (childType != Game.MainGameInfo.GoodChildType) {
            destinationColor = m_goodTimerColor;
            ChangeChildCounter();

            if (m_currentBounce.IsActive()) m_currentBounce.Cancel();

            var oldScale = m_originalTimerLocalScale;
            var newScale = oldScale * m_timerBounceIntensity;
            //Scrapped, Krampus always walking
            //(Game.MainGameInfo.Krampus.Kontroller.CurrentState is not KrampusController.State.Walk?1f:0.9f);
            m_currentBounce = LMotion.Create(oldScale, newScale, m_timerShakeDuration / 2).WithEase(Ease.OutElastic).WithOnComplete(
                () => LMotion.Create(newScale, oldScale, m_timerShakeDuration / 2).WithEase(Ease.OutBounce).BindToLocalScale(m_timer)
            ).BindToLocalScale(m_timer);

            LMotion.Shake.Create(m_timer.localPosition, Vector3.one * m_timerShakeIntensity / 5, m_timerShakeDuration).WithEase(Ease.InOutQuad).BindToLocalPosition(m_timer);

            //Negative feedback
        } else {
            destinationColor = m_badTimerColor;
            LMotion.Shake.Create(m_timer.localPosition, Vector3.one * m_timerShakeIntensity, m_timerShakeDuration).WithEase(Ease.InOutQuad).BindToLocalPosition(m_timer);
        }

        LMotion.Create(m_timerDisplay.Color, destinationColor, 0.1f).WithEase(Ease.InOutCubic).WithOnComplete(
            () => LMotion.Create(destinationColor, m_originalTimerColor, 1.2f).WithEase(Ease.InOutCubic).Bind(m_timerDisplay.SetColor)
                ).Bind(m_timerDisplay.SetColor);

    }

    public void SetSeed(int seed) {
        m_currentSeed.text = $"Map seed: {seed:0000000}<br>";
    }

    public void SwitchPauseMenu(bool activate) {
        if (activate) m_pauseScreen.SetActive(true);
        else m_pauseScreen.SetActive(false);
    }


    private void Update() {
        if (!Game.Balling) {
            m_timerDisplay.gameObject.SetActive(Mathf.RoundToInt(Time.unscaledTime * 2) % 2 == 0);
        } else {
            m_timerDisplay.gameObject.SetActive(true);
        }

        m_timerDisplay.Value = Game.MainGameInfo.Timer.GameTime;
    }

    public void SetChildrenIcon(Sprite icon) {
        m_childIconImage.sprite = icon;
    }

    private void Ready() {
        Game.MainGameInfo.GlobalEvents.onChildEaten += OnChildEaten;
        m_originalTimerColor = m_timerDisplay.Color;
        m_tutorial.gameObject.SetActive(false);
        m_uiBlockLeft.gameObject.SetActive(false);
        m_uiBlockRight.gameObject.SetActive(false);
    }

    public void ChangeChildCounter() {
        float oldValue = m_fillBar.fillAmount;
        LMotion.Create(oldValue, (float)(Game.MainGameInfo.BadChildrenCountOnStart - Game.MainGameInfo.BadChildren.Count()) /
                                 Game.MainGameInfo.BadChildrenCountOnStart, 0.3f).WithDelay(0.5f).BindToFillAmount(m_fillBar);
    }


    public void DisplayScoreBoard() {
        m_scoreboard.SetActive(true);
        float time = Game.MainGameInfo.timeFromStart / 60;
        float val = Mathf.Round(Game.MainGameInfo.Score / time * 100) / 100;
        Debug.Log(val);
        LMotion.Create(0, val, 2).WithEase(Ease.OutExpo)
        .Bind(x => m_childPerMinuteText.SetText(x.ToString("#.00")));
    }

    public void HideBlackBars(bool showTutorial) {
        float basedYSize = m_bottomBarImage.rectTransform.sizeDelta.y;
        LMotion.Create(basedYSize, 0, 0.75f).Bind(x => m_bottomBarImage.rectTransform.sizeDelta = new Vector2(m_bottomBarImage.rectTransform.sizeDelta.x, x));
        LMotion.Create(basedYSize, 0, 0.75f).Bind(x => m_topBarImage.rectTransform.sizeDelta = new Vector2(m_topBarImage.rectTransform.sizeDelta.x, x));
        if (!showTutorial) return;
        if (Game.SetMan.GetValue<bool>("Show Tutorial")) m_tutorial.gameObject.SetActive(true);
    }

    public void UIElementsEntryAnimation() {
        Debug.Log("Entry");
        m_uiBlockLeft.gameObject.SetActive(true);
        LMotion.Create(-200, 50, 0.375f).Bind(x => m_uiBlockLeft.anchoredPosition = new Vector2(x, m_uiBlockLeft.anchoredPosition.y));
        m_uiBlockRight.gameObject.SetActive(true);
        LMotion.Create(300, -50, 0.375f).Bind(x => m_uiBlockRight.anchoredPosition = new Vector2(x, m_uiBlockRight.anchoredPosition.y));
    }

    public void ProcessEnding(Ending ending) {
        m_endScreenHandler.Activate(ending);
        DisplayScoreBoard();
    }

    public void PopupTimer() {
        if (m_currentBounce.IsActive()) m_currentBounce.Cancel();
        var oldScale = m_originalTimerLocalScale;
        m_currentBounce = LMotion.Create(oldScale, oldScale * 1.3f, 0.2f).WithEase(Ease.OutElastic).WithOnComplete(
                () => LMotion.Create(oldScale * 1.3f, oldScale, 0.2f).WithEase(Ease.OutBounce).BindToLocalScale(m_timer)
            ).BindToLocalScale(m_timer);
    }
}
