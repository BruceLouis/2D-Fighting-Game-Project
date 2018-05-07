using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockStun : StateMachineBehaviour {

	private float timer;
	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
		animator.SetBool("isAttacking", false);
		animator.SetBool("isWalkingBackward", false);
		animator.SetBool("isWalkingForward", false);
		timer = animator.GetFloat ("blockStunTimer");
	}
	
	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//decrement timer, then set it to the timer parameter in the animator
		timer -= Time.deltaTime * 15f; //15f is to make the timer go faster so characters don't get stuck in blockstun forever
		animator.SetFloat("blockStunTimer", timer);		
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
