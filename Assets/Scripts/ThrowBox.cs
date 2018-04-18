﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBox : MonoBehaviour {

	public AudioClip getThrownSound;

	float distance;
	
	void OnTriggerEnter2D(Collider2D collider){		
		HurtBox hurtBox = collider.gameObject.GetComponentInChildren<HurtBox>();		
		Animator animator = collider.gameObject.GetComponentInChildren<Animator>();	
		Animator selfAnimator = gameObject.GetComponentInParent<Animator>();
		
		if (hurtBox){	
			Character grabbedCharacter = hurtBox.gameObject.GetComponentInParent<Character>();	
			Character throwingCharacter = gameObject.GetComponentInParent<Character>();		
			distance = grabbedCharacter.transform.position.x - throwingCharacter.transform.position.x;	
			if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
				&& animator.GetBool("isKnockedDown") == false && animator.GetBool("isMidAirRecovering") == false	
			    && animator.GetBool("isAirborne") == false){
				
				AudioSource.PlayClipAtPoint(getThrownSound, transform.position);
				if (distance > 0){
					grabbedCharacter.side = Character.Side.P2;
					grabbedCharacter.SideSwitch();
				}
				else{
					grabbedCharacter.side = Character.Side.P1;
					grabbedCharacter.SideSwitch();
				}
				animator.SetBool("isThrown", true);
				selfAnimator.SetBool("throwTargetAcquired", true);
				animator.Play("KenThrown");
			}
		}
	}
}
