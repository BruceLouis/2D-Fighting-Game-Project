using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

//will implement a delegate later
public class Player : MonoBehaviour {
	
	public GameObject mugShotObject;
	public GameObject[] streetFighterCharacters;
	public Text nameText;
	public bool isAI;
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
	private SharedProperties sharedProperties;
	private ComboCounter comboCounter;
	
	private KenAI kenAI;
	private FeiLongAI feiLongAI;
	private BalrogAI balrogAI;
	
	private bool pressedForward, pressedBackward, pressedCrouch, pressedUp, introPlayed;
	private float distance, distanceFromOpponent;	
	
	void Awake () {
		
		//InitiateCharacter();
				
		gameObject.layer = LayerMask.NameToLayer("Player1");
		gameObject.tag = "Player1";
		foreach(Transform character in gameObject.transform){
			character.gameObject.layer = LayerMask.NameToLayer("Player1");	
			character.gameObject.tag = "Player1";
		}		
		character = GetComponentInChildren<Character>();
		animator = GetComponentInChildren<Animator>();	
		mugShot = mugShotObject.GetComponent<Image>();
		sharedProperties = GetComponent<SharedProperties>();
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
			feiLongAI = GetComponentInChildren<FeiLongAI>();
			mugShot.sprite = feiLongMugShot;
			nameText.text = "Fei Long";
		}
		else if (character.GetComponent<Ken>() != null){
			kenAI = GetComponentInChildren<KenAI>();
			mugShot.sprite = kenMugShot;
			nameText.text = "Ken";
		}
		else if (character.GetComponent<Balrog>() != null){
			balrog = GetComponentInChildren<Balrog>();
			balrogAI = GetComponentInChildren<BalrogAI>();
			mugShot.sprite = balrogMugShot;
			nameText.text = "Balrog";
		}
		
		projectileP1Parent = GameObject.Find("ProjectileP1Parent");
		if (projectileP1Parent == null){
			projectileP1Parent = new GameObject("ProjectileP1Parent");
		}
		
