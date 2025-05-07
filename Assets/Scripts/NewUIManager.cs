using TMPro;
using UnityEngine;

public class NewUIManager : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI m_remainingChildCount;
    [SerializeField] private TextMeshProUGUI m_goodChild;
    [SerializeField] private TextMeshProUGUI m_currentSeed;
    [SerializeField] private GameObject m_gameOverScreen;
    [SerializeField] private GameObject m_gameWinScreen;
    [SerializeField] private GameObject m_pauseScreen;
    [SerializeField] private TextMeshProUGUI m_timerText;
    [SerializeField] private bool m_isGameOver = false;
    [SerializeField] private bool m_isGameWon = false;
    [SerializeField] private bool m_isGamePaused = false;



    public void SetSeed(int seed) {
        m_currentSeed.text = $"Map seed: {seed:0000000}<br>Press [y] to regenerate";
    }

    public void ShowGameOverScreen() {
        m_gameOverScreen.SetActive(true);
        m_timerText.gameObject.SetActive(false);
        m_isGameOver = true;
    }

    public void SwitchPauseMenu() {
        if (!m_isGameOver) {
            if (m_isGamePaused) {
                m_pauseScreen.SetActive(false);
            } else {
                m_pauseScreen.SetActive(true);
            }
            Time.timeScale = m_isGamePaused ? 1 : 0;
            m_isGamePaused = !m_isGamePaused;
        }
    }


    private void Update() {
        var col = Game.MainGameInfo.Types[Game.MainGameInfo.GoodChildIndex];
        m_remainingChildCount.text = $"<color=#{ColorUtility.ToHtmlStringRGB(col.color)}>Do not eat: {col.shape.name}</color><br>Remaining children: {Game.MainGameInfo.Children.Count}";
        m_timerText.text = Game.MainGameInfo.Timer.ToString("00");

        if (InputSubscribe.Raw.UI.Pause.triggered) SwitchPauseMenu();

        if (Game.MainGameInfo.Children.Count == 0 && !Game.IsLoading) {
            m_gameWinScreen.SetActive(true);
            m_isGameWon = true;
            Time.timeScale = 0;
        }
        if (m_isGameOver || m_isGamePaused || m_isGameWon) //if the game is over or game is paused, you can 
        {

            if (InputSubscribe.Raw.UI.Advance.triggered) //go back to menu with the default
                Game.LoadState(Game.State.MainMenu);
            if (InputSubscribe.Raw.UI.Restart.triggered) //restart with R
                Game.LoadState(Game.State.MainGame);
        }
    }
}
