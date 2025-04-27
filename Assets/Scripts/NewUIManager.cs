using TMPro;
using UnityEngine;

public class NewUIManager : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI m_remainingChildCount;
    [SerializeField] private TextMeshProUGUI m_goodChild;
    [SerializeField] private TextMeshProUGUI m_currentSeed;


    public void SetSeed(int seed) {
        m_currentSeed.text = $"Map seed: {seed:0000000}<br>Press [y] to regenerate";
    }

    private void Update() {
        var col = Game.MainGameInfo.Types[Game.MainGameInfo.GoodChildIndex];
        m_remainingChildCount.text = $"<color=#{ColorUtility.ToHtmlStringRGB(col.color)}>Do not eat: {col.shape.name}</color><br>Remaining children: {Game.MainGameInfo.Children.Count}";
    }
}
