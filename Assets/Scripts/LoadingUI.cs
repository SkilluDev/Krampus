using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour {
    [SerializeField] private GameLoader m_loader;
    [SerializeField] private TextMeshProUGUI m_loadingText;
    [SerializeField] private TextMeshProUGUI m_statusText;
    [SerializeField] private Slider m_slider;
    [SerializeField] private Image m_loadingImage;
    [SerializeField] private float m_rotateSpeed;

    private void Update() {
        m_statusText.text = m_loader.Status;
        m_loadingText.text = Game.RequireFullReload ? "Loading" : "Debug Loading";
        m_slider.value = m_loader.Progress;

        m_loadingImage.transform.Rotate(Vector3.up, Time.unscaledDeltaTime*m_rotateSpeed);
    }

}
