using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFacingCamera : MonoBehaviour
{
   private Camera camera;
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(camera.transform);
    }
}
