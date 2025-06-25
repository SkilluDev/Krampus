using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCloseSwiftly : StateMachineBehaviour {
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.SetBool("IsClosingSwiftly", false);
	}

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.SetBool("IsClosingSwiftly", true);
	}
}
