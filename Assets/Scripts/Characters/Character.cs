using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  The backbone of a character's functionality lies here. Attributes shared by all character
 *  including walkspeed, health, the 6 normal attacks crouching, jumping or standing, their states
 *  and plenty more are all implemented here. Most of the methods here are to be used in the animation
 *  events in the Mechanim. 
 */

public class Character : MonoBehaviour {
	
	[SerializeField] float 		walkSpeed, health;
	[SerializeField] GameObject landingDust, superEffect, demonSpark;	
	[SerializeField] AudioClip 	KOsound, fellSound, akumaDemonSound, akumaDemonKOSound;
	[SerializeField] AudioClip 	normalAttackSound, balrogHeadButtSound, superEffectSound;
	
	public enum Side {P1, P2};
    [HideInInspector]
    public Side side;

    public enum MoveType {low, mid, high};
    [HideInInspector]
    public MoveType moveType;

    public enum HitType {normal, sweep, normalKnockDown, shoryuken, hurricaneKick, rekka, dashLow, akumaHurricaneKick, bisonSweep, otherKnockDown};
    [HideInInspector]
    public HitType hitType;

    public enum SparkType {normal, big, shoryuken};
    [HideInInspector]
    public SparkType sparkType;	
	
	private Rigidbody2D physicsbody;
	private Animator animator;
	private HitBox hitBox;	 
	
	private float 	enforceHitStun, enforceBlockStun, enforcePushBack, enforceDamage,
					accumulateSuper, super, leftEdgeDistance, rightEdgeDistance;
	
	private bool 	backPressed, airborne, midAirRecovering, didntHit, changePosition,
					isHitStunned, isBlockStunned, isKnockDown, isThrown, isKO;
    		
	void Start(){
		hitBox = GetComponentInChildren<HitBox>();
		physicsbody = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		animator.SetBool("isStanding", true);
		foreach(Transform child in transform){
			child.gameObject.tag = transform.gameObject.tag;
		}
	}
	
