using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Player : MonoBehaviour {
	
	delegate void AttackStrength();
	delegate void FeiLongClosePunch();
    delegate void AIBehavior();
	AttackStrength attackStrength;
	FeiLongClosePunch feiLongPunch;
    AIBehavior aiBehavior;
	
	public GameObject mugShotObject;
	public GameObject[] streetFighterCharacters;
	public Text nameText;
	public Sprite kenMugShot, feiLongMugShot, balrogMugShot, akumaMugShot, sagatMugShot, mbisonMugShot;
    public bool isAI, doInitiateCharacter;
    
	private Animator animator;
	private Opponent opponent;
	private Character opponentCharacter;
	private Character character;
	private FeiLong feiLong;
	private Rigidbody2D physicsbody;
	private ComboSystem comboSystem;
	private ChargeSystem chargeSystem;
	private HealthBarP1 healthBar;
	private SuperBarP1 superBar;
	private Image mugShot;
	private CharacterChoice characterChoice;
	private GameObject projectileP1Parent;
	private SharedProperties sharedProperties;
	private ComboCounter comboCounter;
	
	private KenAI kenAI;
	private FeiLongAI feiLongAI;
	private BalrogAI balrogAI;
	private AkumaAI akumaAI;
	private SagatAI sagatAI;
	private MBisonAI mbisonAI;
	
	private bool pressedForward, pressedBackward, pressedCrouch, pressedUp, introPlayed;
	private float distance, distanceFromOpponent;	
	private string characterName;
	
	void Awake () {
		
		if (doInitiateCharacter){
			InitiateCharacter();
		}
				
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
		
		opponent = FindObjectOfType<Opponent>();
		opponentCharacter = opponent.GetComponentInChildren<Character>();
		physicsbody = GetComponentInChildren<Rigidbody2D>();
		comboSystem = GetComponent<ComboSystem>();
		chargeSystem = GetComponent<ChargeSystem>();
		character.side = Character.Side.P1;			
		healthBar = FindObjectOfType<HealthBarP1>();	
		superBar = FindObjectOfType<SuperBarP1>();	
		
		if (character.GetComponent<FeiLong>() != null){
			feiLong = GetComponentInChildren<FeiLong>();
			feiLongAI = GetComponentInChildren<FeiLongAI>();
            CharacterInitialize(feiLongMugShot, "Fei Long", feiLongAI.Behaviors);
		}
		else if (character.GetComponent<Ken>() != null){
			kenAI = GetComponentInChildren<KenAI>();
            CharacterInitialize(kenMugShot, "Ken", kenAI.Behaviors);
        }
		else if (character.GetComponent<Balrog>() != null)
        {
			balrogAI = GetComponentInChildren<BalrogAI>();
            CharacterInitialize(balrogMugShot, "Balrog", balrogAI.Behaviors);
        }
		else if (character.GetComponent<Akuma>() != null)
        {
            akumaAI = GetComponentInChildren<AkumaAI>();
            CharacterInitialize(akumaMugShot, "Akuma", akumaAI.Behaviors);
        }
        else if (character.GetComponent<Sagat>() != null){
			sagatAI = GetComponentInChildren<SagatAI>();
            CharacterInitialize(sagatMugShot, "Sagat", sagatAI.Behaviors);
        }
        else if (character.GetComponent<MBison>() != null){
			mbisonAI = GetComponentInChildren<MBisonAI>();
            CharacterInitialize(mbisonMugShot, "M Bison", mbisonAI.Behaviors);
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
		
		if (TimeControl.gameState == TimeControl.GameState.introPose && !introPlayed){
			animator.Play("IntroPose",0);
			introPlayed = true;
		}			
		else if (TimeControl.gameState == TimeControl.GameState.fight && !TimeControl.paused){
								
			sharedProperties.IsThrown(animator, opponentCharacter, character);	
			DetermineSide();	
			if (!isAI){					
				character.SetBackPressed(sharedProperties.GetBackPressed);
				PlayerControls();				
			}
			else{
                aiBehavior();
			}
		}
		else{	
			if (SceneManager.GetActiveScene().name == "Game"){
				sharedProperties.KOSequence("You Win");
			}
			else{
				sharedProperties.KOSequence(characterName + " Wins");
			}
				
		}
		if (character.GetHealth() <= 0){
			animator.SetBool("isKOed", true);
		}		
		if (!animator.GetBool("isInHitStun")){
			if (comboCounter.GetComboCountP2 > 1){
				comboCounter.GetStartTimerP2 = true;
			}
			comboCounter.GetComboCountP2 = 1;
		}
		healthBar.SetHealth(character.GetHealth());	
		superBar.SetSuper(character.GetSuper);	
	}
    
    void PlayerControls (){

        if (!TimeControl.inSuperStartup[0] && !TimeControl.inSuperStartup[1]){
		    CrouchOrStand ();
		    DirectionsInputted ();
		    if (animator.GetBool ("isKnockedDown") == false && animator.GetBool ("isThrown") == false && animator.GetBool ("isMidAirRecovering") == false
			    && animator.GetBool ("isInHitStun") == false && animator.GetBool ("isInBlockStun") == false && animator.GetBool ("isLiftingOff") == false
			    && animator.GetBool ("isLanding") == false) {
			    AttacksInput ();
			    if (animator.GetBool ("isStanding") == true && animator.GetBool ("isAirborne") == false && animator.GetBool ("isAttacking") == false) {
				    WalkPlayer ();
				    if (pressedUp) {
					    if (character.GetComponent<Balrog>() != null || character.GetComponent<MBison>() != null)
                        {
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

        DirectionsReleased();
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

    void CharacterInitialize(Sprite mugShotArg, string name, AIBehavior behavior)
    {
        mugShot.sprite = mugShotArg;
        characterName = name;
        nameText.text = characterName;
        aiBehavior = behavior;
    }

    void CrouchOrStand (){
		if (animator.GetBool ("isAirborne") == false && animator.GetBool ("isThrown") == false
		    && animator.GetBool ("throwTargetAcquired") == false && animator.GetBool ("isLiftingOff") == false) {
			//crouch
			if (sharedProperties.GetDownPressed) {
				animator.SetBool ("isStanding", false);
				animator.SetBool ("isCrouching", true);
			}
			//stand
			else{
				animator.SetBool ("isStanding", true);
				animator.SetBool ("isCrouching", false);
			}
		}
	}
	
	void DirectionsInputted(){

		if (Input.GetKeyDown(KeyCode.UpArrow)){
			pressedUp = true;
		}

        if (Input.GetKey(KeyCode.DownArrow)){
            sharedProperties.GetDownPressed = true;
        }

        if (Input.GetKey(KeyCode.RightArrow)){
            if (character.side == Character.Side.P1){
                sharedProperties.GetForwardPressed = true;
            }
            else{
                sharedProperties.GetBackPressed = true;
            }
        }

        if (Input.GetKey(KeyCode.LeftArrow)){
            if (character.side == Character.Side.P2){
                sharedProperties.GetForwardPressed = true;
            }
            else{
                sharedProperties.GetBackPressed = true;
            }
        }
    }

    void DirectionsReleased(){
        if (Input.GetKeyUp(KeyCode.DownArrow)){
            sharedProperties.GetDownPressed = false;
        }

        if (Input.GetKeyUp(KeyCode.UpArrow)){
            pressedUp = false;
        }

        if (Input.GetKeyUp(KeyCode.RightArrow)){
            if (character.side == Character.Side.P1){
                sharedProperties.GetForwardPressed = false;
                animator.SetBool("isWalkingForward", false);
            }
            else{
                sharedProperties.GetBackPressed = false;
                animator.SetBool("isWalkingBackward", false);
            }

        }
        if (Input.GetKeyUp(KeyCode.LeftArrow)){
            if (character.side == Character.Side.P2){
                sharedProperties.GetForwardPressed = false;
                animator.SetBool("isWalkingForward", false);
            }
            else{
                sharedProperties.GetBackPressed = false;
                animator.SetBool("isWalkingBackward", false);
            }
        }
    }

    void CharacterJumping (){
		character.CharacterJump (sharedProperties.GetForwardPressed, sharedProperties.GetBackPressed);
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
			//the reason for repeated code is so that AI doesn't just switch sides when its attacking willy nilly			
			if (distance < 0 && character.side == Character.Side.P1){			
				character.side = Character.Side.P2;
				sharedProperties.CharacterNeutralState();
				comboSystem.ResetAllSequences();
			}
			else if (distance >= 0 && character.side == Character.Side.P2){
				character.side = Character.Side.P1;
				sharedProperties.CharacterNeutralState();
				comboSystem.ResetAllSequences();
			}
			//only after character is not in these states will the sprite actually switch sides
			//only distinction here is extra booleans of both walks being false if AI is true so animation would play properly
			if (animator.GetBool ("isAirborne") == false && animator.GetBool ("isThrown") == false && animator.GetBool ("isLanding") == false
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
			    && animator.GetBool ("isInHitStun") == false && animator.GetBool ("isInBlockStun") == false && animator.GetBool("isKnockedDown") == false && animator.GetBool ("isLanding") == false) {
				
				character.SideSwitch();
			}
		}
	}
			
	void WalkPlayer(){					
		if (sharedProperties.GetForwardPressed == true && animator.GetBool("isWalkingBackward") == false){
			animator.SetBool("isWalkingForward", true);	
			if (character.side == Character.Side.P1){
				character.transform.Translate(Vector3.right * character.GetWalkSpeed() * Time.deltaTime);	
			}
			else{
				character.transform.Translate(Vector3.left * character.GetWalkSpeed() * Time.deltaTime);	
			}
		}
		else if (sharedProperties.GetBackPressed == true && animator.GetBool("isWalkingForward") == false){
			animator.SetBool("isWalkingBackward", true);					
			if (character.side == Character.Side.P2){
				character.transform.Translate(Vector3.right * character.GetWalkSpeed() * Time.deltaTime);	
			}
			else{
				character.transform.Translate(Vector3.left * character.GetWalkSpeed() * Time.deltaTime);	
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
		else if (CheckShunGokuSatsuSequence() && character.GetComponent<Akuma>() != null && animator.GetBool("isAirborne") == false && character.GetSuper >= 100f){
			Debug.Log("shun goku satsu inputed");		
			character.AttackState ();
			animator.Play ("AkumaShunGokuSatsuStartup", 0);
			character.GetSuper = 0f;
			comboSystem.ResetShunGokuSatsuSequence();
		}
		else{            
			if (Input.GetKeyDown(KeyCode.A)){	
				attackStrength = character.CharacterJab;
				if (character.GetComponent<FeiLong>() != null){
					feiLongPunch = feiLong.FeiLongCloseJab;
				}
				PunchCommands (attackStrength, feiLongPunch, 0, "Jab");
            }
            if (Input.GetKeyDown(KeyCode.S)){				
				attackStrength = character.CharacterStrong;
				if (character.GetComponent<FeiLong>() != null){
					feiLongPunch = feiLong.FeiLongCloseStrong;
				}
				PunchCommands (attackStrength, feiLongPunch, 1, "Strong");
			}			
			if (Input.GetKeyDown(KeyCode.D)){				
				attackStrength = character.CharacterFierce;
				if (character.GetComponent<FeiLong>() != null){
					feiLongPunch = feiLong.FeiLongCloseFierce;
				}
				PunchCommands (attackStrength, feiLongPunch, 2, "Fierce");
			}
			if (Input.GetKeyDown(KeyCode.Z)){					
				attackStrength = character.CharacterShort;
				KickCommands (attackStrength, 0, "Short");
			}	
			if (Input.GetKeyDown(KeyCode.X)){				
				attackStrength = character.CharacterForward;
				KickCommands (attackStrength, 1, "Forward");
			}	
			if (Input.GetKeyDown(KeyCode.C)){		
				attackStrength = character.CharacterRoundhouse;
				KickCommands (attackStrength, 2, "Roundhouse");
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

	void PunchCommands (AttackStrength punch, FeiLongClosePunch feiLongPunches, int punchType, string punchStrength){
		if (character.GetComponent<Ken> () != null)
        {
			if (CheckMotionSuperSequence () && animator.GetBool ("isAirborne") == false && character.GetSuper >= 100f){
				CharacterCompletesMotionSuper ("Ken", "Shinryuken"); 
			}
			else if (CheckHadoukenSequence () && animator.GetBool ("isAirborne") == false && projectileP1Parent.transform.childCount <= 0) {
				ShotoCompletesHadouken ("Ken", punchType);
			}
			else if (CheckShoryukenSequence () && animator.GetBool ("isAirborne") == false) {
				ShotoCompletesShoryuken ("Ken", punchStrength, punchType);
			}
			else if (animator.GetBool ("isAttacking") == false) {
				character.AttackState ();
				punch();
			}
		}
		else if (character.GetComponent<FeiLong> () != null)
        {
			if (CheckMotionSuperSequence () && animator.GetBool ("isAirborne") == false && character.GetSuper >= 100f){
				CharacterCompletesMotionSuper ("FeiLong", "RekkaShinken"); 
			}
			else if (CheckHadoukenSequence () && animator.GetBool ("isAirborne") == false) {
				FeiLongCompletesRekka (punchType);
			}
			else if (animator.GetBool ("isAttacking") == false) {
				if (Mathf.Abs (distance) < 0.75f && animator.GetBool ("isStanding") == true) {
					feiLongPunches();
				}
				else {
					punch();
				}
				character.AttackState ();
			}
		}
		else if (character.GetComponent<Balrog> () != null)
        {
			if (CheckMotionSuperSequence () && animator.GetBool ("isAirborne") == false && character.GetSuper >= 100f){
				CharacterCompletesMotionSuper ("Balrog", "GigatonPunch"); 
			}
			else if (chargeSystem.GetBackCharged () && !sharedProperties.GetBackPressed && animator.GetBool ("isAirborne") == false && sharedProperties.GetForwardPressed) {
				BalrogCompletesDashRushes (punchStrength, punchType);
			}
			else if (chargeSystem.GetDownCharged () && !sharedProperties.GetDownPressed && animator.GetBool ("isAirborne") == false && pressedUp) {
				BalrogCompletesHeadButt (punchStrength, punchType);
			}
			else if (animator.GetBool ("isAttacking") == false) {
				punch();
				character.AttackState ();
			}
		}
		else if (character.GetComponent<Akuma> () != null)
        {
			if (CheckHadoukenSequence () && projectileP1Parent.transform.childCount <= 0) {
				if (animator.GetBool ("isAirborne") == true) {
					AkumaCompletesAirHadouken (punchType);
				}
				else {
					ShotoCompletesHadouken ("Akuma", punchType);
				}
			}
			else if (CheckShoryukenSequence () && animator.GetBool ("isAirborne") == false) {
				ShotoCompletesShoryuken ("Akuma", punchStrength, punchType);
			}
			else if (animator.GetBool ("hyakkishuActive") == true) {
				animator.SetInteger ("hyakkishuAttackType", 1);
			}
			else if (animator.GetBool ("isAttacking") == false) {
				punch();
				character.AttackState ();
			}
        }
        else if (character.GetComponent<Sagat>() != null)
        {
			if (CheckMotionSuperSequence () && animator.GetBool ("isAirborne") == false && character.GetSuper >= 100f){
                CharacterCompletesMotionSuper("Sagat", "TigerCannon");
			}
			else if (CheckHadoukenSequence () && animator.GetBool ("isAirborne") == false && projectileP1Parent.transform.childCount <= 0) {
                SagatCompletesUpperTigerShot(punchType);
            }
			else if (CheckShoryukenSequence () && animator.GetBool ("isAirborne") == false) {
				SagatCompletesTigerUppercut (punchStrength, punchType);
			}
            else if (animator.GetBool ("isAttacking") == false) {
			    punch();
			    character.AttackState ();
            }
        }
        else if (character.GetComponent<MBison>() != null)
        {
			if (chargeSystem.GetBackCharged() && !sharedProperties.GetBackPressed && animator.GetBool("isAirborne") == false && sharedProperties.GetForwardPressed)
            {
                MBisonCompletesPsychoCrusher(punchType);
            }
            else if (chargeSystem.GetDownCharged() && !sharedProperties.GetDownPressed && animator.GetBool("isAirborne") == false && pressedUp)
            {
                MBisonCompletesDevilReverse();
            }
            else if (animator.GetBool("reverseActive") == true || animator.GetBool("devilReverseActive") == true)
            {
                animator.SetTrigger("somerSaultInputed");
            }
            else if (animator.GetBool("isAttacking") == false)
            {
                punch();
                character.AttackState();
            }
        }
    }

	void KickCommands (AttackStrength kick, int kickType, string kickStrength){
		if (character.GetComponent<Ken> () != null) {
			if (CheckHurricaneKickSequence () && animator.GetBool ("isAirborne") == false) {
				ShotoCompletesHurricaneKick ("Ken", kickType);
			}
			else if (CheckHadoukenSequence () && animator.GetBool ("isAirborne") == false) {
				KenCompletesRoll (kickType);
			}
			else if (animator.GetBool ("isAttacking") == false) {
				character.AttackState ();
				kick();
			}
		}
		else if (character.GetComponent<FeiLong> () != null) {
			if (CheckReverseShoryukenSequence () && animator.GetBool ("isAirborne") == false) {
				FeiLongCompletesShienKyaku (kickStrength, kickType);
			}
			else if (CheckShoryukenSequence () && animator.GetBool ("isAirborne") == false) {
				FeiLongCompletesRekkaKun (kickType);
			}
			else if (animator.GetBool ("isAttacking") == false) {
				kick();
				character.AttackState ();
			}
		}
		else if (character.GetComponent<Balrog> () != null) {
			if (chargeSystem.GetBackCharged () && !sharedProperties.GetBackPressed && animator.GetBool ("isAirborne") == false && sharedProperties.GetForwardPressed) {
				BalrogCompletesKickRush (kickStrength, kickType);
			}
			else if (animator.GetBool ("isAttacking") == false) {
				kick();
				character.AttackState ();
			}
		}
		else if (character.GetComponent<Akuma> () != null) {
			if (CheckHurricaneKickSequence () && animator.GetBool ("isAirborne") == false) {
				ShotoCompletesHurricaneKick ("Akuma", kickType);
			}
			else if (CheckShoryukenSequence () && animator.GetBool ("isAirborne") == false) {
				AkumaCompletesHyakkishu (kickType);
			}
			else if (animator.GetBool ("hyakkishuActive") == true) {
				animator.SetInteger ("hyakkishuAttackType", 2);
			}
			else if (animator.GetBool ("isAttacking") == false) {
				kick();
				character.AttackState ();
			}
        }
        else if (character.GetComponent<Sagat>() != null){
			if (CheckHadoukenSequence () && animator.GetBool ("isAirborne") == false && projectileP1Parent.transform.childCount <= 0) {
				SagatCompletesLowerTigerShot (kickType);
			}
			else if (CheckShoryukenSequence () && animator.GetBool ("isAirborne") == false) {
                SagatCompletesTigerKnee (kickType);
			}
			else if (animator.GetBool ("isAttacking") == false) {
				kick();
				character.AttackState ();
			}
        }
        else if (character.GetComponent<MBison>() != null)
        {
            if (CheckMotionSuperSequence() && animator.GetBool("isAirborne") == false && character.GetSuper >= 100f)
            {
                CharacterCompletesMotionSuper("MBison", "KneePressNightmare");
            }
            else if (chargeSystem.GetBackCharged() && !sharedProperties.GetBackPressed && animator.GetBool("isAirborne") == false && sharedProperties.GetForwardPressed)
            {
                MBisonCompletesScissorKick(kickStrength, kickType);
            }
            else if (chargeSystem.GetDownCharged() && !sharedProperties.GetDownPressed && animator.GetBool("isAirborne") == false && pressedUp)
            {
                MBisonCompletesHeadStomp(kickType);
            }
            else if (animator.GetBool("isAttacking") == false)
            {
                kick();
                character.AttackState();
            }
        }
    }
	
	void CharacterCompletesMotionSuper (string fighter, string superName){
		Debug.Log ("Super inputed");
		if (animator.GetBool ("isAttacking") == false) {
			character.AttackState ();
			animator.Play (fighter + superName, 0);
		}
		animator.SetTrigger ("motionSuperInputed");
		comboSystem.ResetMotionSuperSequence ();
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
		Debug.Log ("Shoryuken inputed");
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
		if (sharedProperties.GetForwardPressed && sharedProperties.GetDownPressed) {			
			Debug.Log ("dashed low");
			if (animator.GetBool ("isAttacking") == false) {
				character.AttackState ();
				animator.Play ("BalrogDashLow" + punchName + "StartUp", 0);
			}
			animator.SetTrigger ("dashLowInputed");
		}
		else if (sharedProperties.GetForwardPressed) {
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

    void SagatCompletesUpperTigerShot(int buttonType){
        Debug.Log("Hadouken inputed");
        if (animator.GetBool("isAttacking") == false){
            character.AttackState();
            animator.Play("SagatUpperTigerShot", 0);
        }
        animator.SetTrigger("upperTigerShotInputed");
        animator.SetInteger("upperTigerShotType", buttonType);
        animator.SetInteger("tigerShotOwner", 1);
        comboSystem.ResetHadoukenSequence();
    }

    void SagatCompletesLowerTigerShot(int buttonType){
        Debug.Log("Hadouken inputed");
        if (animator.GetBool("isAttacking") == false){
            character.AttackState();
            animator.Play("SagatLowerTigerShot", 0);
        }
        animator.SetTrigger("lowerTigerShotInputed");
        animator.SetInteger("lowerTigerShotType", buttonType);
        animator.SetInteger("tigerShotOwner", 1);
        comboSystem.ResetHadoukenSequence();
    }

    void SagatCompletesTigerUppercut (string punchName, int punchType){
		Debug.Log ("Shoryuken inputed");
		if (animator.GetBool ("isAttacking") == false) {
			character.AttackState ();
			animator.Play ("SagatTigerUppercut" + punchName, 0);
		}
		animator.SetTrigger ("tigerUppercutInputed");
		animator.SetInteger ("tigerUppercutPunchType", punchType);
		comboSystem.ResetShoryukenSequence ();
	}

	void SagatCompletesTigerKnee (int kickType){
		Debug.Log ("Shoryuken inputed");
		if (animator.GetBool ("isAttacking") == false) {
			character.AttackState ();
			animator.Play ("SagatTigerKnee", 0);
		}
		animator.SetTrigger ("tigerKneeInputed");
		animator.SetInteger ("tigerKneeKickType", kickType);
		comboSystem.ResetShoryukenSequence ();
	}
    
    void MBisonCompletesScissorKick(string kickName, int kickType)
    {
        Debug.Log("Scissor kicked");
        if (animator.GetBool("isAttacking") == false)
        {
            character.AttackState();
            animator.Play("MBisonScissorKicks" + kickName, 0);
        }
        animator.SetTrigger("scissorKicksInputed");
        animator.SetInteger("scissorKicksKickType", kickType);
        chargeSystem.SetBackCharged(false);
        chargeSystem.ResetBackChargedProperties();
    }

    void MBisonCompletesPsychoCrusher(int punchType)
    {
        Debug.Log("Psycho crushered");
        if (animator.GetBool("isAttacking") == false)
        {
            character.AttackState();
            animator.Play("MBisonPsychoCrusherStartUp", 0);
        }
        animator.SetTrigger("psychoCrusherInputed");
        animator.SetInteger("psychoCrusherPunchType", punchType);
        chargeSystem.SetBackCharged(false);
        chargeSystem.ResetBackChargedProperties();
    }

    void MBisonCompletesHeadStomp(int kickType)
    {
        Debug.Log("Head stomped");
        if (animator.GetBool("isAttacking") == false)
        {
            character.AttackState();
            animator.Play("MBisonHeadStomp", 0);
        }
        animator.SetTrigger("headStompInputed");
        animator.SetInteger("headStompKickType", kickType);
        chargeSystem.SetDownCharged(false);
        chargeSystem.ResetDownChargedProperties();
    }

    void MBisonCompletesDevilReverse()
    {
        Debug.Log("Psycho crushered");
        if (animator.GetBool("isAttacking") == false)
        {
            character.AttackState();
            animator.Play("MBisonDevilReverse", 0);
        }
        animator.SetTrigger("headStompInputed");
        chargeSystem.SetDownCharged(false);
        chargeSystem.ResetDownChargedProperties();
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
	
	bool CheckMotionSuperSequence(){
		for (int i=0; i<comboSystem.GetMotionSuperSequence().Length; i++){
			if (comboSystem.GetMotionSuperSequence()[i] == false){
				return false;
			}
		}
		return true;
	}	
	
	public GameObject GetProjectileP1Parent(){
		return projectileP1Parent;
	}
		
	public float GetDistance(){
		return distance;
	}
	
	public float GetDistanceFromOpponent(){
		return distanceFromOpponent;
	}
}
	