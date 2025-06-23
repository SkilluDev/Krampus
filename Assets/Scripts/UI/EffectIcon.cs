using System.Collections;
using System.Collections.Generic;
using LitMotion;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;
using LitMotion.Extensions;
using System;

public class EffectIcon : MonoBehaviour {

    [SerializeField] private Image m_icon;
    [SerializeField] private Image m_fillImage;
    private string m_effectId;
    public string EffectId => m_effectId;

    private string m_effectName;
    public string EffectName => m_effectName;

    [SerializeField] private TextMeshProUGUI m_stackText;

    private MotionHandle m_motionHandle;
    private MotionHandle m_currentBounce;

    private Vector3 oldScale = Vector3.one;
    private int m_stackableTemporaryEffects = 0;

    



    public void SetIcon(string id, Sprite icon = null, string name = "") {
        m_icon.sprite = icon;
        ChangeStackableText(m_stackableTemporaryEffects);
        m_effectName = name;
        m_effectId = id;
        m_fillImage.fillAmount = 0;
        oldScale = m_icon.rectTransform.localScale;
    }
    public void Activate(float duration) {

        m_stackableTemporaryEffects++;

        LMotion.Create(1f, 0f, duration).WithOnComplete(() => DesactivateTemp()).RunWithoutBinding();

        m_motionHandle.TryCancel();
        m_motionHandle = LMotion.Create(1f, 0, duration).BindToFillAmount(m_fillImage);

        m_currentBounce.TryCancel();
        m_currentBounce = LMotion.Create(oldScale, oldScale * 2f, 0.2f).WithEase(Ease.OutElastic).WithOnComplete(
               () => LMotion.Create(oldScale * 2f, oldScale, 0.2f).WithEase(Ease.OutBounce).BindToLocalScale(m_icon.rectTransform)
           ).BindToLocalScale(m_icon.rectTransform);

        ChangeStackableText(m_stackableTemporaryEffects);
    }
    public void Activate() {
        m_fillImage.fillAmount = 1;

        m_currentBounce.TryCancel();
        m_currentBounce = LMotion.Create(oldScale, oldScale * 2f, 0.2f).WithEase(Ease.OutElastic).WithOnComplete(
               () => LMotion.Create(oldScale * 2f, oldScale, 0.2f).WithEase(Ease.OutBounce).BindToLocalScale(m_icon.rectTransform)
           ).BindToLocalScale(m_icon.rectTransform);
    }

    public void Activate(int stacks) {
        m_currentBounce.TryCancel();
        m_currentBounce = LMotion.Create(oldScale, oldScale * 2f, 0.2f).WithEase(Ease.OutElastic).WithOnComplete(
               () => LMotion.Create(oldScale * 2f, oldScale, 0.2f).WithEase(Ease.OutBounce).BindToLocalScale(m_icon.rectTransform)
           ).BindToLocalScale(m_icon.rectTransform);
        m_stackText.text = stacks.ToString();
    }

    public void ActivatePuff() {

        m_currentBounce.TryCancel();
        m_currentBounce = LMotion.Create(oldScale, oldScale * 2f, 0.2f).WithEase(Ease.OutElastic).WithOnComplete(
               () => LMotion.Create(oldScale * 2f, oldScale, 0.2f).WithEase(Ease.OutBounce).BindToLocalScale(m_icon.rectTransform)
           ).BindToLocalScale(m_icon.rectTransform);

    }
    public void Desactivate() {
        m_fillImage.fillAmount = 0;
    }

    public void DesactivateTemp() {

        m_stackableTemporaryEffects--;
        Debug.Log("Ojej: " + m_stackableTemporaryEffects);
        ChangeStackableText(m_stackableTemporaryEffects);
    }

    private void ChangeStackableText(int value) {
        if (value <= 1) {
            m_stackText.text = "";
            return;
        }
        m_stackText.text = value.ToString();
     }
   
   

    

}
