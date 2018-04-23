using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeiLongAI : MonoBehaviour {

	private Animator animator;
	private Player player;
	private Character playerCharacter;
	private Character character;
	private Opponent AIcontroller;
	private int decision;
	
	// Use this for initialization
	void Start () {		
		
		player = FindObjectOfType<Player>();
		playerCharacter = player.GetComponentInChildren<Character>();
		character = GetComponentInChildren<Character>();
		animator = GetComponentInChildren<Animator>();	
		AIcontroller = GetComponent<Opponent>();
		
		decision = Random.Range(0,100);	
	}
		
	
	public void Behaviors(){
		decision = Random.Range(0,100); 
		if (animator.GetBool("isLiftingOff") == false && animator.GetBool("isKnockedDown") == false && animator.GetBool("isThrown") == false 
		 	&& animator.GetBool("isMidAirRecovering") == false && animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false){
			if (animator.GetBool("rekkaKenActive")){
				decision = Random.Range(0,200);
				if (playerCharacter.GetHitStunned() == true){
					if (decision <= 40){
						AIRekkaKens();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
				}
				else{ 
					if (decision <= 2){
						AIRekkaKens();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
				}
			}	
			else if (animator.GetBool("isAirborne") == true && animator.GetBool("isLiftingOff") == false){
				if (decision <= 1){
					AIcontroller.AIJumpFierce();
					AIcontroller.CharacterWalkState();
				}
				else if (decision <= 6 && decision > 1){
					AIcontroller.AIJumpRoundhouse();
					AIcontroller.CharacterWalkState();
				}				
			}
			else if (AIcontroller.GetDistanceFromPlayer() < 1f){
				if (playerCharacter.GetHitStunned() == true){
					if (decision <= 60){
						AIRekkaKens();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if (decision <= 70 && decision > 60){
						AIcontroller.AIFierce(10,8);
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else {
						AIShienKyakus();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
				}
				else if (playerCharacter.GetBlockStunned() == true){
					if (decision <= 30){
						AIcontroller.AIJab(8);
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if (decision <= 50 && decision > 30){
						AIcontroller.AIStrong(2);
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if (decision <= 60 && decision > 50){
						AIcontroller.AIFierce(10,8);
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if (decision <= 80 && decision > 60){
						AIRekkaKens();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if (decision <= 85 && decision > 80){
						AIShortShienKyakus();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else {
						AIRekkaKuns();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
				}
				else if (playerCharacter.GetKnockDown() == true && playerCharacter.GetAirborne() == false){
					
					if (decision <= 60){
						AIcontroller.AIStand();
						AIcontroller.AIPressedForward();
						AIcontroller.DoesAIBlock();		
					}
					else if (decision <= 63 && decision > 60){
						AIcontroller.AIPressedForward();
						AIcontroller.AIJump();
						character.SetBackPressed(false);
					}
					else if (decision <= 75 && decision > 63){
						AIcontroller.CharacterWalkState();
						AIcontroller.AIJump();
						character.SetBackPressed(false);
					}
					else if (decision <= 90 && decision > 75){
						AIRekkaKuns();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else {
						AIRekkaKens();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
				}
				else{
					if (decision <= 10){
						AIcontroller.AIStand();
						AIcontroller.AIPressedForward();
						character.SetBackPressed(false);			
					}
					else if (decision <= 13 && decision > 10){
						AIcontroller.AIFierce(10,8);
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if (decision <= 16 && decision > 13){
						AIcontroller.AIStrong(2);
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if (decision <= 22 && decision > 16){
						AIcontroller.AIJab(8);
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if (decision <= 23 && decision > 22){
						AIcontroller.AIShort();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if (decision <= 25 && decision > 23){
						AIcontroller.AIForward(2);
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if (decision <= 30 && decision > 25){
						AIcontroller.CharacterWalkState();
						AIcontroller.AIJump();
						character.SetBackPressed(false);
					}
					else if (decision <= 32 && decision > 30){
						AIcontroller.AIPressedForward();
						AIcontroller.AIJump();
						character.SetBackPressed(false);
					}
					else if (decision <= 35 && decision > 32){
						AIcontroller.AISweep();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}			
					else if (decision <= 40 && decision > 35){
						AIThrow();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}		
					else if (decision <= 45 && decision > 40){
						AIRekkaKens();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if (decision <= 48 && decision > 45){
						AIShortShienKyakus();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if (decision <= 50 && decision > 48){
						AIRekkaKuns();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else{
						AIcontroller.AICrouch();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
				}
			}	
			else if (AIcontroller.GetDistanceFromPlayer() < 2f && AIcontroller.GetDistanceFromPlayer() >= 1f){
				
				if (playerCharacter.GetKnockDown() == true){				
					if (decision <= 60){
						AIcontroller.AIStand();
						AIcontroller.AIPressedForward();
						AIcontroller.DoesAIBlock();		
					}
					else if (decision <= 75 && decision > 60){
						AIcontroller.AIPressedForward();
						AIcontroller.AIJump();
						character.SetBackPressed(false);
					}
					else if (decision <= 90 && decision > 75){
						AIRekkaKuns();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else {
						AIRekkaKens();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
				}
				else {						
					if (decision <= 30){
						AIcontroller.AIStand();
						AIcontroller.AIPressedForward();
						AIcontroller.DoesAIBlock();		
					}			
					else if (decision <= 34 && decision > 30){
						AIcontroller.AIFierce(2,0);
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if (decision <= 37 && decision > 34){
						AIcontroller.AIStrong(2);
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if (decision <= 39 && decision > 37){
						AIcontroller.AIJab(2);
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if (decision <= 40 && decision > 39){
						AIcontroller.AIShort();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if (decision <= 50 && decision > 40){
						AIcontroller.AIPressedForward();
						AIcontroller.AIJump();
						character.SetBackPressed(false);
					}
					else if (decision <= 55 && decision > 50){
						AIcontroller.CharacterWalkState();
						AIcontroller.AIJump();
						character.SetBackPressed(false);
					}
					else if (decision <= 65 && decision > 55){
						AIRekkaKens();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if (decision <= 75 && decision > 65){
						AIRekkaKuns();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if (decision <= 80 && decision > 75){
						AIcontroller.AIStand();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();		
					}
					else{
						AIcontroller.AICrouch();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
				}
			}
			else{
				if (decision <= 50){
					AIcontroller.AIStand();
					AIcontroller.AIPressedForward();
					AIcontroller.DoesAIBlock();		
				}			
				else if (decision <= 65 && decision > 50){
					AIcontroller.AIPressedForward();
					AIcontroller.AIJump();
					character.SetBackPressed(false);
				}
				else if (decision <= 70 && decision > 65){
					AIcontroller.CharacterWalkState();
					AIcontroller.AIJump();
					character.SetBackPressed(false);
				}
				else if (decision <= 85 && decision > 70){
					AIRekkaKuns();
					AIcontroller.CharacterWalkState();
					AIcontroller.DoesAIBlock();
				}
				else if (decision <= 90 && decision > 85){
					AIcontroller.AIStand();
					AIcontroller.CharacterWalkState();
					AIcontroller.DoesAIBlock();		
				}
				else{
					AIcontroller.AICrouch();
					AIcontroller.CharacterWalkState();
					AIcontroller.DoesAIBlock();
				}
			}
			AIcontroller.CharacterWalkState();
		}			
		AIcontroller.Walk();	
	}
	
	void AIThrow(){
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isAttacking") == false
		    && animator.GetBool("isMidAirRecovering") == false && animator.GetBool("isThrown") == false){	
			AIcontroller.AIStand();
			character.SetBackPressed(false);
			character.AttackState();
			animator.Play("FeiLongThrowStartup");
		}
	}
	
	void AIRekkaKens(){
		int rekkaPunch = Random.Range(0,3);
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isMidAirRecovering") == false
		    && animator.GetBool("isThrown") == false){
			
			animator.SetTrigger("hadoukenInputed");			
			if (animator.GetBool("isAttacking") == false){
				AIcontroller.AIStand();
				character.AttackState();
				animator.Play("FeiLongRekkaKenFirstAttack",0);
				if (rekkaPunch == 0){
					animator.SetInteger("rekkaPunchType", 0);
				}
				else if (rekkaPunch == 1){
					animator.SetInteger("rekkaPunchType", 1);
				}
				else{				 
					animator.SetInteger("rekkaPunchType", 2);
				}
			}
		}
	}
	
	void AIShortShienKyakus(){
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isMidAirRecovering") == false
		    && animator.GetBool("isThrown") == false){
			
			animator.SetTrigger("reverseShoryukenInputed");			
			if (animator.GetBool("isAttacking") == false){
				AIcontroller.AIStand();
				character.AttackState();
				animator.Play("FeiLongShienKyakuShort",0);
				animator.SetInteger("shienKyakuKickType", 0);
			}
		}
	}
		
	void AIRekkaKuns(){
		int rekkaKunKick = Random.Range(0,3);
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isMidAirRecovering") == false
		    && animator.GetBool("isThrown") == false){
			
			animator.SetTrigger("shoryukenInputed");			
			if (animator.GetBool("isAttacking") == false){
				AIcontroller.AIStand();
				character.AttackState();
				animator.Play("FeiLongRekkaKun",0);
				if (rekkaKunKick == 0){
					animator.SetInteger("rekkaKunKickType", 0);
				}
				else if (rekkaKunKick == 1){
					animator.SetInteger("rekkaKunKickType", 1);
				}
				else{				 
					animator.SetInteger("rekkaKunKickType", 2);
				}
			}
		}
	}
	
	void AIShienKyakus(){
		int shienKyakuKick = Random.Range(0,3);
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isMidAirRecovering") == false
		    && animator.GetBool("isThrown") == false){
			
			animator.SetTrigger("reverseShoryukenInputed");			
			if (animator.GetBool("isAttacking") == false){
				AIcontroller.AIStand();
				character.AttackState();
				if (shienKyakuKick == 0){
					animator.Play("FeiLongShienKyakuShort",0);
					animator.SetInteger("shienKyakuKickType", 0);
				}
				else if (shienKyakuKick == 1){
					animator.Play("FeiLongShienKyakuForward",0);
					animator.SetInteger("shienKyakuKickType", 1);
				}
				else{				 
					animator.Play("FeiLongShienKyakuRoundhouse",0);
					animator.SetInteger("shienKyakuKickType", 2);
				}
			}
		}
	}
}
