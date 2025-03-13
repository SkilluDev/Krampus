using UnityEngine;
using UnityEngine.SceneManagement;

public class StartTest : MonoBehaviour {
    public void GoToScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
