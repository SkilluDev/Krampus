using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class PostProcessDistance : MonoBehaviour {
    private Volume vol;
    private Vignette vignette;
    private ChromaticAberration aberration;

    [SerializeField] private float m_minDistance;

    [SerializeField] private float m_abberIntensity = 0.2f;
    [SerializeField] private float m_vignetteIntensity = 0.2f;

    [SerializeField] private float m_maxVignette = 0.4f;
    [SerializeField] private float m_maxChroma = 1f;

    [SerializeField] private float m_resetSpeed = 1f;

    private float m_distanceToClosest;

    private void Start() {
        vol = GetComponent<Volume>();
        if (vol != null && vol.profile != null) {
            // Try to get Vignette effect
            if (!vol.profile.TryGet(out vignette))
                Debug.LogError("Vignette effect not found in Volume profile.");

            // Try to get Chromatic Aberration effect
            if (!vol.profile.TryGet(out aberration))
                Debug.LogError("Chromatic Aberration effect not found in Volume profile.");
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
            resetVignetteToDefault(m_vignetteIntensity);
            resetAberrToDefault(m_abberIntensity);
        }
    }

    public void ChangeVignette(float intensity) {
        if (vignette != null) {
            vignette.intensity.value = intensity;
        } else {
            Debug.Log("Vignette is null.");
        }
    }

    public void ChangeAberration(float intensity) {
        if (aberration != null) {
            aberration.intensity.value = intensity;
        } else {
            Debug.Log("Abber is null.");
        }
    }

    public void resetVignetteToDefault(float intensity) {
        if (vignette != null) {
            if (vignette.intensity.value > intensity) {
                vignette.intensity.value -= Time.deltaTime * m_resetSpeed;
            }
        } else {
            Debug.Log("Vignette is null.");
        }
    }
    public void resetAberrToDefault(float intensity) {
        if (aberration != null) {
            if (aberration.intensity.value > intensity) {
                aberration.intensity.value -= Time.deltaTime * m_resetSpeed;
            }
        } else {
            Debug.Log("Abber is null.");
        }
    }
}
