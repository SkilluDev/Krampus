using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingUI : MonoBehaviour {
    [SerializeField] private GameLoader m_loader;
    [SerializeField] private TextMeshProUGUI m_statusText;

    private void Update() {
        m_statusText.text = m_loader.State;
    }

}
