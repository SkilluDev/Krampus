using TMPro;
using UnityEngine;

public class NewUIManager : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI m_remainingChildCount;
    [SerializeField] private TextMeshProUGUI m_goodChild;
    [SerializeField] private TextMeshProUGUI m_currentSeed;
    [SerializeField] private GameObject m_gameOverScreen;
    [SerializeField] private TextMeshProUGUI m_timerText;


    public void SetSeed(int seed) {
        m_currentSeed.text = $"Map seed: {seed:0000000}<br>Press [y] to regenerate";
    }

    public void ShowGameOverScreen() {
        m_gameOverScreen.SetActive(true);
    }

    private void Update() {
        var col = Game.MainGameInfo.Types[Game.MainGameInfo.GoodChildIndex];
        m_remainingChildCount.text = $"<color=#{ColorUtility.ToHtmlStringRGB(col.color)}>Do not eat: {col.shape.name}</color><br>Remaining children: {Game.MainGameInfo.Children.Count}";
        m_timerText.text =  Game.MainGameInfo.timer.ToString("00");
    }
}
