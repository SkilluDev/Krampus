using System.Collections;
using System.Collections.Generic;
using LitMotion;
using LitMotion.Adapters;
using UnityEngine;

public class PauseButtons : MonoBehaviour {

	private bool m_settingsOpen = false;
	[SerializeField] private RectTransform m_inventory;
	[SerializeField] private RectTransform m_inventoryContainer;

	[SerializeField] private CanvasGroup m_settings;

	public void GoBackToMenu() {
		Game.PogMan.GoBackToMenu();
	}

	public void Restart() {
		Game.PogMan.LoadFirstLevel(false);
	}

	public void RestartAndRegen() {
		Game.PogMan.LoadFirstLevel(true);
	}

	public void GoBackToGame() {
		InputHandler.SwitchPause();
	}

	public void ToggleSettings() {
		m_settingsOpen = !m_settingsOpen;
		if (m_settingsOpen) {
			KrampMotions.ShowAlpha(m_settings, 0.5f, unscaledTime: true);
		} else {
			KrampMotions.HideAlpha(m_settings, 0.5f, unscaledTime: true);
		}
	}
	
	public void ShowHideInventory() {
		
		m_inventory.gameObject.SetActive(!m_inventory.gameObject.activeSelf);
		
		foreach (Transform child in m_inventoryContainer) {
			child.gameObject.GetComponent<InventoryCard>().Hide();
		}
	 }
}
