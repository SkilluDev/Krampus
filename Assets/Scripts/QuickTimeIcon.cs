using System.Collections;
using System.Collections.Generic;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class QuickTimeIcon : MonoBehaviour
{
    [SerializeField] private Image m_fillImage;
    private MotionHandle m_currentFillHandle;
    public void StartQuickActionTimer(float duration) {


	    m_currentFillHandle.TryCancel();
	   // m_currentFillHandle = LMotion.Create(1, 0, duration).WithDelay(0.1f).BindToFillAmount(m_fillImage);

    }
}
