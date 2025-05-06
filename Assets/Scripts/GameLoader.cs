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

    private IEnumerator LoadingProcedure() {
        Status = "Prepare";
        Progress = 0;

        var unloadedScene = SceneManager.GetSceneByBuildIndex((int)Game.SourceState);
        foreach (var go in unloadedScene.GetRootGameObjects()) go.BroadcastMessage("Unready", SendMessageOptions.DontRequireReceiver);
        Time.timeScale = 0;

        Status = "Waiting";
        yield return Delay();
        SceneManager.SetActiveScene(gameObject.scene);

        Status = "Changing Scene";
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

        Status = "Waiting";
        yield return Delay();

        Status = "Finishing";
        SceneManager.UnloadSceneAsync(gameObject.scene);
        Game.FinishedLoading();
        Time.timeScale = 1;

        foreach (var go in loadedScene.GetRootGameObjects()) go.BroadcastMessage("Ready", SendMessageOptions.DontRequireReceiver);

        Status = "Done";
        Progress = 1;
    }

    private IEnumerator UpdateLoadables(Scene loadedScene) {
        var loadables = loadedScene.GetRootGameObjects().SelectMany(w => w.GetComponentsInChildren<IGameLoadable>());
        Status = $"Loading {loadables.Count()} objects";
        yield return null;
        foreach (var w in loadables) {
            var loader = w.Load();
            while (loader.MoveNext()) {
                Status = w.Status;
                Progress = w.Progress;
                yield return loader.Current;
            }
        }
    }

    private void BroadcastAll(Scene loadedScene, string msg) {

    }

    private IEnumerator Delay() {
        float timer = 0;
        while (timer < m_delay) {
            yield return null;
            Progress = timer / m_delay;
        }
    }

    private IEnumerator SoftLoadingProcedure() {
        Status = "Waiting";
        Time.timeScale = 0;
        var loadedScene = SceneManager.GetSceneByBuildIndex((int)Game.DestinationState);
        SceneManager.SetActiveScene(loadedScene);
        yield return UpdateLoadables(loadedScene);
        Status = "Waiting";
        yield return new WaitForSecondsRealtime(m_delay);
        SceneManager.UnloadSceneAsync(gameObject.scene);
        Game.FinishedLoading();
        Time.timeScale = 1;

        foreach (var go in loadedScene.GetRootGameObjects()) go.BroadcastMessage("Ready", SendMessageOptions.DontRequireReceiver);
    }
}
