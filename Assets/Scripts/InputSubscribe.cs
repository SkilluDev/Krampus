using UnityEngine;
using KrampInput;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine.InputSystem;

public class InputSubscribe : MonoBehaviour {

	static InputSubscribe() {
		Application.quitting += Shutdown;
	}

	private static PlayerControls m_playerControls;
	public static PlayerControls Raw {
		get {
			Init();
			return m_playerControls;
		}
	}

	public static Vector2 Movement => Raw.Player.Move.ReadValue<Vector2>();
	public static bool Sneaking => Raw.Player.Crouch.IsPressed();

	private static List<string> PossibleMethods => Raw.controlSchemes.Select(w => w.name).ToList();

	[SerializeField][Dropdown(nameof(PossibleMethods))] private string m_initialMethod;

	private void Awake() {
		ChangeInputMethod(m_initialMethod);
	}

	public static void ChangeInputMethod(string method) {
		Shutdown();
		Init(method);
	}

	public static void Init(string method = null) {
		if (m_playerControls != null) return;
		m_playerControls = new PlayerControls();
		if (method != null) m_playerControls.bindingMask = InputBinding.MaskByGroup(method);
		m_playerControls.Player.Enable();
		m_playerControls.UI.Enable();
	}
	public static void Shutdown() {
		if (m_playerControls == null) return;
		m_playerControls.Player.Disable();
		m_playerControls.UI.Disable();
		m_playerControls.Dispose();
		m_playerControls = null;
	}
}
