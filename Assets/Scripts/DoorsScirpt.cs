using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorsScirpt : MonoBehaviour {
	private Animator animator;
	private bool isOpen = false;

	private void Start() {
		animator = GetComponentInChildren<Animator>();


	}

	private void OnTriggerEnter(Collider other) {
		if (isOpen) { return; }
		if (other.tag == "Player" || other.tag == "Child" || other.tag == "Parent") {

			Vector3 direcion = (other.transform.position - transform.position).normalized;
			OpenDoor(direcion);

		}
	}

	private void OpenDoor(Vector3 direction) {

		if ((gameObject.transform.rotation.y % 180 == 0) ? direction.z >= 0 : direction.x < 0) { //if this should be checked on Z, set Y rotation to 180. (idk if 360 works)
			animator.SetTrigger("Open");
		} else { animator.SetTrigger("Open2"); }
		isOpen = true;
	}
}
