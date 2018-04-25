using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Opponent : MonoBehaviour {
	
	public GameObject mugShotObject;
	public GameObject[] streetFighterCharacters;
	public Text nameText;
	public Sprite kenMugShot, feiLongMugShot, balrogMugShot;
	public bool isAI;
	
	private TimeControl timeControl;
	private Animator animator;
	private Player player;
	private Character playerCharacter;
	private Character character;
	private Rigidbody2D physicsbody;
	private HealthBarP2 healthBar;
	private FeiLong feiLong;
	private Image mugShot;
	private GameObject projectileP2Parent;
	
	private KenAI kenAI;
	private FeiLongAI feiLongAI;
	private BalrogAI balrogAI;
	
	private bool pressedForward, pressedBackward, pressedCrouch;
	private float distance, distanceFromPlayer;
		
	void Awake () {
	
		InitiateCharacter();
				
		gameObject.layer = LayerMask.NameToLayer("Player2");
		gameObject.tag = "Player2";
		foreach(Transform character in gameObject.transform){
			character.gameObject.layer = LayerMask.NameToLayer("Player2");	
			character.gameObject.tag = "Player2";
		}			
		character = GetComponentInChildren<Character>();
		animator = GetComponentInChildren<Animator>();	
		mugShot = mugShotObject.GetComponent<Image>();
	}
	
	// Use this for initialization
	void Start () {		
		
		timeControl = FindObjectOfType<TimeControl>();
		player = FindObjectOfType<Player>();
		playerCharacter = player.GetComponentInChildren<Character>();
		physicsbody = GetComponentInChildren<Rigidbody2D>();	
		healthBar = FindObjectOfType<HealthBarP2>();
		if (character.GetComponent<FeiLong>() != null){
			feiLong = GetComponentInChildren<FeiLong>();
			feiLongAI = GetComponent<FeiLongAI>();
			mugShot.sprite = feiLongMugShot;
			nameText.text = "Fei Long";
		}	
		else if (character.GetComponent<Ken>() != null){
			kenAI = GetComponent<KenAI>();
			mugShot.sprite = kenMugShot;
			nameText.text = "Ken";
		}		
		else if (character.GetComponent<Balrog>() != null){
			balrogAI = GetComponent<BalrogAI>();
			mugShot.sprite = balrogMugShot;
			nameText.text = "Balrog";
		}		
		projectileP2Parent = GameObject.Find("ProjectileP2Parent");
		if (projectileP2Parent == null){
			projectileP2Parent = new GameObject("ProjectileP2Parent");
		}
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
			if (TimeControl.roundOver == false){
				TimeControl.slowDown = true;
				if (animator.GetBool("isKOed") == true){
					character.KOSound();
					TimeControl.roundOver = true;
				}					
			}				
				
			if (TimeControl.victoryPose == true && animator.GetBool("isKOed") == false){
				TimeControl.winner = "You Lose";
				animator.Play("VictoryPose");
			}	
		}
		if (character.GetHealth() <= 0){
			animator.SetBool("isKOed", true);
		}
		DetermineSide();
		healthBar.SetHealth(character.GetHealth());
	}			
	
	
	public void AIThrow(){
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isAttacking") == false
		    && animator.GetBool("isMidAirRecovering") == false && animator.GetBool("isThrown") == false){	
			AIStand();
			character.SetBackPressed(false);
			character.AttackState();
			animator.Play("ThrowStartup");
		}
	}
	
	public void AIJab(int maxNum){
		int crouchOrStand = Random.Range(0, maxNum);
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
			if (character.GetComponent<FeiLong>() != null){						
				if (Mathf.Abs(distance) < 0.75f && animator.GetBool("isStanding") == true){
					feiLong.FeiLongCloseJab();
				}
				else{							
					character.CharacterJab();
				}
			}
			else{
				character.CharacterJab();
			}
			AIStand();
		}
	}
	
	public void AIShort(){
		int crouchOrStand = Random.Range(0, 2);
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
	
	public void AIStrong(int maxNum){
		int crouchOrStand = Random.Range(0, maxNum);
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
			if (character.GetComponent<FeiLong>() != null){						
				if (Mathf.Abs(distance) < 0.75f && animator.GetBool("isStanding") == true){
					feiLong.FeiLongCloseStrong();
				}
				else{							
					character.CharacterStrong();
				}
			}
			else{
				character.CharacterStrong();
			}
			AIStand();
		}
	}
	
	public void AIForward(int maxNum){
		int crouchOrStand = Random.Range(0, maxNum);
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
			AIStand();
		}
	}
	
	public void AIFierce(int maxNum, int standNum){
		int crouchOrStand = Random.Range(0, maxNum);
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isAttacking") == false
		    && animator.GetBool("isMidAirRecovering") == false && animator.GetBool("isThrown") == false){					
			character.AttackState();
			if (crouchOrStand <= standNum){
				AIStand();
			}	
			else{
				AICrouch();
			}	
			if (character.GetComponent<FeiLong>() != null){						
				if (Mathf.Abs(distance) < 0.75f && animator.GetBool("isStanding") == true){
					feiLong.FeiLongCloseFierce();
				}
				else{							
					character.CharacterFierce();
				}
			}
			else{
				character.CharacterFierce();
			}
			AIStand();
		}
	}
	
	public void AIJumpFierce(){
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isThrown") == false
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isAttacking") == false
		    && animator.GetBool("isMidAirRecovering") == false && animator.GetBool("isAirborne") == true){					
			character.AttackState();
			character.CharacterFierce();
		}
	}
	
	public void AIJumpRoundhouse(){
		if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
		    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isThrown") == false
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isAttacking") == false
		    && animator.GetBool("isMidAirRecovering") == false && animator.GetBool("isAirborne") == true){					
			character.AttackState();
			character.CharacterRoundhouse();
		}
	}
	
	public void AILowForward(){
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
	
	public void AISweep(){
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
	
	public void AIJump(){		
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
	
	public void DoesAIBlock(){
		int coinflip = Random.Range(0,6);
		if (coinflip >= 2){
			character.SetBackPressed(true);
		}
		else{
			character.SetBackPressed(false);
		}
	}
	
	public void AIStand(){
		animator.SetBool("isStanding", true);
		animator.SetBool("isCrouching", false);
		pressedCrouch = false;
	}	
	
	public void AICrouch(){
		animator.SetBool("isStanding", false);
		animator.SetBool("isCrouching", true);
		pressedCrouch = true;
	}
	
	public void AIPressedForward(){
		pressedForward = true;
		pressedBackward = false;
	}
	
	public void AIPressedBackward(){
		pressedBackward = true;
		pressedForward = false;
	}
	
	public void Walk(){						
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
		
	public void CharacterNeutralState(){
		pressedForward = false;
		pressedBackward = false;
	}	
	
	public void AICharges(){
		pressedForward = false;
		pressedBackward = true;
	}

	void InitiateCharacter (){
		int randChar = Random.Range (0, streetFighterCharacters.Length);
		GameObject streetFighterCharacter = Instantiate (streetFighterCharacters [randChar]);
		streetFighterCharacter.transform.parent = gameObject.transform;
		streetFighterCharacter.transform.position = gameObject.transform.position;
	}
	
	void SideSwitch(){		
		if (animator.GetBool("isAttacking") == false){
			if (distance < 0 && character.side == Character.Side.P1){			
				character.side = Character.Side.P2;
				CharacterNeutralState();
			}
			else if (distance >= 0 && character.side == Character.Side.P2){
				character.side = Character.Side.P1;
				CharacterNeutralState();
			}
		}
		if (animator.GetBool ("isAirborne") == false && animator.GetBool ("isThrown") == false && animator.GetBool ("isWalkingForward") == false
		    && animator.GetBool ("throwTargetAcquired") == false && animator.GetBool ("isLiftingOff") == false && animator.GetBool ("isWalkingBackward") == false				    
		    && animator.GetBool ("isInHitStun") == false && animator.GetBool ("isInBlockStun") == false && animator.GetBool("isKnockedDown") == false) {
			
			character.SideSwitch();
		}
	}
	
	void IsThrown(){
		if (animator.GetBool("isThrown") == true){
			if (playerCharacter.GetComponent<Ken>() != null){
				if (character.side == Character.Side.P2){
					character.transform.position = new Vector3(playerCharacter.transform.position.x + 0.25f, playerCharacter.transform.position.y, 0f);
				}
				else{
					character.transform.position = new Vector3(playerCharacter.transform.position.x - 0.25f, playerCharacter.transform.position.y, 0f);
				}		
			}
			else if (playerCharacter.GetComponent<FeiLong>() != null){
				if (character.side == Character.Side.P2){
					character.transform.position = new Vector3(playerCharacter.transform.position.x + 0.5f, playerCharacter.transform.position.y, 0f);
				}
				else{
					character.transform.position = new Vector3(playerCharacter.transform.position.x - 0.5f, playerCharacter.transform.position.y, 0f);
				}		
			}	
			else if (playerCharacter.GetComponent<Balrog>() != null){
				if (character.side == Character.Side.P2){
					character.transform.position = new Vector3(playerCharacter.transform.position.x + 0.3f, playerCharacter.transform.position.y, 0f);
				}
				else{
					character.transform.position = new Vector3(playerCharacter.transform.position.x - 0.3f, playerCharacter.transform.position.y, 0f);
				}		
			}	
		}
	}
		
	void DetermineSide(){
		distance = playerCharacter.transform.position.x - character.transform.position.x;	
		distanceFromPlayer = Mathf.Abs(playerCharacter.transform.position.x -  character.transform.position.x);
		SideSwitch();
	}
	
	public GameObject GetProjectileP2Parent(){
		return projectileP2Parent;
	}
	
	public bool GetBackPressed(){
		return pressedBackward;
	}	
	
	public bool GetDownPressed(){
		return pressedCrouch;
	}	
	
	public bool GetForwardPressed(){
		return pressedForward;
	}	
	
	public float GetDistanceFromPlayer(){
		return distanceFromPlayer;
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
				if (character.GetComponent<FeiLong>() != null && Mathf.Abs(distance) < 0.75f && animator.GetBool("isStanding") == true){
					feiLong.FeiLongCloseJab();
				}
				else{							
					character.CharacterJab();
				}
				character.AttackState();
			}		
			if (Input.GetKeyDown(KeyCode.T)){
				if (character.GetComponent<FeiLong>() != null && Mathf.Abs(distance) < 0.75f && animator.GetBool("isStanding") == true){
					feiLong.FeiLongCloseFierce();
				}
				else{							
					character.CharacterFierce();
				}
				character.AttackState();
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
				animator.Play("ThrowStartup");
			}
			if (Input.GetKeyDown(KeyCode.H)){
				character.AttackState();
				character.CharacterStrong();
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
		if (Input.GetKeyDown(KeyCode.Alpha4)){
			if (character.GetComponent<Ken>() != null){
				if (animator.GetBool("isAttacking") == false && animator.GetBool("isAirborne") == false){
					animator.Play("KenHurricaneKickLiftOff",0);
					character.AttackState();
				}
				animator.SetTrigger("hurricaneKickInputed");
				animator.SetInteger("hurricaneKickType", 2);
			}
			else if (character.GetComponent<FeiLong>() != null){
				if (animator.GetBool("isAttacking") == false){
					character.AttackState();
					animator.Play("FeiLongRekkaKun",0);
				}
				animator.SetTrigger("shoryukenInputed");
				animator.SetInteger("rekkaKunKickType", 2);
			}
		}	
		if (Input.GetKey (KeyCode.Q)){	
			
			if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
			    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
			    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isMidAirRecovering") == false){
				if (character.GetComponent<Ken>() != null){
				    animator.SetTrigger("shoryukenInputed");			
					if (animator.GetBool("isAttacking") == false){
						character.AttackState();
						animator.Play("KenShoryukenFierce",0);
					}
					animator.SetTrigger("shoryukenInputed");
					animator.SetInteger("shoryukenPunchType", 2);
				}
				else if (character.GetComponent<FeiLong>() != null){
					if (animator.GetBool("isAttacking") == false){
						character.AttackState();
						animator.Play("FeiLongShienKyakuShort",0);
					}
					animator.SetTrigger("reverseShoryukenInputed");
					animator.SetInteger("shienKyakuKickType", 0);
				}
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
