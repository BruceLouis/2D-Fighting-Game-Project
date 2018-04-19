using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStun : StateMachineBehaviour {

	private float timer;
	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
		animator.SetBool("isAttacking", false);
		animator.SetBool("isWalkingBackward", false);
		animator.SetBool("isWalkingForward", false);
		animator.SetBool("isLiftingOff", false);
		animator.SetBool("isRolling", false);
		timer = animator.GetFloat ("hitStunTimer");
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		timer--;
		animator.SetFloat("hitStunTimer", timer);		
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
