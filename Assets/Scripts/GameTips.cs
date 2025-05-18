using System.IO;
using KrampUtils;
using TMPro;
using UnityEngine;

public class GameTips : MonoBehaviour {
    public readonly string path = Application.dataPath + "/tips.txt";
    [SerializeField] private TextMeshProUGUI m_txt;

    private void Start() {
        if (File.Exists(path)) {
            string[] tips = File.ReadAllLines(path);
            m_txt.SetText(tips.UnityRandomElement());
        } else {
            m_txt.SetText("");
        }
    }
}
