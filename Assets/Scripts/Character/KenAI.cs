using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KenAI : MonoBehaviour {
		
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
	}
	
	public void Behaviors(){
		decision = Random.Range(0,100);
		if (animator.GetBool("isLiftingOff") == false && animator.GetBool("isKnockedDown") == false && animator.GetBool("isThrown") == false 
		    && animator.GetBool("isMidAirRecovering") == false && animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false){
			if (animator.GetBool("isAirborne") == true && animator.GetBool("isLiftingOff") == false){
				decision = Random.Range(0,100);
				if (decision <= 3){
					AIcontroller.AIJumpFierce();
					AIcontroller.CharacterWalkState();
				}
				else if (decision <= 6 && decision > 3){
					AIcontroller.AIJumpRoundhouse();
					AIcontroller.CharacterWalkState();
				}				
			}
			else if (AIcontroller.GetDistanceFromPlayer() >= 2f){
			
				if (decision <= 30){
					AIcontroller.AIPressedForward();
					character.SetBackPressed(false);
				}
				else if (decision <= 35 && decision > 30){
					AIcontroller.AIPressedBackward();
					character.SetBackPressed(true);
				}
				else if (decision <= 65 && decision > 35){
					AIHadoukens();
					AIcontroller.CharacterWalkState();
					AIcontroller.DoesAIBlock();
				}
				else if (decision <= 68 && decision > 65){
					AIcontroller.AIShort();
					AIcontroller.CharacterWalkState();
					AIcontroller.DoesAIBlock();
				}
				else if (decision <= 71 && decision > 68){
					AIcontroller.AIJab(2);
					AIcontroller.CharacterWalkState();
					AIcontroller.DoesAIBlock();
				}
				else if (decision <= 75 && decision > 71){
					AIcontroller.AIJump();
					AIcontroller.DoesAIBlock();
				}
				else if (decision <= 90 && decision > 75){
					AIcontroller.AIPressedForward();
					AIcontroller.AIJump();
					character.SetBackPressed(false);
				}
				else{
					AIcontroller.CharacterWalkState();
					character.SetBackPressed(false);
				}
			}
			else if (AIcontroller.GetDistanceFromPlayer() < 2f && AIcontroller.GetDistanceFromPlayer() >= 0.75f){	
			
				if (playerCharacter.GetBlockStunned() == true){
					if (decision <= 30){
						AIHadoukens();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if (decision <= 40 && decision > 30){
						AIcontroller.AILowForward();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if (decision <= 45 && decision > 40){
						AIcontroller.AIStrong(10);
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if (decision <= 50 && decision > 45){
						AIcontroller.AIJab(2);
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}					
					else if (decision <= 55 && decision > 50){
						AIcontroller.AIShort();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}			
					else if (decision <= 65 && decision > 55){
						AIcontroller.AISweep();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}			
					else{
						AIcontroller.AICrouch();
						AIcontroller.CharacterWalkState();
						character.SetBackPressed(true);
					}
				}
				else{
					if (decision <= 20){
						AIcontroller.AIPressedForward();
						character.SetBackPressed(false);			
					}
					else if(decision <= 35 && decision > 20){
						AIHadoukens();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if(decision <= 37 && decision > 35){
						AIcontroller.AIForward(10);
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if(decision <= 42 && decision > 37){
						AIcontroller.AIStrong(10);
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if(decision <= 52 && decision > 42){
						AIcontroller.AICrouch();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if(decision <= 57 && decision > 52){
						AIcontroller.AIStand();
						AIcontroller.CharacterWalkState();
						character.SetBackPressed(false);
					}
					else if(decision <= 67 && decision > 57){
						AIcontroller.AIPressedForward();
						AIcontroller.AIJump();
						character.SetBackPressed(false);
					}
					else if(decision <= 70 && decision > 67){
						AIcontroller.AIJump();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if(decision <= 73 && decision > 70){
						AIRolls();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if(decision <= 75 && decision > 73){
						AIHurricaneKicks();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if(decision <= 80 && decision > 75){
						AIFootsieLowForward();
						character.SetBackPressed(false);
					}
					else if(decision <= 85 && decision > 80){
						AIcontroller.AISweep();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if(decision <= 90 && decision > 85){
						AIcontroller.AIShort();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if(decision <= 95 && decision > 90){
						AIcontroller.AIJab(2);
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else{
						AIcontroller.AIPressedBackward();
						character.SetBackPressed(true);
					}
				}
			}
			else{
				//anti air
				if (playerCharacter.GetAirborne() == true && playerCharacter.GetKnockDown() == false){
					if (decision <= 60){
						AIShoryukens();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else{
						AIcontroller.AIFierce(2,0);
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
				}
				else if (playerCharacter.GetHitStunned() == true){
					if (decision <= 40){
						AIShoryukens();
						AIcontroller.CharacterWalkState();
					}
					else if (decision <= 70 && decision > 40){
						AIHurricaneKicks();
						AIcontroller.CharacterWalkState();
					}
					else if (decision <= 85 && decision > 70){
						AIcontroller.AIFierce(2,0);
						AIcontroller.CharacterWalkState();
					}
					else{
						AIcontroller.AISweep();
						AIcontroller.CharacterWalkState();
					}
				}
				else if (playerCharacter.GetKnockDown() == true && playerCharacter.GetAirborne() == false){
					if (decision <= 60){
						AIcontroller.AICrouch();
						AIcontroller.CharacterWalkState();
						character.SetBackPressed(true);
					}
					else if (decision <= 70 && decision > 60){
						AIcontroller.AIJab(2);
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}					
					else if (decision <= 75 && decision > 70){
						AIcontroller.AIShort();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}					
					else if (decision <= 80 && decision > 75){
						AIcontroller.AIPressedBackward();
						character.SetBackPressed(true);
					}				
					else if (decision <= 85 && decision > 80){
						AIcontroller.AIPressedForward();
						character.SetBackPressed(false);
					}				
					else if (decision <= 90 && decision > 85){
						AIJabShoryuken();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}				
					else{
						AIRolls();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
				}
				
				
				else if (playerCharacter.GetBlockStunned() == true){
					if (decision <= 30){
						AIHadoukens();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if (decision <= 45 && decision > 30){
						AIcontroller.AILowForward();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if (decision <= 55 && decision > 45){
						AIcontroller.AIStrong(10);
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if (decision <= 65 && decision > 55){
						AIcontroller.AIJab(2);
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}					
					else if (decision <= 75 && decision > 65){
						AIcontroller.AIShort();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}			
					else{
						AIcontroller.AICrouch();
						AIcontroller.CharacterWalkState();
						character.SetBackPressed(true);
					}
				}
				
				else{
					if (decision <= 50){
						AIcontroller.AICrouch();
						AIcontroller.CharacterWalkState();
						character.SetBackPressed(true);
					}
					else if (decision <= 55 && decision > 50){
						AIcontroller.AILowForward();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if (decision <= 60 && decision > 55){
						AIcontroller.AIStrong(10);
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if (decision <= 65 && decision > 60){
						AIcontroller.AIJab(2);
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}					
					else if (decision <= 70 && decision > 65){
						AIcontroller.AIShort();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if (decision <= 73 && decision > 70){
						AIJabShoryuken();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if (decision <= 78 && decision > 73){
						AIThrow();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}
					else if (decision <= 82 && decision > 78){
						AIHadoukens();
						AIcontroller.CharacterWalkState();
						AIcontroller.DoesAIBlock();
					}				
					else if (decision <= 90 && decision > 82){
						AIcontroller.AIPressedForward();
						character.SetBackPressed(false);
					}
					else{
						AIcontroller.AIPressedBackward();
						character.SetBackPressed(true);
					}
				}
				AIcontroller.CharacterWalkState();
			}
			AIcontroller.Walk();	
		}			
	}
	
	
	void AIThrow(){
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isAttacking") == false
		    && animator.GetBool("isMidAirRecovering") == false && animator.GetBool("isThrown") == false){	
			AIcontroller.AIStand();
			character.SetBackPressed(false);
			character.AttackState();
			animator.Play("KenThrowStartup");
		}
	}
	
	
	void AIFootsieLowForward(){
		int distance = Random.Range(0, 30);
		distance--;
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isAttacking") == false
		    && animator.GetBool("isMidAirRecovering") == false && animator.GetBool("isThrown") == false){	
			AIcontroller.AIPressedForward();
			character.SetBackPressed(false);
			if (distance <= 0){		
				character.AttackState();
				AIcontroller.AICrouch();
				character.CharacterForward();
				AIcontroller.AIStand ();
			}
		}
	}	
	
	void AIJabShoryuken(){
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isMidAirRecovering") == false
		    && animator.GetBool("isThrown") == false){
			
			animator.SetTrigger("shoryukenInputed");			
			if (animator.GetBool("isAttacking") == false){
				AIcontroller.AIStand();
				character.AttackState();
				animator.Play("KenShoryukenJab",0);
				animator.SetInteger("shoryukenPunchType", 0);
			}
		}
	}
	
	void AIShoryukens(){
		int shoryukenPunch = Random.Range(0,3);
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isMidAirRecovering") == false
		    && animator.GetBool("isThrown") == false){
			
			animator.SetTrigger("shoryukenInputed");			
			if (animator.GetBool("isAttacking") == false){
				AIcontroller.AIStand();
				character.AttackState();
				if (shoryukenPunch == 0){
					animator.Play("KenShoryukenJab",0);
					animator.SetInteger("shoryukenPunchType", 0);
				}
				else if (shoryukenPunch == 1){
					animator.Play("KenShoryukenStrong",0);
					animator.SetInteger("shoryukenPunchType", 1);
				}
				else{				 
					animator.Play("KenShoryukenFierce",0);
					animator.SetInteger("shoryukenPunchType", 2);
				}
			}
		}
	}
	
	void AIHadoukens(){		
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isMidAirRecovering") == false
		    && animator.GetBool("isThrown") == false){
			
			animator.SetTrigger("hadoukenInputed");
			
			if(animator.GetBool("isAttacking") == false){			
				AIcontroller.AIStand();	
				character.AttackState();
				animator.Play("KenHadouken",0);		
				animator.SetInteger("hadoukenPunchType", Random.Range(0,3));
				animator.SetInteger("hadoukenOwner", 2);
			}
		}
	}
	
	void AIHurricaneKicks(){			
		int hurricaneType = Random.Range(0,3);
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isMidAirRecovering") == false
		    && animator.GetBool("isThrown") == false){	
			
			animator.SetTrigger("hurricaneKickInputed");	
			
			if (animator.GetBool("isAttacking") == false){
				AIcontroller.AIStand();
				character.AttackState();
				animator.SetInteger("hurricaneKickType", hurricaneType);
				animator.Play("KenHurricaneKickLiftOff",0);
			}
		}
	}
	
	void AIRolls(){
		int rollType = Random.Range (0,3);
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isMidAirRecovering") == false
		    && animator.GetBool("isThrown") == false){
			
			animator.SetTrigger("rollInputed");
			if (animator.GetBool("isAttacking") == false){
				AIcontroller.AIStand();
				character.AttackState();
				animator.SetInteger("rollKickType", rollType);
				animator.Play("KenRoll");
			}
		}
	}
}
