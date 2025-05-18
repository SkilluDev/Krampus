using UnityEngine;
using KrampInput;
using UnityEngine.InputSystem;
using System;
using System.Linq;
using UnityEngine.Events;

public class InputSubscribe : MonoBehaviour {
	[Serializable]
	public enum Method {
		PC,
		Console,
		Mobile
	}

	private static PlayerControls m_playerControls;
	public static PlayerControls Raw {
		get {
			Init();
			return m_playerControls;
		}
	}

	public static UnityEvent onChanged = new UnityEvent();

	public static Vector2 Movement => Raw.Player.Move.ReadValue<Vector2>();

	public static Vector2 Aim => Raw.Player.Aim.ReadValue<Vector2>();
	public static Vector2 UITilt => Raw.UI.Tilt.ReadValue<Vector2>();
	public static bool Sneaking => Raw.Player.Crouch.IsPressed();
	public static Method InputMethod { get; private set; }

	[SerializeField] private Method m_method;


	static InputSubscribe() {
		Application.quitting += Shutdown;
	}

	private void Awake() {
		//ChangeInputMethod(m_method);
	}

	public static void ChangeInputMethod(Method method) {
		if (InputMethod == method) return;
		Shutdown();
		InputMethod = method;
		onChanged.Invoke();
		Init(method.ToString());
	}

	[RuntimeInitializeOnLoadMethod]
	private static void GameInit() {
		if (m_playerControls != null) m_playerControls = null;
	}

	private static void Init(string method = null) {
		if (m_playerControls != null) return;
		m_playerControls = new PlayerControls();
		if (method != null) {
			m_playerControls.bindingMask = InputBinding.MaskByGroup(method);
		}
		m_playerControls.Player.Enable();
		m_playerControls.UI.Enable();
	}

	private static void Shutdown() {
		if (m_playerControls == null) return;
		m_playerControls.Player.Disable();
		m_playerControls.UI.Disable();
		m_playerControls.Dispose();
		m_playerControls = null;
	}
}
