using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class InputSubscribe : MonoBehaviour
{
	private PlayerControls playerControls;
	public Vector2 MoveInput { get; private set; } = Vector2.zero;
	public Vector2 AimInput { get; private set; } = Vector2.zero;
	public bool CrouchInput { get; private set; } = false;
	public bool TongueInput { get; private set; } = false;

	public Vector2 NavigateInput { get; private set; } = Vector2.zero;
	public bool PauseInput { get; private set; } = false;
	public bool AdvanceInput { get; private set; } = false;
	public bool RestartInput { get; private set; } = false;

	private void Update() {
		TongueInput = playerControls.Player.Tongue.WasPressedThisFrame();
		PauseInput = playerControls.UI.Pause.WasPressedThisFrame();
		AdvanceInput = playerControls.UI.Advance.WasPressedThisFrame();
		RestartInput = playerControls.UI.Restart.WasPressedThisFrame();
	}
	private void OnEnable() {
		playerControls = new PlayerControls();
		playerControls.Player.Enable();
		playerControls.Player.Move.performed += SetMovement;
		playerControls.Player.Move.canceled += SetMovement;
		playerControls.Player.Aim.performed += SetAim;
		playerControls.Player.Aim.canceled += SetAim;
		playerControls.Player.Crouch.started += SetCrouch;
		playerControls.Player.Crouch.canceled += SetCrouch;
		playerControls.Player.Tongue.started += SetTongue;
		playerControls.Player.Tongue.canceled +=SetTongue;
		playerControls.UI.Enable();
		playerControls.UI.Navigate.performed += SetNavigate;
		playerControls.UI.Navigate.canceled += SetNavigate;
		playerControls.UI.Pause.started += SetPause;
		playerControls.UI.Pause.canceled += SetPause;
		playerControls.UI.Advance.started += SetAdvance;
		playerControls.UI.Advance.canceled +=SetAdvance;
		playerControls.UI.Restart.started += SetRestart;
		playerControls.UI.Restart.canceled +=SetRestart;
	}

	private void OnDisable() {
		playerControls.Player.Disable();
		playerControls.Player.Move.performed -= SetMovement;
		playerControls.Player.Move.canceled -= SetMovement;
		playerControls.Player.Aim.performed -= SetAim;
		playerControls.Player.Aim.canceled -= SetAim;
		playerControls.Player.Crouch.started -= SetCrouch;
		playerControls.Player.Crouch.canceled -= SetCrouch;
		playerControls.Player.Tongue.started -= SetTongue;
		playerControls.Player.Tongue.canceled -= SetTongue;
		playerControls.UI.Disable();
		playerControls.UI.Navigate.performed -= SetNavigate;
		playerControls.UI.Navigate.canceled -= SetNavigate;
		playerControls.UI.Pause.started -= SetPause;
		playerControls.UI.Pause.canceled -= SetPause;
		playerControls.UI.Advance.started -= SetAdvance;
		playerControls.UI.Advance.canceled -=SetAdvance;
		playerControls.UI.Restart.started -= SetRestart;
		playerControls.UI.Restart.canceled -=SetRestart;
	}

	void SetMovement(InputAction.CallbackContext ctx) {
		MoveInput = ctx.ReadValue<Vector2>();
	}

	void SetAim(InputAction.CallbackContext ctx) {
		AimInput = ctx.ReadValue<Vector2>();
	}

	void SetCrouch(InputAction.CallbackContext ctx) {
		CrouchInput = ctx.started;
	}
	void SetTongue(InputAction.CallbackContext ctx) {
		TongueInput = ctx.started;
	}
	void SetNavigate(InputAction.CallbackContext ctx) {
		NavigateInput = ctx.ReadValue<Vector2>();
	}
	void SetPause(InputAction.CallbackContext ctx) {
		PauseInput = ctx.started;
	}
	void SetAdvance(InputAction.CallbackContext ctx) {
		AdvanceInput = ctx.started;
	}
	void SetRestart(InputAction.CallbackContext ctx) {
		RestartInput = ctx.started;
	}
}
