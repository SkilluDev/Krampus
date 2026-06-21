using UnityEngine;

public class EasterEgg : MonoBehaviour {
    [SerializeField] private int m_oncePer;
    private void Start() {
        if (Random.Range(0, m_oncePer) != 1) {
            gameObject.SetActive(false);
        }
    }
}
