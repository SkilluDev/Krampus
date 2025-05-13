using System;
using System.Linq;
using System.Text.RegularExpressions;
using LitMotion;
using LitMotion.Extensions;
using NaughtyAttributes;
using UnityEngine;

public class MenuCredits : MonoBehaviour {

    [SerializeField] private float m_fadeDuration = 0.5f;
    [SerializeField] private float m_defaultInterval = 2f;
    [SerializeField] private AudioSource m_audio;
    private CanvasGroup[] m_groups;
    private MotionHandle m_handle;

    private void Awake() {
        m_groups = GetComponentsInChildren<CanvasGroup>().OrderBy(w => w.transform.GetSiblingIndex()).ToArray();
        Stop();
        //m_groups[0].alpha = 1;
    }

    private void Ready() {
        Game.MainMenuInfo.onStateChanged += StateChanged;
    }

    private void Unready() {
        Game.MainMenuInfo.onStateChanged -= StateChanged;
    }

    private void Stop() {
        m_audio.Stop();
        if (m_handle.IsActive()) m_handle.Cancel();
        foreach (var group in m_groups.Skip(1)) {
            group.gameObject.SetActive(true);
            group.alpha = 0;
        }
        m_groups[0].alpha = 1;
    }

    private void StateChanged(MainMenuInfo.State previous, MainMenuInfo.State current) {
        if (current == MainMenuInfo.State.Credits) {
            PlaySequence();
        } else {
            Stop();
            m_handle = LMotion.Create(m_groups[0].alpha, 1f, m_fadeDuration).BindToAlpha(m_groups[0]);
        }
    }

    private static bool ExtractSeconds(string input, out float seconds) {
        var match = Regex.Match(input, @"\(([\d.]+)s\)");
        if (match.Success && float.TryParse(match.Groups[1].Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float result)) {
            seconds = result;
            return true;
        }
        seconds = float.NaN;
        return false;
    }

    [Button("Play")]
    private void PlaySequence() {
        Stop();
        m_audio.Play();
        var lSequence = LSequence.Create();
        lSequence.AppendInterval(m_defaultInterval);
        lSequence.Append(LMotion.Create(1f, 0f, m_fadeDuration).BindToAlpha(m_groups[0]));

        foreach (var w in m_groups.Skip(1)) {
            lSequence.Append(LMotion.Create(0f, 1f, m_fadeDuration).BindToAlpha(w));
            if (ExtractSeconds(w.gameObject.name, out float secs))
                lSequence.AppendInterval(secs);
            else
                lSequence.AppendInterval(m_defaultInterval);
            lSequence.Append(LMotion.Create(1f, 0f, m_fadeDuration).BindToAlpha(w));
        }
        lSequence.Append(LMotion.Create(0f, 1f, m_fadeDuration).BindToAlpha(m_groups[0]));
        Stop();
        m_handle = lSequence.Run();
    }
}
