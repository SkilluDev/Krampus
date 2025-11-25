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


	[SerializeField] private GameObject m_pauseScreen;


	[Header("Inventory")]
	[BoxGroup("Inventory")][SerializeField] private RectTransform m_inventoryContainer;
	[BoxGroup("Inventory")][SerializeField] private InventoryCard m_inventoryCardPref;

	
	//[SerializeField] private float m_timerBounceDuration;
	


	[SerializeField] private WorldSpaceUI m_worldSpaceUI;
	public WorldSpaceUI WorldSpaceUI => m_worldSpaceUI;



	private MotionHandle m_currentWindUpFillHandle;

	
	

	public BlackBars BlackBars => m_blackBars;
	[SerializeField] protected BlackBars m_blackBars;

	[SerializeField] private ItemChoiceMenuUI m_itemChoiceMenuUI;
	public ItemChoiceMenuUI ItemChoiceMenu => m_itemChoiceMenuUI;

	

	private bool m_uiOn = false;


	[BoxGroup("Wind-up")][SerializeField] private Image m_windUpFiller;
	[BoxGroup("Wind-up")][SerializeField] private RectTransform m_windUpCostBar;
	[BoxGroup("Wind-up")][SerializeField] private Vector2 m_markerRotatorEndPoints;

	[BoxGroup("Tutorial")][SerializeField] protected TutorialHandler m_tutorial;

	[BoxGroup("End Screen")][SerializeField] protected EndScreenHandler m_endScreenHandler;


	[BoxGroup("UI Blocks")][SerializeField] protected RectTransform m_uiBlockLeft;
	[BoxGroup("UI Blocks")][SerializeField] protected RectTransform m_uiBlockRight;
	[ResizableTextArea][BoxGroup("Prompts")][SerializeField] protected string m_bottomBarTutorialKeys;
	[ResizableTextArea][BoxGroup("Prompts")][SerializeField] protected string m_bottomBarLoseKeys;
	[ResizableTextArea][BoxGroup("Prompts")][SerializeField]protected string m_bottomBarWinKeys;

	[ResizableTextArea][BoxGroup("Prompts")][SerializeField]protected string m_topSideBarWinText;
	[ResizableTextArea][BoxGroup("Prompts")][SerializeField] protected string m_topTimerBarText;
	[ResizableTextArea][BoxGroup("Prompts")][SerializeField] protected string m_topSideBarLoseText;




	[BoxGroup("Effect Bar")][SerializeField] private EffectBar m_effectBar;
	public EffectBar EffectBar => m_effectBar;

	private void Awake() {
	}

	public void SwitchPauseMenu(bool activate) {
		if (activate) m_pauseScreen.SetActive(true);
		else m_pauseScreen.SetActive(false);
	}

	private void Start() {
		m_blackBars.ShowInstant();
		m_blackBars.SetTopBarText("");
		m_blackBars.SetBottomBarText(m_bottomBarTutorialKeys);
	}


	protected virtual void Ready() {
		
		Game.MainGameInfo.Krampus.KrampusEvents.onItemActivated.AddListener(m_effectBar.ActivateItem);
		Game.MainGameInfo.Krampus.KrampusEvents.onItemDesactivated.AddListener(m_effectBar.DesactivateIcon);
		Game.GlobalEvents.onLevelStateChanged.AddListener(OnLevelStateChanged);
		
		m_tutorial.gameObject.SetActive(false);
		m_uiBlockLeft.gameObject.SetActive(false);
		m_uiBlockRight.gameObject.SetActive(false);
		UpdateInventory();
	}

	private void OnLevelStateChanged(RoundInfo.State previous, RoundInfo.State next) {
		if (next == RoundInfo.State.Game &&
			(previous == RoundInfo.State.ItemChoosing ||
			 previous == RoundInfo.State.Intro ||
			  previous == RoundInfo.State.WaitingToStart)
			) {
			UIElementsEntryAnimation();
		}

		if (next == RoundInfo.State.WaitingToStart) {
			if (Game.PogMan.GetCurrentLevelStats().Tutorials == 0) {
				Game.MainGameInfo.SetState(RoundInfo.State.ItemChoosing);
			} else {
				Game.GlobalEvents.onTutorialTrigger.Invoke(Game.PogMan.GetCurrentLevelStats().Tutorials);
			}
		}

	}



	public void ChangeWindUpValue(float value, float time = 1f) {
		m_currentWindUpFillHandle.TryCancel();
		float oldValue = m_windUpFiller.fillAmount;
		m_currentWindUpFillHandle = LMotion.Create(oldValue, math.remap(0f, 1f, 0.07f, 0.41f, value / Game.MainGameInfo.MaxWindUpPoints), time)
		.WithDelay(0.4f).BindToFillAmount(m_windUpFiller).AddTo(this);
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
		m_uiBlockLeft.gameObject.SetActive(true);
		float lx = m_uiBlockLeft.anchoredPosition.x;
		LMotion.Create(-200, lx, 0.375f).Bind(x => m_uiBlockLeft.anchoredPosition = new Vector2(x, m_uiBlockLeft.anchoredPosition.y)).AddTo(this);
		m_uiBlockRight.gameObject.SetActive(true);
		LMotion.Create(300, -50, 0.375f).Bind(x => m_uiBlockRight.anchoredPosition = new Vector2(x, m_uiBlockRight.anchoredPosition.y)).AddTo(this);
		m_uiOn = true;
	}

	
	

	public void SetWindUpCostBar(float cost) {
		m_windUpCostBar.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(m_markerRotatorEndPoints.x, m_markerRotatorEndPoints.y, cost / Game.MainGameInfo.MaxWindUpPoints));
	}

	public void UpdateInventory() {

		foreach (Transform child in m_inventoryContainer) {
			Destroy(child.gameObject);
		}
		var items = Game.MainGameInfo.Krampus.Stats.Items;

		foreach (var i in items) {

			InventoryCard ic = Instantiate(m_inventoryCardPref);
			ic.SetInfo(i);
			ic.transform.SetParent(m_inventoryContainer, false);
		}
	}


	public void GoBackToMenu() {
		Game.PogMan.GoBackToMenu();
	}

	


}
