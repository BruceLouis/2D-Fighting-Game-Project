using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AkumaAI : MonoBehaviour {	
	delegate void AIWhichAttack();
	AIWhichAttack AIwhichAttack;
	
	[SerializeField] float decisionTimer, antiAirTimer;
	
	public enum LastUsedNormal {none, lForward, lStrong, lJab, lShort, lRoundhouse, lFierce};
	public LastUsedNormal lastUsedNormal; 
	
	private Animator animator;
	private Player player, playerController;
	private Opponent opponent, opponentController;
	private Character playerCharacter, opponentCharacter;
	private Character character;
	private SharedProperties sharedProperties;
	private AIControls AIcontrols;
	
	private int decision;
	private float decisionTimerInput, antiAirTimerInput;
	
	private bool inComboSequence;
	
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
		lastUsedNormal = LastUsedNormal.none;
	}
	
	void Update () {
		if (!AIcontrols.FreeToMakeDecisions()){
			SetLastUsedNormal(-1);
		}
	}
	
	public void Behaviors(){
		decisionTimer--;
		antiAirTimer--;
		
		if (AIcontrols.FreeToMakeDecisions() && !TimeControl.inSuperStartup[0] && !TimeControl.inSuperStartup[1]){
			if (animator.GetBool("isAirborne") == true && animator.GetBool("isLiftingOff") == false){
				if (animator.GetBool("hyakkishuActive") == true){
					HyakkishuDecisions();
				}
				else{
					AirborneAttacks();
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
						RegularMidRangeDecisions();
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
					if (playerCharacter.GetAirborne() == true && playerCharacter.GetKnockDown() == false && !inComboSequence){
						if (antiAirTimer <= 0f){			
							sharedProperties.AIAntiAirDecision(65, RegularCloseRangeDecisions, PreparationForAntiAir);
							antiAirTimer = antiAirTimerInput;
						}
						else{
							RegularCloseRangeDecisions();
						}					
					}
					else if (playerCharacter.GetKnockDown() == true && playerCharacter.GetAirborne() == false){
						KnockDownCloseRangeDecisions ();
					}
					else if (playerCharacter.GetBlockStunned() == true){
						CloseRangeOtherFighterBlockedDecisions ();		
					}
					else if (playerCharacter.GetHitStunned() == true){
						OtherFighterGotHitDecisions ();
					}
					else{
						RegularCloseRangeDecisions();
					}
				}
				else if (opponent != null){
					if (opponentCharacter.GetAirborne() == true && opponentCharacter.GetKnockDown() == false && !inComboSequence){
						if (antiAirTimer <= 0f){			
							sharedProperties.AIAntiAirDecision(65, RegularCloseRangeDecisions, PreparationForAntiAir);
							antiAirTimer = antiAirTimerInput;
						}
						else{
							RegularCloseRangeDecisions();
						}					
					}
					else if (opponentCharacter.GetKnockDown() == true && opponentCharacter.GetAirborne() == false){
						KnockDownCloseRangeDecisions ();
					}
					else if (opponentCharacter.GetHitStunned() == true){
						OtherFighterGotHitDecisions ();
					}
					else{
						RegularCloseRangeDecisions();
					}
				}
			}
		}
		AIcontrols.AIWalks();
	}
	
	void RegularCloseRangeDecisions(){
		DecisionMade(5,4);
		if (decision <= 20){
			AIcontrols.AICrouch ();
			sharedProperties.CharacterNeutralState ();
			character.SetBackPressed (true);
		}
		else if (decision <= 27 && decision > 20) {
			AIcontrols.AILowForward();			
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}				
		else if (decision <= 30 && decision > 27) {
			AIcontrols.AIJab (2);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 37 && decision > 30) {
			AIcontrols.AIShort (15);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 43 && decision > 37) {
			AIcontrols.AIStrong(50);			
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 53 && decision > 43) {
			AIcontrols.AISweep ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 59 && decision > 53) {
			AIcontrols.AIThrow ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 64 && decision > 59) {
			if (character.GetSuper >= 100f){
				AIShunGokuSatsus();
			}
			else{
				AIHadoukenLimitsWithAttack(AIcontrols.AILowForward);
			}
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 69 && decision > 64) {
			AIcontrols.AIPressedForward ();
			character.SetBackPressed (false);
		}
		else if (decision <= 73 && decision > 69) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
			decisionTimer = 0f;
		}
		else if (decision <= 79 && decision > 73) {
			sharedProperties.CharacterNeutralState ();
			AIcontrols.AIJump ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;		
		}
		else if (decision <= 82 && decision > 79){
			AIcontrols.AIPressedBackward ();
			character.SetBackPressed (true);
		}	
		else if (decision <= 87 && decision > 82){
			AIJabShoryuken ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else{
			AIHyakkishus (1, 3);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;		
		}		
	}
	
	void RegularMidRangeDecisions(){
		DecisionMade (5, 2);
		if (decision <= 30) {
			AIcontrols.AIPressedForward ();
			character.SetBackPressed (false);
		}
		else if (decision <= 40 && decision > 30) {			
			AIHadoukenLimitsWithAttack(AIcontrols.AISweep);
			decisionTimer = 0f;
		}
		else if (decision <= 42 && decision > 40) {
			AIcontrols.AIForward (10);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 44 && decision > 42) {
			AIcontrols.AIStrong (10);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 50 && decision > 44) {
			AIcontrols.AICrouch ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 55 && decision > 50) {
			if (character.GetSuper >= 100f){
				AIShunGokuSatsus();
				sharedProperties.CharacterNeutralState ();
				AIcontrols.DoesAIBlock ();
			}
			else{
				AIcontrols.AIStand ();
				sharedProperties.CharacterNeutralState ();
				character.SetBackPressed (false);
			}
		}
		else if (decision <= 65 && decision > 55) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}		
		else if (decision <= 75 && decision > 65) {
			sharedProperties.CharacterNeutralState ();
			AIcontrols.AIJump ();
			AIcontrols.DoesAIBlock ();
		}		
		else if (decision <= 81 && decision > 75) {
			AIcontrols.AIPressedBackward ();
			AIcontrols.AIJump ();
			AIcontrols.DoesAIBlock ();
		}		
		else if (decision <= 83 && decision > 81) {
			AIcontrols.AISweep ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 84 && decision > 83) {
			AIcontrols.AIShort (10);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 85 && decision > 84) {
			AIcontrols.AIJab (2);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 89 && decision > 85) {
			AIcontrols.AIPressedBackward ();
			character.SetBackPressed (true);
		}
		else {
			AIHyakkishus (1, 3);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
	}
	
	void RegularFarRangeDecisions(){
		DecisionMade(5, 1);
		if (decision <= 30) {
			AIcontrols.AIPressedForward ();
			character.SetBackPressed (false);
		}
		else if (decision <= 35 && decision > 30) {
			AIcontrols.AIPressedBackward ();
			character.SetBackPressed (true);
		}		
		else if (decision <= 41 && decision > 35) {
			sharedProperties.CharacterNeutralState ();
			AIcontrols.AIJump ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 56 && decision > 41) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
			decisionTimer = 0f;
		}
		else if (decision <= 72 && decision > 56) {
			AIHyakkishus (0, 2);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 74 && decision > 72) {
			AIcontrols.AIShort (15);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 76 && decision > 74) {
			AIcontrols.AIJab (2);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 80 && decision > 76){
			AIcontrols.AIPressedBackward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
			decisionTimer = 0f;
		}
		else{			
			AIHadoukenLimitsWithWalk ();
		}
	}
	
	void KnockDownMidRangeDecisions (){
		DecisionMade (5, 3);
		if (decision <= 20) {
			AIcontrols.AICrouch ();
			sharedProperties.CharacterNeutralState ();
			character.SetBackPressed (true);
		}
		else if (decision <= 22 && decision > 20) {
			AIcontrols.AIJab (2);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 24 && decision > 22) {
			AIcontrols.AIShort (10);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 36 && decision > 24) {
			AIcontrols.AIPressedForward ();
			character.SetBackPressed (false);
		}
		else if (decision <= 42 && decision > 36) {
			if (character.GetSuper >= 100f){
				AIShunGokuSatsus();
				sharedProperties.CharacterNeutralState ();
				AIcontrols.DoesAIBlock ();
			}
			else{
				sharedProperties.CharacterNeutralState ();
				AIcontrols.AIJump ();
				AIcontrols.DoesAIBlock ();
			}
			decisionTimer = 0f;
		}
		else if (decision <= 55 && decision > 42) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
			decisionTimer = 0f;
		}
		else if (decision <= 75 && decision > 55) {			
			AIHadoukenLimitsWithAttack(AIcontrols.AISweep);
			decisionTimer = 0f;
		}
		else {
			AIHyakkishus (1, 3);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
	}
	
	void KnockDownCloseRangeDecisions(){
		DecisionMade (5, 4);
		if (decision <= 5) {
			AIcontrols.AIJab (2);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 10 && decision > 5) {
			AIcontrols.AIShort (10);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 20 && decision > 10) {
			AIcontrols.AIPressedBackward ();
			character.SetBackPressed (true);
		}
		else if (decision <= 30 && decision > 20) {
			AIcontrols.AIPressedForward ();
			character.SetBackPressed (false);
		}		
		else if (decision <= 35 && decision > 30) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
			decisionTimer = 0f;
		}
		else if (decision <= 40 && decision > 35) {			
			AIHadoukenLimitsWithWalk();
			decisionTimer = 0f;
		}
		else if (decision <= 60 && decision > 40)
        {
            if (character.GetSuper >= 100f)
            {
                AIShunGokuSatsus();
                sharedProperties.CharacterNeutralState();
                AIcontrols.DoesAIBlock();
            }
            else
            {
                AIcontrols.AICrouch();
                AIcontrols.AIPressedBackward();
                character.SetBackPressed(true);
            }
		}
		else if (decision <= 70 && decision > 60) {
			sharedProperties.CharacterNeutralState ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
			decisionTimer = 0f;
		}
		else if (decision <= 75 && decision > 70) {
			AIcontrols.AIPressedBackward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
			decisionTimer = 0f;
		}
		else{
			AIHyakkishus (0, 2);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}		
	}
	
	void PreparationForAntiAir(){
		decision = Random.Range(0,100);
		if (decision <= 60) {
			AIShoryukens ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else {
			AIcontrols.AIFierce (2, 0);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
	}
	
	void AirborneAttacks(){
		decision = Random.Range (0, 200);
		if (decision <= 3){
			AIcontrols.AIJumpFierce();
			sharedProperties.CharacterNeutralState();
		}
		else if (decision <= 6 && decision > 3){
			AIcontrols.AIJumpRoundhouse();
			sharedProperties.CharacterNeutralState();
		}				
		else if (decision <= 15 && decision > 6){
			if (playerController != null) {
				if (playerController.GetProjectileP1Parent ().transform.childCount <= 0) {
					AIAirHadoukens ();
					sharedProperties.CharacterNeutralState ();
				}
			}
			else if (opponentController != null){
				if (opponentController.GetProjectileP2Parent ().transform.childCount <= 0) {
					AIAirHadoukens ();
					sharedProperties.CharacterNeutralState ();
				}
			}
		}
	}
	
	void HyakkishuDecisions(){
		int hyakkishuType = Random.Range(0, 15);
		if (hyakkishuType <= 2){
			animator.SetInteger("hyakkishuAttackType", 0);
		}
		else if (hyakkishuType <= 4 && hyakkishuType > 2){
			animator.SetInteger("hyakkishuAttackType", 1);
		}
		else {
			animator.SetInteger("hyakkishuAttackType", 2);
		}		
	}
	
	void MidRangeOtherFighterBlockedDecisions (){		
		decision = Random.Range(0,100);
		if (decision <= 30) {			
			AIHadoukenLimitsWithAttack (AIcontrols.AISweep);
		}
		else if (decision <= 35 && decision > 30) {
			AIcontrols.AILowForward ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 40 && decision > 35) {
			AIcontrols.AIStrong (10);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 50 && decision > 40) {
			AIcontrols.AISweep ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 70 && decision > 50) {
			AIHyakkishus (1, 3);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else {
			AIcontrols.AICrouch ();
			sharedProperties.CharacterNeutralState ();
			character.SetBackPressed (true);
		}
	}
	
	void CloseRangeOtherFighterBlockedDecisions (){
		decision = Random.Range(0,100);
		if (decision <= 30) {
			AIHadoukenLimitsWithAttack(AIcontrols.AILowForward);
		}
		else if (decision <= 35 && decision > 30) {
			AIcontrols.AILowForward ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 40 && decision > 35) {
			AIcontrols.AIStrong (10);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 45 && decision > 40) {
			AIcontrols.AIJab (2);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 50 && decision > 45) {
			AIcontrols.AIShort (10);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 70 && decision > 50) {
			AIHyakkishus (0, 3);
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
		int whichSequence = Random.Range (0, 30);
		if (!inComboSequence){
			if (lastUsedNormal == LastUsedNormal.lForward || lastUsedNormal == LastUsedNormal.lFierce
				|| lastUsedNormal == LastUsedNormal.lStrong){
				if (whichSequence <= 2){
                    StartCoroutine(HadoukenEnder());
				}
				else if (whichSequence <= 3 && whichSequence > 2){
					AIShoryukens ();
					sharedProperties.CharacterNeutralState ();
					AIcontrols.DoesAIBlock ();
				}
				else{
					StartCoroutine(TatsuShoryukenSequence());
				}
			}
			else{
				if (whichSequence <= 15){
					AIcontrols.AILowForward ();
					sharedProperties.CharacterNeutralState ();
					AIcontrols.DoesAIBlock ();
				}
				else{
					AIcontrols.AIStrong (300);
					sharedProperties.CharacterNeutralState ();
					AIcontrols.DoesAIBlock ();
				}
			}			
		}
	}
	
	IEnumerator TatsuShoryukenSequence()
    {						
		inComboSequence = true;
		
		AIShortHurricaneKicks ();
		sharedProperties.CharacterNeutralState ();
		
		yield return new WaitUntil( () => !animator.GetBool("isAttacking"));
		
		if (player != null && playerCharacter.GetHitStunned() == true){
			AIShoryukens ();
			sharedProperties.CharacterNeutralState ();				
		}
		else if (opponent != null && opponentCharacter.GetHitStunned() == true){
			AIShoryukens ();
			sharedProperties.CharacterNeutralState ();
		}
		
		inComboSequence = false;
	}

    IEnumerator HadoukenEnder()
    {
        inComboSequence = true;

        AIHadoukens();
        sharedProperties.CharacterNeutralState();
        AIcontrols.DoesAIBlock();

        yield return new WaitUntil(() => !animator.GetBool("isAttacking"));

        inComboSequence = false;
    }
	
	void AIShunGokuSatsus(){
		if (AIcontrols.GetConditions()) {					
			AIcontrols.AIStand ();
			character.AttackState ();
			animator.Play ("AkumaShunGokuSatsuStartup", 0);
		}
	}
		
	void AIJabShoryuken(){
		if (AIcontrols.GetConditionsSpecialAttack()) {
			
			animator.SetTrigger ("shoryukenInputed");			
			if (animator.GetBool ("isAttacking") == false) {
				AIcontrols.AIStand ();
				character.AttackState ();
				animator.Play ("AkumaShoryukenJab", 0);
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
				if (shoryukenPunch == 0){
					animator.Play("AkumaShoryukenJab",0);
					animator.SetInteger("shoryukenPunchType", 0);
				}
				else if (shoryukenPunch == 1){
					animator.Play("AkumaShoryukenStrong",0);
					animator.SetInteger("shoryukenPunchType", 1);
				}
				else{				 
					animator.Play("AkumaShoryukenFierce",0);
					animator.SetInteger("shoryukenPunchType", 2);
				}
			}
		}
	}
	
	void AIShortHurricaneKicks(){			
		if (AIcontrols.GetConditionsSpecialAttack()){	
			
			animator.SetTrigger("hurricaneKickInputed");	
			
			if (animator.GetBool("isAttacking") == false){
				AIcontrols.AIStand();
				character.AttackState();
				animator.Play("KenHurricaneKickLiftOff",0);
				animator.SetInteger("hurricaneKickType", 0);
			}
		}
	}
	
	void AIHyakkishus(int min, int max){
		int hyakkishuType = Random.Range(min, max);
		if (AIcontrols.GetConditionsSpecialAttack()){	
			
			animator.SetTrigger("hyakkishuInputed");	
			
			if (animator.GetBool("isAttacking") == false){
				AIcontrols.AIStand();
				character.AttackState();
				animator.SetInteger("hyakkishuKickType", hyakkishuType);
				animator.Play("AkumaHyakkishu",0);
			}
		}
	}

	void AIHadoukenLimitsWithWalk (){
		if (playerController != null) {
			if (playerController.GetProjectileP1Parent ().transform.childCount <= 0) {
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
		else if (opponentController != null) {
			if (opponentController.GetProjectileP2Parent ().transform.childCount <= 0) {
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
	
	void AIHadoukenLimitsWithAttack (AIWhichAttack aiWhichAttack){
		if (playerController != null) {
			if (playerController.GetProjectileP1Parent ().transform.childCount <= 0) {
				AIHadoukens ();
				AIcontrols.DoesAIBlock ();
				sharedProperties.CharacterNeutralState ();
			}
			else {
				aiWhichAttack ();
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
				aiWhichAttack();
				sharedProperties.CharacterNeutralState ();
				AIcontrols.DoesAIBlock ();
			}
		}
	}
	
	void AIHadoukens(){		
		if (AIcontrols.GetConditionsSpecialAttack()){
			
			animator.SetTrigger("hadoukenInputed");
			
			if(animator.GetBool("isAttacking") == false){			
				AIcontrols.AIStand();	
				character.AttackState();
				animator.Play("AkumaHadouken",0);		
				animator.SetInteger("hadoukenPunchType", Random.Range(0,3));
			}
		}
	}
	
	void AIAirHadoukens(){		
		if (AIcontrols.GetConditionsAirborneAttack()){
			
			animator.SetTrigger("hadoukenInputed");
			
			if(animator.GetBool("isAttacking") == false){			
				AIcontrols.AIStand();	
				character.AttackState();
				animator.Play("AkumaAirHadouken",0);		
				animator.SetInteger("hadoukenPunchType", Random.Range(0,3));
			}
		}
	}
	void SetLastUsedNormal(int lastAttack){
		switch (lastAttack){
			case 0: 
				lastUsedNormal = LastUsedNormal.lJab;
				break;
			case 1:
				lastUsedNormal = LastUsedNormal.lStrong;	
				break;
			case 2: 
				lastUsedNormal = LastUsedNormal.lFierce;
				break;
			case 3:
				lastUsedNormal = LastUsedNormal.lShort;
				break;
			case 4: 
				lastUsedNormal = LastUsedNormal.lForward;
				break;
			case 5:
				lastUsedNormal = LastUsedNormal.lRoundhouse;
				break;
			default:
				lastUsedNormal = LastUsedNormal.none;
				break;
		}
	}
		
	void DecisionMade (int minDivisor, int maxDivisor){
		if (decisionTimer <= 0) {
			decision = Random.Range (0, 100);
			decisionTimer = Random.Range (decisionTimerInput / minDivisor, decisionTimerInput / maxDivisor);
		}
	}	
}
