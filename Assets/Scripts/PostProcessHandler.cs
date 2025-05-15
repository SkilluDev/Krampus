using System;
using System.Collections;
using System.Collections.Generic;
using LitMotion;
using LitMotion.Extensions;
using NaughtyAttributes;
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

    [SerializeField] private Color m_flashVignetteColor;

    [SerializeField] private Color m_originalVignetteColor;

    [SerializeField] private float m_vignetteFlashDuration;
    [SerializeField] private float m_vignetteFlashIntensity;

    private float m_originalMinVignetteIntensity;
    private float m_originalMaxVignetteIntensity;

    private float m_distanceToClosest;

    private void Ready() {
        m_vol.profile.TryGet(out m_vignette);
        m_vol.profile.TryGet(out m_aberration);
        m_vol.profile.TryGet(out m_blur);

        bool b  = Game.SetMan.GetValue<bool>("Motion Blur");
        m_blur.active = b;

        m_vignette.color.Override(m_originalVignetteColor);

        m_originalMaxVignetteIntensity = m_maxVignetteIntensity;
        m_originalMinVignetteIntensity = m_minVignetteIntensity;

        Game.MainGameInfo.GlobalEvents.onChildEaten += OnChildEaten;
    }

    [Button]
    private void EatGoodChild(){
        OnChildEaten(Game.MainGameInfo.GoodChildType);
    }
	private void OnChildEaten(ChildType childType) {
        if (childType != Game.MainGameInfo.GoodChildType) return;
        LMotion.Create(m_originalVignetteColor, m_flashVignetteColor, m_vignetteFlashDuration/2).WithEase(Ease.OutCubic).WithOnComplete(
            ()=>LMotion.Create(m_flashVignetteColor, m_originalVignetteColor, m_vignetteFlashDuration/2).WithEase(Ease.OutCubic).Bind(c=>m_vignette.color.Override(c))
        ).Bind(c=>m_vignette.color.Override(c));

        LMotion.Create(m_originalMinVignetteIntensity, m_originalMinVignetteIntensity+m_vignetteFlashIntensity, m_vignetteFlashDuration/2).WithEase(Ease.OutCubic).WithOnComplete(
            ()=>LMotion.Create(m_originalMinVignetteIntensity+m_vignetteFlashIntensity, m_originalMinVignetteIntensity, m_vignetteFlashDuration/2).WithEase(Ease.OutCubic).Bind(i=>m_minVignetteIntensity=i)
        ).Bind(i=>m_minVignetteIntensity=i);

        LMotion.Create(m_originalMaxVignetteIntensity, m_originalMaxVignetteIntensity+m_vignetteFlashIntensity, m_vignetteFlashDuration/2).WithEase(Ease.OutCubic).WithOnComplete(
            ()=>LMotion.Create(m_originalMaxVignetteIntensity+m_vignetteFlashIntensity, m_originalMaxVignetteIntensity, m_vignetteFlashDuration/2).WithEase(Ease.OutCubic).Bind(i=>m_maxVignetteIntensity=i)
        ).Bind(i=>m_maxVignetteIntensity=i);
    }

	private void Update() {
        if(Game.IsLoading) return;
        m_distanceToClosest = Game.MainGameInfo.Krampus.ChildSensor.Dist;
        float deltaDistance = Mathf.Clamp01((m_minDistance - m_distanceToClosest) / m_minDistance);

        m_vignette.intensity.value = math.remap(0,1, m_minVignetteIntensity, m_maxVignetteIntensity, deltaDistance);
        m_aberration.intensity.value = math.remap(0,1, m_minAbberIntensity, m_maxAbberIntensity, deltaDistance);
        
    }
}
