using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewUIManager : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI m_remainingChildCount;
    [SerializeField] private TextMeshProUGUI m_goodChild;
    [SerializeField] private TextMeshProUGUI m_currentSeed;
    [SerializeField] private GameObject m_gameOverScreen;
    [SerializeField] private GameObject m_gameWinScreen;
    [SerializeField] private GameObject m_pauseScreen;
    [SerializeField] private TextMeshProUGUI m_timerText;
    [SerializeField] private NumericDisplay m_timerDisplay;

    [SerializeField] private Image m_childIconImage;
    [SerializeField] private Image m_fillBar;
    private MainGameInfo.State m_currentGameState;


    public void SetSeed(int seed) {
        m_currentSeed.text = $"Map seed: {seed:0000000}<br>Press [y] to regenerate";
    }

    public void ShowGameOverScreen() {
        m_gameOverScreen.SetActive(true);
        m_timerText.gameObject.SetActive(false);
        Game.MainGameInfo.setState(MainGameInfo.State.Over);
    }

    public void SwitchPauseMenu() {
        if (m_currentGameState != MainGameInfo.State.Over) {
            if (m_currentGameState == MainGameInfo.State.Paused) {
                Game.MainGameInfo.setState(MainGameInfo.State.Ongoing);
                m_pauseScreen.SetActive(false);
            } else {
                Game.MainGameInfo.setState(MainGameInfo.State.Paused);
                m_pauseScreen.SetActive(true);
                Debug.Log("relrrellrelrlelrel");
            }
            Time.timeScale = m_currentGameState == MainGameInfo.State.Paused ? 1 : 0;
        }
    }


    private void Update() {
        m_currentGameState = Game.MainGameInfo.CurrentState;
        var col = Game.MainGameInfo.GoodChildType;
        m_remainingChildCount.text = $"<color=#{ColorUtility.ToHtmlStringRGB(col.color)}>Do not eat: {col.shape.name}</color><br>Remaining children: {Game.MainGameInfo.Children.Count}";
        if (Game.MainGameInfo.BadChildrenCountOnStart > 0) {
            m_fillBar.fillAmount = (float)(Game.MainGameInfo.BadChildrenCountOnStart - Game.MainGameInfo.BadChildren.Count()) /
                                   Game.MainGameInfo.BadChildrenCountOnStart;
        }

        m_timerText.text = Game.MainGameInfo.Timer.GameTime.ToString("00");
        m_timerDisplay.Value = Game.MainGameInfo.Timer.GameTime;

        if (InputSubscribe.Raw.UI.Pause.triggered) SwitchPauseMenu();

        if (!Game.MainGameInfo.BadChildren.Any() && !Game.IsLoading) {
            Debug.Log("won");
            m_gameWinScreen.SetActive(true);
            Game.MainGameInfo.setState(MainGameInfo.State.Won);
            Time.timeScale = 0;
        }
        if (m_currentGameState == MainGameInfo.State.Over || m_currentGameState == MainGameInfo.State.Paused || m_currentGameState == MainGameInfo.State.Won) {//if the game is over, won, or paused, you can
            if (InputSubscribe.Raw.UI.MenuReturn.triggered) //go back to menu with M
                Game.LoadState(Game.State.MainMenu);
            if (InputSubscribe.Raw.UI.Restart.triggered) //restart with R
                Game.LoadState(Game.State.MainGame);
        }
    }

    public void SetChildrenIcon(Sprite icon) {
        m_childIconImage.sprite = icon;
    }
}
