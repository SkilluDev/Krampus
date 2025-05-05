using System.Collections;
using UnityEngine;

public class DebugLoadable : MonoBehaviour, IGameLoadable {
    public string Status { get; private set; }

    public float Progress { get; private set; }

    public IEnumerator Load() {
        while (Progress < 0.5f) {
            Status = "Finding candidates";
            yield return null;
            Progress += Time.unscaledDeltaTime / 5f;
        }
        Status = "Found!";
        yield return new WaitForSecondsRealtime(0.1f);
        while (Progress < 0.99f) {
            Status = "Creating the kids...";
            yield return null;
            Progress += Time.unscaledDeltaTime / 5f;
        }
        yield return new WaitForSecondsRealtime(2);
        Status = "Satisfaction";
        yield return new WaitForSecondsRealtime(1);
        Status = "Done";
    }

}
