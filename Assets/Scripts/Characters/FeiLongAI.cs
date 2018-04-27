using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeiLongAI : MonoBehaviour {
	
	public float decisionTimer;
	
	private Animator animator;
	private Player player, playerController;
	private Opponent opponent, opponentController;
	private Character playerCharacter, opponentCharacter;
	private Character character;
	private SharedProperties sharedProperties;
	private AIControls AIcontrols;
	
	private int decision;
	private float decisionTimerInput;	
	
	// Use this for initialization
	void Start () {		
		
		if (GetComponentInParent<Opponent>() != null){
			player = FindObjectOfType<Player>();
			playerCharacter = player.GetComponentInChildren<Character>();
			opponentController = GetComponentInParent<Opponent>();
		}
		else if (GetComponentInParent<Player>() != null){
			opponent = FindObjectOfType<Opponent>();
			opponentCharacter = opponent.GetComponentInChildren<Character>();
			playerController = GetComponentInParent<Player>();
		}
		
		character = GetComponent<Character>();
		animator = GetComponent<Animator>();	
		AIcontrols = GetComponentInParent<AIControls>();
		sharedProperties = GetComponentInParent<SharedProperties>();
		
		decisionTimerInput = decisionTimer; 
		decision = Random.Range(0,100);	
	}
		
	
	public void Behaviors(){
		decisionTimer--;
		if (animator.GetBool("isLiftingOff") == false && animator.GetBool("isKnockedDown") == false && animator.GetBool("isThrown") == false && animator.GetBool("isMidAirHit") == false
		 	&& animator.GetBool("isMidAirRecovering") == false && animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false){
			if (animator.GetBool("rekkaKenActive")){
				if (player != null){
					RekkaChainDecisions (playerCharacter);
				}
				else if (opponent != null){
					RekkaChainDecisions (opponentCharacter);
				}					
			}	
			else if (animator.GetBool("isAirborne") == true && animator.GetBool("isLiftingOff") == false){
				decision = Random.Range(0,100);	
				if (decision <= 1){
					AIcontrols.AIJumpFierce();
					sharedProperties.CharacterNeutralState();
				}
				else if (decision <= 6 && decision > 1){
					AIcontrols.AIJumpRoundhouse();
					sharedProperties.CharacterNeutralState();
				}				
			}
			else if (sharedProperties.GetDistanceFromOtherFighter() < 1f){
				if (player != null){
					if (playerCharacter.GetHitStunned() == true){
						CloseRangeOtherFighterGotHitDecisions ();
					}
					else if (playerCharacter.GetBlockStunned() == true){
						CloseRangeOtherFighterBlockedDecisions ();
					}
					else if (playerCharacter.GetKnockDown() == true && playerCharacter.GetAirborne() == false){					
						KnockDownCloseRangeDecisions ();
					}
					else{
						RegularCloseRangeDecisions();
					}
				}
				else if (opponent != null){
					if (opponentCharacter.GetHitStunned() == true){
						CloseRangeOtherFighterGotHitDecisions ();
					}
					else if (opponentCharacter.GetBlockStunned() == true){
						CloseRangeOtherFighterBlockedDecisions ();
					}
					else if (opponentCharacter.GetKnockDown() == true && opponentCharacter.GetAirborne() == false){					
						KnockDownCloseRangeDecisions ();
					}
					else{
						RegularCloseRangeDecisions();
					}
				}
			}	
			else if (sharedProperties.GetDistanceFromOtherFighter() < 2f && sharedProperties.GetDistanceFromOtherFighter() >= 1f){
				
				if (player != null){
					if (playerCharacter.GetKnockDown() == true){				
						KnockDownMidRangeDecisions ();
					}
					else {						
						RegularMidRangeDecisions ();
					}	
				}
				else if (opponent != null){			
					if (opponentCharacter.GetKnockDown() == true){				
						KnockDownMidRangeDecisions ();
					}
					else {						
						RegularMidRangeDecisions ();
					}
				}
			}
			else{
				RegularFarRangeDecisions ();
			}
			if (playerController != null){
				playerController.WalkAI();
			}
			else if (opponentController != null){
				opponentController.Walk();
			}	
		}			
	}

	void RegularFarRangeDecisions (){
		DecisionMade (5, 1);
		if (decision <= 30) {
			AIcontrols.AIStand ();
			AIcontrols.AIPressedForward ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 45 && decision > 30) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else if (decision <= 50 && decision > 45) {
			sharedProperties.CharacterNeutralState ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else if (decision <= 85 && decision > 50) {
			AIRekkaKuns ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 90 && decision > 85) {
			AIcontrols.AIStand ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else {
			AIcontrols.AICrouch ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
	}

	void RegularMidRangeDecisions (){
		DecisionMade (5, 2);
		if (decision <= 30) {
			AIcontrols.AIStand ();
			AIcontrols.AIPressedForward ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 34 && decision > 30) {
			AIcontrols.AIFierce (2, 0);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 37 && decision > 34) {
			AIcontrols.AIStrong (2);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 39 && decision > 37) {
			AIcontrols.AIJab (2);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 40 && decision > 39) {
			AIcontrols.AIShort ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 50 && decision > 40) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else if (decision <= 55 && decision > 50) {
			sharedProperties.CharacterNeutralState ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else if (decision <= 65 && decision > 55) {
			AIRekkaKens ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 75 && decision > 65) {
			AIRekkaKuns ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 80 && decision > 75) {
			AIcontrols.AIStand ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else {
			AIcontrols.AICrouch ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
	}

	void RegularCloseRangeDecisions (){
		DecisionMade (5, 4);
		if (decision <= 10) {
			AIcontrols.AIStand ();
			AIcontrols.AIPressedForward ();
			character.SetBackPressed (false);
		}
		else if (decision <= 13 && decision > 10) {
			AIcontrols.AIFierce (10, 8);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 16 && decision > 13) {
			AIcontrols.AIStrong (2);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 22 && decision > 16) {
			AIcontrols.AIJab (8);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 23 && decision > 22) {
			AIcontrols.AIShort ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 25 && decision > 23) {
			AIcontrols.AIForward (2);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 30 && decision > 25) {
			sharedProperties.CharacterNeutralState ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else if (decision <= 32 && decision > 30) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else if (decision <= 35 && decision > 32) {
			AIcontrols.AISweep ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 40 && decision > 35) {
			AIcontrols.AIThrow ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 45 && decision > 40) {
			AIRekkaKens ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 48 && decision > 45) {
			AIShortShienKyakus ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 50 && decision > 48) {
			AIRekkaKuns ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else {
			AIcontrols.AICrouch ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
	}

	void KnockDownMidRangeDecisions (){
		DecisionMade (5, 3);
		if (decision <= 60) {
			AIcontrols.AIStand ();
			AIcontrols.AIPressedForward ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 75 && decision > 60) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else if (decision <= 90 && decision > 75) {
			AIRekkaKuns ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else {
			AIRekkaKens ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
	}

	void KnockDownCloseRangeDecisions (){
		decision = Random.Range(0,100); 
		if (decision <= 60) {
			AIcontrols.AIStand ();
			AIcontrols.AIPressedForward ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 63 && decision > 60) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else if (decision <= 75 && decision > 63) {
			sharedProperties.CharacterNeutralState ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else if (decision <= 90 && decision > 75) {
			AIRekkaKuns ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else {
			AIRekkaKens ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
	}

	void CloseRangeOtherFighterBlockedDecisions (){
		decision = Random.Range(0,100); 
		if (decision <= 30) {
			AIcontrols.AIJab (8);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 50 && decision > 30) {
			AIcontrols.AIStrong (2);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 60 && decision > 50) {
			AIcontrols.AIFierce (10, 8);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 80 && decision > 60) {
			AIRekkaKens ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 85 && decision > 80) {
			AIShortShienKyakus ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else {
			AIRekkaKuns ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
	}

	void CloseRangeOtherFighterGotHitDecisions (){
		decision = Random.Range(0,100); 
		if (decision <= 60) {
			AIRekkaKens ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 70 && decision > 60) {
			AIcontrols.AIFierce (10, 8);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else {
			AIShienKyakus ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
	}

	void RekkaChainDecisions (Character whichFighter){
		decision = Random.Range(0,200);
		if (whichFighter.GetHitStunned () == true) {
			if (decision <= 40) {
				AIRekkaKens ();
				sharedProperties.CharacterNeutralState ();
				AIcontrols.DoesAIBlock ();
			}
		}
		else {
			if (decision <= 2) {
				AIRekkaKens ();
				sharedProperties.CharacterNeutralState ();
				AIcontrols.DoesAIBlock ();
			}
		}
	}
	
	void DecisionMade (int minDivisor, int maxDivisor){
		if (decisionTimer <= 0) {
			decision = Random.Range (0, 100);
			decisionTimer = Random.Range (decisionTimerInput / minDivisor, decisionTimerInput / maxDivisor);
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
				AIcontrols.AIStand();
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
				AIcontrols.AIStand();
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
				AIcontrols.AIStand();
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
				AIcontrols.AIStand();
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
