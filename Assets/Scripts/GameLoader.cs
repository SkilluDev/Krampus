using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour {
    [SerializeField] private float m_delay = 1f;

    public string State { get; private set; }

    private void Start() {
        StartCoroutine(LoadingProcedure());
    }

    private IEnumerator LoadingProcedure() {
        State = "Waiting";
        yield return new WaitForSecondsRealtime(m_delay);
        SceneManager.SetActiveScene(gameObject.scene);
        var unload = SceneManager.UnloadSceneAsync((int)Game.SourceState);
        var load = SceneManager.LoadSceneAsync((int)Game.DestinationState, LoadSceneMode.Additive);
        load.allowSceneActivation = false;

        while (!unload.isDone && load.progress < 0.9f) {
            State = $"Changing scene: {unload.progress} -> {load.progress}";
            yield return null;
        }

        load.allowSceneActivation = true;

        while (!load.isDone) yield return null;

        var loadedScene = SceneManager.GetSceneByBuildIndex((int)Game.DestinationState);
        SceneManager.SetActiveScene(loadedScene);
        Time.timeScale = 0;

        var loadables = loadedScene.GetRootGameObjects().SelectMany(w => w.GetComponentsInChildren<IGameLoadable>());
        State = $"Loading {loadables.Count()} items";
        yield return null;
        foreach (var w in loadables) {
            var loader = w.Load();
            while (loader.MoveNext()) {
                State = $"[{w.GameObject.name}] {w.Status} {w.Progress * 100:0.0}%";
                yield return loader.Current;
            }
        }


        State = "Waiting";
        yield return new WaitForSecondsRealtime(m_delay);
        SceneManager.UnloadSceneAsync(gameObject.scene);
        Game.FinishedLoading();
        Time.timeScale = 1;

        foreach (var go in loadedScene.GetRootGameObjects()) go.BroadcastMessage("Ready", SendMessageOptions.DontRequireReceiver);
    }
}
