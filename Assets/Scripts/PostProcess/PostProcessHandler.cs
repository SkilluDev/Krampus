using LitMotion;
using SaintsField;
using SaintsField.Playa;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class PostProcessHandler : MonoBehaviour {

    private Vignette m_vignette;
    private ChromaticAberration m_aberration;
    private MotionBlur m_blur;

    [SerializeField] private Volume m_vol;

    [SerializeField] private float m_minDistance;

    [SerializeField] private float m_minAberrationIntensity = 0.2f;
    [SerializeField] private float m_minVignetteIntensity = 0.2f;

    [SerializeField] private float m_maxVignetteIntensity = 0.4f;
    [SerializeField] private float m_maxAberrationIntensity = 1f;

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

        SetBlur();
        Game.GlobalEvents.onSetManChange.AddListener(OnSetManChange);

        m_vignette.color.Override(m_originalVignetteColor);

        m_originalMaxVignetteIntensity = m_maxVignetteIntensity;
        m_originalMinVignetteIntensity = m_minVignetteIntensity;

        Game.GlobalEvents.onChildEaten.AddListener(OnChildEaten);
    }

    private void Unready() {
        Game.GlobalEvents.onSetManChange.RemoveListener(OnSetManChange);
    }

    private void SetBlur() {
        bool b = Game.SetMan.GetValue<bool>("Motion Blur");
        m_blur.active = b;
    }


    private void OnSetManChange(string key) {
        if (key == "Motion Blur") {
            SetBlur();
        }
    }

    [Button]
    private void EatNiceChild() {
        var child = new Child();
        child.SetChildType(Game.MainGameInfo.NiceChildType);
        OnChildEaten(Game.MainGameInfo.Krampus, child);
    }
    private void OnChildEaten(Krampus krampus, Child child) {
        if (child.IsNaughty) return;

        /* //Aberr
        LMotion.Create(m_originalMinAberrationIntensity, m_originalMinAberrationIntensity+m_aberrationFlashIntensity, m_aberrationFlashDuration/2).WithEase(Ease.OutCubic).WithOnComplete(
            ()=>LMotion.Create(m_originalMinAberrationIntensity+m_aberrationFlashIntensity, m_originalMinAberrationIntensity, m_aberrationFlashDuration/2).WithEase(Ease.OutCubic).Bind(i=>m_minAberrationIntensity=i)
        ).Bind(i=>m_minAberrationIntensity=i);

        LMotion.Create(m_originalMaxAberrationIntensity, m_originalMaxAberrationIntensity+m_aberrationFlashIntensity, m_aberrationFlashDuration/2).WithEase(Ease.OutCubic).WithOnComplete(
            ()=>LMotion.Create(m_originalMaxAberrationIntensity+m_aberrationFlashIntensity, m_originalMaxAberrationIntensity, m_aberrationFlashDuration/2).WithEase(Ease.OutCubic).Bind(i=>m_maxAberrationIntensity=i)
        ).Bind(i=>m_maxAberrationIntensity=i);
        //EndAberr */

        //Vignette
        LMotion.Create(m_originalVignetteColor, m_flashVignetteColor, m_vignetteFlashDuration / 2).WithEase(Ease.OutCubic).WithOnComplete(
            () => LMotion.Create(m_flashVignetteColor, m_originalVignetteColor, m_vignetteFlashDuration / 2).WithEase(Ease.OutCubic).Bind(c => m_vignette.color.Override(c))
        ).Bind(c => m_vignette.color.Override(c));

        LMotion.Create(m_originalMinVignetteIntensity, m_originalMinVignetteIntensity + m_vignetteFlashIntensity, m_vignetteFlashDuration / 2).WithEase(Ease.OutCubic).WithOnComplete(
            () => LMotion.Create(m_originalMinVignetteIntensity + m_vignetteFlashIntensity, m_originalMinVignetteIntensity, m_vignetteFlashDuration / 2).WithEase(Ease.OutCubic).Bind(i => m_minVignetteIntensity = i)
        ).Bind(i => m_minVignetteIntensity = i);

        LMotion.Create(m_originalMaxVignetteIntensity, m_originalMaxVignetteIntensity + m_vignetteFlashIntensity, m_vignetteFlashDuration / 2).WithEase(Ease.OutCubic).WithOnComplete(
            () => LMotion.Create(m_originalMaxVignetteIntensity + m_vignetteFlashIntensity, m_originalMaxVignetteIntensity, m_vignetteFlashDuration / 2).WithEase(Ease.OutCubic).Bind(i => m_maxVignetteIntensity = i)
        ).Bind(i => m_maxVignetteIntensity = i);
        //EndVignette
    }

    private void Update() {
        if (Game.IsLoading) return;
        m_distanceToClosest = Game.MainGameInfo.Krampus.ChildSensor.Dist;
        float deltaDistance = Mathf.Clamp01((m_minDistance - m_distanceToClosest) / m_minDistance);

        m_vignette.intensity.value = math.remap(0, 1, m_minVignetteIntensity, m_maxVignetteIntensity, deltaDistance);
        m_aberration.intensity.value = math.remap(0, 1, m_minAberrationIntensity, m_maxAberrationIntensity, deltaDistance);

    }
}
