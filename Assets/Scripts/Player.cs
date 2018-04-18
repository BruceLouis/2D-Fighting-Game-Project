using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	
	private TimeControl timeControl;
	private Animator animator;
	private Opponent opponent;
	private Character opponentCharacter;
	private Character character;
	private Rigidbody2D physicsbody;
	private ComboSystem comboSystem;
	private HealthBarP1 healthBar;
	private bool pressedForward;
	private bool pressedBackward;
	private float distance;	
//	private float hScene;
//	private float wScene;
	
	// Use this for initialization
	void Start () {		
	
		timeControl = FindObjectOfType<TimeControl>();
		opponent = FindObjectOfType<Opponent>();
		opponentCharacter = opponent.GetComponentInChildren<Character>();
		character = GetComponentInChildren<Character>();
		animator = GetComponentInChildren<Animator>();		
		physicsbody = GetComponentInChildren<Rigidbody2D>();
		comboSystem = GetComponent<ComboSystem>();
		character.side = Character.Side.P1;			
		gameObject.layer = LayerMask.NameToLayer("Player1");
		healthBar = FindObjectOfType<HealthBarP1>();	
		foreach(Transform character in gameObject.transform){
			character.gameObject.layer = LayerMask.NameToLayer("Player1");	
		}			
	}
	
	// Update is called once per frame
	void Update () {
		
		Debug.Log ("Player gameOn " + timeControl.gameOn);
		
		if (timeControl.gameOn == true){
		
			
			IsThrown();		
					
			distance = opponentCharacter.transform.position.x - character.transform.position.x;	
			if (animator.GetBool("isAirborne") == false && animator.GetBool("isKnockedDown") == false
			    && animator.GetBool("isThrown") == false && animator.GetBool("throwTargetAcquired") == false){
				SideSwitch();
				//crouch
				if (Input.GetKey(KeyCode.DownArrow)){
					animator.SetBool("isStanding", false);
					animator.SetBool("isCrouching", true);
				}	
				//stand
				if (Input.GetKeyUp(KeyCode.DownArrow)){
					animator.SetBool("isStanding", true);
					animator.SetBool("isCrouching", false);
				}	
			}
		
			character.SetBackPressed(pressedBackward);
			if (animator.GetBool("isKnockedDown") == false && animator.GetBool("isThrown") == false && animator.GetBool("isMidAirRecovering") == false){
				WalkInput();
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
			WalkNoMoreInput();
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
				animator.Play("KenVictoryPose");
			}	
		}
		if (character.GetHealth() <= 0){
			animator.SetBool("isKOed", true);
		}
		PushBoxCollisions();	
		healthBar.SetHealth(character.GetHealth());	
	}
	
	void IsThrown(){
		if (animator.GetBool("isThrown") == true){
			if (character.side == Character.Side.P2){
				character.transform.position = new Vector3(opponentCharacter.transform.position.x + 0.25f, opponentCharacter.transform.position.y, 0f);
			}
			else{
				character.transform.position = new Vector3(opponentCharacter.transform.position.x - 0.25f, opponentCharacter.transform.position.y, 0f);
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
			if (animator.GetBool("isAttacking") == false && animator.GetBool("isAirborne") == false){
				character.AttackState();
				animator.Play("KenThrowStartup");
			}
		}
		else{
			if (Input.GetKeyDown(KeyCode.A)){
				if (CheckHadoukenSequence() && animator.GetBool("isAirborne") == false){
					Debug.Log ("Hadouken inputed");
					if (animator.GetBool("isAttacking") == false){
						character.AttackState();
						character.KenHadouken(0, 1);
					}
					animator.SetTrigger("hadoukenInputed");
					comboSystem.ResetHadoukenSequence();
				}
				else if (CheckShoryukenSequence() && animator.GetBool("isAirborne") == false){
					Debug.Log ("Shoryuken inputed");
					if (animator.GetBool("isAttacking") == false){
						character.AttackState();
						animator.Play("KenShoryukenJab",0);
						animator.SetBool("isInvincible", true);		
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
			if (Input.GetKeyDown(KeyCode.S)){
				if (CheckHadoukenSequence() && animator.GetBool("isAirborne") == false){
					Debug.Log ("Hadouken inputed");
					if (animator.GetBool("isAttacking") == false){
						character.AttackState();
						character.KenHadouken(1, 1);
					}
					animator.SetTrigger("hadoukenInputed");
					comboSystem.ResetHadoukenSequence();
				}
				else if (CheckShoryukenSequence() && animator.GetBool("isAirborne") == false){
					Debug.Log ("Shoryuken inputed");
					if (animator.GetBool("isAttacking") == false){
						character.AttackState();
						animator.Play("KenShoryukenStrong",0);
						animator.SetBool("isInvincible", true);		
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
			if (Input.GetKeyDown(KeyCode.D)){
				if (CheckHadoukenSequence() && animator.GetBool("isAirborne") == false){
					Debug.Log ("Hadouken inputed");
					if (animator.GetBool("isAttacking") == false){
						character.AttackState();
						character.KenHadouken(2, 1);
					}
					animator.SetTrigger("hadoukenInputed");
					comboSystem.ResetHadoukenSequence();
				}
				else if (CheckShoryukenSequence() && animator.GetBool("isAirborne") == false){
					Debug.Log ("Shoryuken inputed");
					if (animator.GetBool("isAttacking") == false){
						character.AttackState();
						animator.Play("KenShoryukenFierce",0);
						animator.SetBool("isInvincible", true);		
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
			if (Input.GetKeyDown(KeyCode.Z)){					
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
			if (Input.GetKeyDown(KeyCode.X)){
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
			if (Input.GetKeyDown(KeyCode.C)){
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
		}
	}
	
	void PushBoxCollisions(){		
	//ignore PushBox when character is in the air		
		if (animator.GetBool("isAirborne") == true || animator.GetBool("isLiftingOff") == true){
			if (animator.GetBool("isMidAirRecovering") == false && animator.GetBool("isKnockedDown") == false
				&& animator.GetBool("hurricaneKickActive") == false){
				foreach(Transform character in gameObject.transform){
					character.gameObject.layer = LayerMask.NameToLayer("IgnorePlayer2");	
				}
			}
			else if (animator.GetBool("isMidAirRecovering") == true || animator.GetBool("isKnockedDown") == true){
				foreach(Transform character in gameObject.transform){
					character.gameObject.layer = LayerMask.NameToLayer("KnockedDown");	
				}
			}
		}
		else if (animator.GetBool("isMidAirRecovering") == true || animator.GetBool("isKnockedDown") == true){
			foreach(Transform character in gameObject.transform){
				character.gameObject.layer = LayerMask.NameToLayer("KnockedDown");	
			}
		}
		else if (animator.GetBool("isInvincible") == true || animator.GetBool("throwTargetAcquired") == true){
			foreach(Transform character in gameObject.transform){
				character.gameObject.layer = LayerMask.NameToLayer("InvincibleToP2");	
			}	
		}
		else{
			foreach(Transform character in gameObject.transform){
				character.gameObject.layer = LayerMask.NameToLayer("Player1");	
			}
		}		
	}

	void WhoWonAndLost (string WonOrLost)
	{
		gameObject.tag = WonOrLost;
		foreach (Transform character in gameObject.transform) {
			character.gameObject.tag = WonOrLost;
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
}
	