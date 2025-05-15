using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class PostProcessDistance : MonoBehaviour {
    
    private Vignette m_vignette;
    private ChromaticAberration m_aberration;
    private MotionBlur m_blur;

    [SerializeField] private Volume m_vol;

    [SerializeField] private float m_minDistance;

    [SerializeField] private float m_minAbberIntensity = 0.2f;
    [SerializeField] private float m_minVignetteIntensity = 0.2f;

    [SerializeField] private float m_maxVignetteIntensity = 0.4f;
    [SerializeField] private float m_maxAbberIntensity = 1f;

    [SerializeField] private float m_resetSpeed = 1f;

    [SerializeField] private Color m_flashColor;

    private float m_distanceToClosest;

    private void Ready() {
        m_vol.profile.TryGet(out m_vignette);
        m_vol.profile.TryGet(out m_aberration);
        m_vol.profile.TryGet(out m_blur);

        bool b  = Game.SetMan.GetValue<bool>("Motion Blur");
        m_blur.active = b;
    }

    private void Update() {
        if(!Game.Balling) return;
        m_distanceToClosest = Game.MainGameInfo.Krampus.ChildSensor.Dist;
        float deltaDistance = Mathf.Clamp01((m_minDistance - m_distanceToClosest) / m_minDistance);
        /* if (m_distanceToClosest < m_minDistance) {
            float deltaDistance = m_minDistance - m_distanceToClosest;
            if (deltaDistance / m_minDistance <= m_vignetteIntensity) {
                ChangeVignette(m_vignetteIntensity);
            } else if (deltaDistance / m_minDistance > m_vignetteIntensity && deltaDistance / m_minDistance < m_maxVignette) {
                ChangeVignette(deltaDistance / m_minDistance);
            } else {
                ChangeVignette(m_maxVignette);
            }

            if (deltaDistance / m_minDistance <= m_abberIntensity) {
                ChangeAberration(m_vignetteIntensity);
            } else if (deltaDistance / m_minDistance > m_abberIntensity && deltaDistance / m_minDistance < m_maxChroma) {
                ChangeAberration(deltaDistance / m_minDistance);
            } else {
                ChangeAberration(m_maxChroma);
            }
        } else {
            ResetVignetteToDefault(m_vignetteIntensity);
            ResetAberrToDefault(m_abberIntensity);
        } */

        m_vignette.intensity.value = math.remap(0,1, m_minVignetteIntensity, m_maxVignetteIntensity, deltaDistance);
        m_aberration.intensity.value = math.remap(0,1, m_minAbberIntensity, m_maxAbberIntensity, deltaDistance);
        
    }
}
