using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
	
	public float walkSpeed;
	public float health;
	
	public enum Side {P1, P2};
	public Side side;
	
	public enum MoveType {low, mid, high};
	public MoveType moveType;
	
	public enum HitType {normal, sweep, knockdown, shoryuken, hurricaneKick};
	public HitType hitType;
	
	public GameObject projectile;
	
	public AudioClip hadoukenSound, shoryukenSound,hurricaneKickSound, KOsound; 
	public AudioClip normalAttackSound, hadoukenCreatedSound, fellSound, flameSound;
	
	private Rigidbody2D physicsbody;
	private Animator animator;
	private HitBox hitBox;	 
	
	private float 	enforceHitStun, enforceBlockStun, enforcePushBack, enforceDamage, leftEdgeDistance, rightEdgeDistance;
	
	private bool 	backPressed, airborne, midAirRecovering, invincible, didntHit,
					hurricaneActive, isHitStunned,  isBlockStunned, isKnockDown, isKO;
	
	private int 	shoryukenType, hadoukenType, hadoukenOwner;
	
	
	void Start(){
		hitBox = GetComponentInChildren<HitBox>();
		physicsbody = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		animator.SetBool("isStanding", true);
	}
	// Update is called once per frame
	void Update(){	
		airborne = animator.GetBool("isAirborne");
		midAirRecovering = animator.GetBool("isMidAirRecovering");
		hurricaneActive = animator.GetBool("hurricaneKickActive");
		isHitStunned = animator.GetBool("isInHitStun");
		isBlockStunned = animator.GetBool("isInBlockStun");
		isKnockDown = animator.GetBool("isKnockedDown");
		didntHit = animator.GetBool("hasntHit");
		isKO = animator.GetBool("isKOed");
		animator.SetBool("isCancellable", hitBox.GetCancellabe());
		animator.SetFloat("yVelocity", physicsbody.velocity.y);	
				
		leftEdgeDistance = Mathf.Abs(CameraControl.leftBGEdge) + transform.position.x; 
		rightEdgeDistance = CameraControl.rightBGEdge - transform.position.x; 
		
		if (hurricaneActive){
			physicsbody.isKinematic = true;
			if (rightEdgeDistance < 0.5f && side == Side.P1){ 			
				physicsbody.velocity = new Vector2(0f, 0f);
			}
			if (leftEdgeDistance < 0.5f && side == Side.P2){	
				physicsbody.velocity = new Vector2(0f, 0f);
			}
		}
		else{
			physicsbody.isKinematic = false;
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
				animator.Play("KenStandJab",0);
				MoveProperties(15f, 15f, 7.5f, 20f, 2, 0);
			}
			else if (animator.GetBool("isCrouching")){
				animator.Play("KenCrouchJab",0);
				MoveProperties(20f, 15f, 7.5f, 20f, 2, 0);
			}
			else{
				if (physicsbody.velocity.x != 0){
					animator.Play("KenJumpJab",0);
				}
				else{
					animator.Play("KenJumpNeutralJab",0);
				}		
				MoveProperties(15f, 7f, 7.5f, 20f, 1, 0);	
			}
		}
	}
	
	public void CharacterStrong(){ 
		if (animator.GetBool("isLiftingOff") == false){
			AudioSource.PlayClipAtPoint(normalAttackSound, transform.position);
			if (animator.GetBool("isStanding")){
				animator.Play("KenStandStrong",0);
				MoveProperties(30f, 22.5f, 10f, 40f, 2, 0);
			}
			else if (animator.GetBool("isCrouching")){
				animator.Play("KenCrouchStrong",0);
				MoveProperties(30f, 25f, 10f, 40f, 2, 0);
				}
			else{
				if (physicsbody.velocity.x != 0){
					animator.Play("KenJumpStrong",0);
				}
				else{
					animator.Play("KenJumpNeutralStrong",0);
				}			
				MoveProperties(15f, 8.5f, 10f, 40f, 1, 0);
			}	
		}
	}
	
	public void CharacterFierce(){
		if (animator.GetBool("isLiftingOff") == false){
			AudioSource.PlayClipAtPoint(normalAttackSound, transform.position);
			if (animator.GetBool("isStanding")){
				animator.Play("KenStandFierce",0);
				MoveProperties(40f, 20f, 12f, 55f, 2, 0);
			}
			else if (animator.GetBool("isCrouching")){
				animator.Play("KenCrouchFierce",0);
				MoveProperties(40f, 20f, 12f, 55f, 2, 0);
			}
			else{
				if (physicsbody.velocity.x != 0){
					animator.Play("KenJumpFierce",0);
				}
				else{
					animator.Play("KenJumpNeutralFierce",0);
				}			
				MoveProperties(40f, 8.5f, 6f, 55f, 1, 0);
			}
		}
	}
	
	public void CharacterShort(){	
		if (animator.GetBool("isLiftingOff") == false){	
			AudioSource.PlayClipAtPoint(normalAttackSound, transform.position);
			if (animator.GetBool("isStanding")){
				animator.Play("KenStandShort",0);
				MoveProperties(15f, 15f, 7.5f, 20f, 0, 0);
			}
			else if (animator.GetBool("isCrouching")){
				animator.Play("KenCrouchShort",0);
				MoveProperties(20f, 17.5f, 7.5f, 20f, 0, 0);
			}			
			else{
				if (physicsbody.velocity.x != 0){
					animator.Play("KenJumpShort",0);
				}
				else{
					animator.Play("KenJumpNeutralShort",0);
				}			
				MoveProperties(20f, 7f, 7.5f, 20f, 1, 0);
			}
		}
	}
	
	public void CharacterForward(){
		if (animator.GetBool("isLiftingOff") == false){	
			AudioSource.PlayClipAtPoint(normalAttackSound, transform.position);
			if (animator.GetBool("isStanding")){
				animator.Play("KenStandForward",0);
				MoveProperties(30f, 20f, 10f, 40f, 2, 0);
			}
			else if (animator.GetBool("isCrouching")){
				animator.Play("KenCrouchForward",0);
				MoveProperties(32.5f, 25f, 10f, 40f, 0, 0);
			}
			else{
				if (physicsbody.velocity.x != 0){
					animator.Play("KenJumpForwardAttack",0);
				}
				else{
					animator.Play("KenJumpNeutralForward",0);
				}			
				MoveProperties(35f, 8.5f, 10f, 40f, 1, 0);
			}
		}
	}
	
	public void CharacterRoundhouse(){
		if (animator.GetBool("isLiftingOff") == false){		
			AudioSource.PlayClipAtPoint(normalAttackSound, transform.position);	
			if (animator.GetBool("isStanding")){
				animator.Play("KenStandRoundhouse",0);
				MoveProperties(40f, 20f, 12f, 55f, 2, 0);
			}
			else if (animator.GetBool("isCrouching")){
				animator.Play("KenCrouchRoundhouse",0);
				MoveProperties(40f, 20f, 12f, 60f, 0, 1);
			}
			else{
				if (physicsbody.velocity.x != 0){
					animator.Play("KenJumpRoundhouse",0);
				}
				else{
					animator.Play("KenJumpNeutralRoundhouse",0);
				}			
				MoveProperties(40f, 8.5f, 12f, 55f, 1, 0);
			}	
		}
	}
	
	public void CharacterJump(bool forward, bool backward){
		if (forward == true & backward == false){
			animator.Play("KenJumpForward",0);
		}
		else if (forward == false && backward == false){			
			animator.Play("KenJumpNeutral",0);	
		}
		else{			
			animator.Play("KenJumpBackward",0);	
		}		
	}
	
	public void CharacterNeutralLiftOff(){
		if (animator.GetBool("isAirborne") == false && animator.GetBool("isLiftingOff") == true){
			physicsbody.velocity = new Vector2(0f, 4.5f);
			animator.SetBool("isAirborne", true);
			animator.SetBool("isStanding", false);
			animator.SetBool("isLiftingOff", false);
		}
	}
	
	public void CharacterForwardLiftOff(){
		if (animator.GetBool("isAirborne") == false){
			if (side == Side.P1){
				physicsbody.velocity = new Vector2(2f, 4.5f);
			}
			else{
				physicsbody.velocity = new Vector2(-2f, 4.5f);
			}
			animator.SetBool("isAirborne", true);
			animator.SetBool("isStanding", false);
			animator.SetBool("isLiftingOff", false);
		}
	}
	
	public void CharacterBackwardLiftOff(){
		if (animator.GetBool("isAirborne") == false && animator.GetBool("isLiftingOff") == true){
			if (side == Side.P2){
				physicsbody.velocity = new Vector2(2f, 4.5f);
			}
			else{
				physicsbody.velocity = new Vector2(-2f, 4.5f);
			}
			animator.SetBool("isAirborne", true);
			animator.SetBool("isStanding", false);
			animator.SetBool("isLiftingOff", false);
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
	
	public void Tossed(){
		SetDamage(60f);
		if (GetHealth() <= 0){
			animator.Play("KenKOBlendTree",0);
		}
		if (side == Side.P2){
			physicsbody.velocity = new Vector2(-3f, 4f);
		}
		else{
			physicsbody.velocity = new Vector2(3f, 4f);
		}
		animator.SetBool("isAirborne", true);
	}
	
	
	public void MoveProperties(float hitstun, float blockstun, float pushback, float damage, int moveTypeInt, int hitTypeInt){
		enforceHitStun = hitstun;		
		enforceBlockStun = blockstun;
		enforcePushBack = pushback;
		enforceDamage = damage;
		
		if (moveTypeInt == 0){
			moveType = MoveType.low;
		}
		else if (moveTypeInt == 1){
			moveType = MoveType.mid;
		}
		else{
			moveType = MoveType.high;
		}
		
		if (hitTypeInt == 0){
			hitType = HitType.normal;
		}
		else if (hitTypeInt == 1){
			hitType = HitType.sweep;
		}
		else if (hitTypeInt == 2){
			hitType = HitType.knockdown;
		}
		else if (hitTypeInt == 3){
			hitType = HitType.shoryuken;
		}
		else{
			hitType = HitType.hurricaneKick;
		}
	}
	
	public void KenHadouken(int punchType, int projectileOwner){
		if (animator.GetBool("isLiftingOff") == false){	
			animator.Play("KenHadouken",0);			
			hadoukenType = punchType;
			hadoukenOwner = projectileOwner;
		}
	}
	
	public void KenShoryuken(){
		shoryukenType = animator.GetInteger("shoryukenPunchType");
		if (animator.GetBool("isLiftingOff") == false){	
			if (animator.GetInteger("shoryukenPunchType") == 0){
				MoveProperties(30f, 20f, 15f, 60f, 2, 3);
			}
			else if (animator.GetInteger("shoryukenPunchType") == 1){
				MoveProperties(40f, 25f, 20f, 65f, 2, 3);
			}
			else{
				MoveProperties(60f, 25f, 20f, 70f, 2, 3);
			}
		}
	}
	
	public void KenHurricaneKick(){		
		if (animator.GetBool("isLiftingOff") == false){	
			transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, 0.5f, transform.position.z), 15f * Time.deltaTime);
			if (side == Side.P1){				
				physicsbody.velocity = new Vector2 (1f, 0f);
			}
			else{
				physicsbody.velocity = new Vector2 (-1f, 0f);
			}			
			MoveProperties(40f, 20f, 10f, 15f, 2, 4);
			animator.SetBool("isAirborne", true);
		}
	}
	
	public void KenRoll(){
		if (animator.GetBool("isLiftingOff") == false){	
			animator.SetBool("isInvincible", true);
			if (animator.GetInteger("rollKickType") == 0){
				if (side == Side.P1){
					physicsbody.velocity = new Vector2(2f, 0f);
				}
				else{
					physicsbody.velocity = new Vector2(-2f, 0f);
				}				
			}
			else if (animator.GetInteger("rollKickType") == 1){
				if (side == Side.P1){
					physicsbody.velocity = new Vector2(2.5f, 0f);
				}
				else{
					physicsbody.velocity = new Vector2(-2.5f, 0f);
				}				
			}
			else{
				if (side == Side.P1){
					physicsbody.velocity = new Vector2(3f, 0f);
				}
				else{
					physicsbody.velocity = new Vector2(-3f, 0f);
				}				
			}
		}
	}
	
	public void KenRollSetFalse(){
		animator.SetBool("isInvincible", false);
	}
	
	public void KenHurricaneLanding(){
		animator.SetBool("hurricaneKickActive", false);
	}
	
	public void HadoukenRelease(){
		Vector3 offset = new Vector3(0.75f, 0f, 0f);
		GameObject hadouken = Instantiate(projectile);
		Rigidbody2D rigidbody = hadouken.GetComponent<Rigidbody2D>();
		SpriteRenderer hadoukenSprite = hadouken.GetComponentInChildren<SpriteRenderer>();		
		AudioSource.PlayClipAtPoint(hadoukenCreatedSound, transform.position);
		if (hadoukenOwner == 1){
			hadouken.gameObject.layer = LayerMask.NameToLayer("ProjectileP1");
		}
		else{
			hadouken.gameObject.layer = LayerMask.NameToLayer("ProjectileP2");
		} 
		if (side == Side.P1){
			hadouken.transform.position = transform.position + offset;
		}
		else{
			hadouken.transform.position = transform.position - offset;
		}
		if (hadoukenType == 0){
			if (side == Side.P1){
				rigidbody.velocity = new Vector2(3f, 0f);
			}
			else {
				rigidbody.velocity = new Vector2(-3f, 0f);	
				hadoukenSprite.flipX = true;
			}
		}
		else if (hadoukenType == 1){
			if (side == Side.P1){
				rigidbody.velocity = new Vector2(3.5f, 0f);
			}
			else {
				rigidbody.velocity = new Vector2(-3.5f, 0f);	
				hadoukenSprite.flipX = true;
			}
		}
		else{
			if (side == Side.P1){
				rigidbody.velocity = new Vector2(4f, 0f);
			}
			else {
				rigidbody.velocity = new Vector2(-4f, 0f);	
				hadoukenSprite.flipX = true;
			}
		}
			
	}
	
	public void ShoryukenLiftOff(){		
		if (animator.GetInteger("shoryukenPunchType") == 0){
			if (side == Side.P1){
				physicsbody.velocity = new Vector2(1f, 3f);
			}
			else{
				physicsbody.velocity = new Vector2(-1f, 3f);
			}
		}
		else if (animator.GetInteger("shoryukenPunchType") == 1){
			if (side == Side.P1){
				physicsbody.velocity = new Vector2(1.5f, 4f);
			}
			else{
				physicsbody.velocity = new Vector2(-1.5f, 4f);
			}
		}
		else{
			if (side == Side.P1){
				physicsbody.velocity = new Vector2(2f, 5f);
			}
			else{
				physicsbody.velocity = new Vector2(-2f, 5f);
			}
		}
		AudioSource.PlayClipAtPoint(normalAttackSound,transform.position);
		animator.SetBool("isAirborne", true);
		animator.SetBool("isInvincible", false);
	}
	
	public void ThrowRoll(){
		if (side == Side.P1){
			physicsbody.velocity = new Vector2(-2.5f, 0f);
		}
		else{
			physicsbody.velocity = new Vector2(2.5f, 0f);
		}
	}
	
	public void KO(){
		if (side == Side.P1){
			physicsbody.velocity = new Vector2(-2f, 4f);
		}
		else{
			physicsbody.velocity = new Vector2(2f, 4f);
		}
		animator.SetBool("isAirborne", true);
		animator.Play ("KenKOBlendTree",0);
		AudioSource.PlayClipAtPoint(KOsound, transform.position);		
	}
	
	public void PlayHadoukenSound(){
		AudioSource.PlayClipAtPoint(hadoukenSound, transform.position);
	}
	
	public void PlayShoryukenSound(){
		AudioSource.PlayClipAtPoint(shoryukenSound, transform.position);
	}
	
	public void PlayFlamesSound(){
		AudioSource.PlayClipAtPoint(flameSound, transform.position);
	}
	
	public void PlayNormalAttackSound(){
		AudioSource.PlayClipAtPoint(normalAttackSound, transform.position);
	}
	
	public void PlayHurricaneKickSound(){
		AudioSource.PlayClipAtPoint(hurricaneKickSound, transform.position);
	}
	
	public void PlayFellSound(){
		AudioSource.PlayClipAtPoint(fellSound, transform.position);
	}	
	
	public void WinPoseForOtherGuySoon(){
		TimeControl.victoryPose = true;
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
	
	public int GetShoryukenType(){
		return shoryukenType;
	}
		
	void OnCollisionEnter2D(Collision2D collision){
		if (collision.gameObject.GetComponent<Ground>() && animator.GetFloat("yVelocity") < 0){
			animator.SetBool("isAirborne", false);
			animator.SetBool("shoryukenActive", false);
		}
		if (animator.GetBool("isAirborne") && collision.gameObject.GetComponent<Character>()){			
			physicsbody.velocity = new Vector2 (0f, physicsbody.velocity.y);		
		}
	}
		
}