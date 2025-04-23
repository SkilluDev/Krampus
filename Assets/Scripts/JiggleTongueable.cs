using UnityEngine;

// terrible and temporary

public class JiggleTongueable : MonoBehaviour, ITongueable {
    [SerializeField] private Transform m_model;
    [SerializeField] private float m_strength = 1;
    [SerializeField] private float m_duration = 1;
    private float m_time = 99999;

    private Vector3 m_initialScale;

    private void Awake() {
        m_initialScale = m_model.localScale;
    }

    private float JiggleCurve(float x) {
        const float c4 = (2 * Mathf.PI) / 3;
        return 1f - (x == 0
          ? 0
          : x == 1
          ? 1
          : Mathf.Pow(2, -10 * x) * Mathf.Sin((x * 10f - 0.75f) * c4) + 1f);

    }

    private void Update() {
        float jiggleFactor = JiggleCurve(Mathf.Clamp01(m_time / m_duration)) * m_strength;
        m_model.localScale = (1 + jiggleFactor) * m_initialScale;

        m_time += Time.deltaTime;
    }


    public void DirectHit(Krampus krampus, Vector3 point) {
        m_time = 0;
    }



    public void Passby(Krampus krampus, Vector3 point, float tongueLength) { }
}
