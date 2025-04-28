using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class InputSubscribe {

	private static PlayerControls playerControls;
	private static bool initialized = false;
	public static PlayerControls Input {
		get {
			Init();
			return playerControls;
		}
	}

	public static void Init() {
		if (initialized) return;
		if (playerControls == null) {
			playerControls = new PlayerControls();
			playerControls.Player.Enable();
			/*
			playerControls.Player.Move.performed += SetMovement;
			playerControls.Player.Move.canceled += SetMovement;
			playerControls.Player.Aim.performed += SetAim;
			playerControls.Player.Aim.canceled += SetAim;
			playerControls.Player.AimPad.performed += SetAimPad;
			playerControls.Player.AimPad.canceled += SetAimPad;
			playerControls.Player.Crouch.started += SetCrouch;
			playerControls.Player.Crouch.canceled += SetCrouch;
			playerControls.Player.Tongue.started += SetTongue;
			playerControls.Player.Tongue.canceled +=SetTongue;
			*/

			playerControls.UI.Enable();
			/*
			playerControls.UI.Navigate.performed += SetNavigate;
			playerControls.UI.Navigate.canceled += SetNavigate;
			playerControls.UI.Pause.started += SetPause;
			playerControls.UI.Pause.canceled += SetPause;
			playerControls.UI.Advance.started += SetAdvance;
			playerControls.UI.Advance.canceled +=SetAdvance;
			playerControls.UI.Restart.started += SetRestart;
			playerControls.UI.Restart.canceled +=SetRestart;
			*/
			initialized = true;
		}
	}
	public static void Shutdown() {
		if (playerControls == null) return;
		playerControls.Player.Disable();
		/*
		playerControls.Player.Move.performed -= SetMovement;
		playerControls.Player.Move.canceled -= SetMovement;
		playerControls.Player.Aim.performed -= SetAim;
		playerControls.Player.Aim.canceled -= SetAim;
		playerControls.Player.AimPad.performed -= SetAimPad;
		playerControls.Player.AimPad.canceled -= SetAimPad;
		playerControls.Player.Crouch.started -= SetCrouch;
		playerControls.Player.Crouch.canceled -= SetCrouch;
		playerControls.Player.Tongue.started -= SetTongue;
		playerControls.Player.Tongue.canceled -= SetTongue;
		*/
		playerControls.UI.Disable();
		/*
		playerControls.UI.Navigate.performed -= SetNavigate;
		playerControls.UI.Navigate.canceled -= SetNavigate;
		playerControls.UI.Pause.started -= SetPause;
		playerControls.UI.Pause.canceled -= SetPause;
		playerControls.UI.Advance.started -= SetAdvance;
		playerControls.UI.Advance.canceled -=SetAdvance;
		playerControls.UI.Restart.started -= SetRestart;
		playerControls.UI.Restart.canceled -=SetRestart;
		*/
		playerControls.Dispose();
		playerControls = null;
		initialized = false;
	}
	/*
		public static Vector2 MoveInput { get; private set; } = Vector2.zero;
		public static Vector2 AimInput { get; private set; } = Vector2.zero;
		public static Vector2 AimPadInput { get; private set; } = Vector2.zero;
		public static bool CrouchInput { get; private set; } = false;
		public static bool TongueInput { get; private set; } = false;

		public static Vector2 NavigateInput { get; private set; } = Vector2.zero;
		public static bool PauseInput { get; private set; } = false;
		public static bool AdvanceInput { get; private set; } = false;
		public static bool RestartInput { get; private set; } = false;

		public static void SetAimInput(Vector2 newValue) {
			AimInput = newValue;
		}
		public static void Check() {
			TongueInput = playerControls.Player.Tongue.WasPressedThisFrame();
			PauseInput = playerControls.UI.Pause.WasPressedThisFrame();
			AdvanceInput = playerControls.UI.Advance.WasPressedThisFrame();
			RestartInput = playerControls.UI.Restart.WasPressedThisFrame();
		}
		public static void ResetFrameInputs()
		{
			TongueInput = false;
			PauseInput = false;
			AdvanceInput = false;
			RestartInput = false;
		}
		static void SetMovement(InputAction.CallbackContext ctx) {
			MoveInput = ctx.ReadValue<Vector2>();
		}

		static void SetAim(InputAction.CallbackContext ctx) {
			AimInput = ctx.ReadValue<Vector2>();
		}
		static void SetAimPad(InputAction.CallbackContext ctx) {
			AimPadInput = ctx.ReadValue<Vector2>();
		}
		static void SetCrouch(InputAction.CallbackContext ctx) {
			CrouchInput = ctx.started;
		}
		static void SetTongue(InputAction.CallbackContext ctx) {
			TongueInput = ctx.started;
		}
		static void SetNavigate(InputAction.CallbackContext ctx) {
			NavigateInput = ctx.ReadValue<Vector2>();
		}
		static void SetPause(InputAction.CallbackContext ctx) {
			PauseInput = ctx.started;
		}
		static void SetAdvance(InputAction.CallbackContext ctx) {
			AdvanceInput = ctx.started;
		}
		static void SetRestart(InputAction.CallbackContext ctx) {
			RestartInput = ctx.started;
		}
		*/
}
