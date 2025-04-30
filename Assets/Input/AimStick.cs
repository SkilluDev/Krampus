using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class AimStick : MonoBehaviour {
	[Header("GamePad")]
	public Vector2 aimPadSensitivity = new Vector2(1500f, 1500f); //the higher value, the more mouse moves with stick
	public Vector2 antiBias = new Vector2(0f, -1f); // mitigate the tendency of the cursor to move more easily in certain directions
	public Vector2 rightStick;
	public Vector2 mousePosition;
	public Vector2 warpPosition;
	public Vector2 correction; //overflow for next frame
							   // Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {
		rightStick = InputSubscribe.Raw.Player.AimPad.ReadValue<Vector2>();
		if (rightStick.magnitude < 0.1f) return; //suppress input jitter during stick inactivity
		mousePosition = InputSubscribe.Raw.Player.Aim.ReadValue<Vector2>();//capture mouse position to add to stick movement
		warpPosition = mousePosition + antiBias + correction + aimPadSensitivity * Time.deltaTime * rightStick; //accurate value representing the intended cursor position
		warpPosition = new Vector2(Mathf.Clamp(warpPosition.x, 0, Screen.width), Mathf.Clamp(warpPosition.y, 0, Screen.height)); //keeping cursor in the game screen
		correction = new Vector2(warpPosition.x % 1, warpPosition.y % 1); //preserve floating-point precision separately, as WarpCursorPosition applies FloorToInt and truncates values
		Mouse.current.WarpCursorPosition(warpPosition);
		//Debug.Log($"Triggered: {InputSubscribe.Input.Player.Tongue.triggered}, KrampusMove: {GetComponent<KrampusController>().shouldKrampusMove}");
		//InputSubscribe.SetAimInput(warpPosition);
	}

}
