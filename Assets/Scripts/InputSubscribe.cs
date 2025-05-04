using UnityEngine;
using KrampInput;

public class InputSubscribe {

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

	public static void Init() {
		if (m_playerControls != null) return;

		m_playerControls = new PlayerControls();
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
