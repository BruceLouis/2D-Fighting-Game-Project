﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Opponent : MonoBehaviour {

    delegate void AIBehavior();
    AIBehavior aiBehavior;

    public GameObject mugShotObject;
	public GameObject[] streetFighterCharacters;
	public Text nameText;
	public Sprite kenMugShot, feiLongMugShot, balrogMugShot, akumaMugShot, sagatMugShot, mbisonMugShot;
	public bool isAI, doInitiateCharacter;
	
	private Animator animator;
	private Player player;
	private Character playerCharacter;
	private Character character;
	private Rigidbody2D physicsbody;
	private HealthBarP2 healthBar;
	private SuperBarP2 superBar;
	private FeiLong feiLong;
	private Image mugShot;
	private GameObject projectileP2Parent;
	private SharedProperties sharedProperties;
	private ComboCounter comboCounter;
	
	private KenAI kenAI;
	private FeiLongAI feiLongAI;
	private BalrogAI balrogAI;
	private AkumaAI akumaAI;
	private SagatAI sagatAI;
	private MBisonAI mbisonAI;
	
	private bool pressedForward, pressedBackward, pressedCrouch, introPlayed;
	private float distance, distanceFromPlayer;
	private string characterName;
		
	void Awake () {
		
		if (doInitiateCharacter){
			InitiateCharacter();
		}
				
		gameObject.layer = LayerMask.NameToLayer("Player2");
		gameObject.tag = "Player2";
		foreach(Transform character in gameObject.transform){
			character.gameObject.layer = LayerMask.NameToLayer("Player2");	
			character.gameObject.tag = "Player2";
		}			
		character = GetComponentInChildren<Character>();
		animator = GetComponentInChildren<Animator>();	
		mugShot = mugShotObject.GetComponent<Image>();
		sharedProperties = GetComponent<SharedProperties>();
	}
	
	// Use this for initialization
	void Start () {		
		
		player = FindObjectOfType<Player>();
		playerCharacter = player.GetComponentInChildren<Character>();
		physicsbody = GetComponentInChildren<Rigidbody2D>();	
		healthBar = FindObjectOfType<HealthBarP2>();
		superBar = FindObjectOfType<SuperBarP2>();	
		
		if (character.GetComponent<FeiLong>() != null){
			feiLong = GetComponentInChildren<FeiLong>();
			feiLongAI = GetComponentInChildren<FeiLongAI>();
            CharacterInitialize(feiLongMugShot, "Fei Long", feiLongAI.Behaviors);
        }	
		else if (character.GetComponent<Ken>() != null){
			kenAI = GetComponentInChildren<KenAI>();
            CharacterInitialize(kenMugShot, "Ken", kenAI.Behaviors);
        }		
		else if (character.GetComponent<Balrog>() != null){
			balrogAI = GetComponentInChildren<BalrogAI>();
            CharacterInitialize(balrogMugShot, "Balrog", balrogAI.Behaviors);
        }		
		else if (character.GetComponent<Akuma>() != null){
			akumaAI = GetComponentInChildren<AkumaAI>();
            CharacterInitialize(akumaMugShot, "Akuma", akumaAI.Behaviors);
        }
        else if (character.GetComponent<Sagat>() != null)
        {
			sagatAI = GetComponentInChildren<SagatAI>();
            CharacterInitialize(sagatMugShot, "Sagat", sagatAI.Behaviors);
        }
        else if (character.GetComponent<MBison>() != null)
        {
			mbisonAI = GetComponentInChildren<MBisonAI>();
            CharacterInitialize(mbisonMugShot, "M Bison", mbisonAI.Behaviors);
        }

        projectileP2Parent = GameObject.Find("ProjectileP2Parent");
		if (projectileP2Parent == null){
			projectileP2Parent = new GameObject("ProjectileP2Parent");
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
			
			sharedProperties.IsThrown(animator, playerCharacter, character);		
			
			DetermineSide();
			
			if (!isAI){
				PunchBagControls();
			}
			else{
                aiBehavior();
            }
		}
		else{	
			if (SceneManager.GetActiveScene().name == "Game"){
				sharedProperties.KOSequence("You Lose");
			}
			else{
				sharedProperties.KOSequence(characterName + " Wins");
			}				
		}
		if (character.GetHealth() <= 0){
			animator.SetBool("isKOed", true);
		}
		if (!animator.GetBool("isInHitStun")){
			if (comboCounter.GetComboCountP1 > 1){
				comboCounter.GetStartTimerP1 = true;
			}
			comboCounter.GetComboCountP1 = 1;
		}
		DetermineSide();
		healthBar.SetHealth(character.GetHealth());	
		superBar.SetSuper(character.GetSuper);	
	}		
				
	void InitiateCharacter (){
		int randChar = Random.Range (0, streetFighterCharacters.Length);
		GameObject streetFighterCharacter = Instantiate (streetFighterCharacters [randChar]);
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

    void SideSwitch(){		
		//determine which side			
//		if (animator.GetBool("isAttacking") == false){
			if (distance < 0 && character.side == Character.Side.P1){			
				character.side = Character.Side.P2;
				sharedProperties.CharacterNeutralState();
			}
			else if (distance >= 0 && character.side == Character.Side.P2){
				character.side = Character.Side.P1;
				sharedProperties.CharacterNeutralState();
			}
//		}
		//only after character is not in these states will the sprite actually switch sides
		if (animator.GetBool ("isAirborne") == false && animator.GetBool ("isThrown") == false && animator.GetBool ("isWalkingForward") == false && animator.GetBool ("isLanding") == false
		    && animator.GetBool ("throwTargetAcquired") == false && animator.GetBool ("isLiftingOff") == false && animator.GetBool ("isWalkingBackward") == false && animator.GetBool("isAttacking") == false
            && animator.GetBool ("isInHitStun") == false && animator.GetBool ("isInBlockStun") == false && animator.GetBool("isKnockedDown") == false) {
			
			character.SideSwitch();
		}
	}
	
	void DetermineSide(){
		distance = playerCharacter.transform.position.x - character.transform.position.x;	
		distanceFromPlayer = Mathf.Abs(distance);
		SideSwitch();
	}
	
	public GameObject GetProjectileP2Parent(){
		return projectileP2Parent;
	}
	
	public float GetDistanceFromPlayer(){
		return distanceFromPlayer;
	}
	
	public float GetDistance(){
		return distance;
	}
	
	void PunchBagControls(){
		if (Input.GetKey(KeyCode.Alpha2)){
			if (character.side == Character.Side.P1){
				pressedForward = true;
                animator.SetBool("isWalkingForward", true);
            }
			else{
				pressedBackward = true;
                animator.SetBool("isWalkingBackward", true);
            }
            character.transform.Translate(Vector3.right * character.GetWalkSpeed() * Time.deltaTime);
        }
		if (Input.GetKey(KeyCode.Alpha1)){
			if (character.side == Character.Side.P2){
				pressedForward = true;
                animator.SetBool("isWalkingForward", true);
            }
			else{
				pressedBackward = true;
                animator.SetBool("isWalkingBackward", true);
            }
            character.transform.Translate(Vector3.left * character.GetWalkSpeed() * Time.deltaTime);
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
//		Walk();
		
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
			if (character.GetComponent<Ken>() != null){
				if (animator.GetBool ("isAttacking") == false) {
					character.AttackState ();
					animator.Play ("KenShinryuken", 0);
				}
				animator.SetTrigger ("motionSuperInputed");
			}
			else if (character.GetComponent<FeiLong>() != null){		
				if (animator.GetBool ("isAttacking") == false) {
					character.AttackState ();
					animator.Play ("FeiLongRekkaShinken", 0);
				}
				animator.SetTrigger ("motionSuperInputed");
			}
			else if (character.GetComponent<Sagat>() != null){		
				if (animator.GetBool ("isAttacking") == false) {
					character.AttackState ();
					animator.Play ("SagatTigerCannon", 0);
				}
				animator.SetTrigger ("motionSuperInputed");
			}
			else if (character.GetComponent<MBison>() != null){		
				if (animator.GetBool ("isAttacking") == false) {
					character.AttackState ();
					animator.Play ("MBisonKneePressNightmare", 0);
				}
				animator.SetTrigger ("motionSuperInputed");
			}
            else if (character.GetComponent<Akuma>() != null)
            {
                character.AttackState();
                animator.Play("AkumaShunGokuSatsuStartup", 0);
            }

        }	
		if (Input.GetKeyDown(KeyCode.Alpha4))
        {
			if (character.GetComponent<Ken>() != null)
            {
				if (animator.GetBool("isAttacking") == false && animator.GetBool("isAirborne") == false)
                {
					animator.Play("KenHurricaneKickLiftOff",0);
					character.AttackState();
				}
				animator.SetTrigger("hurricaneKickInputed");
				animator.SetInteger("hurricaneKickType", 2);
			}
			else if (character.GetComponent<FeiLong>() != null)
            {
				if (animator.GetBool("isAttacking") == false)
                {
					character.AttackState();
					animator.Play("FeiLongRekkaKun",0);
				}
				animator.SetTrigger("shoryukenInputed");
				animator.SetInteger("rekkaKunKickType", 2);
			}
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (character.GetComponent<Ken>() != null)
            {
                if (animator.GetBool("isAttacking") == false && animator.GetBool("isAirborne") == false)
                {
                    animator.Play("KenHadouken", 0);
                    character.AttackState();
                }
                animator.SetTrigger("hadoukenInputed");
                animator.SetInteger("hadoukenPunchType", 0);
            }
        }
        if (Input.GetKey (KeyCode.Q)){	
			
			if (animator.GetBool("isInHitStun") == false && animator.GetBool("isInBlockStun") == false 
			    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isAirborne") == false 
			    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isMidAirRecovering") == false)
            {
				if (character.GetComponent<Ken>() != null)
                {
				    animator.SetTrigger("shoryukenInputed");			
					if (animator.GetBool("isAttacking") == false)
                    {
						character.AttackState();
						animator.Play("KenShoryukenFierce",0);
					}
					animator.SetTrigger("shoryukenInputed");
					animator.SetInteger("shoryukenPunchType", 2);
				}
				else if (character.GetComponent<FeiLong>() != null)
                {
					if (animator.GetBool("isAttacking") == false)
                    {
						character.AttackState();
						animator.Play("FeiLongShienKyakuShort",0);
					}
					animator.SetTrigger("reverseShoryukenInputed");
					animator.SetInteger("shienKyakuKickType", 0);
				}
				else if (character.GetComponent<Akuma>() != null)
                {
					animator.SetTrigger("shoryukenInputed");			
					if (animator.GetBool("isAttacking") == false)
                    {
						character.AttackState();
						animator.Play("AkumaShoryukenFierce",0);
					}
					animator.SetTrigger("shoryukenInputed");
					animator.SetInteger("shoryukenPunchType", 2);
                }
                else if (character.GetComponent<MBison>() != null)
                {
                    if (animator.GetBool("isAttacking") == false)
                    {
                        character.AttackState();
                        animator.Play("MBisonHeadStomp", 0);
                    }
                    animator.SetTrigger("headStompInputed");
                    animator.SetInteger("headStompKickType", 2);
                    animator.SetTrigger("somerSaultInputed");
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