		comboCounter = FindObjectOfType<ComboCounter>();
		introPlayed = false;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (timeControl.gameState == TimeControl.GameState.introPose && !introPlayed){
			animator.Play("IntroPose",0);
			introPlayed = true;
		}			
		else if (timeControl.gameState == TimeControl.GameState.fight){
								
			sharedProperties.IsThrown(animator, opponentCharacter, character);	
			DetermineSide();	
			if (!isAI){					
				character.SetBackPressed(pressedBackward);
				PlayerControls();		
			}
			else{				
				if (character.GetComponent<Ken>() != null){
					kenAI.Behaviors();
				}	
				else if (character.GetComponent<FeiLong>() != null){
					feiLongAI.Behaviors();
				}
				else if (character.GetComponent<Balrog>() != null){
					balrogAI.Behaviors();
				}
			}
		}
		else{	
			sharedProperties.KOSequence("You Win");
		}
		if (character.GetHealth() <= 0){
			animator.SetBool("isKOed", true);
		}		
		if (!animator.GetBool("isInHitStun")){
			if (comboCounter.GetComboCountP2 > 1){
				comboCounter.GetStartTimer = true;
			}
			comboCounter.GetComboCountP2 = 1;
		}
		healthBar.SetHealth(character.GetHealth());	
	}

	void PlayerControls (){
		CrouchOrStand ();
		IsUpPressed ();
		WalkInput ();
		WalkNoMoreInput ();
		if (animator.GetBool ("isKnockedDown") == false && animator.GetBool ("isThrown") == false && animator.GetBool ("isMidAirRecovering") == false
			&& animator.GetBool ("isInHitStun") == false && animator.GetBool ("isInBlockStun") == false && animator.GetBool ("isLiftingOff") == false) {
			AttacksInput ();
			if (animator.GetBool ("isStanding") == true && animator.GetBool ("isAirborne") == false && animator.GetBool ("isAttacking") == false) {
				WalkPlayer ();
				if (pressedUp) {
					if (character.GetComponent<Balrog>() != null){
						//give balrog a window of time to input the headbutt before he jumps
						if (chargeSystem.downChargedWindow < (chargeSystem.GetDownChargedWindowInput()/2) && chargeSystem.GetDownCharged()) {
							CharacterJumping ();
						}
						else if (!chargeSystem.GetDownCharged()){
							CharacterJumping ();
						}
					}
					else{
						CharacterJumping ();
					}						
				}
			}
		}
	}
	
	void InitiateCharacter (){
		GameObject streetFighterCharacter;
		if (GameObject.Find ("CharacterChoice") != null) {
			characterChoice = FindObjectOfType<CharacterChoice> ();
			streetFighterCharacter = Instantiate (streetFighterCharacters [characterChoice.GetChosenChar ()]);
		}
		else{
			int randChar = Random.Range (0, streetFighterCharacters.Length);
			streetFighterCharacter = Instantiate (streetFighterCharacters [randChar]);
		}
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
		if (Input.GetKeyDown(KeyCode.UpArrow)){
			pressedUp = true;
		}
		else if (Input.GetKeyUp(KeyCode.UpArrow)){
			pressedUp = false;
		}
	}

	void CharacterJumping (){
		character.CharacterJump (pressedForward, pressedBackward);
		animator.SetBool ("isStanding", false);
		animator.SetBool ("isLiftingOff", true);
	}

	
	void DetermineSide (){
		distance = opponentCharacter.transform.position.x - character.transform.position.x;
		distanceFromOpponent = Mathf.Abs (distance);
		SideSwitch ();
	}
		
	void SideSwitch(){			
		if (!isAI){
			//determine which side
			//again the reason for repeated code is so that AI doesn't just switch sides when its attacking willy nilly			
			if (distance < 0 && character.side == Character.Side.P1){			
				character.side = Character.Side.P2;
				sharedProperties.CharacterNeutralState();
			}
			else if (distance >= 0 && character.side == Character.Side.P2){
				character.side = Character.Side.P1;
				sharedProperties.CharacterNeutralState();
			}
			//only after character is not in these states will the sprite actually switch sides
			//only distinction here is extra booleans of both walks being false if AI is true so animation would play properly
			if (animator.GetBool ("isAirborne") == false && animator.GetBool ("isThrown") == false
			    && animator.GetBool ("throwTargetAcquired") == false && animator.GetBool ("isLiftingOff") == false && animator.GetBool("isAttacking") == false				    
			    && animator.GetBool ("isInHitStun") == false && animator.GetBool ("isInBlockStun") == false && animator.GetBool("isKnockedDown") == false) {
				
				character.SideSwitch();
			}
		}
		else{
			if (animator.GetBool("isAttacking") == false){
				if (distance < 0 && character.side == Character.Side.P1){			
					character.side = Character.Side.P2;
					sharedProperties.CharacterNeutralState();
				}
				else if (distance >= 0 && character.side == Character.Side.P2){
					character.side = Character.Side.P1;
					sharedProperties.CharacterNeutralState();
				}
			}
			if (animator.GetBool ("isAirborne") == false && animator.GetBool ("isThrown") == false && animator.GetBool ("isWalkingForward") == false 
			    && animator.GetBool ("throwTargetAcquired") == false && animator.GetBool ("isLiftingOff") == false && animator.GetBool ("isWalkingBackward") == false				    
			    && animator.GetBool ("isInHitStun") == false && animator.GetBool ("isInBlockStun") == false && animator.GetBool("isKnockedDown") == false) {
				
				character.SideSwitch();
			}
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
		
	void WalkPlayer(){					
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
						ShotoCompletesHadouken("Ken", 0);
					}
					else if (CheckShoryukenSequence() && animator.GetBool("isAirborne") == false){
						ShotoCompletesShoryuken("Ken", "Jab", 0);
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
				else if (character.GetComponent<Akuma>() != null){
					if (CheckHadoukenSequence() && projectileP1Parent.transform.childCount <= 0){
						if (animator.GetBool("isAirborne") == true){
							AkumaCompletesAirHadouken(0);
						}
						else{
							ShotoCompletesHadouken("Akuma", 0);
						}
					}
					else if (CheckShoryukenSequence() && animator.GetBool("isAirborne") == false){
						ShotoCompletesShoryuken("Akuma", "Jab", 0);
					}
					else if (animator.GetBool("hyakkishuActive") == true){
						animator.SetInteger("hyakkishuAttackType", 1);
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
						ShotoCompletesHadouken("Ken", 1);
					}
					else if (CheckShoryukenSequence() && animator.GetBool("isAirborne") == false){
						ShotoCompletesShoryuken("Ken", "Strong", 1);
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
				else if (character.GetComponent<Akuma>() != null){
					if (CheckHadoukenSequence() && projectileP1Parent.transform.childCount <= 0){
						if (animator.GetBool("isAirborne") == true){
							AkumaCompletesAirHadouken(1);
						}
						else{
							ShotoCompletesHadouken("Akuma", 1);
						}
					}
					else if (CheckShoryukenSequence() && animator.GetBool("isAirborne") == false){
						ShotoCompletesShoryuken("Akuma", "Strong", 1);
					}
					else if (animator.GetBool("hyakkishuActive") == true){
						animator.SetInteger("hyakkishuAttackType", 1);
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
						ShotoCompletesHadouken("Ken", 2);
					}
					else if (CheckShoryukenSequence() && animator.GetBool("isAirborne") == false){
						ShotoCompletesShoryuken("Ken", "Fierce", 2);
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
				else if (character.GetComponent<Akuma>() != null){
					if (CheckHadoukenSequence() && projectileP1Parent.transform.childCount <= 0){
						if (animator.GetBool("isAirborne") == true){
							AkumaCompletesAirHadouken(2);
						}
						else{
							ShotoCompletesHadouken("Akuma", 2);
						}
					}
					else if (CheckShoryukenSequence() && animator.GetBool("isAirborne") == false){
						ShotoCompletesShoryuken("Akuma", "Fierce", 2);
					}
					else if (animator.GetBool("hyakkishuActive") == true){
						animator.SetInteger("hyakkishuAttackType", 1);
					}
					else if (CheckShunGokuSatsuSequence() && animator.GetBool("isAirborne") == false){
						Debug.Log ("shun goku satsu inputed");
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
						ShotoCompletesHurricaneKick("Ken", 0);
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
				else if (character.GetComponent<Akuma>() != null){	
					if (CheckHurricaneKickSequence() && animator.GetBool("isAirborne") == false){
						ShotoCompletesHurricaneKick("Akuma", 0);
					}
					else if (CheckShoryukenSequence() && animator.GetBool("isAirborne") == false){
						AkumaCompletesHyakkishu(0);
					}	
					else if (animator.GetBool("hyakkishuActive") == true){
						animator.SetInteger("hyakkishuAttackType", 2);
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
						ShotoCompletesHurricaneKick("Ken", 1);
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
				else if (character.GetComponent<Akuma>() != null){
					if (CheckHurricaneKickSequence() && animator.GetBool("isAirborne") == false){
						ShotoCompletesHurricaneKick("Akuma", 1);
					}
					else if (CheckShoryukenSequence() && animator.GetBool("isAirborne") == false){
						AkumaCompletesHyakkishu(1);
					}	
					else if (animator.GetBool("hyakkishuActive") == true){
						animator.SetInteger("hyakkishuAttackType", 2);
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
						ShotoCompletesHurricaneKick("Ken", 2);
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
				else if (character.GetComponent<Akuma>() != null){
					if (CheckHurricaneKickSequence() && animator.GetBool("isAirborne") == false){
						ShotoCompletesHurricaneKick("Akuma", 2);
					}
					else if (CheckShoryukenSequence() && animator.GetBool("isAirborne") == false){
						AkumaCompletesHyakkishu(2);
					}	
					else if (animator.GetBool("hyakkishuActive") == true){
						animator.SetInteger("hyakkishuAttackType", 2);
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
					BalrogCompletesTurnPunch();
				}
			}
		}
	}	
	
	void ShotoCompletesHadouken (string shoto, int punchType){
		Debug.Log ("Hadouken inputed");
		if (animator.GetBool ("isAttacking") == false) {
			character.AttackState ();
			animator.Play (shoto + "Hadouken", 0);
		}
		animator.SetTrigger ("hadoukenInputed");
		animator.SetInteger ("hadoukenPunchType", punchType);
		animator.SetInteger ("hadoukenOwner", 1);
		comboSystem.ResetHadoukenSequence ();
	}	
	
	void ShotoCompletesShoryuken (string shoto, string punchName, int punchType){
		Debug.Log ("Shoryuken inputed");
		if (animator.GetBool ("isAttacking") == false) {
			character.AttackState ();
			animator.Play (shoto + "Shoryuken" + punchName, 0);
		}
		animator.SetTrigger ("shoryukenInputed");
		animator.SetInteger ("shoryukenPunchType", punchType);
		comboSystem.ResetShoryukenSequence ();
	}

	void ShotoCompletesHurricaneKick (string shoto, int kickType){
		Debug.Log ("Hurricane kick inputed");
		if (animator.GetBool ("isAttacking") == false) {
			animator.Play (shoto + "HurricaneKickLiftOff", 0);
			character.AttackState ();
		}
		animator.SetTrigger ("hurricaneKickInputed");
		animator.SetInteger ("hurricaneKickType", kickType);
		comboSystem.ResetHurricaneKickSequence ();
	}
	
	void AkumaCompletesAirHadouken (int punchType){
		Debug.Log ("Hadouken inputed");
		if (animator.GetBool ("isAttacking") == false) {
			character.AttackState ();
			animator.Play ("AkumaAirHadouken", 0);
		}
		animator.SetTrigger ("hadoukenInputed");
		animator.SetInteger ("hadoukenPunchType", punchType);
		animator.SetInteger ("hadoukenOwner", 1);
		comboSystem.ResetHadoukenSequence ();
	}
	
	void AkumaCompletesHyakkishu (int kickType){
		Debug.Log ("Hadouken inputed");
		if (animator.GetBool ("isAttacking") == false) {
			character.AttackState ();
			animator.Play ("AkumaHyakkishu", 0);
		}
		animator.SetTrigger ("hyakkishuInputed");
		animator.SetInteger ("hyakkishuKickType", kickType);
		comboSystem.ResetShoryukenSequence ();
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
		else if (pressedForward) {
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

	void BalrogCompletesKickRush (string kickName, int kickType){
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
	
	void BalrogCompletesTurnPunch (){
		if (chargeSystem.GetTurnPunchCharge() >= 30){
			if (animator.GetBool ("isAttacking") == false && animator.GetBool("isAirborne") == false){
				character.AttackState();
				animator.Play ("BalrogTurnPunch", 0);
			}
			animator.SetTrigger ("turnPunchInputed");
			if (chargeSystem.GetTurnPunchCharge() < 80){
				animator.SetInteger("turnPunchStrength", 1);
			}
			else if (chargeSystem.GetTurnPunchCharge() >= 80 && chargeSystem.GetTurnPunchCharge() < 140){
				animator.SetInteger("turnPunchStrength", 2);
			}
			else if (chargeSystem.GetTurnPunchCharge() >= 140 && chargeSystem.GetTurnPunchCharge() < 200){
				animator.SetInteger("turnPunchStrength", 3);
			}
			else if (chargeSystem.GetTurnPunchCharge() >= 200 && chargeSystem.GetTurnPunchCharge() < 350){
				animator.SetInteger("turnPunchStrength", 4);
			}
			else if (chargeSystem.GetTurnPunchCharge() >= 350 && chargeSystem.GetTurnPunchCharge() < 550){
				animator.SetInteger("turnPunchStrength", 5);
			}
			else if (chargeSystem.GetTurnPunchCharge() >= 550 && chargeSystem.GetTurnPunchCharge() < 800){
				animator.SetInteger("turnPunchStrength", 6);
			}
			else{
				animator.SetInteger("turnPunchStrength", 7);
			}			
			chargeSystem.ResetTurnPunch();
		}
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
	
	bool CheckShunGokuSatsuSequence(){
		for (int i=0; i<comboSystem.GetShunGokuSatsuSequence().Length; i++){
			if (comboSystem.GetShunGokuSatsuSequence()[i] == false){
				return false;
			}
		}
		return true;
	}	
	
	public void WalkAI(){						
		if (animator.GetBool("isAttacking") == false && animator.GetBool("isInHitStun") == false
		    && animator.GetBool("isInBlockStun") == false && animator.GetBool("isLiftingOff") == false
		    && animator.GetBool("isStanding") == true && animator.GetBool("isAirborne") == false
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isMidAirRecovering") == false
		    && animator.GetBool("isThrown") == false && animator.GetBool("isCrouching") == false){
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
	
	public GameObject GetProjectileP1Parent(){
		return projectileP1Parent;
	}
	
	public bool GetBackPressed{
		get { return pressedBackward; }
		set { pressedBackward = value; }
	}	
	
	public bool GetDownPressed{   
		get { return pressedCrouch; }
		set { pressedCrouch = value; }
	}	
	
	public bool GetForwardPressed{
		get { return pressedForward; }
		set { pressedForward = value; }
	}	
	
	public float GetDistance(){
		return distance;
	}
	
	public float GetDistanceFromOpponent(){
		return distanceFromOpponent;
	}
}
	