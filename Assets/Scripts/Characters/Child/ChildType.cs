using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Child Type", menuName = "Game/Child Type")]
public class ChildType : ScriptableObject {
	public Color color;
	public Sprite shape;
	public Sprite uiIcon;
}
