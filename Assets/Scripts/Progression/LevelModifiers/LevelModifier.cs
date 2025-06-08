using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/LevelModifier", fileName = "LevelModifier")]
public class LevelModifier : ScriptableObject {

    [SerializeField] protected string m_modifierName = "";
	public string ModifierName => m_modifierName;

	[TextArea(4, 25)]
	[SerializeField] private string m_description;
	public string Description => m_description;

}
