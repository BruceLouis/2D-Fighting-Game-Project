using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalrogAI : MonoBehaviour {

	public float decisionTimer;
	public AudioClip one, two, three;
	
	private Animator animator;
	private Player player;
	private Character playerCharacter;
	private Character character;
	private Opponent AIcontroller;
	private ChargeSystem chargeSystem;
	
	private int decision;
	private float decisionTimerInput;	

	// Use this for initialization
	void Start () {
	
		player = FindObjectOfType<Player>();
		playerCharacter = player.GetComponentInChildren<Character>();
		character = GetComponentInChildren<Character>();
		animator = GetComponentInChildren<Animator>();	
		AIcontroller = GetComponent<Opponent>();
		chargeSystem = GetComponent<ChargeSystem>();
		
		decisionTimerInput = decisionTimer; 
		decision = Random.Range(0,100);	
	}
		
	public void Behaviors(){
		decisionTimer--;
		if (animator.GetBool("isLiftingOff") == false && animator.GetBool("isKnockedDown") == false && animator.GetBool("isThrown") == false && animator.GetBool("isMidAirHit") == false
		    && animator.GetBool("isMidAirRecovering") == false && animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false){
			
			if (animator.GetBool("isAirborne") == true && animator.GetBool("isLiftingOff") == false){				
				decision = Random.Range(0,100);	
				if (decision <= 1){
					AIcontroller.AIJumpFierce();
				}
				else if (decision <= 6 && decision > 1){
					AIcontroller.AIJumpRoundhouse();
				}				
				AIcontroller.AICharges();
				character.SetBackPressed(true);
			}
			else if (character.GetKnockDown() == true){
				AIcontroller.AICrouch();
				AIcontroller.AICharges();
				character.SetBackPressed(true);
			}				
			else if (AIcontroller.GetDistanceFromPlayer() >= 2f){
				if (playerCharacter.GetKnockDown() == true && playerCharacter.GetAirborne() == false){		
					decision = Random.Range (0, 100);
					if (decision <= 40){
						AITurnPunches();
						AIcontroller.AICharges();
						character.SetBackPressed(true);
					}	
					else if (decision <= 45 && decision > 40){
						AIcontroller.AIPressedForward();
						AIcontroller.AIJump();
						character.SetBackPressed(false);
					}
					else if (decision <= 80 && decision > 45){
						AIcontroller.AIStand();
						AIcontroller.AIPressedForward();
						character.SetBackPressed(false);
					}
					else if (decision <= 82 && decision > 80){
						
						if (chargeSystem.GetBackCharged() && !animator.GetBool("isAttacking")){
							AIDashLowJabOrStrong();
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
						else{						
							AIcontroller.AIJab(10);
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
					}	
					else if (decision <= 90 && decision > 82){
						
						if (chargeSystem.GetBackCharged() && !animator.GetBool("isAttacking")){
							AIDashLowFierce();
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
						else{						
							AIcontroller.AIJab(10);
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
					}			
					else if (decision <= 92 && decision > 90){					
						if (chargeSystem.GetBackCharged() && !animator.GetBool("isAttacking")){
							AIKickRushesShortOrForward();
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
						else{						
							AIcontroller.AIStrong(20);
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
					}
					else{						
						if (chargeSystem.GetBackCharged() && !animator.GetBool("isAttacking")){
							AIKickRushesRoundhouse();
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
						else{						
							AIcontroller.AIStrong(20);
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
					}			
				}
				else{	
					DecisionMade (5,1);
					if (playerCharacter.GetComponent<Ken>() != null){
						if (decision <= 15){
							AIcontroller.AIStand();
							AIcontroller.AIPressedForward();
							character.SetBackPressed(false);
						}
						else if (decision <= 20 && decision > 15){
							AIcontroller.AIPressedForward();
							AIcontroller.AIJump();
							character.SetBackPressed(false);
						}			
						else if (decision <= 22 && decision > 20){
							AIcontroller.CharacterNeutralState();
							AIcontroller.AIJump();
							character.SetBackPressed(false);
						}			
						else if (decision <= 65 && decision > 22){
							AIcontroller.AICrouch();
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}			
						else if (decision <= 67 && decision > 65){
						
							if (chargeSystem.GetBackCharged() && !animator.GetBool("isAttacking")){
								AIKickRushesShortOrForward();
								AIcontroller.AICharges();
								character.SetBackPressed(true);
							}
							else{						
								AIcontroller.AIShort();
								AIcontroller.AICharges();
								character.SetBackPressed(true);
							}
						}	
						else if (decision <= 70 && decision > 67){
							
							if (chargeSystem.GetBackCharged() && !animator.GetBool("isAttacking")){
								AIDashLowJabOrStrong();
								AIcontroller.AICharges();
								character.SetBackPressed(true);
							}
							else{						
								AIcontroller.AIJab(4);
								AIcontroller.AICharges();
								character.SetBackPressed(true);
							}
						}
						else if (decision <= 71 && decision > 70){
							
							if (chargeSystem.GetBackCharged() && !animator.GetBool("isAttacking")){
								AIDashStraightJabOrStrong();
								AIcontroller.AICharges();
								character.SetBackPressed(true);
							}
							else{						
								AIcontroller.AIStrong(5);
								AIcontroller.AICharges();
								character.SetBackPressed(true);
							}
						}
						else if (decision <= 85 && decision > 71){
							
							if (chargeSystem.GetDownCharged() && !animator.GetBool("isAttacking")){
								AIHeadButt();
								AIcontroller.AICharges();
								character.SetBackPressed(true);
							}
							else{						
								AIcontroller.AIFierce(1,1);
								AIcontroller.AICharges();
								character.SetBackPressed(true);
							}
						}
						else{
							AITurnPunches();
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
					}
					else{					
						if (decision <= 15){
							AIcontroller.AIStand();
							AIcontroller.AIPressedForward();
							character.SetBackPressed(false);
						}
						else if (decision <= 17 && decision > 15){
							AIcontroller.AIPressedForward();
							AIcontroller.AIJump();
							character.SetBackPressed(false);
						}			
						else if (decision <= 19 && decision > 17){
							AIcontroller.CharacterNeutralState();
							AIcontroller.AIJump();
							character.SetBackPressed(false);
						}			
						else if (decision <= 65 && decision > 19){
							AIcontroller.AICrouch();
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}			
						else if (decision <= 72 && decision > 65){
							
							if (chargeSystem.GetBackCharged() && !animator.GetBool("isAttacking")){
								AIKickRushesShortOrForward();
								AIcontroller.AICharges();
								character.SetBackPressed(true);
							}
							else{						
								AIcontroller.AIShort();
								AIcontroller.AICharges();
								character.SetBackPressed(true);
							}
						}	
						else if (decision <= 79 && decision > 72){
							
							if (chargeSystem.GetBackCharged() && !animator.GetBool("isAttacking")){
								AIDashLowJabOrStrong();
								AIcontroller.AICharges();
								character.SetBackPressed(true);
							}
							else{						
								AIcontroller.AIJab(4);
								AIcontroller.AICharges();
								character.SetBackPressed(true);
							}
						}
						else if (decision <= 82 && decision > 79){
							
							if (chargeSystem.GetBackCharged() && !animator.GetBool("isAttacking")){
								AIDashStraightJabOrStrong();
								AIcontroller.AICharges();
								character.SetBackPressed(true);
							}
							else{						
								AIcontroller.AIStrong(5);
								AIcontroller.AICharges();
								character.SetBackPressed(true);
							}
						}
						else if (decision <= 85 && decision > 82){
							
							if (chargeSystem.GetDownCharged() && !animator.GetBool("isAttacking")){
								AIHeadButt();
								AIcontroller.AICharges();
								character.SetBackPressed(true);
							}
							else{						
								AIcontroller.AIFierce(1,1);
								AIcontroller.AICharges();
								character.SetBackPressed(true);
							}
						}
						else{
							AITurnPunches();
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
					}
				}
			}
			else if (AIcontroller.GetDistanceFromPlayer() >= 1f && AIcontroller.GetDistanceFromPlayer() < 2f){
				if (playerCharacter.GetKnockDown() == true && playerCharacter.GetAirborne() == false){				
					decision = Random.Range (0, 100);
					if (decision <= 40){
						AITurnPunches();
						AIcontroller.AICharges();
						character.SetBackPressed(true);
					}
					else if (decision <= 50 && decision > 40){
						AIcontroller.AIPressedForward();
						AIcontroller.AIJump();
						character.SetBackPressed(false);
					}
					else if (decision <= 60 && decision > 50){
						AIcontroller.AIStand();
						AIcontroller.AIPressedForward();
						character.SetBackPressed(false);
					}
					else if (decision <= 65 && decision > 60){
						AIcontroller.CharacterNeutralState();
						AIcontroller.AIJump();
						character.SetBackPressed(false);
					}
					else if (decision <= 80 && decision > 65){
						AIcontroller.AICrouch();
						AIcontroller.AICharges();
						character.SetBackPressed(true);
					}
					else if (decision <= 85 && decision > 80){
						
						if (chargeSystem.GetBackCharged() && !animator.GetBool("isAttacking")){
							AIDashLowJabOrStrong();
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
						else{						
							AIcontroller.AIJab(10);
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
					}		
					else if (decision <= 90 && decision > 85){					
						if (chargeSystem.GetBackCharged() && !animator.GetBool("isAttacking")){
							AIKickRushesShortOrForward();
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
						else{						
							AIcontroller.AIStrong(20);
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
					}	
					else {
						AIcontroller.AIJab(80);
						AIcontroller.AICharges();
						character.SetBackPressed(true);
					}	
				}
				else {
					DecisionMade(5,2);
					if (decision <= 10){
						AIcontroller.AIStand();
						AIcontroller.AIPressedForward();
						character.SetBackPressed(false);
					}
					else if (decision <= 15 && decision > 10){
						AIcontroller.AIPressedForward();
						AIcontroller.AIJump();
						character.SetBackPressed(false);
					}			
					else if (decision <= 65 && decision > 15){
						AIcontroller.AICrouch();
						AIcontroller.AICharges();
						character.SetBackPressed(true);
					}							
					else if (decision <= 70 && decision > 65){					
						if (chargeSystem.GetBackCharged() && !animator.GetBool("isAttacking")){
							AIKickRushesShortOrForward();
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
						else{						
							AIcontroller.AIShort();
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
					}	
					else if (decision <= 80 && decision > 70){
						
						if (chargeSystem.GetBackCharged() && !animator.GetBool("isAttacking")){
							AIDashLowJabOrStrong();
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
						else{						
							AIcontroller.AIJab(4);
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
					}
					else if (decision <= 90 && decision > 80){
						
						if (chargeSystem.GetDownCharged() && !animator.GetBool("isAttacking")){
							AIHeadButt();
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
						else{						
							AIcontroller.AIStrong(5);
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
					}
					else if (decision <= 95 && decision > 90){				
						AIcontroller.AIFierce(1,1);
						AIcontroller.AICharges();
						character.SetBackPressed(true);
					}				
					else{
						AITurnPunches();
						AIcontroller.AICharges();
						character.SetBackPressed(true);
					}	
				}						
			}
			else{				
				if (playerCharacter.GetBlockStunned() == true){					
					decision = Random.Range (0, 100);
					if (decision <= 50){
						
						if (chargeSystem.GetBackCharged()){
							AIDashLowFierce();
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
						else{						
							AIcontroller.AIJab(60);
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
					}
					else if (decision <= 80 && decision > 50){
						if (chargeSystem.GetBackCharged()){
							AIKickRushesRoundhouse();
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
						else{				
							AIcontroller.AIStrong(80);
							AIcontroller.AICharges();
							character.SetBackPressed(true);		
						}
					}
					else if (decision <= 90 && decision > 80){
						if (chargeSystem.GetDownCharged()){
							AIHeadButt();
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
						else{						
							AIcontroller.AIStrong(80);
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
					}
					else{						
						AIcontroller.AIStrong(80);
						AIcontroller.AICharges();
						character.SetBackPressed(true);
					}
				}
				else if (playerCharacter.GetHitStunned() == true){					
					decision = Random.Range (0, 100);
					if (decision <= 50){
						
						if (chargeSystem.GetBackCharged()){
							AIDashLowFierce();
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
						else{						
							AIcontroller.AIStrong(60);
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
					}
					else if (decision <= 60 && decision > 50){
						if (chargeSystem.GetBackCharged()){
							AIKickRushesRoundhouse();
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
						else{				
							AIcontroller.AIStrong(80);
							AIcontroller.AICharges();
							character.SetBackPressed(true);		
						}
					}
					else if (decision <= 90 && decision > 60){
						if (chargeSystem.GetDownCharged()){
							AIHeadButt();
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
						else{						
							AIcontroller.AIJab(80);
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
					}
					else{						
						AIcontroller.AIStrong(80);
						AIcontroller.AICharges();
						character.SetBackPressed(true);
					}
				}		
				
				else if (playerCharacter.GetKnockDown() == true && playerCharacter.GetAirborne() == false){				
					decision = Random.Range (0, 100);
					if (decision <= 40){
						AITurnPunches();
						AIcontroller.AICharges();
						character.SetBackPressed(true);
					}
					else if (decision <= 50 && decision > 40){
						AIcontroller.AIPressedForward();
						AIcontroller.AIJump();
						character.SetBackPressed(false);
					}
					else if (decision <= 60 && decision > 50){
						AIcontroller.AIStand();
						AIcontroller.AIPressedForward();
						character.SetBackPressed(false);
					}
					else if (decision <= 65 && decision > 60){
						AIcontroller.CharacterNeutralState();
						AIcontroller.AIJump();
						character.SetBackPressed(false);
					}
					else if (decision <= 80 && decision > 65){
						AIcontroller.AICrouch();
						AIcontroller.AICharges();
						character.SetBackPressed(true);
					}
					else if (decision <= 85 && decision > 80){
						
						if (chargeSystem.GetBackCharged() && !animator.GetBool("isAttacking")){
							AIDashLowJabOrStrong();
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
						else{						
							AIcontroller.AIJab(10);
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
					}		
					else if (decision <= 90 && decision > 85){					
						if (chargeSystem.GetBackCharged() && !animator.GetBool("isAttacking")){
							AIKickRushesShortOrForward();
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
						else{						
							AIcontroller.AIStrong(20);
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
					}	
					else {
						AIcontroller.AIJab(80);
						AIcontroller.AICharges();
						character.SetBackPressed(true);
					}						
				}
				else if (playerCharacter.GetAirborne() == true && playerCharacter.GetKnockDown() == false){		
					decision = Random.Range (0, 100);
					if (decision <= 50){
						if (chargeSystem.GetDownCharged() && !animator.GetBool("isAttacking")){
							AIHeadButt();
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
						else{						
							AIcontroller.AIStrong(1);
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
					}
					else if (decision <= 60 && decision > 50){
						AIcontroller.AIStrong(1);
						AIcontroller.AICharges();
						character.SetBackPressed(true);
					}
					else if (decision <= 90 && decision > 60){
						if (chargeSystem.GetBackCharged() && !animator.GetBool("isAttacking")){
							AIKickRushesRoundhouse();
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
						else{
							AIcontroller.AIFierce(1,1);
							AIcontroller.CharacterNeutralState();
							AIcontroller.DoesAIBlock();
						}
					}						
					else{
						AIcontroller.AIFierce(1,1);
						AIcontroller.CharacterNeutralState();
						AIcontroller.DoesAIBlock();
					}
				}
				else {
					DecisionMade(5,3);
					if (decision <= 10){
						AIcontroller.AIStand();
						AIcontroller.AIPressedForward();
						character.SetBackPressed(false);
					}
					else if (decision <= 13 && decision > 10){
						AIcontroller.AIPressedForward();
						AIcontroller.AIJump();
						character.SetBackPressed(false);
					}			
					else if (decision <= 50 && decision > 13){
						AIcontroller.AICrouch();
						AIcontroller.AICharges();
						character.SetBackPressed(true);
					}					
					else if (decision <= 60 && decision > 50){					
						if (chargeSystem.GetBackCharged() && !animator.GetBool("isAttacking")){
							AIKickRushesShortOrForward();
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
						else{						
							AIcontroller.AIStrong(20);
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
					}		
					else if (decision <= 75 && decision > 60){
						
						if (chargeSystem.GetBackCharged() && !animator.GetBool("isAttacking")){
							AIDashLowJabOrStrong();
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
						else{						
							AIcontroller.AIJab(10);
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
					}
					else if (decision <= 80 && decision > 75){
						
						if (chargeSystem.GetDownCharged() && !animator.GetBool("isAttacking")){
							AIHeadButt();
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
						else{						
							AIcontroller.AIShort();
							AIcontroller.AICharges();
							character.SetBackPressed(true);
						}
					}
					else if (decision <= 83 && decision > 80){
						AIcontroller.AIJab(20);
						AIcontroller.AICharges();
						character.SetBackPressed(true);
					}
					else if (decision <= 86 && decision > 83){
						AIcontroller.AIStrong(20);
						AIcontroller.AICharges();
						character.SetBackPressed(true);
					}
					else if (decision <= 89 && decision > 86){
						AIcontroller.AISweep();
						AIcontroller.AICharges();
						character.SetBackPressed(true);
					}
					else if (decision <= 95 && decision > 89){
						AIcontroller.AIThrow();
						AIcontroller.AICharges();
						character.SetBackPressed(true);
					}
					else{
						AITurnPunches();
						AIcontroller.AICharges();
						character.SetBackPressed(true);
					}	
				}		
			}
			AIcontroller.Walk();
		}
	}

	void DecisionMade (int minDivisor, int maxDivisor){
		if (decisionTimer <= 0) {
			decision = Random.Range (0, 100);
			decisionTimer = Random.Range (decisionTimerInput / minDivisor, decisionTimerInput / maxDivisor);
		}
	}
			
	void AIKickRushesShortOrForward(){		
		int randNum = Random.Range (0,10);
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
	       && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
	       && animator.GetBool("isKnockedDown") == false && animator.GetBool("isMidAirRecovering") == false
	       && animator.GetBool("isThrown") == false){			
			animator.SetTrigger("kickRushInputed");		
			if (animator.GetBool("isAttacking") == false){
				character.AttackState();
				if (randNum < 8){
					animator.Play("BalrogKickRushShortStartUp",0);	
					animator.SetInteger("dashRushPunchType", 0);
				}
				else{
					animator.Play("BalrogKickRushForwardStartUp",0);	
					animator.SetInteger("dashRushPunchType", 1);
				}
				chargeSystem.SetBackCharged(false);
				chargeSystem.ResetBackChargedProperties();
				chargeSystem.ResetBackCharged();
			}
//			chargeSystem.SetBackCharged(false);
//			chargeSystem.ResetBackChargedProperties();
//			chargeSystem.ResetBackCharged();
		}
	}
	
	void AIKickRushesRoundhouse(){		
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isMidAirRecovering") == false
		    && animator.GetBool("isThrown") == false){			
			animator.SetTrigger("kickRushInputed");		
			if (animator.GetBool("isAttacking") == false){
				character.AttackState();
				animator.Play("BalrogKickRushRoundhouseStartUp",0);	
				animator.SetInteger("dashRushPunchType", 2);
							
				chargeSystem.SetBackCharged(false);
				chargeSystem.ResetBackChargedProperties();
				chargeSystem.ResetBackCharged();
			}
//			chargeSystem.SetBackCharged(false);
//			chargeSystem.ResetBackChargedProperties();
//			chargeSystem.ResetBackCharged();
		}
	}
	
	void AIDashLowJabOrStrong(){	
		int randNum = Random.Range (0,10);	
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isMidAirRecovering") == false
		    && animator.GetBool("isThrown") == false){			
			animator.SetTrigger("dashLowInputed");		
			if (animator.GetBool("isAttacking") == false){
				character.AttackState();
				if (randNum < 8){
					animator.Play("BalrogDashLowJabStartUp",0);	
					animator.SetInteger("dashRushPunchType", 0);
				}
				else{
					animator.Play("BalrogDashLowStrongStartUp",0);		
					animator.SetInteger("dashRushPunchType", 1);
				}			
				chargeSystem.SetBackCharged(false);
				chargeSystem.ResetBackChargedProperties();
				chargeSystem.ResetBackCharged();
			}
//			chargeSystem.SetBackCharged(false);
//			chargeSystem.ResetBackChargedProperties();
//			chargeSystem.ResetBackCharged();
		}
	}
	
	void AIDashLowFierce(){	
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isMidAirRecovering") == false
		    && animator.GetBool("isThrown") == false){			
			animator.SetTrigger("dashLowInputed");		
			if (animator.GetBool("isAttacking") == false){
				character.AttackState();
				animator.Play("BalrogDashLowFierceStartUp",0);	
				animator.SetInteger("dashRushPunchType", 2);
				
				chargeSystem.SetBackCharged(false);
				chargeSystem.ResetBackChargedProperties();
				chargeSystem.ResetBackCharged();
			}
//			chargeSystem.SetBackCharged(false);
//			chargeSystem.ResetBackChargedProperties();
//			chargeSystem.ResetBackCharged();
		}
	}
	
	void AIDashStraightJabOrStrong(){	
		int randNum = Random.Range (0,10);	
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isMidAirRecovering") == false
		    && animator.GetBool("isThrown") == false){			
			animator.SetTrigger("dashLowInputed");		
			if (animator.GetBool("isAttacking") == false){
				character.AttackState();
				if (randNum < 8){
					animator.Play("BalrogDashStraightJabStartUp",0);	
					animator.SetInteger("dashRushPunchType", 0);
				}
				else{
					animator.Play("BalrogDashStraightStrongStartUp",0);		
					animator.SetInteger("dashRushPunchType", 1);
				}
				chargeSystem.SetBackCharged(false);
				chargeSystem.ResetBackChargedProperties();
				chargeSystem.ResetBackCharged();
			}
//			chargeSystem.SetBackCharged(false);
//			chargeSystem.ResetBackChargedProperties();
//			chargeSystem.ResetBackCharged();
		}
	}
	
	void AIHeadButt(){	
		int randNum = Random.Range (0,3);	
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isMidAirRecovering") == false
		    && animator.GetBool("isThrown") == false){			
			animator.SetTrigger("headButtInputed");		
			if (animator.GetBool("isAttacking") == false){
				character.AttackState();
				switch(randNum){
					case 0:
						animator.Play("BalrogHeadButtJab",0);	
						animator.SetInteger("dashRushPunchType", 0);
						break;
					case 1:
						animator.Play("BalrogHeadButtStrong",0);	
						animator.SetInteger("dashRushPunchType", 1);
						break;
					case 2:
						animator.Play("BalrogHeadButtFierce",0);	
						animator.SetInteger("dashRushPunchType", 2);
						break;
				}
				chargeSystem.SetDownCharged(false);
				chargeSystem.ResetDownChargedProperties();
				chargeSystem.ResetDownCharged();
			}
//			chargeSystem.SetDownCharged(false);
//			chargeSystem.ResetDownChargedProperties();
//			chargeSystem.ResetDownCharged();
		}
	}
	
	void AITurnPunches(){	
		int randNum = Random.Range (0,10);	
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isMidAirRecovering") == false
		    && animator.GetBool("isThrown") == false){			
			animator.SetTrigger("turnPunchInputed");
			if (animator.GetBool("isAttacking") == false){
				character.AttackState();
				animator.Play ("BalrogTurnPunch", 0);		
				switch(randNum){
				case 0:
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
					{
					Debug.Log ("level one");
					character.MoveProperties(30f, 25f, 12f, 50f, 2, 2);
					AudioSource.PlayClipAtPoint(one, transform.position);
					break;
					}
				case 6:
				case 7:
				case 8:
					{
					Debug.Log ("level two");			
					character.MoveProperties(30f, 25f, 12f, 60f, 2, 2);
					AudioSource.PlayClipAtPoint(two, transform.position);
					break;
					}
				case 9:
					Debug.Log ("level three");
					character.MoveProperties(30f, 25f, 12f, 80f, 2, 2);
					AudioSource.PlayClipAtPoint(three, transform.position);
					break;
				}
			}
		}
	}
}
