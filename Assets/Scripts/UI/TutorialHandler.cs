using System;
using System.Collections;
using System.Collections.Generic;

using LitMotion;
using LitMotion.Extensions;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
[Flags]
public enum TutorialPage {
	WalkAndRun = 1,
	AttackNaughty = 2,
	Timer = 4,
	DontAttackNice = 8,
	AvoidNuns = 16,
	InteractAndStun = 32,
	WindupAndDash = 64,
	ItemsAndLockIn = 128
}
[Serializable]
public class TutorialScreen : ValueConnectedToEnum<TutorialPage> {
	[SerializeField] private GameObject m_page;
	public GameObject Page => m_page;
}


public class TutorialHandler : MonoBehaviour {
	//[SerializeField] private TutorialPage m_tutorialPagesChosen;

	[SerializeField] private SerializedEnumDictionary<TutorialPage, TutorialScreen> m_tutorialPagesDict;
	private IList<Transform> m_tutorialPages;
	private int m_tutorialCounter = 0;


	[SerializeField] private float m_transitionLength;
	[SerializeField] private float m_rotateAngle;
	[SerializeField] private float m_slideLength;
	[SerializeField] private RectTransform m_keybindPrompt;
	[SerializeField] private GameObject m_tutorialHolder;

	[SerializeField] private Image m_naughtyChildIcon;

	private float m_distanceBetween = 1f;

	private bool m_isMoving = false;

	private MotionHandle m_handle;

	[SerializeField] private int m_uiMoveInCounter = 3;

	private void PrepareTutorials(TutorialPage pages) {
		m_tutorialPages = new List<Transform>();
		foreach (TutorialPage page in Enum.GetValues(typeof(TutorialPage))) {
			if (pages.HasFlag(page)) {
				GameObject p = m_tutorialPagesDict[page].Page;
				p.SetActive(true);
				p.transform.SetAsFirstSibling();
				m_tutorialPages.Add(p.transform);
			}
		}
		for (int i = 0; i < m_tutorialPages.Count; i++) {
			m_tutorialPages[m_tutorialPages.Count - 1 - i].transform.localPosition += new Vector3(0, 0, m_distanceBetween * i);
		}
	}

	private void Start() {
		Game.GlobalEvents.onTutorialTrigger.AddListener(OnTutorialTrigger);
	}

	private void OnTutorialTrigger(TutorialPage pages) {
		Debug.Log($"Tutorial triggered with pages: {pages}");
		if (pages == 0) return;
		gameObject.SetActive(true);
		PrepareTutorials(pages);
	}
	private void Update() {
		//LMB to go forward RMB to skip
		if (InputSubscribe.Raw.UI.QuitTutorial.WasPerformedThisFrame() && Game.MainGameInfo.CurrentState == MainGameInfo.State.WaitingToStart) {
			Debug.Log("Should quit tutorial now");
			if (m_handle.IsActive()) m_handle.Cancel();
			gameObject.SetActive(false);
			Game.MainGameInfo.SetState(MainGameInfo.State.ItemChoosing);

		}
		if (InputSubscribe.Raw.UI.Advance.WasPerformedThisFrame() && !m_isMoving && Game.MainGameInfo.CurrentState == MainGameInfo.State.WaitingToStart) {
			MoveBack(m_tutorialCounter++ % m_tutorialPages.Count);
			//if (--m_uiMoveInCounter == 0) Game.MainGameInfo.UI.UIElementsEntryAnimation();
		}

	}

