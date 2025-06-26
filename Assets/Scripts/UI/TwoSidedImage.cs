using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using LitMotion;
using LitMotion.Extensions;
using NaughtyAttributes;
using Sound;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TwoSidedImage : MonoBehaviour {
	[SerializeField] private Sprite m_frontSprite;
	public Sprite FrontSprite { get => m_frontSprite; set => m_frontSprite = value; }

	[SerializeField] private Sprite m_backSprite;
	public Sprite BackSprite { get => m_backSprite; set => m_backSprite = value; }

	[SerializeField] private Image m_image;
	private bool m_isFlipped = false;

	[SerializeField] private float m_flipDuration = 1f;
	public float FlipDuration { get => m_flipDuration; set => m_flipDuration = value; }

	[SerializeField] private float m_delay = 0f;
	public float Delay { get => m_delay; set => m_delay = value; }

	public (Sex sound, float vol) FlipSound { get; set; }

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
		if (m_handle != null && m_handle.IsPlaying()) {
			m_handle.Cancel();
		}
		m_handle = LSequence.Create()
		.Append(LMotion.Create(0, 0, m_delay!=0?m_delay:0.01f).WithOnComplete(() => FlipSound.sound.Play(transform.position, FlipSound.vol)).RunWithoutBinding())
		.Append(LMotion.Create(m_image.transform.localRotation, m_image.transform.localRotation * Quaternion.Euler(new Vector3(0f, 90f, 0f)), m_flipDuration / 2).WithEase(Ease.InCubic).WithImmediateBind(false).WithOnComplete(SwitchSprites).BindToRotation(m_image.transform))
		.Join(LMotion.Create(m_image.transform.localScale, m_image.transform.localScale * m_scaleBounceFactor, m_flipDuration / 2).WithEase(Ease.OutCubic).WithImmediateBind(false).Bind(x => m_image.transform.localScale = x))
		.Append(LMotion.Create(m_image.transform.localRotation * Quaternion.Euler(new Vector3(0f, 90f, 0f)), m_image.transform.localRotation * Quaternion.Euler(new Vector3(0f, 180f, 0f)), m_flipDuration / 2).WithEase(Ease.OutCubic).WithImmediateBind(false).BindToRotation(m_image.transform))
		.Join(LMotion.Create(m_image.transform.localScale * m_scaleBounceFactor, m_image.transform.localScale, m_flipDuration / 2).WithEase(Ease.OutCubic).WithImmediateBind(false).Bind(x => m_image.transform.localScale = x))
		.Append(LMotion.Create(0f, 0f, 0.01f).WithOnComplete(onComplete).RunWithoutBinding()) //for some reason 0 didn't trigger complete
		.Run().AddTo(this);

	}

	public static void FlipImagesSequential(Queue<TwoSidedImage> images, Action onLastComplete = null) {
		if (images.Count() == 0) {
			if (onLastComplete != null) {
				onLastComplete.Invoke();
			}
			return;
		}
		if (images.Count() == 1) {
			images.Dequeue().FlipImage(onLastComplete);
		} else {
			images.Dequeue().FlipImage(() => FlipImagesSequential(images, onLastComplete));
		}
	}

	public static void FlipImagesSimultaneous(Queue<TwoSidedImage> images, float timer = 0f, Action onLastComplete = null) {
		if (images.Count() == 0) {
			if (onLastComplete != null) {
				onLastComplete.Invoke();
			}
			return;
		}
		while (images.Count() > 0) {
			var image = images.Dequeue();
			image.Delay = timer;
			if (images.Count() == 0) {
				image.FlipImage(onLastComplete);
			} else {
				image.FlipImage();
			}
		}
	}

	public static void FlipImagesSpaced(Queue<TwoSidedImage> images, float timeToStart = 0f, float timeBetween = 0f, Action onLastComplete = null) {
		if (images.Count() == 0) {
			if (onLastComplete != null) {
				onLastComplete.Invoke();
			}
			return;
		}

		if (images.Count() == 1) {
			var image = images.Dequeue();
			image.Delay = timeToStart;
			image.FlipImage();
			return;
		}
		images.ElementAt(0).Delay = timeToStart;
		for (int i = 1; i < images.Count(); i++) {
			images.ElementAt(i).Delay = timeToStart + (timeBetween * i);
		}

		while (images.Count() > 0) {
			if (images.Count() == 1) {
				images.Dequeue().FlipImage(onLastComplete);
			} else {
				images.Dequeue().FlipImage();
			}
		}
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
