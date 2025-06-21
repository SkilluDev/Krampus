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
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class NewUIManager : MonoBehaviour {

    public NumericDisplay TimerDisplay => m_timerDisplay;

    [SerializeField] private TextMeshProUGUI m_currentSeed;
    [SerializeField] private GameObject m_pauseScreen;

    [BoxGroup("Inventory")][SerializeField] private RectTransform m_inventoryContainer;
    [BoxGroup("Inventory")][SerializeField] private InventoryCard m_inventoryCardPref;

    [SerializeField] private Transform m_timer;
    [SerializeField] private NumericDisplay m_timerDisplay;
    [SerializeField] private float m_timerShakeDuration;
    [SerializeField] private float m_timerShakeIntensity;
    //[SerializeField] private float m_timerBounceDuration;
    [SerializeField] private float m_timerBounceIntensity;
    [SerializeField] private Image m_childIconImage;
    [SerializeField] private Image m_fillBar;
    [SerializeField] private AnimationCurve m_fillUpCurve;


    [SerializeField] private GameObject m_quickActionIcon;


    public GameObject QuickActionIcon => m_quickActionIcon;

    private float m_startFill;

    private MotionHandle m_currentFillHandle;
    private MotionHandle m_currentWindUpFillHandle;
    private Color m_originalTimerColor;
    private Vector3 m_originalTimerLocalScale;
    [SerializeField] private Color m_niceTimerColor;
    [SerializeField] private Color m_naughtyTimerColor;

    public BlackBars BlackBars => m_blackBars;
    [SerializeField] private BlackBars m_blackBars;

    [SerializeField] private ItemChoiceMenuUI m_itemChoiceMenuUI;
    public ItemChoiceMenuUI ItemChoiceMenu => m_itemChoiceMenuUI;

    private MotionHandle m_currentBounce;

    private bool m_uiOn = false;


    [BoxGroup("Wind-up")][SerializeField] private Image m_windUpFiller;
    [BoxGroup("Wind-up")][SerializeField] private RectTransform m_windUpCostBar;
    [BoxGroup("Wind-up")][SerializeField] private Vector2 m_markerRotatorEndPoints;

    [BoxGroup("Tutorial")][SerializeField] private TutorialHandler m_tutorial;

    [BoxGroup("End Screen")][SerializeField] private EndScreenHandler m_endScreenHandler;


    [BoxGroup("UI Blocks")][SerializeField] private RectTransform m_uiBlockLeft;
    [BoxGroup("UI Blocks")][SerializeField] private RectTransform m_uiBlockRight;
    [ResizableTextArea][BoxGroup("Prompts")][SerializeField] private string m_bottomBarTutorialKeys;
    [ResizableTextArea][BoxGroup("Prompts")][SerializeField] private string m_bottomBarLoseKeys;
    [ResizableTextArea][BoxGroup("Prompts")][SerializeField] private string m_bottomBarWinKeys;

    [ResizableTextArea][BoxGroup("Prompts")][SerializeField] private string m_topSideBarWinText;
	[ResizableTextArea][BoxGroup("Prompts")][SerializeField] private string m_topTimerBarText;
    [ResizableTextArea][BoxGroup("Prompts")][SerializeField] private string m_topSideBarLoseText;




    [BoxGroup("Effect Bar")][SerializeField] private EffectIcon m_effectIconPref;
    [BoxGroup("Effect Bar")][SerializeField] private RectTransform m_effectBar;





    [Button]
    private void EatNiceChild() {
        var child = new Child();
        child.SetChildType(Game.MainGameInfo.NiceChildType);
        OnChildEaten(Game.MainGameInfo.Krampus, child);
    }

    [Button]
    private void EatNaughtyChild() {
        var child = new Child();
        child.SetChildType(Game.MainGameInfo.RandomNaughtyChildType);
        OnChildEaten(Game.MainGameInfo.Krampus, child);
    }

    private void Awake() {
        m_originalTimerLocalScale = m_timer.localScale;
        m_startFill = m_fillBar.fillAmount;
    }

    private void OnChildEaten(Krampus krampus, Child child) {
        Color destinationColor;
        //Positive feedback
        if (child.IsNaughty) {
            destinationColor = m_niceTimerColor;
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
            destinationColor = m_naughtyTimerColor;
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
        ShowQuickActionIcon(false);
    }

    public void DisplayEffect(Krampus krampus, Effect effect) {
        EffectIcon effectIcon = Instantiate(m_effectIconPref);
        effectIcon.transform.SetParent(m_effectBar, false);
        if (effect.EffectType == Effect.Type.Permanent) {
            effectIcon.SetIcon(effect.Id, effect.ItemIcon, "test");
        } else {
            effectIcon.SetIcon(effect.Id, effect.ItemIcon, effect.Timer, "test2");
        }
        //Debug.Log("SHOWDISPLAY" + effect.Id + effect.Timer);
    }

    public void RemoveEffect(Krampus krampus, Effect effect) {

        for (int i = 0; i < m_effectBar.childCount; i++) {
            var effectIcon = m_effectBar.GetChild(i).GetComponent<EffectIcon>();
            if (effectIcon.EffectId == effect.Id) {
                Destroy(effectIcon.gameObject);
                break;
            }
        }

        //Debug.Log("HIDEDISPLAY" + effect.Id);
    }


    private void Update() {

        if (!Game.Balling) {
			m_timerDisplay.gameObject.SetActive(Mathf.RoundToInt(Time.unscaledTime * 2) % 2 == 0);
		} else {
			m_timerDisplay.gameObject.SetActive(true);
		}
        if (Game.MainGameInfo.CurrentState == MainGameInfo.State.ItemChoosing) {
            DisplayItemChoiceMenu();
        }

		//cursor


        m_timerDisplay.Value = Game.MainGameInfo.Timer.GameTime;
    }

    private void FixedUpdate() {
        Vector3 mousePos;
        mousePos = Mouse.current.position.ReadValue();
        m_quickActionIcon.transform.position = mousePos + new Vector3(-1f, 17f, 0f);
       
	}

	public void SetChildrenIcon(Sprite icon) {
		m_childIconImage.sprite = icon;
	}

    private void Ready() {
        Game.GlobalEvents.onChildEaten.AddListener(OnChildEaten);
        Game.MainGameInfo.Krampus.KrampusEvents.onEffectRegistered.AddListener(DisplayEffect);
        Game.MainGameInfo.Krampus.KrampusEvents.onEffectUnregistered.AddListener(RemoveEffect);
        Game.GlobalEvents.onLevelStateChanged.AddListener(OnLevelStateChanged);

		SetSeed(Game.RoomGenInfo.Seed);


        m_originalTimerColor = m_timerDisplay.Color;
        m_tutorial.gameObject.SetActive(false);
        m_uiBlockLeft.gameObject.SetActive(false);
        m_uiBlockRight.gameObject.SetActive(false);
        UpdateInventory();
    }

    private void OnLevelStateChanged(MainGameInfo.State previous, MainGameInfo.State next) {
        //Debug.Log("PREVIOUS:" + previous + "NEXT:" + next);
        if (next == MainGameInfo.State.Game &&
            (previous == MainGameInfo.State.ItemChoosing ||
             previous == MainGameInfo.State.Intro ||
              previous == MainGameInfo.State.WaitingToStart)
            ) {
            UIElementsEntryAnimation();
        }

        if (next == MainGameInfo.State.WaitingToStart) {
            if (Game.PogMan.GetCurrentLevelStats().Tutorials == 0) {
                //Debug.Log("ZERO");
                Game.MainGameInfo.SetState(MainGameInfo.State.ItemChoosing);
            } else {
                Game.GlobalEvents.onTutorialTrigger.Invoke(Game.PogMan.GetCurrentLevelStats().Tutorials);
            }
        }

    }

    public void ChangeChildCounter() {
        m_currentFillHandle.TryCancel();
        float oldValue = m_fillBar.fillAmount;
        m_currentFillHandle = LMotion.Create(oldValue, math.remap(0, 1, m_startFill, 0.95f, (float)(Game.MainGameInfo.NaughtyChildrenCountOnStart - Game.MainGameInfo.NaughtyChildren.Count()) /
                                 Game.MainGameInfo.NaughtyChildrenCountOnStart), 2f).WithDelay(0.4f).WithEase(m_fillUpCurve).BindToFillAmount(m_fillBar);
    }

    public void ChangeWindUpValue(float value, float time = 1f) {
        m_currentWindUpFillHandle.TryCancel();
        float oldValue = m_windUpFiller.fillAmount;
        m_currentWindUpFillHandle = LMotion.Create(oldValue, math.remap(0f,1f, 0.07f, 0.41f, value / Game.MainGameInfo.MaxWindUpPoints), time)
        .WithDelay(0.4f).BindToFillAmount(m_windUpFiller);
    }
    public void HideBlackBars() {
        m_blackBars.Hide();
    }


    public void DisplayItemChoiceMenu() {
        if (m_itemChoiceMenuUI.isActiveAndEnabled == true) return;
        m_itemChoiceMenuUI.gameObject.SetActive(true);
    }

    public void UIElementsEntryAnimation() {
        if (m_uiOn) return;
        //Debug.Log("Entry");
		m_uiBlockLeft.gameObject.SetActive(true);
        LMotion.Create(-200, 50, 0.375f).Bind(x => m_uiBlockLeft.anchoredPosition = new Vector2(x, m_uiBlockLeft.anchoredPosition.y));
        m_uiBlockRight.gameObject.SetActive(true);
        LMotion.Create(300, -50, 0.375f).Bind(x => m_uiBlockRight.anchoredPosition = new Vector2(x, m_uiBlockRight.anchoredPosition.y));
        m_uiOn = true;
    }

    public IEnumerator ProcessEnding(Ending ending) {
        m_blackBars.Show();
        m_blackBars.SetTopBarText("");
		m_blackBars.SetTopTimerText(m_topTimerBarText);
        if (ending.IsWin()) {
            if (Game.PogMan.IsThereNextLevel) {
                m_blackBars.SetBottomBarText(m_bottomBarWinKeys);
            } else {
                m_blackBars.SetBottomBarText(m_bottomBarLoseKeys);
            }
            m_blackBars.AnimateResultText(true, m_topSideBarWinText);
        } else {
            m_blackBars.SetBottomBarText(m_bottomBarLoseKeys);
            m_blackBars.SetTopBarSideText(m_topSideBarLoseText);
             m_blackBars.AnimateResultText(false,m_topSideBarWinText);
        }
        m_endScreenHandler.PreActivate(ending);
        yield return new WaitForSeconds(3);
        m_endScreenHandler.Activate(ending);
    }

    public void PopupTimer() {
        if (m_currentBounce.IsActive()) m_currentBounce.Cancel();
        var oldScale = m_originalTimerLocalScale;
        m_currentBounce = LMotion.Create(oldScale, oldScale * 1.3f, 0.2f).WithEase(Ease.OutElastic).WithOnComplete(
                () => LMotion.Create(oldScale * 1.3f, oldScale, 0.2f).WithEase(Ease.OutBounce).BindToLocalScale(m_timer)
            ).BindToLocalScale(m_timer);
    }

    public void SetWindUpCostBar(float cost) {
        m_windUpCostBar.rotation =  Quaternion.Euler(0,0,Mathf.Lerp(m_markerRotatorEndPoints.x,m_markerRotatorEndPoints.y,cost / Game.MainGameInfo.MaxWindUpPoints));
    }

	public void ShowQuickActionIcon(bool canDash) {
		Game.MainGameInfo.UI.QuickActionIcon.gameObject.SetActive(canDash);
		if(InputSubscribe.ShouldCursorBeVisible) Cursor.visible = !canDash;
    }

    public void UpdateInventory() {

        foreach (Transform child in m_inventoryContainer) {
            Destroy(child.gameObject);
        }
        var items = Game.MainGameInfo.Krampus.Stats.Items;

        foreach (var i in items) {

            InventoryCard ic = Instantiate(m_inventoryCardPref);
            ic.SetInfo(i.ItemIcon);
            ic.transform.SetParent(m_inventoryContainer);
         }


     }
}
