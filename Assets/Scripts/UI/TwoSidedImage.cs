using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using LitMotion;
using LitMotion.Extensions;
using NaughtyAttributes;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TwoSidedImage : MonoBehaviour {
	[SerializeField] private Sprite m_frontSprite;

	public Sprite FrontSprite {get => m_frontSprite; set => m_frontSprite = value;}

	[SerializeField] private Sprite m_backSprite;

	public Sprite BackSprite {get => m_backSprite; set => m_backSprite = value;}

	[SerializeField] private Image m_image;

	private bool m_isFlipped = false;

	[SerializeField] private float m_flipDuration = 1f;
	public float FlipDuration { get => m_flipDuration; set => m_flipDuration = value; }

	[SerializeField] private float m_delay = 0f;

	public float Delay { get => m_delay; set => m_delay = value;}

	[SerializeField] private float m_scaleBounceFactor = 1.5f;

	private MotionHandle m_handle;


	private void Start() {
		SetToFrontSprite();
	}

	public void SetToFrontSprite() {
		m_image.sprite = m_frontSprite;
	}

	[Button("Flip Image")]
	public void FlipImage(Action onComplete = null) {
		if(m_handle != null && m_handle.IsPlaying()) {
			m_handle.Cancel()	;
		}
		m_handle = LSequence.Create()
		.Append(LMotion.Create(0,0,m_delay).RunWithoutBinding())
		.Append(LMotion.Create(m_image.transform.localRotation, m_image.transform.localRotation * Quaternion.Euler(new Vector3(0f, 90f, 0f)), m_flipDuration / 2).WithEase(Ease.InCubic).WithImmediateBind(false).WithOnComplete(SwitchSprites).BindToRotation(m_image.transform))
		.Join(LMotion.Create(m_image.transform.localScale, m_image.transform.localScale * m_scaleBounceFactor, m_flipDuration / 2).WithEase(Ease.OutCubic).WithImmediateBind(false).Bind(x => m_image.transform.localScale = x))
		.Append(LMotion.Create(m_image.transform.localRotation * Quaternion.Euler(new Vector3(0f, 90f, 0f)), m_image.transform.localRotation * Quaternion.Euler(new Vector3(0f, 180f, 0f)), m_flipDuration / 2).WithEase(Ease.OutCubic).WithImmediateBind(false).BindToRotation(m_image.transform))
		.Join(LMotion.Create(m_image.transform.localScale * m_scaleBounceFactor, m_image.transform.localScale, m_flipDuration / 2).WithEase(Ease.OutCubic).WithImmediateBind(false).Bind(x => m_image.transform.localScale = x))
		.Append(LMotion.Create(0f,0f,0.01f).WithOnComplete(onComplete).RunWithoutBinding()) //for some reason 0 didn't trigger complete
		.Run();
	}

	public static void FlipImages(Queue<TwoSidedImage> images) {
		if (images.Count() == 0) {
			return;
		}
		images.Dequeue().FlipImage(()=>FlipImages(images));

	}

	public void SwitchSprites() {
		m_isFlipped = !m_isFlipped;
		if (m_isFlipped) {
			m_image.sprite = m_backSprite;
		} else {
			m_image.sprite = m_frontSprite;
		}
	}
}
