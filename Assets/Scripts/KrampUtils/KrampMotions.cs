using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

public class KrampMotions {
	public static void ShowHideAlpha(CanvasGroup toShow, CanvasGroup toHide, float duration, Ease ease = Ease.Linear, bool unscaledTime = false) {
		ShowAlpha(toShow, duration, ease, unscaledTime);
		HideAlpha(toHide, duration, ease, unscaledTime);
	}

	public static void ShowAlpha(CanvasGroup toShow, float duration, Ease ease = Ease.Linear, bool unscaledTime = false) {
		toShow.gameObject.SetActive(true);
		LMotion.Create(0f, 1f, duration).WithEase(ease).WithScheduler(unscaledTime?MotionScheduler.UpdateIgnoreTimeScale:MotionScheduler.Update).BindToAlpha(toShow);
	}

	public static void HideAlpha(CanvasGroup toHide, float duration, Ease ease = Ease.Linear, bool unscaledTime = false) {
		LMotion.Create(1f, 0f, duration).WithEase(ease).WithScheduler(unscaledTime?MotionScheduler.UpdateIgnoreTimeScale:MotionScheduler.Update).WithOnComplete(()=>toHide.gameObject.SetActive(false)).BindToAlpha(toHide);
	}

}
