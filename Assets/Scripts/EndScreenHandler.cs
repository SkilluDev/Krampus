using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

public class EndScreenHandler : MonoBehaviour
{

    [SerializeField] private float m_fadeDuration = 0.5f;
    [SerializeField] private float m_defaultInterval = 0.5f;
    private CanvasGroup[] m_groups;
    private MotionHandle m_handle;

    private void Awake() {
        m_groups = GetComponentsInChildren<CanvasGroup>().OrderBy(w => w.transform.GetSiblingIndex()).ToArray();
        Stop();
        //m_groups[0].alpha = 1;
    }

    private void Stop() {
        if (m_handle.IsActive()) m_handle.Cancel();
        foreach (var group in m_groups.Skip(1)) {
            group.gameObject.SetActive(true);
            group.alpha = 0;
        }
        m_groups[0].alpha = 1;
    }
    private void OnEnable() {
		PlaySequence();
	}

	private void OnDisable() {
        m_handle.Cancel();
	}

	private void PlaySequence() {
        Stop();
        var lSequence = LSequence.Create();
        lSequence.AppendInterval(m_defaultInterval);

        foreach (var w in m_groups.Skip(1)) {
            lSequence.Append(LMotion.Create(0f, 1f, m_fadeDuration).BindToAlpha(w));
        }
        Stop();
        m_handle = lSequence.Run();
    }
}
