using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyer : MonoBehaviour {
    // Start is called before the first frame update
    private float time = 3f;
    private void Start() {
        Destroy(gameObject, time);
    }



}
