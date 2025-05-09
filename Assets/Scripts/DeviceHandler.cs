using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class DeviceHandler : MonoBehaviour
{
	private IDisposable m_EventListener;
    private void OnEnable() {
	    m_EventListener =
		    InputSystem.onAnyButtonPress
			    .Call(OnButtonPressed);
    }

    private void OnDisable()
    {
	    m_EventListener.Dispose();
    }

    private void OnControlsChanged(PlayerInput playerInput) {
	    string deviceClass = playerInput.currentControlScheme;

	    if (deviceClass.Equals("PC"))
	    {
		    InputSubscribe.ChangeInputMethod(InputSubscribe.Method.PC);
	    }
	    else
	    {
		    InputSubscribe.ChangeInputMethod(InputSubscribe.Method.Console);
	    }
    }

    private void OnButtonPressed(InputControl button)
    {
        string deviceClass = button.device.description.deviceClass;

        if (deviceClass.Equals("Keyboard") || deviceClass.Equals("Mouse"))
        {

            InputSubscribe.ChangeInputMethod(InputSubscribe.Method.PC);
        }
        else
        {
            InputSubscribe.ChangeInputMethod(InputSubscribe.Method.Console);
        }

        // Optional: Log the device that triggered the switch
        Debug.Log($"Input from device: {button.device.name} ({deviceClass}). Switching to {InputSubscribe.InputMethod} method.");
    }

}
