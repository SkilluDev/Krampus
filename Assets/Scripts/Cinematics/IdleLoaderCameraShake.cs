using UnityEngine;

public class IdleLoaderCameraShake : MonoBehaviour
{
    [SerializeField] private float m_amplitude = 0.1f;
    [SerializeField] private float m_frequency = 1.0f;

    private Vector3 m_initialPosition;
    private float m_elapsedTime;

	private void Start()
    {
        m_initialPosition = transform.localPosition;
    }

	private void Update()
    {
        m_elapsedTime += Time.unscaledDeltaTime;

        float offsetX = Mathf.PerlinNoise(m_elapsedTime * m_frequency, 0.0f) - 0.5f;
        float offsetY = Mathf.PerlinNoise(0.0f, m_elapsedTime * m_frequency) - 0.5f;

        Vector3 shakeOffset = new Vector3(offsetX, offsetY, 0f) * m_amplitude;

        transform.localPosition = m_initialPosition + shakeOffset;
    }

	private void OnDisable()
    {
        transform.localPosition = m_initialPosition;
    }
}