	private void MoveBack(int id) {

		m_isMoving = true;

		var page = m_tutorialPages[id].transform;
		var oldLocalPosition = page.localPosition;
		var oldLocalMainPosition = m_tutorialHolder.transform.localPosition;

		var oldPosition = page.position;
		var oldRotation = page.rotation;
		var lSequence = LSequence.Create();




		lSequence.Append(LMotion.Create(page.localRotation, page.localRotation * Quaternion.Euler(new Vector3(0, 0, m_rotateAngle)), m_transitionLength).WithEase(Ease.InOutCubic)
		.BindToLocalRotation(page));

		var currentLocalPagePosition = oldLocalPosition;
		var nextLocalPagePosition = currentLocalPagePosition + Vector3.right * m_slideLength * 2;
		// Debug.Log(currentLocalPagePosition+"->"+nextLocalPagePosition);

		lSequence.Join(LMotion.Create(currentLocalPagePosition, nextLocalPagePosition, m_transitionLength).WithEase(Ease.InOutCubic)
		.BindToLocalPosition(page));

		var currentLocalMainPosition = oldLocalMainPosition;
		var nextLocalMainPosition = currentLocalMainPosition - Vector3.right * m_slideLength;

		var keybindPromptLocalMainPosition = m_keybindPrompt.localPosition;
		var nextKeybindPromptLocalMainPosition = m_keybindPrompt.localPosition - Vector3.right * m_slideLength;

		lSequence.Join(LMotion.Create(currentLocalMainPosition, nextLocalMainPosition, m_transitionLength).WithEase(Ease.InOutCubic)
		.BindToLocalPosition(m_tutorialHolder.transform));

		lSequence.Join(LMotion.Create(keybindPromptLocalMainPosition, nextKeybindPromptLocalMainPosition, m_transitionLength).WithEase(Ease.InOutCubic)
	   .BindToLocalPosition(m_keybindPrompt));

		currentLocalPagePosition = nextLocalPagePosition;
		nextLocalPagePosition = currentLocalPagePosition - Vector3.forward * (m_tutorialPages.Count + 1) * m_distanceBetween;

		lSequence.Append(LMotion.Create(currentLocalPagePosition, nextLocalPagePosition, m_transitionLength).WithEase(Ease.InOutCubic).
		WithOnComplete(() => page.SetAsFirstSibling()).BindToLocalPosition(page));

		lSequence.Append(LMotion.Create(page.localRotation * Quaternion.Euler(new Vector3(0, 0, m_rotateAngle)), page.localRotation, m_transitionLength).WithEase(Ease.InOutCubic)
		.BindToLocalRotation(page));

		currentLocalPagePosition = nextLocalPagePosition;
		nextLocalPagePosition = currentLocalPagePosition - Vector3.right * m_slideLength * 2;

		lSequence.Join(LMotion.Create(currentLocalPagePosition, nextLocalPagePosition, m_transitionLength).WithEase(Ease.InOutCubic)
		.BindToLocalPosition(page));

		currentLocalMainPosition = nextLocalMainPosition;
		nextLocalMainPosition = currentLocalMainPosition + Vector3.right * m_slideLength;

		lSequence.Join(LMotion.Create(currentLocalMainPosition, nextLocalMainPosition, m_transitionLength).WithEase(Ease.InOutCubic)
		.BindToLocalPosition(m_tutorialHolder.transform));

		lSequence.Join(LMotion.Create(nextKeybindPromptLocalMainPosition, keybindPromptLocalMainPosition, m_transitionLength).WithEase(Ease.InOutCubic)
	   .BindToLocalPosition(m_keybindPrompt));

		currentLocalMainPosition = nextLocalMainPosition;
		nextLocalMainPosition = currentLocalMainPosition + Vector3.forward * m_distanceBetween;

		lSequence.Append(LMotion.Create(currentLocalMainPosition, nextLocalMainPosition, m_transitionLength).WithEase(Ease.InOutCubic)
		.WithOnComplete(() => m_isMoving = false).BindToLocalPosition(m_tutorialHolder.transform));

		m_keybindPrompt.localPosition = keybindPromptLocalMainPosition;

		m_tutorialPages[id].transform.position = oldPosition;
		m_tutorialPages[id].transform.localPosition = oldLocalPosition;

		m_tutorialPages[id].transform.rotation = oldRotation;
		m_tutorialHolder.transform.localPosition = oldLocalMainPosition;

		m_handle = lSequence.Run();
	}
}
