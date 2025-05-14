using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class PostProcessDistance : MonoBehaviour {
    private Volume m_vol;
    private Vignette m_vignette;
    private ChromaticAberration m_aberration;

    [SerializeField] private float m_minDistance;

    [SerializeField] private float m_abberIntensity = 0.2f;
    [SerializeField] private float m_vignetteIntensity = 0.2f;

    [SerializeField] private float m_maxVignette = 0.4f;
    [SerializeField] private float m_maxChroma = 1f;

    [SerializeField] private float m_resetSpeed = 1f;

    private float m_distanceToClosest;

    private void Start() {

		    bool b  = Game.SetMan.GetValue<bool>("Motion Blur");
		    MotionBlur blur;
        m_vol = GetComponent<Volume>();
        if (m_vol != null && m_vol.profile != null) {
            // Try to get Vignette effect
            if (!m_vol.profile.TryGet(out m_vignette))
                Debug.LogError("Vignette effect not found in Volume profile.");

            // Try to get Chromatic Aberration effect
            if (!m_vol.profile.TryGet(out m_aberration))
                Debug.LogError("Chromatic Aberration effect not found in Volume profile.");

            if (!m_vol.profile.TryGet(out blur))
	            Debug.LogError("Chromatic Aberration effect not found in Volume profile.");
            else {
	            blur.active = b;
            }


        } else {
            Debug.LogError("Volume or Volume profile is missing.");
        }
        ChangeAberration(m_abberIntensity);
        ChangeVignette(m_vignetteIntensity);
    }

    private void Update() {
        m_distanceToClosest = Mathf.Sqrt(Game.MainGameInfo.Krampus.ChildSensor.Dist);
        if (m_distanceToClosest < m_minDistance) {
            if ((m_minDistance - m_distanceToClosest) / m_minDistance <= m_vignetteIntensity) {
                //Debug.Log("1");
                ChangeVignette(m_vignetteIntensity);
            } else if ((m_minDistance - m_distanceToClosest) / m_minDistance > m_vignetteIntensity && (m_minDistance - m_distanceToClosest) / m_minDistance < m_maxVignette) {
                //Debug.Log("2");
                ChangeVignette((m_minDistance - m_distanceToClosest) / m_minDistance);
            } else {
                //Debug.Log("3");
                ChangeVignette(m_maxVignette);
            }

            if ((m_minDistance - m_distanceToClosest) / m_minDistance <= m_abberIntensity) {
                //Debug.Log("1");
                ChangeAberration(m_vignetteIntensity);
            } else if ((m_minDistance - m_distanceToClosest) / m_minDistance > m_abberIntensity && (m_minDistance - m_distanceToClosest) / m_minDistance < m_maxChroma) {
                //Debug.Log("2");
                ChangeAberration((m_minDistance - m_distanceToClosest) / m_minDistance);
            } else {
                //Debug.Log("3");
                ChangeAberration(m_maxChroma);
            }

        } else {
            ResetVignetteToDefault(m_vignetteIntensity);
            ResetAberrToDefault(m_abberIntensity);
        }
    }

    public void ChangeVignette(float intensity) {
        if (m_vignette != null) {
            m_vignette.intensity.value = intensity;
        } else {
            Debug.Log("Vignette is null.");
        }
    }

    public void ChangeAberration(float intensity) {
        if (m_aberration != null) {
            m_aberration.intensity.value = intensity;
        } else {
            Debug.Log("Abber is null.");
        }
    }

    public void ResetVignetteToDefault(float intensity) {
        if (m_vignette != null) {
            if (m_vignette.intensity.value > intensity) {
                m_vignette.intensity.value -= Time.deltaTime * m_resetSpeed;
            }
        } else {
            Debug.Log("Vignette is null.");
        }
    }
    public void ResetAberrToDefault(float intensity) {
        if (m_aberration != null) {
            if (m_aberration.intensity.value > intensity) {
                m_aberration.intensity.value -= Time.deltaTime * m_resetSpeed;
            }
        } else {
            Debug.Log("Abber is null.");
        }
    }
}
