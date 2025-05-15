using System;
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

        Game.MainGameInfo.GlobalEvents.onChildEaten += OnChildEaten;
    }

	private void OnChildEaten(ChildType arg0) => throw new NotImplementedException();

	private void Update() {
        if(Game.IsLoading) return;
        m_distanceToClosest = Game.MainGameInfo.Krampus.ChildSensor.Dist;
        float deltaDistance = Mathf.Clamp01((m_minDistance - m_distanceToClosest) / m_minDistance);

        m_vignette.intensity.value = math.remap(0,1, m_minVignetteIntensity, m_maxVignetteIntensity, deltaDistance);
        m_aberration.intensity.value = math.remap(0,1, m_minAbberIntensity, m_maxAbberIntensity, deltaDistance);
        
    }
}
