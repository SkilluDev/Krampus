using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitMotion;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Video;

public class IdleDisplay : MonoBehaviour {

    [SerializeField] private float m_idleTime = 30f;
    [SerializeField] private VideoPlayer m_videoPlayer;

    [SerializeField] private CanvasGroup m_canvasGroup;

    private float m_timer = 0;

    private bool m_playing;
    
    private IDisposable m_listener;

    private MotionHandle m_motionHandle;
    [SerializeField] private float m_fadeInTime = 20f;
    private void Awake() {

        if (File.Exists(Application.dataPath+"\\idle.mp4")) m_videoPlayer.url = Application.dataPath+"\\idle.mp4";
        m_listener = InputSystem.onAnyButtonPress.Call(OnButtonPressed);
        m_videoPlayer.gameObject.SetActive(false);
    }

    private void OnDisable() {
        m_listener.Dispose();
		
	}

	private void OnButtonPressed(InputControl control) {
        m_timer = 0;
        m_playing = false;
        m_videoPlayer.Stop();
        m_videoPlayer.gameObject.SetActive(false);
        m_motionHandle.TryCancel();
    }

    private void Update() {
        m_timer += Time.deltaTime;
        if (m_timer > m_idleTime && !m_playing && Game.MainMenuInfo.CurrentState == MainMenuInfo.State.Default) {
            m_timer = 0;
            m_playing = true;
            m_videoPlayer.gameObject.SetActive(true);
            m_videoPlayer.Play();
            m_motionHandle = LMotion.Create(0f, 1f, m_fadeInTime).Bind(a => m_canvasGroup.alpha = a);
        }

    }

}
