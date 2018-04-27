using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
	
	public float walkSpeed;
	public float health;
	public GameObject landingDust;
	
	public enum Side {P1, P2};
	public Side side;
	
	public enum MoveType {low, mid, high};
	public MoveType moveType;
	
	public enum HitType {normal, sweep, rekkaKnockdown, shoryuken, hurricaneKick, rekka, dashLow, akumaHurricaneKick};
	public HitType hitType;
	
	public AudioClip normalAttackSound, KOsound, fellSound, flameSound, balrogHeadButtSound;
	
	private Rigidbody2D physicsbody;
	private Animator animator;
	private HitBox hitBox;	 
	
	private float 	enforceHitStun, enforceBlockStun, enforcePushBack, enforceDamage, leftEdgeDistance, rightEdgeDistance;
	
	private bool 	backPressed, airborne, midAirRecovering, didntHit, 
					isHitStunned, isBlockStunned, isKnockDown, isKO;
		
		
	void Start(){
		hitBox = GetComponentInChildren<HitBox>();
		physicsbody = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		animator.SetBool("isStanding", true);
		foreach(Transform child in transform){
			child.gameObject.tag = transform.gameObject.tag;
		}
	}
	
	// Specifically for the gap between being airborne and lifting off	
	void FixedUpdate(){
		if (physicsbody.velocity.y != 0f && animator.GetBool("isLiftingOff")){
			animator.SetBool("isAirborne", true);
			animator.SetBool("isStanding", false);
			animator.SetBool("isLiftingOff", false);
		}
	}
	
	// Update is called once per frame
	void Update(){	
		airborne = animator.GetBool("isAirborne");
		midAirRecovering = animator.GetBool("isMidAirRecovering");
		isHitStunned = animator.GetBool("isInHitStun");
		isBlockStunned = animator.GetBool("isInBlockStun");
		isKnockDown = animator.GetBool("isKnockedDown");
		didntHit = animator.GetBool("hasntHit");
		isKO = animator.GetBool("isKOed");
		animator.SetBool("isCancellable", hitBox.GetCancellabe());
		animator.SetFloat("yVelocity", physicsbody.velocity.y);	
						
		leftEdgeDistance = Mathf.Abs(CameraControl.leftBGEdge) + transform.position.x; 
		rightEdgeDistance = CameraControl.rightBGEdge - transform.position.x; 
		
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
				MoveProperties(15f, 15f, 7.5f, 20f, 2, 0);
			}
			else if (animator.GetBool("isCrouching")){
				animator.Play("CrouchJab",0);
				MoveProperties(20f, 15f, 7.5f, 20f, 2, 0);
			}
			else if (animator.GetBool("isAirborne")){
				if (physicsbody.velocity.x != 0){
					animator.Play("JumpJab",0);
				}
				else{
					animator.Play("JumpNeutralJab",0);
				}		
				MoveProperties(15f, 7f, 7.5f, 20f, 1, 0);	
			}
		}
	}
	
	public void CharacterStrong(){ 
		if (animator.GetBool("isLiftingOff") == false){
			AudioSource.PlayClipAtPoint(normalAttackSound, transform.position);
			if (animator.GetBool("isStanding")){
				animator.Play("StandStrong",0);
				MoveProperties(30f, 22.5f, 10f, 40f, 2, 0);
			}
			else if (animator.GetBool("isCrouching")){
				animator.Play("CrouchStrong",0);
				MoveProperties(30f, 25f, 10f, 40f, 2, 0);
			}
			else if (animator.GetBool("isAirborne")){
				if (physicsbody.velocity.x != 0){
					animator.Play("JumpStrong",0);
				}
				else{
					animator.Play("JumpNeutralStrong",0);
				}			
				MoveProperties(15f, 8.5f, 10f, 40f, 1, 0);
			}	
		}
	}
	
	public void CharacterFierce(){
		if (animator.GetBool("isLiftingOff") == false){
			AudioSource.PlayClipAtPoint(normalAttackSound, transform.position);
			if (animator.GetBool("isStanding")){
				animator.Play("StandFierce",0);
				MoveProperties(40f, 20f, 12f, 55f, 2, 0);
			}
			else if (animator.GetBool("isCrouching")){
				animator.Play("CrouchFierce",0);
				MoveProperties(40f, 20f, 12f, 55f, 2, 0);
			}
			else if (animator.GetBool("isAirborne")){
				if (physicsbody.velocity.x != 0){
					animator.Play("JumpFierce",0);
				}
				else{
					animator.Play("JumpNeutralFierce",0);
				}			
				MoveProperties(40f, 8.5f, 6f, 55f, 1, 0);
			}
		}
	}
	
	public void CharacterShort(){	
		if (animator.GetBool("isLiftingOff") == false){	
			AudioSource.PlayClipAtPoint(normalAttackSound, transform.position);
			if (animator.GetBool("isStanding")){
				animator.Play("StandShort",0);
				if (GetComponent<Balrog>() != null){
					MoveProperties(15f, 15f, 7.5f, 20f, 2, 0);
				}
				else{					
					MoveProperties(15f, 15f, 7.5f, 20f, 0, 0);
				}
			}
			else if (animator.GetBool("isCrouching")){
				animator.Play("CrouchShort",0);
				MoveProperties(20f, 17.5f, 7.5f, 20f, 0, 0);
			}			
			else if (animator.GetBool("isAirborne")){
				if (physicsbody.velocity.x != 0){
					animator.Play("JumpShort",0);
				}
				else{
					animator.Play("JumpNeutralShort",0);
				}			
				MoveProperties(20f, 7f, 7.5f, 20f, 1, 0);
			}
		}
	}
	
	public void CharacterForward(){
		if (animator.GetBool("isLiftingOff") == false){	
			AudioSource.PlayClipAtPoint(normalAttackSound, transform.position);
			if (animator.GetBool("isStanding")){
				animator.Play("StandForward",0);
				MoveProperties(30f, 20f, 10f, 40f, 2, 0);
			}
			else if (animator.GetBool("isCrouching")){
				animator.Play("CrouchForward",0);
				MoveProperties(32.5f, 25f, 10f, 40f, 0, 0);
			}
			else if (animator.GetBool("isAirborne")){
				if (physicsbody.velocity.x != 0){
					animator.Play("JumpForwardAttack",0);
				}
				else{
					animator.Play("JumpNeutralForward",0);
				}			
				MoveProperties(35f, 8.5f, 10f, 40f, 1, 0);
			}
		}
	}
	
	public void CharacterRoundhouse(){
		if (animator.GetBool("isLiftingOff") == false){		
			AudioSource.PlayClipAtPoint(normalAttackSound, transform.position);	
			if (animator.GetBool("isStanding")){
				animator.Play("StandRoundhouse",0);
				MoveProperties(40f, 20f, 12f, 55f, 2, 0);
			}
			else if (animator.GetBool("isCrouching")){
				animator.Play("CrouchRoundhouse",0);
				MoveProperties(40f, 20f, 12f, 60f, 0, 1);
			}
			else if (animator.GetBool("isAirborne")){
				if (physicsbody.velocity.x != 0){
					animator.Play("JumpRoundhouse",0);
				}
				else{
					animator.Play("JumpNeutralRoundhouse",0);
				}			
				MoveProperties(40f, 8.5f, 12f, 55f, 1, 0);
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
		if (animator.GetBool("isAirborne") == false && animator.GetBool("isInHitStun") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isInBlockStun") == false
	    	&& animator.GetBool("isThrown") == false && animator.GetBool("isMidAirRecovering") == false
		   	&& animator.GetBool("isLiftingOff") == true){			
			if (side == Side.P1){
				physicsbody.velocity = new Vector2(x, 4.5f);
			}
			else{
				physicsbody.velocity = new Vector2(-x, 4.5f);
			}
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
	
	public void MidAirRecovery(){
		if (side == Side.P2){
			physicsbody.velocity = new Vector2(1.5f, 2f);
		}
		else{
			physicsbody.velocity = new Vector2(-1.5f, 2f);
		}
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
		SetDamage(60f);
		if (GetHealth() <= 0){
			animator.Play("KOBlendTree",0);
			TimeControl.slowDownTimer = 100;	
		}
		if (side == Side.P2){
			physicsbody.velocity = new Vector2(-3f, 4f);
		}
		else{
			physicsbody.velocity = new Vector2(3f, 4f);
		}
		animator.SetBool("isAirborne", true);
	}
	
	public void TossedByFeiLong(){
		SetDamage(60f);
		if (GetHealth() <= 0){
			animator.Play("KOBlendTree",0);
			TimeControl.slowDownTimer = 100;	
		}
		if (side == Side.P2){
			physicsbody.velocity = new Vector2(3f, 4f);
		}
		else{
			physicsbody.velocity = new Vector2(-3f, 4f);
		}
		animator.SetBool("isAirborne", true);
	}
		
	public void HeadButtedByBalrog(){
		SetDamage(20f);
		if (GetHealth() <= 0){
			animator.Play("KOBlendTree",0);
			TimeControl.slowDownTimer = 100;	
		}
		if (side == Side.P2){
			physicsbody.velocity = new Vector2(2f, 4f);
		}
		else{
			physicsbody.velocity = new Vector2(-2f, 4f);
		}
		animator.SetBool("isAirborne", true);
	}
	
	public void MoveProperties(float hitstun, float blockstun, float pushback, float damage, int moveTypeInt, int hitTypeInt){
		enforceHitStun = hitstun;		
		enforceBlockStun = blockstun;
		enforcePushBack = pushback;
		enforceDamage = damage;
		
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
				hitType = HitType.rekkaKnockdown;
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
			default:
				hitType = HitType.akumaHurricaneKick;
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
	
	//will eventually replace all character specific takeoffvelocity methods
	public void TakeOffVelocity (float x, float y){
		if (transform.localScale.x == 1) {
			physicsbody.velocity = new Vector2 (x, y);
		}
		else {
			physicsbody.velocity = new Vector2 (-x, y);
		}
	}	
	
	public void KnockedDownDust(){
		Vector3 offset = new Vector3(0f, -0.4f, 0f);
		Instantiate(landingDust, transform.position + offset, Quaternion.Euler(new Vector3(-90f, 0f, 0f)));
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
	
	public void SetBackPressed(bool isBackPressed){
		backPressed = isBackPressed;
	}
	
	public void ResetHasntHit(){
		animator.SetBool("hasntHit", true);
	}
	
	public void SetDamage(float damage){
		health -= damage;
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
	
	public float GetDamage(){
		return enforceDamage;
	}	
	
	public float GetRightEdgeDistance(){
		return rightEdgeDistance;
	}
	
	public float GetLeftEdgeDistance(){
		return leftEdgeDistance;
	}
	
	public MoveType GetMoveType(){
		return moveType;
	}
	
	public HitType GetHitType(){
		return hitType;
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
	
	public bool GetKOed(){
		return isKO;
	}
			
	void OnCollisionEnter2D(Collision2D collision){
		if (collision.gameObject.GetComponent<Ground>()	&& animator.GetBool("isMidAirHit") == false && animator.GetFloat("yVelocity") <= 0){
			animator.SetBool("isAirborne", false);
			if (gameObject.GetComponent<Ken>() != null || gameObject.GetComponent<Akuma>() != null){
				animator.SetBool("shoryukenActive", false);
			}
			else if (gameObject.GetComponent<FeiLong>() != null){
				if (animator.GetBool("rekkaKunActive") == true){
					physicsbody.velocity = new Vector2(0f, physicsbody.velocity.y);
				}
				animator.SetBool("shienKyakuActive", false);
				animator.SetBool("rekkaKunActive", false);
			}				
			else if (gameObject.GetComponent<Balrog>() != null){	
				animator.SetBool("isHeadButting", false);
			}				
		}
	}
		
}