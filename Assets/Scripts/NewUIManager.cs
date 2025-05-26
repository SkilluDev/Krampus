using System;
using System.Collections;
using System.Linq;
using System.Net.Mime;
using LitMotion;
using LitMotion.Extensions;
using NaughtyAttributes;
using Roomgen;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;


public class NewUIManager : MonoBehaviour {

    public NumericDisplay TimerDisplay => m_timerDisplay;

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


    [SerializeField] private GameObject m_quickActionIcon;
    public GameObject quickActionIcon => m_quickActionIcon;

    private float m_startFill;

    private MotionHandle m_currentFillHandle;

    private Color m_originalTimerColor;
    private Vector3 m_originalTimerLocalScale;
    [SerializeField] private Color m_goodTimerColor;
    [SerializeField] private Color m_badTimerColor;

    public BlackBars BlackBars => m_blackBars;
    [SerializeField] private BlackBars m_blackBars;

    private MotionHandle m_currentBounce;

    private bool m_uiOn = false;

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


    [BoxGroup("UI Blocks")][SerializeField] private RectTransform m_uiBlockLeft;
    [BoxGroup("UI Blocks")][SerializeField] private RectTransform m_uiBlockRight;
    [ResizableTextArea][BoxGroup("Prompts")][SerializeField] private string m_bottomBarTutorialKeys;
    [ResizableTextArea][BoxGroup("Prompts")][SerializeField] private string m_bottomBarMenuKeys;

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
        m_startFill = m_fillBar.fillAmount;

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

    private void Start() {
        m_blackBars.ShowInstant();
        m_blackBars.SetTopBarText("");
        m_blackBars.SetBottomBarText(m_bottomBarTutorialKeys);
        quickActionIcon.SetActive(false);
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
        m_currentFillHandle.TryCancel();
        float oldValue = m_fillBar.fillAmount;
        m_currentFillHandle = LMotion.Create(oldValue, math.remap(0, 1, m_startFill, 1, (float)(Game.MainGameInfo.BadChildrenCountOnStart - Game.MainGameInfo.BadChildren.Count()) /
                                 Game.MainGameInfo.BadChildrenCountOnStart), 2f).WithDelay(0.7f).BindToFillAmount(m_fillBar);
    }


    public void DisplayScoreBoard() {
        // m_scoreboard.SetActive(true);
    }

    public void HideBlackBars(bool showTutorial) {
        m_blackBars.Hide();
        if (!showTutorial) return;
        if (Game.SetMan.GetValue<bool>("Show Tutorial")) m_tutorial.gameObject.SetActive(true);
    }

    public void UIElementsEntryAnimation() {
        if (m_uiOn) return;
        Debug.Log("Entry");
        m_uiBlockLeft.gameObject.SetActive(true);
        LMotion.Create(-200, 50, 0.375f).Bind(x => m_uiBlockLeft.anchoredPosition = new Vector2(x, m_uiBlockLeft.anchoredPosition.y));
        m_uiBlockRight.gameObject.SetActive(true);
        LMotion.Create(300, -50, 0.375f).Bind(x => m_uiBlockRight.anchoredPosition = new Vector2(x, m_uiBlockRight.anchoredPosition.y));
        m_uiOn = true;
    }

    public IEnumerator ProcessEnding(Ending ending) {
        m_blackBars.Show();
        m_blackBars.SetTopBarText("");
        m_blackBars.SetBottomBarText(m_bottomBarMenuKeys);
        m_endScreenHandler.PreActivate(ending);
        yield return new WaitForSeconds(3);
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

    public void StartQuickTimeTimer(float seconds) {


    }
}
