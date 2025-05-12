using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Button Set", menuName = "Game/Button Set")]
public class ButtonSet : ScriptableObject {

	public string setName;
	public InputSubscribe.Method method;

	public  Sprite backToMenu_Button;
	public Sprite restart_Button;
	public Sprite pause_Button;
	public Sprite sneak_Button;
	public Sprite attack_Button;


}