	// Fixed Update for the gap between being airborne and lifting off and to turn hurtbox off when a KO happens
	void FixedUpdate(){
		if (physicsbody.velocity.y != 0f && animator.GetBool("isLiftingOff")){
			animator.SetBool("isAirborne", true);
			animator.SetBool("isStanding", false);
			animator.SetBool("isLiftingOff", false);
        }
        if (TimeControl.gameState == TimeControl.GameState.KOHappened && GetComponentInChildren<HurtBox>() != null){
            GetComponentInChildren<HurtBox>().gameObject.SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update(){
		airborne = animator.GetBool("isAirborne");
		didntHit = animator.GetBool("hasntHit");
		midAirRecovering = animator.GetBool("isMidAirRecovering");	
		isHitStunned = animator.GetBool("isInHitStun");
		isBlockStunned = animator.GetBool("isInBlockStun");
		isKnockDown = animator.GetBool("isKnockedDown");
		isThrown = animator.GetBool("isThrown");
		isKO = animator.GetBool("isKOed");		
		animator.SetBool("isCancellable", hitBox.GetCancellabe());
		animator.SetFloat("yVelocity", physicsbody.velocity.y);	
						
		leftEdgeDistance = Mathf.Abs(CameraControl.leftBGEdge) + transform.position.x; 
		rightEdgeDistance = CameraControl.rightBGEdge - transform.position.x; 		
	}
	
				
	//when character lands on the ground without getting hit
	void OnCollisionEnter2D(Collision2D collision){
		if (collision.gameObject.GetComponent<Ground>()	&& animator.GetBool("isMidAirHit") == false && animator.GetFloat("yVelocity") <= 0){
			animator.SetBool("isAirborne", false);
			animator.SetBool("shoryukenActive", false);
			animator.SetBool("shienKyakuActive", false);
			animator.SetBool("rekkaKunActive", false);
			animator.SetBool("isHeadButting", false);
			animator.SetBool("tigerUppercutActive", false);
			animator.SetBool("tigerKneeActive", false);			
			animator.SetBool("scissorKicksActive", false);			
			animator.SetBool("headStompActive", false);
            animator.SetBool("devilReverseActive", false);
        }
	}

	public void AttackState(){
		animator.SetBool("isAttacking", true);
		animator.SetBool("isWalkingBackward", false);
		animator.SetBool("isWalkingForward", false);
	}
	
	public void CharacterJab(){ 
		if (animator.GetBool("isLiftingOff") == false){
			AudioSource.PlayClipAtPoint(normalAttackSound, transform.position);
			if (animator.GetBool("isStanding")){
				animator.Play("StandJab",0);
				MoveProperties(15f, 15f, 10f, 20f, 2, 0, 0);
			}
			else if (animator.GetBool("isCrouching")){
				animator.Play("CrouchJab",0);
				MoveProperties(25f, 15f, 10f, 20f, 2, 0, 0);
			}
			else if (animator.GetBool("isAirborne")){
				if (physicsbody.velocity.x != 0){
					animator.Play("JumpJab",0);
				}
				else{
					animator.Play("JumpNeutralJab",0);
				}		
				MoveProperties(15f, 7f, 7.5f, 20f, 1, 0, 0);	
			}
		}
	}
	
	public void CharacterStrong(){ 
		if (animator.GetBool("isLiftingOff") == false){
			AudioSource.PlayClipAtPoint(normalAttackSound, transform.position);
			if (animator.GetBool("isStanding")){
				animator.Play("StandStrong",0);
				MoveProperties(30f, 22.5f, 10f, 40f, 2, 0, 0);
			}
			else if (animator.GetBool("isCrouching")){
				animator.Play("CrouchStrong",0);
				MoveProperties(30f, 25f, 10f, 40f, 2, 0, 0);
			}
			else if (animator.GetBool("isAirborne")){
				if (physicsbody.velocity.x != 0){
					animator.Play("JumpStrong",0);
				}
				else{
					animator.Play("JumpNeutralStrong",0);
				}			
				MoveProperties(15f, 8.5f, 10f, 40f, 1, 0, 0);
			}	
		}
	}
	
	public void CharacterFierce(){
		if (animator.GetBool("isLiftingOff") == false){
			AudioSource.PlayClipAtPoint(normalAttackSound, transform.position);
			if (animator.GetBool("isStanding")){
				animator.Play("StandFierce",0);
				MoveProperties(40f, 20f, 12f, 55f, 2, 0, 1);
			}
			else if (animator.GetBool("isCrouching")){
				animator.Play("CrouchFierce",0);
				MoveProperties(40f, 20f, 12f, 55f, 2, 0, 1);
			}
			else if (animator.GetBool("isAirborne")){
				if (physicsbody.velocity.x != 0){
					animator.Play("JumpFierce",0);
				}
				else{
					animator.Play("JumpNeutralFierce",0);
				}			
				MoveProperties(40f, 8.5f, 6f, 55f, 1, 0, 1);
			}
		}
	}
	
	public void CharacterShort(){	
		if (animator.GetBool("isLiftingOff") == false){	
			AudioSource.PlayClipAtPoint(normalAttackSound, transform.position);
			if (animator.GetBool("isStanding")){
				animator.Play("StandShort",0);
				if (GetComponent<Balrog>() != null){
					MoveProperties(15f, 15f, 10f, 20f, 2, 0, 0);
				}
				else{					
					MoveProperties(15f, 15f, 10f, 20f, 0, 0, 0);
				}
			}
			else if (animator.GetBool("isCrouching")){
				animator.Play("CrouchShort",0);
				MoveProperties(30f, 17.5f, 10f, 20f, 0, 0, 0);
			}			
			else if (animator.GetBool("isAirborne")){
				if (physicsbody.velocity.x != 0){
					animator.Play("JumpShort",0);
				}
				else{
					animator.Play("JumpNeutralShort",0, 0);
				}			
				MoveProperties(20f, 7f, 7.5f, 20f, 1, 0, 0);
			}
		}
	}
	
	public void CharacterForward(){
		if (animator.GetBool("isLiftingOff") == false){	
			AudioSource.PlayClipAtPoint(normalAttackSound, transform.position);
			if (animator.GetBool("isStanding")){
				animator.Play("StandForward",0);
				MoveProperties(30f, 20f, 10f, 40f, 2, 0, 0);
			}
			else if (animator.GetBool("isCrouching")){
				animator.Play("CrouchForward",0);
				MoveProperties(32.5f, 25f, 10f, 40f, 0, 0, 0);
			}
			else if (animator.GetBool("isAirborne")){
				if (physicsbody.velocity.x != 0){
					animator.Play("JumpForwardAttack",0);
				}
				else{
					animator.Play("JumpNeutralForward",0);
				}			
				MoveProperties(35f, 8.5f, 10f, 40f, 1, 0, 0);
			}
		}
	}
	
	public void CharacterRoundhouse(){
		if (animator.GetBool("isLiftingOff") == false){		
			if (animator.GetBool("isStanding")){
				animator.Play("StandRoundhouse",0);
				MoveProperties(40f, 20f, 12f, 55f, 2, 0, 1);
                AudioSource.PlayClipAtPoint(normalAttackSound, transform.position);
            }
			else if (animator.GetBool("isCrouching")){
				animator.Play("CrouchRoundhouse",0);
                if (GetComponent<MBison>() != null)
                {
                    MoveProperties(40f, 20f, 12f, 60f, 0, 8, 1);
                }
                else
                {
                    MoveProperties(40f, 20f, 12f, 60f, 0, 1, 1);
                    AudioSource.PlayClipAtPoint(normalAttackSound, transform.position);
                }
			}
			else if (animator.GetBool("isAirborne")){
				if (physicsbody.velocity.x != 0){
					animator.Play("JumpRoundhouse",0);
				}
				else{
					animator.Play("JumpNeutralRoundhouse",0);
				}			
				MoveProperties(40f, 8.5f, 6f, 55f, 1, 0, 1);
                AudioSource.PlayClipAtPoint(normalAttackSound, transform.position);
            }	
		}
	}
	
	public void CharacterJump(bool forward, bool backward){
		if (forward == true & backward == false){
			animator.Play("JumpForward",0);
		}
		else if (forward == false && backward == true){	
			animator.Play("JumpBackward",0);			
		}
		else{			
			animator.Play("JumpNeutral",0);	
		}		
	}
	
	public void CharacterLiftOff(float x){
        //need to get rid of that giant conditional and put it into a function that returns this instead
		if (animator.GetBool("isAirborne") == false && animator.GetBool("isInHitStun") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isInBlockStun") == false
	    	&& animator.GetBool("isThrown") == false && animator.GetBool("isMidAirRecovering") == false
		   	&& animator.GetBool("isLanding") == false && animator.GetBool("isAttacking") == false 
            && animator.GetBool("isLiftingOff") == true){			
			if (transform.localScale.x == 1){
                if (GetComponent<Sagat>() != null){
				    physicsbody.velocity = new Vector2(x, 3.75f);
                }
                else{
				    physicsbody.velocity = new Vector2(x, 4.5f);
                }
			}
			else{
                if (GetComponent<Sagat>() != null){
				    physicsbody.velocity = new Vector2(-x, 3.75f);
                }
                else{
				    physicsbody.velocity = new Vector2(-x, 4.5f);
                }
			}
		}
	}
	
	public void AtTheCorner (){
		physicsbody.isKinematic = true;
		// if he's near the right corner & travelling towards the right corner facing the right corner on p1 side
		if (GetRightEdgeDistance () < 0.25f && transform.localScale.x == 1) {
			physicsbody.velocity = new Vector2 (0f, physicsbody.velocity.y);
		}
		// if he's near the left corner & travelling towards the left corner facing the left corner on p2 side
		if (GetLeftEdgeDistance () < 0.25f && transform.localScale.x == -1) {
			physicsbody.velocity = new Vector2 (0f, physicsbody.velocity.y);
		}
	}
	
	public void SideSwitch(){	
		Vector3 theScale = transform.localScale;
		if (side == Side.P2){	
			theScale.x = -1;
			transform.localScale = theScale;	
		}
		else{
			theScale.x = 1;
			transform.localScale = theScale;	
		}
		animator.SetBool("isWalkingForward", false);
		animator.SetBool("isWalkingBackward", false);
	}
    	
	public void MidAirRecovery()
    {
        float distance = GetComponentInParent<SharedProperties>().GetDistanceFromOtherFighter();
        physicsbody.velocity = distance < 0f ? new Vector2(1.5f, 2f) : new Vector2(-1.5f, 2f); //will always roll away from the other character
    }	
	
	public void Sweep(){
		if (side == Side.P2){
			physicsbody.velocity = new Vector2(1f, 3.5f);
		}
		else{
			physicsbody.velocity = new Vector2(-1f, 3.5f);
		}
		animator.SetBool("isAirborne", true);
	}
	
	public void TossedByKen(){
		YouGonnaGetTossed (60f, -3f, 4f);
	}
	
	public void TossedByFeiLong(){
		YouGonnaGetTossed (60f, 3f, 4f);
	}
		
	public void HeadButtedByBalrog(){
		YouGonnaGetTossed (20f, 2f, 4f);
	}
	
	public void TossedByAkuma(){
		YouGonnaGetTossed (60f, 3f, 4f);
	}
    
	public void TossedBySagat(){
		YouGonnaGetTossed (20f, 3f, 4f);
	}

	public void TossedByMBison(){
		YouGonnaGetTossed (60f, -4f, 3f);
	}

	public void MoveProperties(	float hitstun, float blockstun, float pushback, float damage, 
								int moveTypeInt, int hitTypeInt, int sparkTypeInt, float superBuilt = 3f){
								
		enforceHitStun = hitstun;		
		enforceBlockStun = blockstun;
		enforcePushBack = pushback;
		enforceDamage = damage;
		accumulateSuper = superBuilt;
		
		switch(moveTypeInt){
			case 0:
				moveType = MoveType.low;
				break;
			case 1:
				moveType = MoveType.mid;
				break;
			default:
				moveType = MoveType.high;
				break;
		}
		
		switch(hitTypeInt){
			case 0:
				hitType = HitType.normal;
				break;
			case 1:
				hitType = HitType.sweep;
				break;
			case 2:
				hitType = HitType.normalKnockDown;
				break;
			case 3:
				hitType = HitType.shoryuken;
				break;
			case 4:
				hitType = HitType.hurricaneKick;
				break;
			case 5:
				hitType = HitType.rekka;
				break;
			case 6:
				hitType = HitType.dashLow;
				break;
            case 7:
                hitType = HitType.akumaHurricaneKick;
				break;
            case 8:
                hitType = HitType.bisonSweep;
                break;
            default:
                hitType = HitType.otherKnockDown;
                break;
        }
		
		switch(sparkTypeInt){
			case 0:
				sparkType = SparkType.normal;
				break;
			case 1:
				sparkType = SparkType.big;
				break;
			default:
				sparkType = SparkType.shoryuken;
				break;
		}
	}
	
	public void ThrowRoll(){
		if (side == Side.P1){
			physicsbody.velocity = new Vector2(-2.5f, 0f);
		}
		else{
			physicsbody.velocity = new Vector2(2.5f, 0f);
		}
	}
	
	public void TakeOffVelocity (float x, float y)
    {
        if (!animator.GetBool("isInHitStun") && !animator.GetBool("isKnockedDown"))
        {
            if (transform.localScale.x == 1)
            {
                physicsbody.velocity = new Vector2(x, y);
            }
            else
            {
                physicsbody.velocity = new Vector2(-x, y);
            }
        }
	}	

    public void SlowXVelocity()
    {
        if (!animator.GetBool("isInHitStun") && !animator.GetBool("isKnockedDown"))
        {
            physicsbody.velocity = new Vector2(0f, physicsbody.velocity.y);
        }
    }
	
	public void KnockedDownDust(){
		Vector3 offset = new Vector3(0f, -0.4f, 0f);
		Instantiate(landingDust, transform.position + offset, Quaternion.Euler(new Vector3(-90f, 0f, 0f)));
	}	
	
	public void SuperStartUp(){
		if (gameObject.tag == "Player1"){
			TimeControl.inSuperStartup[0] = true;
		}
		else if (gameObject.tag == "Player2"){
			TimeControl.inSuperStartup[1] = true;
		}
		
		Vector3 offset = Vector3.zero;
		GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Effects";
		GetSuper = 0f;
		if (GetComponent<Akuma>() != null){
            offset = SetOffset(offset, 0.1f, 0.25f);
        }
        else if (GetComponent<Ken>() != null){
            offset = SetOffset(offset, 0.1f, -0.2f);
		}
		else if (GetComponent<FeiLong>() != null){
            offset = SetOffset(offset, 0.5f, 0.25f);
		}
		else if (GetComponent<Balrog>() != null){
            offset = SetOffset(offset, -0.4f, 0f);
		}
        else if (GetComponent<Sagat>() != null){
            offset = SetOffset(offset, -0.2f, 0.2f);
		}
        else if (GetComponent<MBison>() != null){
            offset = SetOffset(offset, 0.35f, 0.2f);
		}
		GameObject newSuperEffect = Instantiate(superEffect, transform.position + offset, Quaternion.identity) as GameObject;
		newSuperEffect.tag = gameObject.tag;
	}
    
	void YouGonnaGetTossed (float dmg, float x, float y){
		SetDamage (dmg);
		if (GetHealth () <= 0) {
			animator.Play ("KOBlendTree", 0);
            TimeControl.slowDown = true;
			TimeControl.slowDownTimer = 100;
		}
		if (side == Side.P2) {
			physicsbody.velocity = new Vector2 (x, y);
		}
		else {
			physicsbody.velocity = new Vector2 (-x, y);
		}
		animator.SetBool ("isAirborne", true);
	}	
	
    Vector3 SetOffset(Vector3 vector, float x, float y){
        if (side == Side.P1){
            vector = new Vector3(x, y, 0f);
        }
        else{
            vector = new Vector3(-x, y, 0f);
        }
        return vector;
    }
    
    public void CreateDemonSparks(){
		float x = Random.Range (-0.25f, 0.25f);
		float y = Random.Range (-0.25f, 0.25f);
		Vector3 offset = new Vector3 (x, y, 0f);
		Instantiate(demonSpark, transform.position + offset, Quaternion.identity);
	}
	
	public void KOSound(){
		AudioSource.PlayClipAtPoint(KOsound, transform.position);		
	}
		
	public void PlayNormalAttackSound(){
		AudioSource.PlayClipAtPoint(normalAttackSound, transform.position);
	}
	
	public void PlayFellSound(){
		AudioSource.PlayClipAtPoint(fellSound, transform.position);
	}	
	
	public void PlayBalrogHeadButtSound(){
		AudioSource.PlayClipAtPoint(balrogHeadButtSound, transform.position);
	}
	
	public void PlayAkumaDemonSound(){
		AudioSource.PlayClipAtPoint(akumaDemonSound, transform.position);
	}
	
	public void LastAkumaDemon(){
		SetDamage(300f);
		if (GetHealth() <= 0f){
			AudioSource.PlayClipAtPoint(akumaDemonKOSound, transform.position);
			TimeControl.demonKO = true;
			
		}
		else{
			AudioSource.PlayClipAtPoint(akumaDemonSound, transform.position);
		}
	}
	
	public void PlaySuperEffectSound(){
		AudioSource.PlayClipAtPoint(superEffectSound, transform.position);
	}	
	
	public void SetBackPressed(bool isBackPressed){
		backPressed = isBackPressed;
	}
		
	public void ResetHasntHit(){
		animator.SetBool("hasntHit", true);
	}
	
	public void SetDamage(float damage){
		health -= damage;
	}
		
	public void ThrowDamage(float damage){
		if (health - damage < 1f){
			health = 1f;
		}
		else{
			SetDamage(damage);
		}
	}
	
	public float GetEnforceHitStun(){
		return enforceHitStun;
	}
	
	public float GetEnforceBlockStun(){
		return enforceBlockStun;
	}
	
	public float GetEnforcePushBack(){
		return enforcePushBack;
	}
	
	public float GetHealth(){
		return health;
	}
	
	public float GetWalkSpeed(){
		return walkSpeed;
	}
	
	public float GetDamage(){
		return enforceDamage;
	}	
	
	public float GetSuperAccumulated(){
		return accumulateSuper;
	}
	
	public float GetRightEdgeDistance(){
		return rightEdgeDistance;
	}
	
	public float GetLeftEdgeDistance(){
		return leftEdgeDistance;
	}
	
	public float GetSuper{
		get { return super; }
		set { super = value; }
	}
	
	public MoveType GetMoveType(){
		return moveType;
	}
	
	public HitType GetHitType(){
		return hitType;
	}
		
	public SparkType GetSparkType(){
		return sparkType;
	}
	
	public bool GetBackPressed(){
		return backPressed;
	}
	
	public bool GetAirborne(){
		return airborne;
	}
	
	public bool GetHitStunned(){
		return isHitStunned;
	}
	
	public bool GetBlockStunned(){
		return isBlockStunned;
	}
	
	public bool GetHasntHit(){
		return didntHit;
	}
			
	public bool GetMidAirRecovering(){
		return midAirRecovering;
	}
	
	public bool GetKnockDown(){
		return isKnockDown;
	}
	
	public bool GetThrown(){
		return isThrown;
	}
	
	public bool GetKOed(){
		return isKO;
	}		
}