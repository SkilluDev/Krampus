using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class lookWhereWalking : MonoBehaviour
{
    Rigidbody parentRigid;
    Quaternion destination;

    private float timeCount = 0.0f;
    void Start()
    {
        parentRigid = GetComponentInParent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (parentRigid.velocity != new Vector3(0, 0, 0)) {
            destination = Quaternion.LookRotation(parentRigid.velocity);
            transform.rotation = (Quaternion.Slerp(transform.rotation, destination, timeCount));
            timeCount = timeCount + Time.deltaTime;
            if (timeCount >= 1) {
                timeCount = 0;
            }
        }
    }
}
