using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thrown : StateMachineBehaviour {

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.SetBool("isAttacking", false);
        animator.SetBool("isSweeping", false);
        animator.SetBool("isWalkingBackward", false);
		animator.SetBool("isWalkingForward", false);
		animator.SetBool("isLiftingOff", false);
        animator.SetBool("isLanding", false);
        animator.SetBool("hurricaneKickActive", false);
		animator.SetBool("isRolling", false);
		animator.SetBool("isMidAirHit", false);		
		animator.SetBool("isThrown", true);
		animator.SetInteger("hyakkishuAttackType", 0);
		animator.ResetTrigger("shoryukenInputed");	
		animator.ResetTrigger("reverseShoryukenInputed");	
		animator.ResetTrigger("hadoukenInputed");
        animator.ResetTrigger("upperTigerShotInputed");
        animator.ResetTrigger("lowerTigerShotInputed");
        animator.ResetTrigger("tigerUppercutInputed");
        animator.ResetTrigger("tigerKneeInputed");
        animator.ResetTrigger("hurricaneKickInputed");
		animator.ResetTrigger("rollInputed");
		animator.ResetTrigger("dashStraightInputed");
		animator.ResetTrigger("dashLowInputed");	
		animator.ResetTrigger("kickRushInputed");
		animator.ResetTrigger("turnPunchInputed");
		animator.ResetTrigger("headButtInputed");
		animator.ResetTrigger("hyakkishuInputed");
		animator.ResetTrigger("motionSuperInputed");
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

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
