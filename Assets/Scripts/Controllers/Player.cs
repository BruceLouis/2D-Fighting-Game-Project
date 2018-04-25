using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Player : MonoBehaviour {
	
	public GameObject mugShotObject;
	public GameObject[] streetFighterCharacters;
	public Text nameText;
	public Sprite kenMugShot, feiLongMugShot, balrogMugShot;
	
	private TimeControl timeControl;
	private Animator animator;
	private Opponent opponent;
	private Character opponentCharacter;
	private Character character;
	private FeiLong feiLong;
	private Balrog balrog;
	private Rigidbody2D physicsbody;
	private ComboSystem comboSystem;
	private ChargeSystem chargeSystem;
	private HealthBarP1 healthBar;
	private Image mugShot;
	private CharacterChoice characterChoice;
	private GameObject projectileP1Parent;
	
	private bool pressedForward, pressedBackward, pressedCrouch, pressedUp;
	private float distance, distanceFromOpponent;	
	
	void Awake () {
		
		InitiateCharacter();
				
		gameObject.layer = LayerMask.NameToLayer("Player1");
		gameObject.tag = "Player1";
		foreach(Transform character in gameObject.transform){
			character.gameObject.layer = LayerMask.NameToLayer("Player1");	
			character.gameObject.tag = "Player1";
		}		
		character = GetComponentInChildren<Character>();
		animator = GetComponentInChildren<Animator>();	
		mugShot = mugShotObject.GetComponent<Image>();
	}
	
	// Use this for initialization
	void Start () {		
		
		timeControl = FindObjectOfType<TimeControl>();
		opponent = FindObjectOfType<Opponent>();
		opponentCharacter = opponent.GetComponentInChildren<Character>();
		physicsbody = GetComponentInChildren<Rigidbody2D>();
		comboSystem = GetComponent<ComboSystem>();
		chargeSystem = GetComponent<ChargeSystem>();
		character.side = Character.Side.P1;			
		healthBar = FindObjectOfType<HealthBarP1>();	
		if (character.GetComponent<FeiLong>() != null){
			feiLong = GetComponentInChildren<FeiLong>();
			mugShot.sprite = feiLongMugShot;
			nameText.text = "Fei Long";
		}
		else if (character.GetComponent<Ken>() != null){
			mugShot.sprite = kenMugShot;
			nameText.text = "Ken";
		}
		else if (character.GetComponent<Balrog>() != null){
			balrog = GetComponentInChildren<Balrog>();
			mugShot.sprite = balrogMugShot;
			nameText.text = "Balrog";
		}
		
		projectileP1Parent = GameObject.Find("ProjectileP1Parent");
		if (projectileP1Parent == null){
			projectileP1Parent = new GameObject("ProjectileP1Parent");
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if (timeControl.gameOn == true){
					
			character.SetBackPressed(pressedBackward);
			
			IsThrown();		
			CrouchOrStand();
			IsUpPressed();		
			DetermineSide();	
		
			if (animator.GetBool("isKnockedDown") == false && animator.GetBool("isThrown") == false && animator.GetBool("isMidAirRecovering") == false){
				if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
					&& animator.GetBool("isLiftingOff") == false){
					AttacksInput();					
					if (animator.GetBool("isStanding") == true && animator.GetBool("isAirborne") == false && animator.GetBool("isAttacking") == false){
						Walk();	
						if (pressedUp && animator.GetBool("isHeadButting") == false){
							CharacterJumping();
						}				
					}				
				}
			}
		}
		else{	
			if (TimeControl.roundOver == false){
				TimeControl.slowDown = true;
				if (animator.GetBool("isKOed") == true){
					character.KOSound();
					TimeControl.roundOver = true;
				}					
			}			
			if (TimeControl.victoryPose == true && animator.GetBool("isKOed") == false){
				TimeControl.winner = "You Win";
				animator.Play("VictoryPose");
			}	
		}
		if (character.GetHealth() <= 0){
			animator.SetBool("isKOed", true);
		}
		
		WalkInput();	
		WalkNoMoreInput();
		healthBar.SetHealth(character.GetHealth());	
	}

	void InitiateCharacter (){
		if (GameObject.Find ("CharacterChoice") != null) {
			characterChoice = FindObjectOfType<CharacterChoice> ();
		}
		GameObject streetFighterCharacter = Instantiate (streetFighterCharacters [characterChoice.GetChosenChar ()]);
		streetFighterCharacter.transform.parent = gameObject.transform;
		streetFighterCharacter.transform.position = gameObject.transform.position;
	}
		
	void CrouchOrStand (){
		if (animator.GetBool ("isAirborne") == false && animator.GetBool ("isThrown") == false
		    && animator.GetBool ("throwTargetAcquired") == false && animator.GetBool ("isLiftingOff") == false) {
			//crouch
			if (pressedCrouch) {
				animator.SetBool ("isStanding", false);
				animator.SetBool ("isCrouching", true);
			}
			//stand
			else{
				animator.SetBool ("isStanding", true);
				animator.SetBool ("isCrouching", false);
			}
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			pressedCrouch = true;
		}
		if (Input.GetKeyUp (KeyCode.DownArrow)){
			pressedCrouch = false;
		}
	}
	
	void IsUpPressed(){
		if (Input.GetKey(KeyCode.UpArrow)){
			pressedUp = true;
		}
		if (Input.GetKeyUp(KeyCode.UpArrow)){
			pressedUp = false;
		}
	}

	void CharacterJumping ()
	{
		character.CharacterJump (pressedForward, pressedBackward);
		animator.SetBool ("isStanding", false);
		animator.SetBool ("isLiftingOff", true);
	}
	
	void IsThrown(){
		if (animator.GetBool("isThrown") == true){
			if (opponentCharacter.GetComponent<Ken>() != null){
				if (character.side == Character.Side.P2){
					character.transform.position = new Vector3(opponentCharacter.transform.position.x + 0.25f, opponentCharacter.transform.position.y, 0f);
				}
				else{
					character.transform.position = new Vector3(opponentCharacter.transform.position.x - 0.25f, opponentCharacter.transform.position.y, 0f);
				}		
			}
			else if (opponentCharacter.GetComponent<FeiLong>() != null){
				if (character.side == Character.Side.P2){
					character.transform.position = new Vector3(opponentCharacter.transform.position.x + 0.5f, opponentCharacter.transform.position.y, 0f);
				}
				else{
					character.transform.position = new Vector3(opponentCharacter.transform.position.x - 0.5f, opponentCharacter.transform.position.y, 0f);
				}		
			}		
			else if (opponentCharacter.GetComponent<Balrog>() != null){
				if (character.side == Character.Side.P2){
					character.transform.position = new Vector3(opponentCharacter.transform.position.x + 0.3f, opponentCharacter.transform.position.y, 0f);
				}
				else{
					character.transform.position = new Vector3(opponentCharacter.transform.position.x - 0.3f, opponentCharacter.transform.position.y, 0f);
				}		
			}		
		}
	}
	
	void CharacterNeutralState(){
		pressedForward = false;
		pressedBackward = false;
	}

	void DetermineSide ()
	{
		distance = opponentCharacter.transform.position.x - character.transform.position.x;
		distanceFromOpponent = Mathf.Abs (distance);
		SideSwitch ();
	}
		
	void SideSwitch(){			
		//determine which side
		if (distance < 0 && character.side == Character.Side.P1){			
			character.side = Character.Side.P2;
			CharacterNeutralState();
		}
		else if (distance >= 0 && character.side == Character.Side.P2){
			character.side = Character.Side.P1;
			CharacterNeutralState();
		}
		//only after character is not in these states will the sprite actually switch sides
		if (animator.GetBool ("isAirborne") == false && animator.GetBool ("isThrown") == false
		    && animator.GetBool ("throwTargetAcquired") == false && animator.GetBool ("isLiftingOff") == false && animator.GetBool("isAttacking") == false				    
		    && animator.GetBool ("isInHitStun") == false && animator.GetBool ("isInBlockStun") == false && animator.GetBool("isKnockedDown") == false) {
			
			character.SideSwitch();
		}
	}
	
	void WalkInput(){
		if (Input.GetKey(KeyCode.RightArrow)){
			if (character.side == Character.Side.P1){
				pressedForward = true;
			}
			else{
				pressedBackward = true;
			}
		}
		if (Input.GetKey(KeyCode.LeftArrow)){
			if (character.side == Character.Side.P2){
				pressedForward = true;
			}
			else{
				pressedBackward = true;
			}
		}
	}
		
	void Walk(){					
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
	
	void WalkNoMoreInput(){			
		if (Input.GetKeyUp(KeyCode.RightArrow)){
			if (character.side == Character.Side.P1){
				pressedForward = false;
				animator.SetBool("isWalkingForward", false);	
			}
			else{
				pressedBackward = false;
				animator.SetBool("isWalkingBackward", false);	
			}
			
		}	
		if (Input.GetKeyUp(KeyCode.LeftArrow)){
			if (character.side == Character.Side.P2){
				pressedForward = false;
				animator.SetBool("isWalkingForward", false);	
			}
			else{
				pressedBackward = false;
				animator.SetBool("isWalkingBackward", false);	
			}
		}
	}
	
	void AttacksInput(){		
		
		if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.Z)){
			if (animator.GetBool("isAttacking") == false && animator.GetBool("isAirborne") == false){
				character.AttackState();
				animator.Play("ThrowStartup");
			}				
		}
		else{
			if (Input.GetKeyDown(KeyCode.A)){				
				if (character.GetComponent<Ken>() != null){
					if (CheckHadoukenSequence() && animator.GetBool("isAirborne") == false 
						&& projectileP1Parent.transform.childCount <= 0){
						KenCompletesHadouken(0);
					}
					else if (CheckShoryukenSequence() && animator.GetBool("isAirborne") == false){
						KenCompletesShoryuken("Jab", 0);
					}
					else if (animator.GetBool("isAttacking") == false){					
						character.AttackState();
						character.CharacterJab();
					}
				}
				else if (character.GetComponent<FeiLong>() != null){
					if (CheckHadoukenSequence() && animator.GetBool("isAirborne") == false){
						FeiLongCompletesRekka(0);
					}
					else if (animator.GetBool("isAttacking") == false){	
						if (Mathf.Abs(distance) < 0.75f && animator.GetBool("isStanding") == true){
							feiLong.FeiLongCloseJab();
						}
						else{							
							character.CharacterJab();
						}
						character.AttackState();
					}
				}		
				else if (character.GetComponent<Balrog>() != null){
					if (chargeSystem.GetBackCharged() && !pressedBackward && animator.GetBool("isAirborne") == false){
						BalrogCompletesDashRushes("Jab", 0);
					}
					else if (chargeSystem.GetDownCharged() && !pressedCrouch 
						&& animator.GetBool("isAirborne") == false && pressedUp){
						BalrogCompletesHeadButt("Jab", 0);
							
					}
					else if (animator.GetBool("isAttacking") == false){	
						character.CharacterJab();
						character.AttackState();
					}
				}		
			}		
			if (Input.GetKeyDown(KeyCode.S)){
				
				if (character.GetComponent<Ken>() != null){
					if (CheckHadoukenSequence() && animator.GetBool("isAirborne") == false
					    && projectileP1Parent.transform.childCount <= 0){
						KenCompletesHadouken(1);
					}
					else if (CheckShoryukenSequence() && animator.GetBool("isAirborne") == false){
						KenCompletesShoryuken("Strong", 1);
					}	
					else if (animator.GetBool("isAttacking") == false){					
						character.AttackState();
						character.CharacterStrong();
					}
				}
				else if (character.GetComponent<FeiLong>() != null){
					if (CheckHadoukenSequence() && animator.GetBool("isAirborne") == false){
						FeiLongCompletesRekka(1);
					}
					else if (animator.GetBool("isAttacking") == false){	
						if (Mathf.Abs(distance) < 0.75f && animator.GetBool("isStanding") == true){
							feiLong.FeiLongCloseStrong();
						}
						else{							
							character.CharacterStrong();
						}
						character.AttackState();
					}
				}	
				else if (character.GetComponent<Balrog>() != null){
					if (chargeSystem.GetBackCharged() && !pressedBackward && animator.GetBool("isAirborne") == false){
						BalrogCompletesDashRushes("Strong", 1);
					}
					else if (chargeSystem.GetDownCharged() && !pressedCrouch 
						&& animator.GetBool("isAirborne") == false && pressedUp){
						BalrogCompletesHeadButt("Strong", 1);						
					}
					else if (animator.GetBool("isAttacking") == false){	
						character.CharacterStrong();
						character.AttackState();
					}
				}		
			}			
			if (Input.GetKeyDown(KeyCode.D)){
				
				if (character.GetComponent<Ken>() != null){
					if (CheckHadoukenSequence() && animator.GetBool("isAirborne") == false
					    && projectileP1Parent.transform.childCount <= 0){
						KenCompletesHadouken(2);
					}
					else if (CheckShoryukenSequence() && animator.GetBool("isAirborne") == false){
						KenCompletesShoryuken("Fierce", 2);
					}	
					else if (animator.GetBool("isAttacking") == false){					
						character.AttackState();
						character.CharacterFierce();
					}
				}
				else if (character.GetComponent<FeiLong>() != null){
					if (CheckHadoukenSequence() && animator.GetBool("isAirborne") == false){
						FeiLongCompletesRekka(2);
					}
					else if (animator.GetBool("isAttacking") == false){	
						if (Mathf.Abs(distance) < 0.75f && animator.GetBool("isStanding") == true){
							feiLong.FeiLongCloseFierce();
						}
						else{							
							character.CharacterFierce();
						}
						character.AttackState();
					}
				}	
				else if (character.GetComponent<Balrog>() != null){
					if (chargeSystem.GetBackCharged() && !pressedBackward && animator.GetBool("isAirborne") == false){
						BalrogCompletesDashRushes("Fierce", 2);
					}
					else if (chargeSystem.GetDownCharged() && !pressedCrouch 
						&& animator.GetBool("isAirborne") == false && pressedUp){						
						BalrogCompletesHeadButt("Fierce", 2);						
					}
					else if (animator.GetBool("isAttacking") == false){	
						character.CharacterFierce();
						character.AttackState();
					}
				}		
			}
			if (Input.GetKeyDown(KeyCode.Z)){	
			
				if (character.GetComponent<Ken>() != null){				
					if (CheckHurricaneKickSequence() && animator.GetBool("isAirborne") == false){
						KenCompletesHurricaneKick(0);
					}
					else if (CheckHadoukenSequence() && animator.GetBool("isAirborne") == false){
						KenCompletesRoll(0);
					}
					else if (animator.GetBool("isAttacking") == false){
						character.AttackState();
						character.CharacterShort();
					}
				}
				else if (character.GetComponent<FeiLong>() != null){
					if (CheckReverseShoryukenSequence() && animator.GetBool("isAirborne") == false){
						FeiLongCompletesShienKyaku("Short", 0);	
					}	
					else if (CheckShoryukenSequence() && animator.GetBool("isAirborne") == false){
						FeiLongCompletesRekkaKun(0);
					}	
					else if (animator.GetBool("isAttacking") == false){						
						character.CharacterShort();
						character.AttackState();
					}
				}	
				else if (character.GetComponent<Balrog>() != null){
					if (chargeSystem.GetBackCharged() && !pressedBackward
						&& animator.GetBool("isAirborne") == false && pressedForward){
						BalrogCompletesKickRush("Short", 0);
					}
					else if (animator.GetBool("isAttacking") == false){	
						character.CharacterShort();
						character.AttackState();
					}
				}		
			}	
			if (Input.GetKeyDown(KeyCode.X)){
			
				if (character.GetComponent<Ken>() != null){
					if (CheckHurricaneKickSequence() && animator.GetBool("isAirborne") == false){
						KenCompletesHurricaneKick(1);
					}
					else if (CheckHadoukenSequence() && animator.GetBool("isAirborne") == false){
						KenCompletesRoll(1);
					}
					else if (animator.GetBool("isAttacking") == false){
						character.AttackState();
						character.CharacterForward();
					}
				}
				else if (character.GetComponent<FeiLong>() != null){
					if (CheckReverseShoryukenSequence() && animator.GetBool("isAirborne") == false){
						FeiLongCompletesShienKyaku("Forward", 1);	
					}	
					else if (CheckShoryukenSequence() && animator.GetBool("isAirborne") == false){
						FeiLongCompletesRekkaKun(1);
					}	
					else if (animator.GetBool("isAttacking") == false){						
						character.CharacterForward();
						character.AttackState();
					}
				}	
				else if (character.GetComponent<Balrog>() != null){
					if (chargeSystem.GetBackCharged() && !pressedBackward
						&& animator.GetBool("isAirborne") == false && pressedForward){
						BalrogCompletesKickRush("Forward", 1);
					}
					else if (animator.GetBool("isAttacking") == false){	
						character.CharacterForward();
						character.AttackState();
					}
				}		
			}	
			if (Input.GetKeyDown(KeyCode.C)){
				
				if (character.GetComponent<Ken>() != null){
					if (CheckHurricaneKickSequence() && animator.GetBool("isAirborne") == false){
						KenCompletesHurricaneKick(2);
					}
					else if (CheckHadoukenSequence() && animator.GetBool("isAirborne") == false){
						KenCompletesRoll(2);
					}
					else if (animator.GetBool("isAttacking") == false){
						character.AttackState();
						character.CharacterRoundhouse();
					}
				}
				else if (character.GetComponent<FeiLong>() != null){					
					if (CheckReverseShoryukenSequence() && animator.GetBool("isAirborne") == false){
						FeiLongCompletesShienKyaku("Roundhouse", 2);	
					}	
					else if (CheckShoryukenSequence() && animator.GetBool("isAirborne") == false){
						FeiLongCompletesRekkaKun(2);
					}	
					else if (animator.GetBool("isAttacking") == false){						
						character.CharacterRoundhouse();
						character.AttackState();
					}
				}	
				else if (character.GetComponent<Balrog>() != null){
					if (chargeSystem.GetBackCharged() && !pressedBackward 
						&& animator.GetBool("isAirborne") == false && pressedForward){
						BalrogCompletesKickRush("Roundhouse", 2);
					}
					else if (animator.GetBool("isAttacking") == false){	
						character.CharacterRoundhouse();
						character.AttackState();
					}
				}		
			}	
			if (Input.GetKey(KeyCode.F)){
				if (character.GetComponent<Balrog>() != null){
					chargeSystem.ChargeTurnPunch();
				}
			}
			if (Input.GetKeyUp(KeyCode.F)){
				if (character.GetComponent<Balrog>() != null){
					character.AttackState();
					balrog.TurnPunch();
					chargeSystem.ResetTurnPunch();
				}
			}
		}
	}	
	
	void KenCompletesHadouken (int punchType){
		Debug.Log ("Hadouken inputed");
		if (animator.GetBool ("isAttacking") == false) {
			character.AttackState ();
			animator.Play ("KenHadouken", 0);
		}
		animator.SetTrigger ("hadoukenInputed");
		animator.SetInteger ("hadoukenPunchType", punchType);
		animator.SetInteger ("hadoukenOwner", 1);
		comboSystem.ResetHadoukenSequence ();
	}
	
	void KenCompletesShoryuken (string punchName, int punchType){
		Debug.Log ("Shoryuken inputed");
		if (animator.GetBool ("isAttacking") == false) {
			character.AttackState ();
			animator.Play ("KenShoryuken" + punchName, 0);
		}
		animator.SetTrigger ("shoryukenInputed");
		animator.SetInteger ("shoryukenPunchType", punchType);
		comboSystem.ResetShoryukenSequence ();
	}

	void KenCompletesHurricaneKick (int kickType){
		Debug.Log ("Hurricane kick inputed");
		if (animator.GetBool ("isAttacking") == false) {
			animator.Play ("KenHurricaneKickLiftOff", 0);
			character.AttackState ();
		}
		animator.SetTrigger ("hurricaneKickInputed");
		animator.SetInteger ("hurricaneKickType", kickType);
		comboSystem.ResetHurricaneKickSequence ();
	}

	void KenCompletesRoll (int kickType){
		Debug.Log ("Hadouken inputed");
		if (animator.GetBool ("isAttacking") == false) {
			character.AttackState ();
			animator.Play ("KenRoll");
		}
		animator.SetTrigger ("rollInputed");
		animator.SetInteger ("rollKickType", kickType);
		comboSystem.ResetHadoukenSequence ();
	}

	void FeiLongCompletesRekka (int punchType){
		Debug.Log ("Hadouken inputed");
		if (animator.GetBool ("isAttacking") == false) {
			character.AttackState ();
			animator.Play ("FeiLongRekkaKenFirstAttack", 0);
		}
		animator.SetTrigger ("hadoukenInputed");
		animator.SetInteger ("rekkaPunchType", punchType);
		comboSystem.ResetHadoukenSequence ();
	}

	void FeiLongCompletesShienKyaku (string kickName, int kickType){
		Debug.Log ("Reverse Shoryuken inputed");
		if (animator.GetBool ("isAttacking") == false) {
			character.AttackState ();
			animator.Play ("FeiLongShienKyaku" + kickName, 0);
		}
		animator.SetTrigger ("reverseShoryukenInputed");
		animator.SetInteger ("shienKyakuKickType", kickType);
		comboSystem.ResetShoryukenSequence ();
	}

	void FeiLongCompletesRekkaKun (int kickType){
		Debug.Log ("Shoryuken inputed");
		if (animator.GetBool ("isAttacking") == false) {
			character.AttackState ();
			animator.Play ("FeiLongRekkaKun", 0);
		}
		animator.SetTrigger ("shoryukenInputed");
		animator.SetInteger ("rekkaKunKickType", kickType);
		comboSystem.ResetShoryukenSequence ();
	}

	void BalrogCompletesDashRushes (string punchName, int punchType){
		if (pressedForward && pressedCrouch) {
			Debug.Log ("dashed low");
			if (animator.GetBool ("isAttacking") == false) {
				character.AttackState ();
				animator.Play ("BalrogDashLow" + punchName + "StartUp", 0);
			}
			animator.SetTrigger ("dashLowInputed");
		}
		else
			if (pressedForward) {
				Debug.Log ("dashed straight");
				if (animator.GetBool ("isAttacking") == false) {
					character.AttackState ();
					animator.Play ("BalrogDashStraight" + punchName + "StartUp", 0);
				}
				animator.SetTrigger ("dashStraightInputed");
			}
		animator.SetInteger ("dashRushPunchType", punchType);
		chargeSystem.SetBackCharged (false);
		chargeSystem.ResetBackChargedProperties ();
	}

	void BalrogCompletesKickRush (string kickName, int kickType)	{
		Debug.Log ("kicked rush");
		if (animator.GetBool ("isAttacking") == false) {
			character.AttackState ();
			animator.Play ("BalrogKickRush" + kickName + "StartUp", 0);
		}
		animator.SetTrigger ("kickRushInputed");
		animator.SetInteger ("dashRushPunchType", kickType);
		chargeSystem.SetBackCharged (false);
		chargeSystem.ResetBackChargedProperties ();
	}

	void BalrogCompletesHeadButt (string punchName, int punchType){
		Debug.Log ("headbutted");
		if (animator.GetBool ("isAttacking") == false) {
			character.AttackState ();
			animator.Play ("BalrogHeadButt" + punchName, 0);
			animator.SetBool ("isHeadButting", true);
		}
		animator.SetTrigger ("headButtInputed");
		animator.SetInteger ("dashRushPunchType", punchType);
		chargeSystem.SetDownCharged (false);
		chargeSystem.ResetDownChargedProperties ();
	}
	
	bool CheckHadoukenSequence(){
		for (int i=0; i<comboSystem.GetHadoukenSequence().Length; i++){
			if (comboSystem.GetHadoukenSequence()[i] == false){
				return false;
			}
		}
		return true;
	}
	bool CheckShoryukenSequence(){
		for (int i=0; i<comboSystem.GetShoryukenSequence().Length; i++){
			if (comboSystem.GetShoryukenSequence()[i] == false){
				return false;
			}
		}
		return true;
	}
	
	bool CheckHurricaneKickSequence(){
		for (int i=0; i<comboSystem.GetHurricaneKickSequence().Length; i++){
			if (comboSystem.GetHurricaneKickSequence()[i] == false){
				return false;
			}
		}
		return true;
	}
	
	bool CheckReverseShoryukenSequence(){
		for (int i=0; i<comboSystem.GetReverseShoryukenSequence().Length; i++){
			if (comboSystem.GetReverseShoryukenSequence()[i] == false){
				return false;
			}
		}
		return true;
	}	
	
	public bool GetBackPressed(){
		return pressedBackward;
	}	
	
	public bool GetDownPressed(){
		return pressedCrouch;
	}	
	
	public float GetDistanceFromOpponent(){
		return distanceFromOpponent;
	}
}
	