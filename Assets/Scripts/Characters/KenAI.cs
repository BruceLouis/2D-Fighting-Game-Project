using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KenAI : MonoBehaviour {
		
	[SerializeField] float decisionTimer, antiAirTimer;
		
	private Animator animator;
	private Player player, playerController;
	private Opponent opponent, opponentController;
	private Character playerCharacter, opponentCharacter;
	private Character character;
	private SharedProperties sharedProperties;
	private AIControls AIcontrols;
	
	private int decision;
	private float decisionTimerInput, antiAirTimerInput;	
	
	// Use this for initialization
	void Start () {				
	
		character = GetComponent<Character>();
		animator = GetComponent<Animator>();	
		AIcontrols = GetComponentInParent<AIControls>();
		sharedProperties = GetComponentInParent<SharedProperties>();
		
		if (GetComponentInParent<Opponent>() != null){
			player = FindObjectOfType<Player>();
			playerCharacter = player.GetComponentInChildren<Character>();
			opponentController = GetComponentInParent<Opponent>();
			animator.SetInteger("hadoukenOwner", 2);
		}
		else if (GetComponentInParent<Player>() != null){
			opponent = FindObjectOfType<Opponent>();
			opponentCharacter = opponent.GetComponentInChildren<Character>();
			playerController = GetComponentInParent<Player>();
			animator.SetInteger("hadoukenOwner", 1);
		}
		
		decisionTimerInput = decisionTimer; 
		antiAirTimerInput = antiAirTimer;
		antiAirTimer = 0f;
		decision = Random.Range(0,100);	
	}
	
	public void Behaviors(){
		decisionTimer--;
		antiAirTimer--;
		if (AIcontrols.FreeToMakeDecisions() && !TimeControl.inSuperStartup[0] && !TimeControl.inSuperStartup[1]){
			if (animator.GetBool("isAirborne") == true && animator.GetBool("isLiftingOff") == false){
				decision = Random.Range(0,100);
				if (decision <= 3){
					AIcontrols.AIJumpFierce();
					sharedProperties.CharacterNeutralState();
				}
				else if (decision <= 6 && decision > 3){
					AIcontrols.AIJumpRoundhouse();
					sharedProperties.CharacterNeutralState();
				}				
			}
			else if (sharedProperties.GetDistanceFromOtherFighter() >= 2f){			
				RegularFarRangeDecisions();
			}
			else if (sharedProperties.GetDistanceFromOtherFighter() < 2f && sharedProperties.GetDistanceFromOtherFighter() >= 1f){	
				
				if (player != null){
					if (playerCharacter.GetBlockStunned() == true){
						MidRangeOtherFighterBlockedDecisions ();
					}
					else if (playerCharacter.GetKnockDown() == true && playerCharacter.GetAirborne() == false){
						KnockDownMidRangeDecisions ();
					}
					else{
						RegularMidRangeDecisions ();
					}
				}
				else if (opponent != null){
					if (opponentCharacter.GetBlockStunned() == true){
						MidRangeOtherFighterBlockedDecisions ();
					}
					else if (opponentCharacter.GetKnockDown() == true && opponentCharacter.GetAirborne() == false){
						KnockDownMidRangeDecisions ();
					}
					else{
						RegularMidRangeDecisions ();
					}
				}				
			}
			else{
				if (player != null){
				//anti air
					if (playerCharacter.GetAirborne() == true && playerCharacter.GetKnockDown() == false && playerCharacter.GetThrown() == false){
						if (antiAirTimer <= 0f){			
							sharedProperties.AIAntiAirDecision(67, RegularCloseRangeDecisions, PreparationForAntiAir);
							antiAirTimer = antiAirTimerInput;
						}
						else{
							RegularCloseRangeDecisions();
						}
					}
				//other guy got hit
					else if (playerCharacter.GetHitStunned() == true){
						OtherFighterGotHitDecisions ();
					}
				//other guy got knocked down
					else if (playerCharacter.GetKnockDown() == true && playerCharacter.GetAirborne() == false){
						KnockDownCloseRangeDecisions ();
					}
				//other guy blocked
					else if (playerCharacter.GetBlockStunned() == true){
						CloseRangeOtherFighterBlockedDecisions ();		
					}
					else{
						RegularCloseRangeDecisions ();
					}
				}				
				else if (opponent != null){
					//anti air
					if (opponentCharacter.GetAirborne() == true && opponentCharacter.GetKnockDown() == false && opponentCharacter.GetThrown() == false){
						if (antiAirTimer <= 0f){			
							sharedProperties.AIAntiAirDecision(67, RegularCloseRangeDecisions, PreparationForAntiAir);
							antiAirTimer = antiAirTimerInput;
						}
						else{
							RegularCloseRangeDecisions();
						}
					}
					//other guy got hit
					else if (opponentCharacter.GetHitStunned() == true){
						OtherFighterGotHitDecisions ();
					}
					//other guy got knocked down
					else if (opponentCharacter.GetKnockDown() == true && opponentCharacter.GetAirborne() == false){
						KnockDownCloseRangeDecisions ();
					}
					//other guy blocked
					else if (opponentCharacter.GetBlockStunned() == true){
						CloseRangeOtherFighterBlockedDecisions ();				
					}
					else{
						RegularCloseRangeDecisions ();
					}
				}				
			}
			AIcontrols.AIWalks();
		}			
	}

	void RegularCloseRangeDecisions (){
		DecisionMade (5, 3);
		if (decision <= 25) {
			AIcontrols.AICrouch ();
			sharedProperties.CharacterNeutralState ();
			character.SetBackPressed (true);
		}
		else if (decision <= 30 && !animator.GetBool("isAttacking")) {
			AIHurricaneKicks ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 36) {
			AIcontrols.AILowForward ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 41) {
			AIcontrols.AIStrong (10);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 46) {
			AIcontrols.AIJab (2);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 52) {
			if (character.GetSuper >= 100f){
				AIShinryukens();
			}
			else{
				AIcontrols.AIShort (10);
			}
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 62 && !animator.GetBool("isAttacking")) {
			AIJabShoryuken ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 66) {
			AIcontrols.AIThrow ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 71) {
			if (playerController != null){
				if (playerController.GetProjectileP1Parent ().transform.childCount <= 0 && !animator.GetBool("isAttacking")) {
					AIHadoukens ();
					sharedProperties.CharacterNeutralState ();
					AIcontrols.DoesAIBlock ();
					decisionTimer = 0f;
				}
				else {
					AIcontrols.AIShort (10);
					sharedProperties.CharacterNeutralState ();
					AIcontrols.DoesAIBlock ();
					decisionTimer = 0f;
				}
			}
			else if (opponentController != null){
				if (opponentController.GetProjectileP2Parent ().transform.childCount <= 0 && !animator.GetBool("isAttacking")) {
					AIHadoukens ();
					sharedProperties.CharacterNeutralState ();
					AIcontrols.DoesAIBlock ();
					decisionTimer = 0f;
				}
				else {
					AIcontrols.AIShort (10);
					sharedProperties.CharacterNeutralState ();
					AIcontrols.DoesAIBlock ();
					decisionTimer = 0f;
				}
			}
		}
		else if (decision <= 81) {
			AIcontrols.AIPressedForward ();
			character.SetBackPressed (false);
		}
		else if (decision <= 88) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
			decisionTimer = 0f;
		}
		else if (decision <= 90) {
			sharedProperties.CharacterNeutralState ();
			AIcontrols.AIJump ();
			AIcontrols.DoesAIBlock ();
		}
		else {
			AIcontrols.AIPressedBackward ();
			character.SetBackPressed (true);
		}
	}

	void RegularMidRangeDecisions (){
		DecisionMade (5, 2);
		if (decision <= 40) {
			AIcontrols.AIPressedForward ();
			character.SetBackPressed (false);
		}
		else if (decision <= 55 && !animator.GetBool("isAttacking")) {			
			AIHadoukenLimitsWithLowForward();
			decisionTimer = 0f;
		}
		else if (decision <= 57) {
			AIcontrols.AIForward (10);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 59) {
			AIcontrols.AIStrong (10);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 70) {
			AIcontrols.AICrouch ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 75) {
			AIcontrols.AIStand ();
			sharedProperties.CharacterNeutralState ();
			character.SetBackPressed (false);
		}
		else if (decision <= 85) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else if (decision <= 88) {
			sharedProperties.CharacterNeutralState ();
			AIcontrols.AIJump ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 89) {
			AIRolls ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 91 && !animator.GetBool("isAttacking")) {
			AIHurricaneKicks ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 93) {
			AIcontrols.AISweep ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 94) {
			AIcontrols.AIShort (10);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 95) {
			AIcontrols.AIJab (2);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else {
			AIcontrols.AIPressedBackward ();
			character.SetBackPressed (true);
		}
	}

	void RegularFarRangeDecisions (){
		DecisionMade (5, 1);
		if (decision <= 30) {
			AIcontrols.AIPressedForward ();
			character.SetBackPressed (false);
		}
		else if (decision <= 35) {
			AIcontrols.AIPressedBackward ();
			character.SetBackPressed (true);
		}
		else if (decision <= 65) {
			if (playerController != null){
				if (playerController.GetProjectileP1Parent ().transform.childCount <= 0 && !animator.GetBool("isAttacking")) {
					AIHadoukens ();
					AIcontrols.DoesAIBlock ();
					sharedProperties.CharacterNeutralState ();
					decisionTimer = 0f;
				}
				else {
					AIcontrols.AIPressedForward ();
					character.SetBackPressed (false);
				}				
			}
			else if (opponentController != null){
				if (opponentController.GetProjectileP2Parent ().transform.childCount <= 0 && !animator.GetBool("isAttacking")) {
					AIHadoukens ();
					AIcontrols.DoesAIBlock ();
					sharedProperties.CharacterNeutralState ();
					decisionTimer = 0f;
				}
				else {
					AIcontrols.AIPressedForward ();
					character.SetBackPressed (false);
				}				
			}
		}
		else if (decision <= 68) {
			AIcontrols.AIShort (10);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 71) {
			AIcontrols.AIJab (2);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 75) {
			AIcontrols.AIJump ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 90) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else {
			sharedProperties.CharacterNeutralState ();
			character.SetBackPressed (false);
		}
	}

	void KnockDownCloseRangeDecisions (){
		DecisionMade (5, 4);
		if (decision <= 30) {
			AIcontrols.AICrouch ();
			sharedProperties.CharacterNeutralState ();
			character.SetBackPressed (true);
		}
		else if (decision <= 35) {
			AIcontrols.AIJab (2);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 45) {
			AIcontrols.AIShort (10);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 50) {
			AIcontrols.AIPressedBackward ();
			character.SetBackPressed (true);
		}
		else if (decision <= 60) {
			AIcontrols.AIPressedForward ();
			character.SetBackPressed (false);
		}
		else if (decision <= 65) {
			AIJabShoryuken ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 70) {
			sharedProperties.CharacterNeutralState ();
			AIcontrols.AIJump ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 80) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
			decisionTimer = 0f;
		}
		else if (decision <= 85) {			
			AIHadoukenLimitsWithLowForward();
			decisionTimer = 0f;
		}
		else {
			sharedProperties.CharacterNeutralState ();
			AIRolls ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
	}
	
	void KnockDownMidRangeDecisions (){
		DecisionMade (5, 3);
		if (decision <= 20) {
			AIcontrols.AICrouch ();
			sharedProperties.CharacterNeutralState ();
			character.SetBackPressed (true);
		}
		else if (decision <= 23) {
			AIcontrols.AIJab (2);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 26) {
			AIcontrols.AIShort (10);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 36) {
			AIcontrols.AIPressedForward ();
			character.SetBackPressed (false);
		}
		else if (decision <= 38) {
			AIJabShoryuken ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 42) {
			sharedProperties.CharacterNeutralState ();
			AIcontrols.AIJump ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 65) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
			decisionTimer = 0f;
		}
		else if (decision <= 90) {			
			AIHadoukenLimitsWithLowForward();
			decisionTimer = 0f;
		}
		else {
			sharedProperties.CharacterNeutralState ();
			AIRolls ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
	}

    void MidRangeOtherFighterBlockedDecisions()
    {
        decision = Random.Range(0, 100);
        if (decision <= 5)
        {
            AIHadoukenLimitsWithLowForward();
        }
        else if (decision <= 20)
        {
            AIcontrols.AILowForward();
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
        }
        else if (decision <= 35)
        {
            AIcontrols.AIStrong(10);
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
        }
        else if (decision <= 40)
        {
            AIcontrols.AIJab(2);
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
        }
        else if (decision <= 45)
        {
            AIcontrols.AIShort(10);
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
        }
        else if (decision <= 55)
        {
            AIcontrols.AISweep();
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
        }
        else
        {
            AIcontrols.AICrouch();
            sharedProperties.CharacterNeutralState();
            character.SetBackPressed(true);
        }
    }

    void CloseRangeOtherFighterBlockedDecisions (){
		decision = Random.Range(0,100);
		if (decision <= 5) {
			AIHadoukenLimitsWithLowForward();
        }
        else if (decision <= 8){
            AIJabShoryuken();
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
        }
        else if (decision <= 25) {
			AIcontrols.AILowForward ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 35) {
			if (character.GetSuper >= 100f){
				AIShinryukens();
			}
			else{
				AIcontrols.AIStrong (10);
			}
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 45) {
			AIcontrols.AIJab (2);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 55) {
			AIcontrols.AIShort (10);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else {
			AIcontrols.AICrouch ();
			sharedProperties.CharacterNeutralState ();
			character.SetBackPressed (true);
		}
	}

	void OtherFighterGotHitDecisions (){
		decision = Random.Range(0,100);
		if (decision <= 40) {
			AIShoryukens ();
			sharedProperties.CharacterNeutralState ();
		}
		else if (decision <= 70) {
			AIHurricaneKicks ();
			sharedProperties.CharacterNeutralState ();
		}
		else if (decision <= 85) {
			AIcontrols.AIFierce (2, 0);
			sharedProperties.CharacterNeutralState ();
		}
		else {
			if (character.GetSuper >= 100f){
				AIShinryukens();
			}
			else{
				AIcontrols.AISweep ();
			}
			sharedProperties.CharacterNeutralState ();
		}
	}
	
	void PreparationForAntiAir (){
		decision = Random.Range(0,100);
		if (decision <= 60) {
			AIShoryukens ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 80)
        {
            if (character.GetSuper >= 100f)
            {
                AIShinryukens();
            }
            else
            {
                AIcontrols.AIFierce(2, 0);
            }
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
        else
        {
            AIcontrols.AISweep();
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
        }
	}
	
	void DecisionMade (int minDivisor, int maxDivisor){
		if (decisionTimer <= 0) {
			decision = Random.Range (0, 100);
			decisionTimer = Random.Range (decisionTimerInput / minDivisor, decisionTimerInput / maxDivisor);
		}
	}

	void AIHadoukenLimitsWithLowForward (){
		if (playerController != null) {
			if (playerController.GetProjectileP1Parent ().transform.childCount <= 0) {
				AIHadoukens ();
				AIcontrols.DoesAIBlock ();
				sharedProperties.CharacterNeutralState ();
			}
			else {
				AIcontrols.AILowForward ();
				sharedProperties.CharacterNeutralState ();
				AIcontrols.DoesAIBlock ();
			}
		}
		else if (opponentController != null) {
			if (opponentController.GetProjectileP2Parent ().transform.childCount <= 0) {
				AIHadoukens ();
				AIcontrols.DoesAIBlock ();
				sharedProperties.CharacterNeutralState ();
			}
			else {
				AIcontrols.AILowForward ();
				sharedProperties.CharacterNeutralState ();
				AIcontrols.DoesAIBlock ();
			}
		}
	}
	
	void AIShinryukens(){
		if (AIcontrols.GetConditionsSpecialAttack()){			
			animator.SetTrigger("motionSuperInputed");		
			if (animator.GetBool("isAttacking") == false){
				AIcontrols.AIStand ();
				character.AttackState();
				animator.Play("KenShinryuken",0);					
			}
		}
	}
	
	void AIJabShoryuken(){
		if (AIcontrols.GetConditionsSpecialAttack()) {
			
			animator.SetTrigger ("shoryukenInputed");			
			if (animator.GetBool ("isAttacking") == false) {
				AIcontrols.AIStand ();
				character.AttackState ();
				animator.Play ("KenShoryukenJab", 0);
				animator.SetInteger ("shoryukenPunchType", 0);
			}
		}
	}
	
	void AIShoryukens(){
		int shoryukenPunch = Random.Range(0,3);
		if (AIcontrols.GetConditionsSpecialAttack()){
			
			animator.SetTrigger("shoryukenInputed");			
			if (animator.GetBool("isAttacking") == false){
				AIcontrols.AIStand();
				character.AttackState();
                switch (shoryukenPunch)
                {
                    case 0:
                        animator.Play("KenShoryukenJab", 0);
                        animator.SetInteger("shoryukenPunchType", 0);
                        break;

                    case 1:
                        animator.Play("KenShoryukenStrong", 0);
                        animator.SetInteger("shoryukenPunchType", 1);
                        break;

                    default:
                        animator.Play("KenShoryukenFierce", 0);
                        animator.SetInteger("shoryukenPunchType", 2);
                        break;
                }
            }
        }
	}
	
	void AIHadoukens(){		
		if (AIcontrols.GetConditionsSpecialAttack()){
			
			animator.SetTrigger("hadoukenInputed");
			
			if(animator.GetBool("isAttacking") == false){			
				AIcontrols.AIStand();	
				character.AttackState();
				animator.Play("KenHadouken",0);		
				animator.SetInteger("hadoukenPunchType", Random.Range(0,3));
			}
		}
	}
	
	void AIHurricaneKicks(){			
		int hurricaneType = Random.Range(0,3);
		if (AIcontrols.GetConditionsSpecialAttack()){	
			
			animator.SetTrigger("hurricaneKickInputed");	
			
			if (animator.GetBool("isAttacking") == false){
				AIcontrols.AIStand();
				character.AttackState();
				animator.SetInteger("hurricaneKickType", hurricaneType);
				animator.Play("KenHurricaneKickLiftOff",0);
			}
		}
	}
	
	void AIRolls(){
		int rollType = Random.Range (0,3);
		if (AIcontrols.GetConditionsSpecialAttack()){
			
			animator.SetTrigger("rollInputed");
			if (animator.GetBool("isAttacking") == false){
				AIcontrols.AIStand();
				character.AttackState();
				animator.SetInteger("rollKickType", rollType);
				animator.Play("KenRoll");
			}
		}
	}
}
