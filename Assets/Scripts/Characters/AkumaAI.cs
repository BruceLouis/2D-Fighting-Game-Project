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
	
	private bool inComboSequence, hyakkishuDecisionChosen;
	
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
			else if (sharedProperties.GetAbDistanceFromOtherFighter() >= 2f){			
				RegularFarRangeDecisions();
			}
			else if (sharedProperties.GetAbDistanceFromOtherFighter() < 2f && sharedProperties.GetAbDistanceFromOtherFighter() >= 1f){	
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
		else if (decision <= 27) {
			AIcontrols.AILowForward();			
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}				
		else if (decision <= 30)
        {
            AIcontrols.AISweep();
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
            decisionTimer = 0f;
        }
		else if (decision <= 33) {
			AIcontrols.AIShort (15);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 43) {
			AIcontrols.AIStrong(50);			
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 53)
        {
            AIcontrols.AIJab(2);
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
            decisionTimer = 0f;
		}
		else if (decision <= 59)
        {
            if (character.GetSuper >= 100f)
            {
                AIShunGokuSatsus();
            }
            else
            {
                AIcontrols.AIThrow();
            }
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 64) {
			AIHadoukenLimitsWithAttack(AIcontrols.AILowForward);
            decisionTimer = 0f;
        }
		else if (decision <= 69) {
			AIcontrols.AIPressedForward ();
			character.SetBackPressed (false);
		}
		else if (decision <= 73) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
			decisionTimer = 0f;
		}
		else if (decision <= 79) {
			sharedProperties.CharacterNeutralState ();
			AIcontrols.AIJump ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;		
		}
		else if (decision <= 82){
			AIcontrols.AIPressedBackward ();
			character.SetBackPressed (true);
		}	
		else if (decision <= 87 && !animator.GetBool("isAttacking"))
        {
			AIJabShoryuken ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else
        {
			AIHyakkishus (0, 2);
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
		else if (decision <= 33 && !animator.GetBool("isAttacking")) {			
			AIHadoukenLimitsWithAttack(AIcontrols.AISweep);
			decisionTimer = 0f;
		}
		else if (decision <= 38) {
			AIcontrols.AIForward (10);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 43) {
			AIcontrols.AIStrong (10);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 50) {
			AIcontrols.AICrouch ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 55) {
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
		else if (decision <= 65) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}		
		else if (decision <= 75) {
			sharedProperties.CharacterNeutralState ();
			AIcontrols.AIJump ();
			AIcontrols.DoesAIBlock ();
		}		
		else if (decision <= 81) {
			AIcontrols.AIPressedBackward ();
			AIcontrols.AIJump ();
			AIcontrols.DoesAIBlock ();
		}		
		else if (decision <= 83) {
			AIcontrols.AISweep ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 87) {
			AIcontrols.AIShort (10);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 92) {
			AIcontrols.AIJab (2);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 95 && !animator.GetBool("isAttacking")) {
			AIHyakkishus (0, 3);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else {
			AIcontrols.AIPressedBackward ();
			character.SetBackPressed (true);
		}
	}
	
	void RegularFarRangeDecisions(){
		DecisionMade(5, 1);
		if (decision <= 30) {
			AIcontrols.AIPressedForward ();
			character.SetBackPressed (false);
		}
		else if (decision <= 35) {
			AIcontrols.AIPressedBackward ();
			character.SetBackPressed (true);
		}		
		else if (decision <= 41) {
			sharedProperties.CharacterNeutralState ();
			AIcontrols.AIJump ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 56) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
			decisionTimer = 0f;
		}
		else if (decision <= 72) {
			AIHyakkishus (1, 3);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 74) {
			AIcontrols.AIShort (15);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 76) {
			AIcontrols.AIJab (2);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 80){
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
		else if (decision <= 22) {
			AIcontrols.AIJab (2);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 24) {
			AIcontrols.AIShort (10);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 36) {
			AIcontrols.AIPressedForward ();
			character.SetBackPressed (false);
		}
		else if (decision <= 42) {
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
		else if (decision <= 55) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
			decisionTimer = 0f;
		}
		else if (decision <= 75) {			
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
		else if (decision <= 10) {
			AIcontrols.AIShort (10);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 20) {
			AIcontrols.AIPressedBackward ();
			character.SetBackPressed (true);
		}
		else if (decision <= 30) {
			AIcontrols.AIPressedForward ();
			character.SetBackPressed (false);
		}		
		else if (decision <= 35) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
			decisionTimer = 0f;
		}
		else if (decision <= 40) {			
			AIHadoukenLimitsWithWalk();
			decisionTimer = 0f;
		}
		else if (decision <= 60)
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
		else if (decision <= 70) {
			sharedProperties.CharacterNeutralState ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
			decisionTimer = 0f;
		}
		else if (decision <= 75) {
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
		else if (decision <= 6){
			AIcontrols.AIJumpRoundhouse();
			sharedProperties.CharacterNeutralState();
		}				
		else if (decision <= 15){
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
		int hyakkishuType = Random.Range(0, 45);
        if (!hyakkishuDecisionChosen)
        {
            StartCoroutine(HyakkishuCoroutine(hyakkishuType));
        }
	}
	
	void MidRangeOtherFighterBlockedDecisions (){		
		decision = Random.Range(0,100);
		if (decision <= 15)
        {			
			AIHadoukenLimitsWithAttack (AIcontrols.AISweep);
        }
        else if (decision <= 25)
        {
            AIHyakkishus(1, 3);
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
        }
        else if (decision <= 40)
        {
			AIcontrols.AILowForward ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 60)
        {
			AIcontrols.AIStrong (10);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 65)
        {
			AIcontrols.AISweep ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else
        {
			AIcontrols.AICrouch ();
			sharedProperties.CharacterNeutralState ();
			character.SetBackPressed (true);
		}
	}
	
	void CloseRangeOtherFighterBlockedDecisions (){
		decision = Random.Range(0,100);
		if (decision <= 10) {
			AIHadoukenLimitsWithAttack(AIcontrols.AILowForward);
        }
        else if (decision <= 20)
        {
            AIHyakkishus(0, 3);
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
        }
        else if (decision <= 25)
        {
            AIJabShoryuken();
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
            decisionTimer = 0f;
        }
        else if (decision <= 35) {
			AIcontrols.AILowForward ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 45) {
			AIcontrols.AIStrong (10);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 55) {
			AIcontrols.AIJab (2);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 65) {
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

    IEnumerator HyakkishuCoroutine(int num)
    {
        hyakkishuDecisionChosen = true;
        if (num <= 3)
        {
            animator.SetInteger("hyakkishuAttackType", 0);
        }
        else if (num <= 10)
        {
            animator.SetInteger("hyakkishuAttackType", 1);
        }
        else
        {
            animator.SetInteger("hyakkishuAttackType", 2);
        }

        yield return new WaitUntil(() => !animator.GetBool("isAttacking"));

        hyakkishuDecisionChosen = false;
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
