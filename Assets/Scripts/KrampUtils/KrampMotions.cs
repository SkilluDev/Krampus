using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

public class KrampMotions {
	public static void ShowHideAlpha(CanvasGroup toShow, CanvasGroup toHide, float duration) {
        LMotion.Create(0f, 1f, duration).WithEase(Ease.InOutExpo).BindToAlpha(toShow);
        LMotion.Create(1f, 0f, duration).WithEase(Ease.InOutExpo).BindToAlpha(toHide);
	}
}
