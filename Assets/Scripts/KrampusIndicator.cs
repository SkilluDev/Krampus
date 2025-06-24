using System.Collections;
using System.Collections.Generic;
using LitMotion;
using UnityEngine;

public class KrampusIndicator : MonoBehaviour {

    [SerializeField] private SpriteRenderer m_spriteRenderer;
    [SerializeField] private SpriteRenderer m_timerIcon;
    private bool m_IsVisible = false;
    private MotionHandle m_motionHandle;


    public void Start() {
        Hide();

    }

    private void Update() {
        if(m_IsVisible) 
        transform.forward = Game.MainGameInfo.Krampus.Kamera.transform.forward;
    }


    public void PlayAniamtion(float duration) {

        Show();

        m_motionHandle.TryCancel();
        m_motionHandle = LMotion.Create(0, 0.9f, duration).WithOnComplete(() => Hide()).Bind(x => m_timerIcon.material.SetFloat("_FillRate",x));



     }

    public void Hide() {
        m_IsVisible = false;
        m_spriteRenderer.gameObject.SetActive(false);
         m_timerIcon.gameObject.SetActive(false);
    }
    public void Show() {
        m_IsVisible = true;
        m_spriteRenderer.gameObject.SetActive(true);
         m_timerIcon.gameObject.SetActive(true);
     }
}
