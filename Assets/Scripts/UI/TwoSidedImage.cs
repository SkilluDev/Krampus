using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LitMotion;
using LitMotion.Extensions;
using NaughtyAttributes;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TwoSidedImage : MonoBehaviour {

	[SerializeField] private Sprite m_frontSprite;
	[SerializeField] private Sprite m_backSprite;

	[SerializeField] private Image m_image;

	private bool m_isFlipped = false;

	[SerializeField] private float m_flipDuration = 1f;

	[SerializeField] private float m_scaleBounceFactor = 1.5f;

	private MotionSequenceBuilder m_rotateSequence;

	private MotionHandle m_handle;

	private void Start() {
		m_image.sprite = m_frontSprite;


	}
	[Button("Flip Image")]
	public void FlipImage() {
		if(m_handle != null && m_handle.IsPlaying()) {
			return;
		}
		m_handle = LSequence.Create()
		.Append(LMotion.Create(m_image.transform.localRotation, m_image.transform.localRotation * Quaternion.Euler(new Vector3(0f, 90f, 0f)), m_flipDuration / 2).WithEase(Ease.InCubic).WithImmediateBind(false).WithOnComplete(SwitchSprites).BindToRotation(m_image.transform))
		.Join(LMotion.Create(m_image.transform.localScale, m_image.transform.localScale * m_scaleBounceFactor, m_flipDuration / 2).WithEase(Ease.OutCubic).WithImmediateBind(false).Bind(x => m_image.transform.localScale = x))
		.Append(LMotion.Create(m_image.transform.localRotation * Quaternion.Euler(new Vector3(0f, 90f, 0f)), m_image.transform.localRotation * Quaternion.Euler(new Vector3(0f, 180f, 0f)), m_flipDuration / 2).WithEase(Ease.OutCubic).WithImmediateBind(false).BindToRotation(m_image.transform))
		.Join(LMotion.Create(m_image.transform.localScale * m_scaleBounceFactor, m_image.transform.localScale, m_flipDuration/2).WithEase(Ease.OutCubic).WithImmediateBind(false).Bind(x=>m_image.transform.localScale = x))
		.Run();

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
