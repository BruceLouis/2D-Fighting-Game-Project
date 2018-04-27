using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControls : MonoBehaviour {

	private Animator animator;
	private Character character;
	private FeiLong feiLong;
	private Opponent opponent;
	private Player player;
	private SharedProperties sharedProperties;
	
	// Use this for initialization
	
	void Start () {
		if (GetComponent<Opponent>() != null){
			opponent = GetComponent<Opponent>();
		}
		else if (GetComponent<Player>() != null){
			player = GetComponent<Player>();
		}
		animator = GetComponentInChildren<Animator>();
		character = GetComponentInChildren<Character>();	
		sharedProperties = GetComponent<SharedProperties>();	
		if (GetComponentInChildren<FeiLong>() != null){
			feiLong = GetComponentInChildren<FeiLong>();
		}
	}
	
	public void AIThrow(){
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isAttacking") == false
		    && animator.GetBool("isMidAirRecovering") == false && animator.GetBool("isThrown") == false){	
			AIStand();
			character.SetBackPressed(false);
			character.AttackState();
			animator.Play("ThrowStartup");
		}
	}
	
	public void AIJab(int maxNum){
		int crouchOrStand = Random.Range(0, maxNum);
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isAttacking") == false
		    && animator.GetBool("isMidAirRecovering") == false && animator.GetBool("isThrown") == false){					
			character.AttackState();
			if (crouchOrStand == 0){
				AIStand();
			}	
			else{
				AICrouch();
			}	
			if (character.GetComponent<FeiLong>() != null){						
				if (sharedProperties.GetDistanceFromOtherFighter() < 0.75f && animator.GetBool("isStanding") == true){
					feiLong.FeiLongCloseJab();
				}
				else{							
					character.CharacterJab();
				}
			}
			else{
				character.CharacterJab();
			}
			AIStand();
		}
	}
	
	public void AIShort(){
		int crouchOrStand = Random.Range(0, 2);
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isAttacking") == false
		    && animator.GetBool("isMidAirRecovering") == false && animator.GetBool("isThrown") == false){			
			character.AttackState();
			if (crouchOrStand == 0){
				AIStand();
			}	
			else{
				AICrouch();
			}	
			character.CharacterShort();
			AIStand();
		}
	}
	
	public void AIStrong(int maxNum){
		int crouchOrStand = Random.Range(0, maxNum);
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isAttacking") == false
		    && animator.GetBool("isMidAirRecovering") == false && animator.GetBool("isThrown") == false){			
			character.AttackState();
			if (crouchOrStand == 0){
				AIStand();
			}	
			else{
				AICrouch();
			}	
			if (character.GetComponent<FeiLong>() != null){						
				if (sharedProperties.GetDistanceFromOtherFighter() < 0.75f && animator.GetBool("isStanding") == true){
					feiLong.FeiLongCloseStrong();
				}
				else{							
					character.CharacterStrong();
				}
			}
			else{
				character.CharacterStrong();
			}
			AIStand();
		}
	}
	
	public void AIForward(int maxNum){
		int crouchOrStand = Random.Range(0, maxNum);
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isAttacking") == false
		    && animator.GetBool("isMidAirRecovering") == false && animator.GetBool("isThrown") == false){			
			character.AttackState();
			if (crouchOrStand == 0){
				AIStand();
			}	
			else{
				AICrouch();
			}	
			character.CharacterForward();
			AIStand();
		}
	}
	
	public void AIFierce(int maxNum, int standNum){
		int crouchOrStand = Random.Range(0, maxNum);
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isAttacking") == false
		    && animator.GetBool("isMidAirRecovering") == false && animator.GetBool("isThrown") == false){					
			character.AttackState();
			if (crouchOrStand <= standNum){
				AIStand();
			}	
			else{
				AICrouch();
			}	
			if (character.GetComponent<FeiLong>() != null){						
				if (sharedProperties.GetDistanceFromOtherFighter() < 0.75f && animator.GetBool("isStanding") == true){
					feiLong.FeiLongCloseFierce();
				}
				else{							
					character.CharacterFierce();
				}
			}
			else{
				character.CharacterFierce();
			}
			AIStand();
		}
	}
	
	public void AIJumpFierce(){
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isThrown") == false
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isAttacking") == false
		    && animator.GetBool("isMidAirRecovering") == false && animator.GetBool("isAirborne") == true){					
			character.AttackState();
			character.CharacterFierce();
		}
	}
	
	public void AIJumpRoundhouse(){
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isThrown") == false
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isAttacking") == false
		    && animator.GetBool("isMidAirRecovering") == false && animator.GetBool("isAirborne") == true){					
			character.AttackState();
			character.CharacterRoundhouse();
		}
	}
	
	public void AILowForward(){
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isAttacking") == false
		    && animator.GetBool("isMidAirRecovering") == false && animator.GetBool("isThrown") == false){	
			character.AttackState();
			AICrouch();
			character.CharacterForward();
			AIStand ();
		}
	}
	
	public void AISweep(){
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isAttacking") == false
		    && animator.GetBool("isMidAirRecovering") == false && animator.GetBool("isThrown") == false){	
			character.AttackState();
			AICrouch();
			character.CharacterRoundhouse();
			AIStand ();
		}
	}
	
	public void AIJump(){		
		if (animator.GetBool("isAttacking") == false && animator.GetBool("isInHitStun") == false
		    && animator.GetBool("isInBlockStun") == false && animator.GetBool("isLiftingOff") == false
		    && animator.GetBool("isStanding") == true && animator.GetBool("isAirborne") == false
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isMidAirRecovering") == false
		    && animator.GetBool("isThrown") == false){
			AIStand();
			if (player != null){
				character.CharacterJump(player.GetForwardPressed, player.GetBackPressed);
			}
			else{
				character.CharacterJump(opponent.GetForwardPressed, opponent.GetBackPressed);
			}			
			animator.SetBool ("isStanding",false);
			animator.SetBool ("isLiftingOff",true);
		}
	}
	
	public void DoesAIBlock(){
		int coinflip = Random.Range(0,6);
		if (coinflip >= 2){
			character.SetBackPressed(true);
		}
		else{
			character.SetBackPressed(false);
		}
	}
	
	public void AIStand(){
		animator.SetBool("isStanding", true);
		animator.SetBool("isCrouching", false);
		if (player != null){
			player.GetDownPressed = false;
		}
		else{
			opponent.GetDownPressed = false;
		}
	}	
	
	public void AICrouch(){
		animator.SetBool("isStanding", false);
		animator.SetBool("isCrouching", true);
		if (player != null){
			player.GetDownPressed = true;
		}
		else{
			opponent.GetDownPressed = true;
		}
	}
	
	public void AIPressedForward(){
		if (player != null){
			player.GetForwardPressed = true;
			player.GetBackPressed = false;
		}
		else{			
			opponent.GetForwardPressed = true;
			opponent.GetBackPressed = false;
		}
	}
	
	public void AIPressedBackward(){
		if (player != null){
			player.GetBackPressed = true;
			player.GetForwardPressed = false;
		}
		else{			
			opponent.GetBackPressed = true;
			opponent.GetForwardPressed = false;
		}
	}	
	
	public void AICharges(){
		if (player != null){
			player.GetForwardPressed = false;
			player.GetBackPressed = true;
		}
		else{			
			opponent.GetForwardPressed = false;
			opponent.GetBackPressed = true;
		}
	}
	
}
