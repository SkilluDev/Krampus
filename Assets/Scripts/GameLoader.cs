using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour {
    [SerializeField] private float m_delay = 1f;

    public string Status { get; private set; }
    public float Progress { get; private set; }

    private void Start() {
        StartCoroutine(Game.RequireFullReload ? LoadingProcedure() : SoftLoadingProcedure());
    }

    private void BeginStatus(string status) {
        Status = status;
        Progress = 0;
    }

    private IEnumerator LoadingProcedure() {
        BeginStatus("Prepare");

        var unloadedScene = SceneManager.GetSceneByBuildIndex((int)Game.SourceState);
        BroadcastAll(unloadedScene, "Unready");
        Time.timeScale = 0;

        BeginStatus("Waiting");
        yield return Delay();
        SceneManager.SetActiveScene(gameObject.scene);

        BeginStatus("Changing Scene");
        var unload = SceneManager.UnloadSceneAsync((int)Game.SourceState);
        var load = SceneManager.LoadSceneAsync((int)Game.DestinationState, LoadSceneMode.Additive);
        load.allowSceneActivation = false;

        while (!unload.isDone && load.progress < 0.9f) {
            Progress = (unload.progress + load.progress) / 2f;
            yield return null;
        }

        load.allowSceneActivation = true;

        while (!load.isDone) yield return null;

        var loadedScene = SceneManager.GetSceneByBuildIndex((int)Game.DestinationState);
        SceneManager.SetActiveScene(loadedScene);
        yield return UpdateLoadables(loadedScene);

        BeginStatus("Waiting");
        yield return Delay();

        BeginStatus("Finishing Scene");
        SceneManager.UnloadSceneAsync(gameObject.scene);
        Game.FinishedLoading();
        Time.timeScale = 1;

        BroadcastAll(loadedScene, "Ready");

        BeginStatus("Done");
        Progress = 1;
    }

    private IEnumerator UpdateLoadables(Scene loadedScene) {
        var loadables = loadedScene.GetRootGameObjects().SelectMany(w => w.GetComponentsInChildren<IGameLoadable>());
        int current = 0;
        int count = loadables.Count();
        BeginStatus($"Loading {count} objects");
        yield return null;

        foreach (var w in loadables) {
            var loader = w.Load();
            while (loader.MoveNext()) {
                Status = $"({current}/{count}) {w.Status}";
                Progress = w.Progress;
                yield return loader.Current;
            }
            current++;
        }
    }

    private void BroadcastAll(Scene loadedScene, string msg) {
        foreach (var go in loadedScene.GetRootGameObjects()) go.BroadcastMessage(msg, SendMessageOptions.DontRequireReceiver);
    }

    private IEnumerator Delay() {
        float timer = 0;
        while (timer < m_delay) {
            yield return null;
            timer += Time.unscaledDeltaTime;
            Progress = timer / m_delay;
        }
    }

    private IEnumerator SoftLoadingProcedure() {
        BeginStatus("Waiting");
        Time.timeScale = 0;
        var loadedScene = SceneManager.GetSceneByBuildIndex((int)Game.DestinationState);
        SceneManager.SetActiveScene(loadedScene);
        yield return UpdateLoadables(loadedScene);

        BeginStatus("Waiting");
        yield return Delay();

        BeginStatus("Finishing Scene");
        SceneManager.UnloadSceneAsync(gameObject.scene);
        Game.FinishedLoading();
        Time.timeScale = 1;

        BroadcastAll(loadedScene, "Ready");

        BeginStatus("Done");
        Progress = 1;
    }
}
