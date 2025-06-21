using UnityEngine;
using UnityEngine.InputSystem;

public class TestDot : MonoBehaviour {

	private void Update() {
		Vector3 mousePos;
		mousePos = Mouse.current.position.ReadValue();
		transform.position = mousePos;
	}
}
