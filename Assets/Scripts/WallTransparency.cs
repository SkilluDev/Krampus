using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTransparency : MonoBehaviour
{
    bool z;
    Material material;
    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Renderer>().material;
        player = GameObject.Find("Player").transform;
        if (transform.rotation.z == 90)
        {
            z = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (z)
        {
            if (player.position.x > transform.position.x)
            {
                gameObject.SetActive(true);
            }
            else
            {
                GetComponent<Renderer>().material = material;
            }
        }
    }
}
