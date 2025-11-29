using System.Collections;
using System.Linq;
using LitMotion;
using LitMotion.Extensions;
using NaughtyAttributes;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class RoundUIManager : NewUIManager
{
	
   	public NumericDisplay TimerDisplay => m_timerDisplay;

	[SerializeField] private TextMeshProUGUI m_currentSeed;

    [Header("Timer")]
    [SerializeField] private Transform m_timer;
	[SerializeField] private NumericDisplay m_timerDisplay;
	[SerializeField] private float m_timerShakeDuration;
	[SerializeField] private float m_timerShakeIntensity;
    [SerializeField] private float m_timerBounceIntensity;
    private MotionHandle m_currentBounce;
    private Vector3 m_originalTimerLocalScale;
    private Color m_originalTimerColor;

	



//-------
    [Header("Fill Bar")]
	[SerializeField] private Image m_childIconImage;
	[SerializeField] private Image m_fillBar;
	[SerializeField] private AnimationCurve m_fillUpCurve;

    [SerializeField] private Color m_niceTimerColor;
	[SerializeField] private Color m_naughtyTimerColor;

    private float m_startFill;
	private MotionHandle m_currentFillHandle;

    private void Awake() {
		m_originalTimerLocalScale = m_timer.localScale;
		m_startFill = m_fillBar.fillAmount;
	}
    protected override void Ready() {
            base.Ready();
            Game.GlobalEvents.onChildEaten.AddListener(OnChildEaten);
            SetSeed(Game.RoomGenInfo.Seed);
            m_originalTimerColor = m_timerDisplay.Color;
    }

    [Button]
	private void EatNiceChild() {
		var child = new Child();
		child.SetChildType(Game.roundInfo.NiceChildType);
		OnChildEaten(Game.MainGameInfo.Krampus, child);
	}

	[Button]
	private void EatNaughtyChild() {
		var child = new Child();
		child.SetChildType(Game.roundInfo.RandomNaughtyChildType);
		OnChildEaten(Game.MainGameInfo.Krampus, child);
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
			() => LMotion.Create(destinationColor, m_originalTimerColor, 1.2f).WithEase(Ease.InOutCubic).Bind(m_timerDisplay.SetColor).AddTo(this)
				).Bind(m_timerDisplay.SetColor).AddTo(this);

	}
    public void SetSeed(int seed) {
		m_currentSeed.text = $"Map seed: {seed:0000000}<br>";
	}
    	private void Update() {

		if (!Game.Balling) {
			m_timerDisplay.gameObject.SetActive(Mathf.RoundToInt(Time.unscaledTime * 2) % 2 == 0);
		} else {
			m_timerDisplay.gameObject.SetActive(true);
		}
		if (Game.MainGameInfo.CurrentState == RoundInfo.State.ItemChoosing) {
			DisplayItemChoiceMenu();
		}

		//cursor

		m_timerDisplay.Value = Game.roundInfo.Timer.GameTime;
	}
    public void SetChildrenIcon(Sprite icon) {
		m_childIconImage.sprite = icon;
	}
    public void ChangeChildCounter() {
		m_currentFillHandle.TryCancel();
		float oldValue = m_fillBar.fillAmount;
		m_currentFillHandle = LMotion.Create(oldValue, math.remap(0, 1, m_startFill, 0.95f, (float)(Game.roundInfo.NaughtyChildrenCountOnStart - Game.roundInfo.NaughtyChildren.Count()) /
								 Game.roundInfo.NaughtyChildrenCountOnStart), 2f).WithDelay(0.4f).WithEase(m_fillUpCurve).BindToFillAmount(m_fillBar).AddTo(this);
	}

    public void PopupTimer() {
		if (m_currentBounce.IsActive()) m_currentBounce.Cancel();
		var oldScale = m_originalTimerLocalScale;
		m_currentBounce = LMotion.Create(oldScale, oldScale * 1.3f, 0.2f).WithEase(Ease.OutElastic).WithOnComplete(
				() => LMotion.Create(oldScale * 1.3f, oldScale, 0.2f).WithEase(Ease.OutBounce).BindToLocalScale(m_timer)
			).BindToLocalScale(m_timer);
	}
    public void NewSeed() {
		Game.PogMan.LoadFirstLevel(true);
	}

	public void SameSeed() {
		Game.PogMan.LoadFirstLevel(false);
	 }

     public IEnumerator ProcessEnding(Ending ending) {
		m_blackBars.Show();
		m_blackBars.SetTopBarText("");
		m_blackBars.SetTopTimerText(m_topTimerBarText);
		if (ending.IsWin()) {
			m_blackBars.SetBottomBarText(m_topSideBarWinText);
			m_blackBars.AnimateResultText(true);    
		} else {
			m_blackBars.SetBottomBarText(m_bottomBarLoseKeys);
			m_blackBars.ShowBottomBarText(false);

			m_blackBars.AnimateResultText(false);
		}
		m_endScreenHandler.PreActivate(ending);
		yield return new WaitForSeconds(3);
		m_endScreenHandler.Activate(ending);
	}


}
