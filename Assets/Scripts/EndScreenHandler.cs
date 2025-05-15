using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitMotion;
using LitMotion.Extensions;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
public enum Ending {
    Win,
    LoseNun,
    LoseTime
}
public class EndScreenHandler : MonoBehaviour {

    [SerializeField] private float m_fadeDuration = 0.5f;
    [SerializeField] private float m_beginSpeed = 1f;
    [BoxGroup("Ending textures")][SerializeField] private GameObject m_wonTextures;
    [BoxGroup("Ending textures")][SerializeField] private GameObject m_lostTextures;

    [BoxGroup("Ending texts")][SerializeField] private TextMeshProUGUI m_endingText;
    [BoxGroup("Ending texts")][SerializeField] private string m_winText;
    [BoxGroup("Ending texts")][SerializeField] private string m_loseNunText;
    [BoxGroup("Ending texts")][SerializeField] private string m_loseTimeText;
    private CanvasGroup[] m_groupsWon;
    private CanvasGroup[] m_groupsLost;
    private MotionHandle m_handle;

    private void Awake() {
        m_groupsWon = m_wonTextures.GetComponentsInChildren<CanvasGroup>().OrderBy(w => w.transform.GetSiblingIndex()).ToArray();
        m_groupsLost = m_lostTextures.GetComponentsInChildren<CanvasGroup>().OrderBy(w => w.transform.GetSiblingIndex()).ToArray();
    }

    private void Stop(CanvasGroup[] groups) {
        if (m_handle.IsActive()) m_handle.Cancel();
        foreach (var group in groups.Skip(2)) {
            group.gameObject.SetActive(true);
            group.alpha = 0;
        }
        groups[0].alpha = 1;
        groups[1].alpha = 1;
    }

    private void OnDisable() {
        m_handle.Cancel();
    }

    private void PlaySequence(CanvasGroup[] groups) {
        Stop(groups);
        var lSequence = LSequence.Create();
        lSequence.AppendInterval(m_beginSpeed);

        foreach (var w in groups.Skip(2)) {
            lSequence.Append(LMotion.Create(0f, 1f, m_fadeDuration).BindToAlpha(w));
        }
        Stop(groups);
        m_handle = lSequence.Run();
    }

    public void Activate(Ending ending) {
        gameObject.SetActive(true);
        switch (ending) {
            case Ending.Win:
                PlaySequence(m_groupsWon);
                m_lostTextures.SetActive(false);
                m_endingText.SetText(m_winText);
                break;
            case Ending.LoseNun:
                PlaySequence(m_groupsLost);
                m_endingText.SetText(m_loseNunText);
                break;
            case Ending.LoseTime:
                PlaySequence(m_groupsLost);
                m_endingText.SetText(m_loseTimeText);
                break;
        }
    }
}
