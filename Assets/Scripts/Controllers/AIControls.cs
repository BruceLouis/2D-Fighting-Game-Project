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
		animator = GetComponentInChildren<Animator>();
		character = GetComponentInChildren<Character>();	
		sharedProperties = GetComponent<SharedProperties>();	
		if (GetComponentInChildren<FeiLong>() != null){
			feiLong = GetComponentInChildren<FeiLong>();
		}
	}
	
	public void AIThrow(){
		if (GetConditions()){					
			AIStand();
			character.SetBackPressed(false);
			character.AttackState();
			animator.Play("ThrowStartup");
		}
	}
	
	public void AIJab(int maxNum){
		int crouchOrStand = Random.Range(0, maxNum);
		if (GetConditions()){					
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
	
	public void AIShort(int maxNum){
		int crouchOrStand = Random.Range(0, maxNum);
		if (GetConditions()){			
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
		if (GetConditions()){			
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
		if (GetConditions()){			
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

    public void AIRoundhouse(int cutOff, int maxNum){
		int crouchOrStand = Random.Range(0, maxNum);
		if (GetConditions()){			
			character.AttackState();
			if (crouchOrStand <= cutOff){
				AICrouch();
			}	
			else{
				AIStand();
			}	
			character.CharacterRoundhouse();
			AIStand();
		}
	}
	
	public void AIFierce(int maxNum, int standNum){
		int crouchOrStand = Random.Range(0, maxNum);
		if (GetConditions()){					
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
		if (GetConditionsAirborneAttack()) {					
			character.AttackState();
			character.CharacterFierce();
		}
	}
	
	public void AIJumpRoundhouse(){
		if (GetConditionsAirborneAttack()){					
			character.AttackState();
			character.CharacterRoundhouse();
		}
	}
	
	public void AILowForward(){
		if (GetConditions()){	
			character.AttackState();
			AICrouch();
			character.CharacterForward();
			AIStand ();
		}
	}
	
	public void AISweep(){
		if (GetConditions()){	
			character.AttackState();
			AICrouch();
			character.CharacterRoundhouse();
			AIStand ();
		}
	}
	
	public void AIJump(){		
		if (GetConditions () && animator.GetBool("isStanding") == true){		    
			AIStand();
			character.CharacterJump(sharedProperties.GetForwardPressed, sharedProperties.GetBackPressed);
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
	
	
	public void AIWalks(){
		if (GetConditions() && animator.GetBool("isStanding") == true && animator.GetBool("isCrouching") == false){
			if (sharedProperties.GetForwardPressed == true && animator.GetBool("isWalkingBackward") == false){
				animator.SetBool("isWalkingForward", true);	
				
				if (character.side == Character.Side.P1){
					character.transform.Translate(Vector3.right * character.GetWalkSpeed() * Time.deltaTime);	
				}
				else{
					character.transform.Translate(Vector3.left * character.GetWalkSpeed() * Time.deltaTime);	
				}
			}
			else if (sharedProperties.GetBackPressed == true && animator.GetBool("isWalkingForward") == false){
				animator.SetBool("isWalkingBackward", true);				
				if (character.side == Character.Side.P2){
					character.transform.Translate(Vector3.right * character.GetWalkSpeed() * Time.deltaTime);	
				}
				else{
					character.transform.Translate(Vector3.left * character.GetWalkSpeed() * Time.deltaTime);	
				}
			}	
		}
		if (sharedProperties.GetForwardPressed == false){
			animator.SetBool("isWalkingForward", false);	
		}
		if (sharedProperties.GetBackPressed == false){	
			animator.SetBool("isWalkingBackward", false);	
		}
	}
	
	public void AIStand(){
		animator.SetBool("isStanding", true);
		animator.SetBool("isCrouching", false);
		sharedProperties.GetDownPressed = false;
	}	
	
	public void AICrouch(){
		animator.SetBool("isStanding", false);
		animator.SetBool("isCrouching", true);
		sharedProperties.GetDownPressed = true;
	}
	
	public void AIPressedForward(){
		sharedProperties.GetForwardPressed = true;
		sharedProperties.GetBackPressed = false;
	}
	
	public void AIPressedBackward(){
		sharedProperties.GetBackPressed = true;
		sharedProperties.GetForwardPressed = false;
	}	
	
	public void AICharges(){
		sharedProperties.GetForwardPressed = false;
		sharedProperties.GetBackPressed = true;
	}	
	
	public bool FreeToMakeDecisions (){
		return 	animator.GetBool("isLiftingOff") == false && animator.GetBool("isKnockedDown") == false 
				&& animator.GetBool("isThrown") == false && animator.GetBool("isMidAirHit") == false
				&& animator.GetBool("isMidAirRecovering") == false && animator.GetBool("isInHitStun") == false
				&& animator.GetBool("isInBlockStun") == false && animator.GetBool("isLanding") == false;				
	}										
	
	public bool GetConditionsSpecialAttack (){
		return 	FreeToMakeDecisions() && animator.GetBool ("isAirborne") == false; 
	}

	public bool GetConditionsAirborneAttack (){
		return 	FreeToMakeDecisions() && animator.GetBool ("isAttacking") == false && animator.GetBool ("isAirborne") == true;
	}
	
	public bool GetConditions (){		
		return 	FreeToMakeDecisions() && animator.GetBool ("isAttacking") == false && animator.GetBool ("isAirborne") == false;
	}
	
}
