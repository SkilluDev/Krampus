using System.Collections;
using UnityEngine;

public interface IGameLoadable {
    public IEnumerator Load();
    public string Status { get; }
    public float Progress { get; }
    public GameObject GameObject => ((MonoBehaviour)this).gameObject;
}
