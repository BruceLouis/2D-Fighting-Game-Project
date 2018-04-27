using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalrogAI : MonoBehaviour {

	public float decisionTimer;
	public AudioClip one, two, three;
	
	private Animator animator;
	private Player player, playerController;
	private Opponent opponent, opponentController;
	private Character playerCharacter, opponentCharacter;
	private Character character;
	private SharedProperties sharedProperties;
	private AIControls AIcontrols;
	private ChargeSystem chargeSystem;
	
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
		chargeSystem = GetComponentInParent<ChargeSystem>();
		sharedProperties = GetComponentInParent<SharedProperties>();
		
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
					AIcontrols.AIJumpFierce();
				}
				else if (decision <= 6 && decision > 1){
					AIcontrols.AIJumpRoundhouse();
				}				
				AIcontrols.AICharges();
				character.SetBackPressed(true);
			}
			else if (character.GetKnockDown() == true){
				AIcontrols.AICrouch();
				AIcontrols.AICharges();
				character.SetBackPressed(true);
			}				
			else if (sharedProperties.GetDistanceFromOtherFighter() >= 2f){
				if (player != null){
					if (playerCharacter.GetKnockDown() == true && playerCharacter.GetAirborne() == false){		
						KnockDownFromFarDecisions ();
					}
					else if (playerCharacter.GetComponent<Ken>() != null){
						DecisionMade (5,1);
						VsKenFromDistanceDecisions ();
					}
					else{	
						RegularFarRangeDecisions ();	
					}
				}
				else if (opponent != null){
					if (opponentCharacter.GetKnockDown() == true && opponentCharacter.GetAirborne() == false){	
						KnockDownFromFarDecisions ();
					}
					else if (opponentCharacter.GetComponent<Ken>() != null){
						DecisionMade (5,1);
						VsKenFromDistanceDecisions ();
					}
					else{	
						RegularFarRangeDecisions ();	
					}
				}
			}
			else if (sharedProperties.GetDistanceFromOtherFighter() >= 1f && sharedProperties.GetDistanceFromOtherFighter() < 2f){
				if (player != null){
					if (playerCharacter.GetKnockDown() == true && playerCharacter.GetAirborne() == false){						
						KnockDownFromMidRangeDecisions ();
					}
					else {
						RegularMidRangeDecisions ();
					}		
				} 
				else if (opponent != null){
					if (opponentCharacter.GetKnockDown() == true && opponentCharacter.GetAirborne() == false){				
						KnockDownFromMidRangeDecisions ();
					}
					else {
						RegularMidRangeDecisions ();
					}		
				}				
			}
			else{				
				if (player != null){
					if (playerCharacter.GetBlockStunned() == true){					
						OtherFighterBlockedDecisions ();
					}					
					else if (playerCharacter.GetHitStunned() == true){					
						OtherFighterGotHitDecisions ();
					}		
					else if (playerCharacter.GetKnockDown() == true && playerCharacter.GetAirborne() == false){				
						KnockDownFromCloseRangeDecisions ();
					}
					else if (playerCharacter.GetAirborne() == true && playerCharacter.GetKnockDown() == false){	
						if (playerCharacter.GetComponent<FeiLong>() != null){
							VsFeiLongsChickenWing ();
						}
						else{	
							PreparationForAntiAir ();
						}
					}
					else {
						RegularCloseRangeDecisions ();
					}	
				}
				else if (opponent != null){
					if (opponentCharacter.GetBlockStunned() == true){	
						OtherFighterBlockedDecisions ();
					}					
					else if (opponentCharacter.GetHitStunned() == true){					
						OtherFighterGotHitDecisions ();
					}		
					else if (opponentCharacter.GetKnockDown() == true && opponentCharacter.GetAirborne() == false){				
						KnockDownFromCloseRangeDecisions ();
					}
					else if (opponentCharacter.GetAirborne() == true && opponentCharacter.GetKnockDown() == false){		
						if (opponentCharacter.GetComponent<FeiLong>() != null){
							VsFeiLongsChickenWing ();
						}
						else{	
							PreparationForAntiAir ();
						}
					}
					else {
						RegularCloseRangeDecisions ();
					}	
				}
					
			}
			if (playerController != null){
				playerController.WalkAI();
			}
			else if (opponentController != null){
				opponentController.Walk();
			}
		}
	}

	void RegularCloseRangeDecisions (){
		DecisionMade (5, 3);
		if (decision <= 10) {
			AIcontrols.AIStand ();
			AIcontrols.AIPressedForward ();
			character.SetBackPressed (false);
		}
		else if (decision <= 13 && decision > 10) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else if (decision <= 50 && decision > 13) {
			AIcontrols.AICrouch ();
			AIcontrols.AICharges ();
			character.SetBackPressed (true);
		}
		else if (decision <= 60 && decision > 50) {
			if (chargeSystem.GetBackCharged () && !animator.GetBool ("isAttacking")) {
				AIKickRushesShortOrForward ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AIStrong (20);
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
				decisionTimer = 0f;
			}
		}
		else if (decision <= 75 && decision > 60) {
			if (chargeSystem.GetBackCharged () && !animator.GetBool ("isAttacking")) {
				AIDashLowJabOrStrong ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AIJab (10);
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
				decisionTimer = 0f;
			}
		}
		else if (decision <= 80 && decision > 75) {
			if (chargeSystem.GetDownCharged () && !animator.GetBool ("isAttacking")) {
				AIHeadButt ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AIShort ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
		}
		else if (decision <= 83 && decision > 80) {
			AIcontrols.AIJab (20);
			AIcontrols.AICharges ();
			character.SetBackPressed (true);
			decisionTimer = 0f;
		}
		else if (decision <= 86 && decision > 83) {
			AIcontrols.AIStrong (20);
			AIcontrols.AICharges ();
			character.SetBackPressed (true);
			decisionTimer = 0f;
		}
		else if (decision <= 89 && decision > 86) {
			AIcontrols.AISweep ();
			AIcontrols.AICharges ();
			character.SetBackPressed (true);
			decisionTimer = 0f;
		}
		else if (decision <= 95 && decision > 89) {
			AIcontrols.AIThrow ();
			AIcontrols.AICharges ();
			character.SetBackPressed (true);
		}
		else {
			AITurnPunches ();
			AIcontrols.AICharges ();
			character.SetBackPressed (true);
			decisionTimer = 0f;
		}
	}

	void RegularMidRangeDecisions (){
		DecisionMade (5, 2);
		if (decision <= 10) {
			AIcontrols.AIStand ();
			AIcontrols.AIPressedForward ();
			character.SetBackPressed (false);
		}
		else if (decision <= 15 && decision > 10) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else if (decision <= 65 && decision > 15) {
			AIcontrols.AICrouch ();
			AIcontrols.AICharges ();
			character.SetBackPressed (true);
		}
		else if (decision <= 70 && decision > 65) {
			if (chargeSystem.GetBackCharged () && !animator.GetBool ("isAttacking")) {
				AIKickRushesShortOrForward ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AIShort ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
				decisionTimer = 0f;
			}
		}
		else if (decision <= 80 && decision > 70) {
			if (chargeSystem.GetBackCharged () && !animator.GetBool ("isAttacking")) {
				AIDashLowJabOrStrong ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AIJab (4);
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
				decisionTimer = 0f;
			}
		}
		else if (decision <= 90 && decision > 80) {
			if (chargeSystem.GetDownCharged () && !animator.GetBool ("isAttacking")) {
				AIHeadButt ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AIStrong (5);
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
				decisionTimer = 0f;
			}
		}
		else if (decision <= 95 && decision > 90) {
			AIcontrols.AIFierce (1, 1);
			AIcontrols.AICharges ();
			character.SetBackPressed (true);
			decisionTimer = 0f;
		}
		else {
			AITurnPunches ();
			AIcontrols.AICharges ();
			character.SetBackPressed (true);
			decisionTimer = 0f;
		}
	}

	void RegularFarRangeDecisions (){
		DecisionMade (5, 1);
		if (decision <= 15) {
			AIcontrols.AIStand ();
			AIcontrols.AIPressedForward ();
			character.SetBackPressed (false);
		}
		else if (decision <= 17 && decision > 15) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else if (decision <= 19 && decision > 17) {
			sharedProperties.CharacterNeutralState ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else if (decision <= 65 && decision > 19) {
			AIcontrols.AICrouch ();
			AIcontrols.AICharges ();
			character.SetBackPressed (true);
		}
		else if (decision <= 72 && decision > 65) {
			if (chargeSystem.GetBackCharged () && !animator.GetBool ("isAttacking")) {
				AIKickRushesShortOrForward ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AIShort ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
				decisionTimer = 0f;
			}
		}
		else if (decision <= 79 && decision > 72) {
			if (chargeSystem.GetBackCharged () && !animator.GetBool ("isAttacking")) {
				AIDashLowJabOrStrong ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AIJab (4);
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
				decisionTimer = 0f;
			}
		}
		else if (decision <= 82 && decision > 79) {
			if (chargeSystem.GetBackCharged () && !animator.GetBool ("isAttacking")) {
				AIDashStraightJabOrStrong ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AIStrong (5);
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
				decisionTimer = 0f;
			}
		}
		else if (decision <= 85 && decision > 82) {
			if (chargeSystem.GetDownCharged () && !animator.GetBool ("isAttacking")) {
				AIHeadButt ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AIFierce (1, 1);
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
				decisionTimer = 0f;
			}
		}
		else {
			AITurnPunches ();
			AIcontrols.AICharges ();
			character.SetBackPressed (true);
			decisionTimer = 0f;
		}
	}

	void VsKenFromDistanceDecisions (){
		if (decision <= 15) {
			AIcontrols.AIStand ();
			AIcontrols.AIPressedForward ();
			character.SetBackPressed (false);
		}
		else if (decision <= 20 && decision > 15) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else if (decision <= 22 && decision > 20) {
			sharedProperties.CharacterNeutralState ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else if (decision <= 65 && decision > 22) {
			AIcontrols.AICrouch ();
			AIcontrols.AICharges ();
			character.SetBackPressed (true);
		}
		else if (decision <= 67 && decision > 65) {
			if (chargeSystem.GetBackCharged () && !animator.GetBool ("isAttacking")) {
				AIKickRushesShortOrForward ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AIShort ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
				decisionTimer = 0f;
			}
		}
		else if (decision <= 70 && decision > 67) {
			if (chargeSystem.GetBackCharged () && !animator.GetBool ("isAttacking")) {
				AIDashLowJabOrStrong ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AIJab (4);
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
				decisionTimer = 0f;
			}
		}
		else if (decision <= 71 && decision > 70) {
			if (chargeSystem.GetBackCharged () && !animator.GetBool ("isAttacking")) {
				AIDashStraightJabOrStrong ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AIStrong (5);
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
				decisionTimer = 0f;
			}
		}
		else if (decision <= 85 && decision > 71) {
			if (chargeSystem.GetDownCharged () && !animator.GetBool ("isAttacking")) {
				AIHeadButt ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AIFierce (1, 1);
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
				decisionTimer = 0f;
			}
		}
		else {
			AITurnPunches ();
			AIcontrols.AICharges ();
			character.SetBackPressed (true);
			decisionTimer = 0f;
		}
	}
	
	void VsFeiLongsChickenWing (){
		decision = Random.Range (0, 100);
		if (decision <= 50) {
			if (chargeSystem.GetDownCharged () && !animator.GetBool ("isAttacking")) {
				AIHeadButt ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AICrouch ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
		}
		else if (decision <= 60 && decision > 50) {
			AIcontrols.AIStrong (1);
			AIcontrols.AICharges ();
			character.SetBackPressed (true);
		}
		else if (decision <= 90 && decision > 60) {
			if (chargeSystem.GetBackCharged () && !animator.GetBool ("isAttacking")) {
				AIKickRushesRoundhouse ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AICrouch ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
		}
		else {
			AIcontrols.AIFierce (1, 1);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
	}
	
	void PreparationForAntiAir (){
		decision = Random.Range (0, 100);
		if (decision <= 50) {
			if (chargeSystem.GetDownCharged () && !animator.GetBool ("isAttacking")) {
				AIHeadButt ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AIStrong (1);
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
		}
		else if (decision <= 60 && decision > 50) {
			AIcontrols.AIStrong (1);
			AIcontrols.AICharges ();
			character.SetBackPressed (true);
		}
		else if (decision <= 90 && decision > 60) {
			if (chargeSystem.GetBackCharged () && !animator.GetBool ("isAttacking")) {
				AIKickRushesRoundhouse ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AIFierce (1, 1);
				sharedProperties.CharacterNeutralState ();
				AIcontrols.DoesAIBlock ();
			}
		}
		else {
			AIcontrols.AIFierce (1, 1);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
	}

	void OtherFighterGotHitDecisions (){
		decision = Random.Range (0, 100);
		if (decision <= 50) {
			if (chargeSystem.GetBackCharged ()) {
				AIDashLowFierce ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AIStrong (60);
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
		}
		else if (decision <= 60 && decision > 50) {
			if (chargeSystem.GetBackCharged ()) {
				AIKickRushesRoundhouse ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AIJab (80);
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
		}
		else if (decision <= 90 && decision > 60) {
			if (chargeSystem.GetDownCharged ()) {
				AIHeadButt ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AIJab (80);
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
		}
		else {
			AIcontrols.AIJab (80);
			AIcontrols.AICharges ();
			character.SetBackPressed (true);
		}
	}

	void OtherFighterBlockedDecisions (){
		decision = Random.Range (0, 100);
		if (decision <= 50) {
			if (chargeSystem.GetBackCharged ()) {
				AIDashLowFierce ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AIJab (80);
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
		}
		else if (decision <= 70 && decision > 50) {
			if (chargeSystem.GetBackCharged ()) {
				AIKickRushesRoundhouse ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AIStrong (80);
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
		}
		else if (decision <= 90 && decision > 70) {
			if (chargeSystem.GetDownCharged ()) {
				AIHeadButt ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AIJab (80);
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
		}
		else {
			AIcontrols.AIStrong (80);
			AIcontrols.AICharges ();
			character.SetBackPressed (true);
		}
	}

	void KnockDownFromCloseRangeDecisions (){
		decision = Random.Range (0, 100);
		if (decision <= 40) {
			AITurnPunches ();
			AIcontrols.AICharges ();
			character.SetBackPressed (true);
		}
		else if (decision <= 50 && decision > 40) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else if (decision <= 60 && decision > 50) {
			AIcontrols.AIStand ();
			AIcontrols.AIPressedForward ();
			character.SetBackPressed (false);
		}
		else if (decision <= 65 && decision > 60) {
			sharedProperties.CharacterNeutralState ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else if (decision <= 80 && decision > 65) {
			AIcontrols.AICrouch ();
			AIcontrols.AICharges ();
			character.SetBackPressed (true);
		}
		else if (decision <= 85 && decision > 80) {
			if (chargeSystem.GetBackCharged () && !animator.GetBool ("isAttacking")) {
				AIDashLowJabOrStrong ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AIJab (10);
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
		}
		else if (decision <= 90 && decision > 85) {
			if (chargeSystem.GetBackCharged () && !animator.GetBool ("isAttacking")) {
				AIKickRushesShortOrForward ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AIStrong (20);
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
		}
		else {
			AIcontrols.AIJab (80);
			AIcontrols.AICharges ();
			character.SetBackPressed (true);
		}
	}

	void KnockDownFromMidRangeDecisions (){
		decision = Random.Range (0, 100);
		if (decision <= 40) {
			AITurnPunches ();
			AIcontrols.AICharges ();
			character.SetBackPressed (true);
		}
		else if (decision <= 50 && decision > 40) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else if (decision <= 60 && decision > 50) {
			AIcontrols.AIStand ();
			AIcontrols.AIPressedForward ();
			character.SetBackPressed (false);
		}
		else if (decision <= 65 && decision > 60) {
			sharedProperties.CharacterNeutralState ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else if (decision <= 80 && decision > 65) {
			AIcontrols.AICrouch ();
			AIcontrols.AICharges ();
			character.SetBackPressed (true);
		}
		else if (decision <= 85 && decision > 80) {
			if (chargeSystem.GetBackCharged () && !animator.GetBool ("isAttacking")) {
				AIDashLowJabOrStrong ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AIJab (10);
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
		}
		else if (decision <= 90 && decision > 85) {
			if (chargeSystem.GetBackCharged () && !animator.GetBool ("isAttacking")) {
				AIKickRushesShortOrForward ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AIStrong (20);
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
		}
		else {
			AIcontrols.AIJab (80);
			AIcontrols.AICharges ();
			character.SetBackPressed (true);
		}
	}

	void KnockDownFromFarDecisions (){
		decision = Random.Range (0, 100);
		if (decision <= 40) {
			AITurnPunches ();
			AIcontrols.AICharges ();
			character.SetBackPressed (true);
		}
		else if (decision <= 45 && decision > 40) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else if (decision <= 80 && decision > 45) {
			AIcontrols.AIStand ();
			AIcontrols.AIPressedForward ();
			character.SetBackPressed (false);
		}
		else if (decision <= 82 && decision > 80) {
			if (chargeSystem.GetBackCharged () && !animator.GetBool ("isAttacking")) {
				AIDashLowJabOrStrong ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AIJab (10);
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
		}
		else if (decision <= 90 && decision > 82) {
			if (chargeSystem.GetBackCharged () && !animator.GetBool ("isAttacking")) {
				AIDashLowFierce ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AIJab (10);
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
		}
		else if (decision <= 92 && decision > 90) {
			if (chargeSystem.GetBackCharged () && !animator.GetBool ("isAttacking")) {
				AIKickRushesShortOrForward ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AIStrong (20);
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
		}
		else {
			if (chargeSystem.GetBackCharged () && !animator.GetBool ("isAttacking")) {
				AIKickRushesRoundhouse ();
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
			else {
				AIcontrols.AIStrong (20);
				AIcontrols.AICharges ();
				character.SetBackPressed (true);
			}
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
		}
	}
	
	void AITurnPunches(){	
		int randNum = Random.Range (0,50);	
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isMidAirRecovering") == false
		    && animator.GetBool("isThrown") == false){			
			animator.SetTrigger("turnPunchInputed");
			if (animator.GetBool("isAttacking") == false){
				character.AttackState();
				if (randNum <= 35){
					animator.SetInteger("turnPunchStrength", 1);
				}
				else if (randNum <= 47 && randNum > 35){
					animator.SetInteger("turnPunchStrength", 2);
				}
				else if (randNum <= 49 && randNum > 47)	{
					animator.SetInteger("turnPunchStrength", 3);
				}
				else {
					animator.SetInteger("turnPunchStrength", 4);
				}					
				animator.Play ("BalrogTurnPunch", 0);	
			}
		}
	}
}
