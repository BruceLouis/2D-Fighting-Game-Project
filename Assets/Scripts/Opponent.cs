using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : MonoBehaviour {
	
	public bool isAI;
	public int decisionTimer;
	
	private TimeControl timeControl;
	private Animator animator;
	private Player player;
	private Character playerCharacter;
	private Character character;
	private Rigidbody2D physicsbody;
	private HealthBarP2 healthBar;
	private bool pressedForward, pressedBackward;
	private float distance, distanceFromPlayer;
	
	private int decision, decisionTimerInput;
	
	void Awake () {
		gameObject.layer = LayerMask.NameToLayer("Player2");
		gameObject.tag = "Player2";
		foreach(Transform character in gameObject.transform){
			character.gameObject.layer = LayerMask.NameToLayer("Player2");	
			character.gameObject.tag = "Player2";
		}			
	}
	
	// Use this for initialization
	void Start () {		
		
		timeControl = FindObjectOfType<TimeControl>();
		player = FindObjectOfType<Player>();
		playerCharacter = player.GetComponentInChildren<Character>();
		character = GetComponentInChildren<Character>();
		animator = GetComponentInChildren<Animator>();	
		physicsbody = GetComponentInChildren<Rigidbody2D>();	
		healthBar = FindObjectOfType<HealthBarP2>();	
		
		decisionTimerInput = decisionTimer;		
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if (timeControl.gameOn == true){
		
			IsThrown();
			
			DetermineSide();
			
			if (!isAI){
				PunchBagControls();
			}
			else{
				
				if (character.GetComponent<Ken>() != null){
					KenAIControls();
				}
			}
		}
		else{
			if (TimeControl.roundOver == false){
				TimeControl.slowDown = true;
				if (animator.GetBool("isKOed") == true){
					character.KO();
					TimeControl.roundOver = true;
				}					
			}				
				
			if (TimeControl.victoryPose == true && animator.GetBool("isKOed") == false){
				TimeControl.winner = "You Lose";
				animator.Play("KenVictoryPose");
			}	
		}
		if (character.GetHealth() <= 0){
			animator.SetBool("isKOed", true);
		}
		DetermineSide();
		healthBar.SetHealth(character.GetHealth());
	}
	
	void KenAIControls(){
		decisionTimer--;
		if (distanceFromPlayer >= 2f){
			if (decisionTimer <= 0){
				decision = Random.Range(0,100);
				decisionTimer = Random.Range (decisionTimerInput/2, decisionTimerInput);
			}
			if (decision <= 20){
				AIPressedForward();
				character.SetBackPressed(false);
			}
			else if (decision <= 30 && decision > 20){
				AIPressedBackward();
				character.SetBackPressed(true);
			}
			else if (decision <= 75 && decision > 30){
				AIHadoukens();
				CharacterWalkState();
				DoesAIBlock();
			}
			else if (decision <= 80 && decision > 75){
				AIShort();
				CharacterWalkState();
				DoesAIBlock();
			}
			else if (decision <= 85 && decision > 80){
				AIJab();
				CharacterWalkState();
				DoesAIBlock();
			}
			else if (decision <= 90 && decision > 85){
				AIJump();
				DoesAIBlock();
			}
			else{
				CharacterWalkState();
				character.SetBackPressed(false);
			}
		}
		else if (distanceFromPlayer < 2f && distanceFromPlayer >= 0.75f){		
			if (decisionTimer <= 0){
				decision = Random.Range(0,100);
				decisionTimer = Random.Range (decisionTimerInput/4, decisionTimerInput/2);
			}
			if (playerCharacter.GetBlockStunned() == true){
				if (decision <= 30){
					AIHadoukens();
					CharacterWalkState();
					DoesAIBlock();
				}
				else if (decision <= 40 && decision > 30){
					AILowForward();
					CharacterWalkState();
					DoesAIBlock();
				}
				else if (decision <= 45 && decision > 40){
					AIStrong();
					CharacterWalkState();
					DoesAIBlock();
				}
				else if (decision <= 50 && decision > 45){
					AIJab();
					CharacterWalkState();
					DoesAIBlock();
				}					
				else if (decision <= 55 && decision > 50){
					AIShort();
					CharacterWalkState();
					DoesAIBlock();
				}			
				else if (decision <= 65 && decision > 55){
					AISweep();
					CharacterWalkState();
					DoesAIBlock();
				}			
				else{
					AICrouch();
					CharacterWalkState();
					character.SetBackPressed(true);
				}
			}
			else{
				if (decision <= 10){			
					AIPressedBackward();
					character.SetBackPressed(true);
				}
				else if(decision <= 30 && decision > 10){
					AIHadoukens();
					CharacterWalkState();
					DoesAIBlock();
				}
				else if(decision <= 32 && decision > 30){
					AIForward();
					CharacterWalkState();
					DoesAIBlock();
				}
				else if(decision <= 35 && decision > 30){
					AIStrong();
					CharacterWalkState();
					DoesAIBlock();
				}
				else if(decision <= 45 && decision > 35){
					AICrouch();
					CharacterWalkState();
					DoesAIBlock();
				}
				else if(decision <= 50 && decision > 45){
					AIStand();
					CharacterWalkState();
					character.SetBackPressed(false);
				}
				else if(decision <= 57 && decision > 50){
					AIJump();
					AIPressedForward();
					character.SetBackPressed(false);
				}
				else if(decision <= 60 && decision > 57){
					AIJump();
					CharacterWalkState();
					DoesAIBlock();
				}
				else if(decision <= 63 && decision > 60){
					AIRolls();
					CharacterWalkState();
					DoesAIBlock();
				}
				else if(decision <= 65 && decision > 63){
					AIHurricaneKicks();
					CharacterWalkState();
					DoesAIBlock();
				}
				else if(decision <= 75 && decision > 65){
					AIFootsieLowForward();
					character.SetBackPressed(false);
				}
				else if(decision <= 80 && decision > 75){
					AISweep();
					CharacterWalkState();
					DoesAIBlock();
				}
				else if(decision <= 85 && decision > 80){
					AIShort();
					CharacterWalkState();
					DoesAIBlock();
				}
				else if(decision <= 90 && decision > 85){
					AIJab();
					CharacterWalkState();
					DoesAIBlock();
				}
				else{
					AIPressedForward();
					character.SetBackPressed(false);
				}
			}
		}
		else{
			decision = Random.Range(0,100);
			//anti air
			if (playerCharacter.GetAirborne() == true && playerCharacter.GetKnockDown() == false){
				if (decision <= 60){
					AIShoryukens();
					CharacterWalkState();
					DoesAIBlock();
				}
				else{
					AIFierce();
					CharacterWalkState();
					DoesAIBlock();
				}
			}
			else if (playerCharacter.GetHitStunned() == true){
				if (decision <= 40){
					AIShoryukens();
					CharacterWalkState();
				}
				else if (decision <= 60 && decision > 40){
					AIHurricaneKicks();
					CharacterWalkState();
				}
				else if (decision <= 80 && decision > 60){
					AIFierce();
					CharacterWalkState();
				}
				else{
					AISweep();
					CharacterWalkState();
				}
			}
			else if (playerCharacter.GetKnockDown() == true && playerCharacter.GetAirborne() == false){
				if (decision <= 60){
					AICrouch();
					CharacterWalkState();
					character.SetBackPressed(true);
				}
				else if (decision <= 70 && decision > 60){
					AIJab();
					CharacterWalkState();
					DoesAIBlock();
				}					
				else if (decision <= 75 && decision > 70){
					AIShort();
					CharacterWalkState();
					DoesAIBlock();
				}					
				else if (decision <= 80 && decision > 75){
					AIPressedBackward();
					character.SetBackPressed(true);
				}				
				else if (decision <= 85 && decision > 80){
					AIPressedForward();
					character.SetBackPressed(false);
				}				
				else if (decision <= 90 && decision > 85){
					AIJabShoryuken();
					CharacterWalkState();
					DoesAIBlock();
				}				
				else{
					AIRolls();
					CharacterWalkState();
					DoesAIBlock();
				}
			}
						
			
			else if (playerCharacter.GetBlockStunned() == true){
				if (decision <= 30){
					AIHadoukens();
					CharacterWalkState();
					DoesAIBlock();
				}
				else if (decision <= 45 && decision > 30){
					AILowForward();
					CharacterWalkState();
					DoesAIBlock();
				}
				else if (decision <= 55 && decision > 45){
					AIStrong();
					CharacterWalkState();
					DoesAIBlock();
				}
				else if (decision <= 65 && decision > 55){
					AIJab();
					CharacterWalkState();
					DoesAIBlock();
				}					
				else if (decision <= 75 && decision > 65){
					AIShort();
					CharacterWalkState();
					DoesAIBlock();
				}			
				else{
					AICrouch();
					CharacterWalkState();
					character.SetBackPressed(true);
				}
			}
					
			else{
				if (decision <= 50){
					AICrouch();
					CharacterWalkState();
					character.SetBackPressed(true);
				}
				else if (decision <= 55 && decision > 50){
					AILowForward();
					CharacterWalkState();
					DoesAIBlock();
				}
				else if (decision <= 60 && decision > 55){
					AIStrong();
					CharacterWalkState();
					DoesAIBlock();
				}
				else if (decision <= 65 && decision > 60){
					AIJab();
					CharacterWalkState();
					DoesAIBlock();
				}					
				else if (decision <= 70 && decision > 65){
					AIShort();
					CharacterWalkState();
					DoesAIBlock();
				}
				else if (decision <= 73 && decision > 70){
					AIJabShoryuken();
					CharacterWalkState();
					DoesAIBlock();
				}
				else if (decision <= 78 && decision > 73){
					AIThrow();
					CharacterWalkState();
					DoesAIBlock();
				}
				else if (decision <= 82 && decision > 78){
					AIHadoukens();
					CharacterWalkState();
					DoesAIBlock();
				}				
				else if (decision <= 90 && decision > 82){
					AIPressedForward();
					character.SetBackPressed(false);
				}
				else{
					AIPressedBackward();
					character.SetBackPressed(true);
				}
			}
			CharacterWalkState();
		}
		Walk();		
	}
	
	void AIThrow(){
		int distance = Random.Range(0, 10);
		distance--;
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isAttacking") == false
		    && animator.GetBool("isMidAirRecovering") == false && animator.GetBool("isThrown") == false){	
		    AIStand();
			AIPressedForward();
			character.SetBackPressed(false);
			if (distance <= 0){		
				character.AttackState();
				animator.Play("KenThrowStartup");
			}
		}
	}
		
	
	void AIFootsieLowForward(){
		int distance = Random.Range(0, 30);
		distance--;
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isAttacking") == false
		    && animator.GetBool("isMidAirRecovering") == false && animator.GetBool("isThrown") == false){	
			AIPressedForward();
			character.SetBackPressed(false);
			if (distance <= 0){		
				character.AttackState();
				AICrouch();
				character.CharacterForward();
				AIStand ();
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
				AIStand();
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
				AIStand();
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
				AIStand();	
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
				AIStand();
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
				AIStand();
				character.AttackState();
				animator.SetInteger("rollKickType", rollType);
				animator.Play("KenRoll");
			}
		}
	}
		
	void AIJab(){
		int crouchOrStand = Random.Range(0, 1);
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
			character.CharacterJab();
			AIStand();
		}
	}
	
	void AIShort(){
		int crouchOrStand = Random.Range(0, 1);
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
	
	void AIStrong(){
		int crouchOrStand = Random.Range(0, 10);
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
			character.CharacterStrong();
		}
	}
	
	void AIForward(){
		int crouchOrStand = Random.Range(0, 10);
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
		}
	}
	
	void AIFierce(){
		int crouchOrStand = Random.Range(0, 1);
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
			character.CharacterFierce();
			AIStand();
		}
	}
	
	void AILowForward(){
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
	
	void AISweep(){
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
	
	void AIJump(){		
		if (animator.GetBool("isAttacking") == false && animator.GetBool("isInHitStun") == false
            && animator.GetBool("isInBlockStun") == false && animator.GetBool("isLiftingOff") == false
		    && animator.GetBool("isStanding") == true && animator.GetBool("isAirborne") == false
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isMidAirRecovering") == false
		    && animator.GetBool("isThrown") == false){
			AIStand();
			character.CharacterJump(pressedForward, pressedBackward);
			animator.SetBool ("isStanding",false);
			animator.SetBool ("isLiftingOff",true);
		}
	}
	
	void DoesAIBlock(){
		int coinflip = Random.Range(0,6);
		if (coinflip >= 2){
			character.SetBackPressed(true);
		}
		else{
			character.SetBackPressed(false);
		}
	}
	
	void AIStand(){
		animator.SetBool("isStanding", true);
		animator.SetBool("isCrouching", false);
	}	
	
	void AICrouch(){
		animator.SetBool("isStanding", false);
		animator.SetBool("isCrouching", true);
	}
	
	void AIPressedForward(){
		pressedForward = true;
		pressedBackward = false;
	}
	
	void AIPressedBackward(){
		pressedBackward = true;
		pressedForward = false;
	}
		
	void SideSwitch(){			
		if (animator.GetBool("isAttacking") == false){
			if (distance < 0 && character.side == Character.Side.P1){			
				character.side = Character.Side.P2;
				character.SideSwitch();
				CharacterWalkState();
			}
			else if (distance >= 0 && character.side == Character.Side.P2){
				character.side = Character.Side.P1;
				character.SideSwitch();
				CharacterWalkState();
			}
		}
	}
	
	void IsThrown(){
		if (animator.GetBool("isThrown") == true){
			if (character.side == Character.Side.P2){
				character.transform.position = new Vector3(playerCharacter.transform.position.x + 0.25f, playerCharacter.transform.position.y, 0f);
			}
			else{
				character.transform.position = new Vector3(playerCharacter.transform.position.x - 0.25f, playerCharacter.transform.position.y, 0f);
			}
		}
	}
	
	void Walk(){						
		if (animator.GetBool("isAttacking") == false && animator.GetBool("isInHitStun") == false
			&& animator.GetBool("isInBlockStun") == false && animator.GetBool("isLiftingOff") == false
			&& animator.GetBool("isStanding") == true && animator.GetBool("isAirborne") == false
			&& animator.GetBool("isKnockedDown") == false && animator.GetBool("isMidAirRecovering") == false
		    && animator.GetBool("isThrown") == false){
		    AIStand();	
			if (pressedForward == true && animator.GetBool("isWalkingBackward") == false){
				animator.SetBool("isWalkingForward", true);	
				if (character.side == Character.Side.P1){
					character.transform.Translate(Vector3.right * character.walkSpeed * Time.deltaTime);		
				}
				else{
					character.transform.Translate(Vector3.left * character.walkSpeed * Time.deltaTime);		
				}
			}
			else if (pressedBackward == true && animator.GetBool("isWalkingForward") == false){
				animator.SetBool("isWalkingBackward", true);				
				if (character.side == Character.Side.P2){
					character.transform.Translate(Vector3.right * character.walkSpeed * Time.deltaTime);		
				}
				else{
					character.transform.Translate(Vector3.left * character.walkSpeed * Time.deltaTime);		
				}		
			}	
		}
		if (pressedForward == false){
			animator.SetBool("isWalkingForward", false);	
		}
		if (pressedBackward == false){	
			animator.SetBool("isWalkingBackward", false);	
		}
	}
	
	void DetermineSide(){
		distanceFromPlayer = Mathf.Abs(playerCharacter.transform.position.x -  character.transform.position.x);
		distance = playerCharacter.transform.position.x - character.transform.position.x;	
		if (animator.GetBool("isAirborne") == false && animator.GetBool("isKnockedDown") == false
		    && animator.GetBool("isThrown") == false && animator.GetBool("throwTargetAcquired") == false){
			//determine which side
			SideSwitch();
		}
	}
		
	void CharacterWalkState(){
		pressedForward = false;
		pressedBackward = false;
	}
	
	
	void WhoWonAndLost (string WonOrLost)
	{
		gameObject.tag = WonOrLost;
		foreach (Transform character in gameObject.transform) {
			character.gameObject.tag = WonOrLost;
		}
	}
		
	void PunchBagControls(){
		if (Input.GetKey(KeyCode.Alpha2)){
			if (character.side == Character.Side.P1){
				pressedForward = true;
			}
			else{
				pressedBackward = true;
			}
		}
		if (Input.GetKey(KeyCode.Alpha1)){
			if (character.side == Character.Side.P2){
				pressedForward = true;
			}
			else{
				pressedBackward = true;
			}
		}
		if (Input.GetKeyUp(KeyCode.Alpha2)){
			if (character.side == Character.Side.P1){
				pressedForward = false;
				animator.SetBool("isWalkingForward", false);	
			}
			else{
				pressedBackward = false;
				animator.SetBool("isWalkingBackward", false);	
			}
			
		}	
		if (Input.GetKeyUp(KeyCode.Alpha1)){
			if (character.side == Character.Side.P2){
				pressedForward = false;
				animator.SetBool("isWalkingForward", false);	
			}
			else{
				pressedBackward = false;
				animator.SetBool("isWalkingBackward", false);	
			}
		}
		
		if (animator.GetBool("isAttacking") == false && animator.GetBool("isInHitStun") == false
		    && animator.GetBool("isInBlockStun") == false && animator.GetBool("isLiftingOff") == false
		    && animator.GetBool("isStanding") == true && animator.GetBool("isAirborne") == false){
			if (Input.GetKeyDown(KeyCode.R)){
				character.CharacterJump(pressedForward, pressedBackward);
				animator.SetBool ("isStanding",false);
				animator.SetBool ("isLiftingOff",true);
			}
		}
		Walk();
		
		if (animator.GetBool("isAttacking") == false){
			if (Input.GetKeyDown(KeyCode.Y)){
				character.AttackState();
				character.CharacterJab();
			}		
			if (Input.GetKeyDown(KeyCode.E)){
				character.AttackState();
				character.CharacterRoundhouse();
			}		
			if (Input.GetKeyDown(KeyCode.W)){
				character.AttackState();
				character.CharacterForward();
			}				
			if (Input.GetKeyDown(KeyCode.G)){
				character.AttackState();
				animator.Play("KenThrowStartup");
			}
			
		}
		if (Input.GetKeyDown(KeyCode.Alpha3)){
			if(animator.GetBool("isAttacking") == false && animator.GetBool("isAirborne") == false){				
				character.AttackState();
				animator.Play("KenHadouken",0);		
				animator.SetInteger("hadoukenPunchType", Random.Range(0,3));
				animator.SetInteger("hadoukenOwner", 2);
			}
			animator.SetTrigger("hadoukenInputed");
		}	
		if (Input.GetKey (KeyCode.Q)){	
			
			if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
			    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
			    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isMidAirRecovering") == false){
				animator.SetTrigger("shoryukenInputed");			
				if (animator.GetBool("isAttacking") == false){
					character.AttackState();
					animator.Play("KenShoryukenFierce",0);
				}
				animator.SetTrigger("shoryukenInputed");
				animator.SetInteger("shoryukenPunchType", 2);
			}
		}
		if (Input.GetKey(KeyCode.L)){
			animator.SetBool("isStanding", false);
			animator.SetBool("isCrouching", true);
		}	
		if (Input.GetKey(KeyCode.O)){
			animator.SetBool("isStanding", true);
			animator.SetBool("isCrouching", false);
		}	
		character.SetBackPressed(pressedBackward);
	}
	
}
