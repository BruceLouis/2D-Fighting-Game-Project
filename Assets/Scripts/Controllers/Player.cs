using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Player : MonoBehaviour {
	
	public GameObject mugShotObject;
	public GameObject[] streetFighterCharacters;
	public Text nameText;
	public Sprite kenMugShot, feiLongMugShot;
	
	private TimeControl timeControl;
	private Animator animator;
	private Opponent opponent;
	private Character opponentCharacter;
	private Character character;
	private FeiLong feiLong;
	private Rigidbody2D physicsbody;
	private ComboSystem comboSystem;
	private HealthBarP1 healthBar;
	private Image mugShot;
	private CharacterChoice characterChoice;
	
	private bool pressedForward, pressedBackward, pressedCrouch;
	private float distance;	
	
	void Awake () {
		
		if (GameObject.Find("CharacterChoice") != null){
			characterChoice = FindObjectOfType<CharacterChoice>();
		}
		GameObject streetFighterCharacter = Instantiate(streetFighterCharacters[characterChoice.GetChosenChar()]);
		streetFighterCharacter.transform.parent = gameObject.transform;
		streetFighterCharacter.transform.position = gameObject.transform.position;
				
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
	}
	
	// Update is called once per frame
	void Update () {
		
		
		if (timeControl.gameOn == true){
					
			distance = opponentCharacter.transform.position.x - character.transform.position.x;	
			character.SetBackPressed(pressedBackward);
			
			IsThrown();		
			CrouchOrStand();
		
			if (animator.GetBool("isKnockedDown") == false && animator.GetBool("isThrown") == false && animator.GetBool("isMidAirRecovering") == false){
				if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
					&& animator.GetBool("isLiftingOff") == false){
					if (animator.GetBool("isStanding") == true && animator.GetBool("isAirborne") == false && animator.GetBool("isAttacking") == false){
						Walk();
						if (Input.GetKey(KeyCode.UpArrow)){
							character.CharacterJump(pressedForward, pressedBackward);
							animator.SetBool ("isStanding",false);
							animator.SetBool ("isLiftingOff",true);
						}				
						
					}				
					AttacksInput();
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
		
	void CrouchOrStand ()
	{
		if (animator.GetBool ("isAirborne") == false && animator.GetBool ("isThrown") == false && animator.GetBool ("throwTargetAcquired") == false) {
			if (animator.GetBool ("isInHitStun") == false && animator.GetBool ("isInBlockStun") == false && animator.GetBool("isKnockedDown") == false) {
				SideSwitch ();
			}
			//crouch
			if (Input.GetKey (KeyCode.DownArrow)) {
				animator.SetBool ("isStanding", false);
				animator.SetBool ("isCrouching", true);
			}
			//stand
			else if (Input.GetKeyUp (KeyCode.DownArrow)){
				animator.SetBool ("isStanding", true);
				animator.SetBool ("isCrouching", false);
			}
		}
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
		}
	}
	
	void CharacterButtonStates(){
		pressedForward = false;
		pressedBackward = false;
	}
	
	void SideSwitch(){			
		//determine which side
		if (animator.GetBool("isAttacking") == false){
			if (distance < 0 && character.side == Character.Side.P1){			
				character.side = Character.Side.P2;
				character.SideSwitch();
				CharacterButtonStates();
			}
			else if (distance >= 0 && character.side == Character.Side.P2){
				character.side = Character.Side.P1;
				character.SideSwitch();
				CharacterButtonStates();
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
			if (character.GetComponent<Ken>() != null){
				if (animator.GetBool("isAttacking") == false && animator.GetBool("isAirborne") == false){
					character.AttackState();
					animator.Play("KenThrowStartup");
				}				
			}
			else if (character.GetComponent<FeiLong>() != null){
				if (animator.GetBool("isAttacking") == false && animator.GetBool("isAirborne") == false){
					character.AttackState();
					animator.Play("FeiLongThrowStartup");
				}				
			}
		}
		else{
			if (Input.GetKeyDown(KeyCode.A)){
				
				if (character.GetComponent<Ken>() != null){
					if (CheckHadoukenSequence() && animator.GetBool("isAirborne") == false){
						Debug.Log ("Hadouken inputed");
						if (animator.GetBool("isAttacking") == false){
							character.AttackState();
							animator.Play("KenHadouken",0);			
						}
						animator.SetTrigger("hadoukenInputed");
						animator.SetInteger("hadoukenPunchType", 0);
						animator.SetInteger("hadoukenOwner", 1);
						comboSystem.ResetHadoukenSequence();
					}
					else if (CheckShoryukenSequence() && animator.GetBool("isAirborne") == false){
						Debug.Log ("Shoryuken inputed");
						if (animator.GetBool("isAttacking") == false){
							character.AttackState();
							animator.Play("KenShoryukenJab",0);	
						}
						animator.SetTrigger("shoryukenInputed");
						animator.SetInteger("shoryukenPunchType", 0);
						comboSystem.ResetShoryukenSequence();
					}
					else if (animator.GetBool("isAttacking") == false){					
						character.AttackState();
						character.CharacterJab();
					}
				}
				else if (character.GetComponent<FeiLong>() != null){
					if (CheckHadoukenSequence() && animator.GetBool("isAirborne") == false){
						Debug.Log ("Hadouken inputed");
						if (animator.GetBool("isAttacking") == false){
							character.AttackState();
							animator.Play("FeiLongRekkaKenFirstAttack",0);			
						}
						animator.SetTrigger("hadoukenInputed");
						animator.SetInteger("rekkaPunchType", 0);
						comboSystem.ResetHadoukenSequence();
					}
					if (animator.GetBool("isAttacking") == false){	
						if (Mathf.Abs(distance) < 0.75f && animator.GetBool("isStanding") == true){
							feiLong.FeiLongCloseJab();
						}
						else{							
							character.CharacterJab();
						}
						character.AttackState();
					}
				}				
			}		
			if (Input.GetKeyDown(KeyCode.S)){
				
				if (character.GetComponent<Ken>() != null){
					if (CheckHadoukenSequence() && animator.GetBool("isAirborne") == false){
						Debug.Log ("Hadouken inputed");
						if (animator.GetBool("isAttacking") == false){
							character.AttackState();
							animator.Play("KenHadouken",0);			
						}
						animator.SetTrigger("hadoukenInputed");
						animator.SetInteger("hadoukenPunchType", 1);
						animator.SetInteger("hadoukenOwner", 1);
						comboSystem.ResetHadoukenSequence();
					}
					else if (CheckShoryukenSequence() && animator.GetBool("isAirborne") == false){
						Debug.Log ("Shoryuken inputed");
						if (animator.GetBool("isAttacking") == false){
							character.AttackState();
							animator.Play("KenShoryukenStrong",0);
						}
						animator.SetTrigger("shoryukenInputed");
						animator.SetInteger("shoryukenPunchType", 1);
						comboSystem.ResetShoryukenSequence();
					}	
					else if (animator.GetBool("isAttacking") == false){					
						character.AttackState();
						character.CharacterStrong();
					}
				}
				else if (character.GetComponent<FeiLong>() != null){
					if (CheckHadoukenSequence() && animator.GetBool("isAirborne") == false){
						Debug.Log ("Hadouken inputed");
						if (animator.GetBool("isAttacking") == false){
							character.AttackState();
							animator.Play("FeiLongRekkaKenFirstAttack",0);			
						}
						animator.SetTrigger("hadoukenInputed");
						animator.SetInteger("rekkaPunchType", 1);
						comboSystem.ResetHadoukenSequence();
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
			}			
			if (Input.GetKeyDown(KeyCode.D)){
				
				if (character.GetComponent<Ken>() != null){
					if (CheckHadoukenSequence() && animator.GetBool("isAirborne") == false){
						Debug.Log ("Hadouken inputed");
						if (animator.GetBool("isAttacking") == false){
							character.AttackState();				
							animator.Play("KenHadouken",0);			
						}
						animator.SetTrigger("hadoukenInputed");
						animator.SetInteger("hadoukenPunchType", 2);
						animator.SetInteger("hadoukenOwner", 1);
						comboSystem.ResetHadoukenSequence();
					}
					else if (CheckShoryukenSequence() && animator.GetBool("isAirborne") == false){
						Debug.Log ("Shoryuken inputed");
						if (animator.GetBool("isAttacking") == false){
							character.AttackState();
							animator.Play("KenShoryukenFierce",0);
						}
						animator.SetTrigger("shoryukenInputed");
						animator.SetInteger("shoryukenPunchType", 2);
						comboSystem.ResetShoryukenSequence();
					}	
					else if (animator.GetBool("isAttacking") == false){					
						character.AttackState();
						character.CharacterFierce();
					}
				}
				else if (character.GetComponent<FeiLong>() != null){
					if (CheckHadoukenSequence() && animator.GetBool("isAirborne") == false){
						Debug.Log ("Hadouken inputed");
						if (animator.GetBool("isAttacking") == false){
							character.AttackState();
							animator.Play("FeiLongRekkaKenFirstAttack",0);			
						}
						animator.SetTrigger("hadoukenInputed");
						animator.SetInteger("rekkaPunchType", 2);
						comboSystem.ResetHadoukenSequence();
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
			}
			if (Input.GetKeyDown(KeyCode.Z)){	
			
				if (character.GetComponent<Ken>() != null){				
					if (CheckHurricaneKickSequence() && animator.GetBool("isAirborne") == false){
						Debug.Log ("Hurricane kick inputed");
						if (animator.GetBool("isAttacking") == false){
							animator.Play("KenHurricaneKickLiftOff",0);
							character.AttackState();
						}
						animator.SetTrigger("hurricaneKickInputed");
						animator.SetInteger("hurricaneKickType", 0);
						comboSystem.ResetHurricaneKickSequence();
					}
					else if (CheckHadoukenSequence() && animator.GetBool("isAirborne") == false){
						Debug.Log ("Hadouken inputed");
						if (animator.GetBool("isAttacking") == false){
							character.AttackState();
							animator.Play("KenRoll");
						}
						animator.SetTrigger("rollInputed");
						animator.SetInteger("rollKickType", 0);
						comboSystem.ResetHadoukenSequence();
					}
					else if (animator.GetBool("isAttacking") == false){
						character.AttackState();
						character.CharacterShort();
					}
				}
				else if (character.GetComponent<FeiLong>() != null){
					if (CheckReverseShoryukenSequence() && animator.GetBool("isAirborne") == false){
						Debug.Log ("Reverse Shoryuken inputed");	
						if (animator.GetBool("isAttacking") == false){
							character.AttackState();
							animator.Play("FeiLongShienKyakuShort",0);
						}
						animator.SetTrigger("reverseShoryukenInputed");
						animator.SetInteger("shienKyakuKickType", 0);
						comboSystem.ResetShoryukenSequence();
					}	
					else if (CheckShoryukenSequence() && animator.GetBool("isAirborne") == false){
						Debug.Log ("Shoryuken inputed");
						if (animator.GetBool("isAttacking") == false){
							character.AttackState();
							animator.Play("FeiLongRekkaKun",0);
						}
						animator.SetTrigger("shoryukenInputed");
						animator.SetInteger("rekkaKunKickType", 0);
						comboSystem.ResetShoryukenSequence();
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
						Debug.Log ("Hurricane kick inputed");
						if (animator.GetBool("isAttacking") == false){
							animator.Play("KenHurricaneKickLiftOff",0);
							character.AttackState();
						}
						animator.SetTrigger("hurricaneKickInputed");
						animator.SetInteger("hurricaneKickType", 1);
						comboSystem.ResetHurricaneKickSequence();
					}
					else if (CheckHadoukenSequence() && animator.GetBool("isAirborne") == false){
						Debug.Log ("Hadouken inputed");
						if (animator.GetBool("isAttacking") == false){
							character.AttackState();
							animator.Play("KenRoll");
						}
						animator.SetTrigger("rollInputed");
						animator.SetInteger("rollKickType", 1);
						comboSystem.ResetHadoukenSequence();
					}
					else if (animator.GetBool("isAttacking") == false){
						character.AttackState();
						character.CharacterForward();
					}
				}
				else if (character.GetComponent<FeiLong>() != null){
					if (CheckReverseShoryukenSequence() && animator.GetBool("isAirborne") == false){
						Debug.Log ("Reverse Shoryuken inputed");	
						if (animator.GetBool("isAttacking") == false){
							character.AttackState();
							animator.Play("FeiLongShienKyakuForward",0);
						}
						animator.SetTrigger("reverseShoryukenInputed");
						animator.SetInteger("shienKyakuKickType", 1);
						comboSystem.ResetShoryukenSequence();
					}	
					else if (CheckShoryukenSequence() && animator.GetBool("isAirborne") == false){
						Debug.Log ("Shoryuken inputed");
						if (animator.GetBool("isAttacking") == false){
							character.AttackState();
							animator.Play("FeiLongRekkaKun",0);
						}
						animator.SetTrigger("shoryukenInputed");
						animator.SetInteger("rekkaKunKickType", 1);
						comboSystem.ResetShoryukenSequence();
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
						Debug.Log ("Hurricane kick inputed");
						if (animator.GetBool("isAttacking") == false){
							animator.Play("KenHurricaneKickLiftOff",0);
							character.AttackState();
						}
						animator.SetTrigger("hurricaneKickInputed");
						animator.SetInteger("hurricaneKickType", 2);
						comboSystem.ResetHurricaneKickSequence();
					}
					else if (CheckHadoukenSequence() && animator.GetBool("isAirborne") == false){
						Debug.Log ("Hadouken inputed");
						if (animator.GetBool("isAttacking") == false){
							character.AttackState();
							animator.Play("KenRoll");
						}
						animator.SetTrigger("rollInputed");
						animator.SetInteger("rollKickType", 1);
						comboSystem.ResetHadoukenSequence();
					}
					else if (animator.GetBool("isAttacking") == false){
						character.AttackState();
						character.CharacterRoundhouse();
					}
				}
				else if (character.GetComponent<FeiLong>() != null){					
					if (CheckReverseShoryukenSequence() && animator.GetBool("isAirborne") == false){
						Debug.Log ("Reverse Shoryuken inputed");	
						if (animator.GetBool("isAttacking") == false){
							character.AttackState();
							animator.Play("FeiLongShienKyakuRoundhouse",0);
						}
						animator.SetTrigger("reverseShoryukenInputed");
						animator.SetInteger("shienKyakuKickType", 2);
						comboSystem.ResetShoryukenSequence();
					}	
					else if (CheckShoryukenSequence() && animator.GetBool("isAirborne") == false){
						Debug.Log ("Shoryuken inputed");
						if (animator.GetBool("isAttacking") == false){
							character.AttackState();
							animator.Play("FeiLongRekkaKun",0);
						}
						animator.SetTrigger("shoryukenInputed");
						animator.SetInteger("rekkaKunKickType", 2);
						comboSystem.ResetShoryukenSequence();
					}	
					else if (animator.GetBool("isAttacking") == false){						
						character.CharacterRoundhouse();
						character.AttackState();
					}
				}	
			}	
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
}
	