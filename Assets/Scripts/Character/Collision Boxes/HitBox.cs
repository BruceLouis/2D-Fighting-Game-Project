using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour {
	public GameObject hitSpark;
	public GameObject shoryukenSpark;
	public GameObject blockSpark;
	
	public AudioClip normalHit;
	public AudioClip bigHit;
	public AudioClip blockHit;
	
	private float timer, distance, leftEdgeDistance, rightEdgeDistance;
	private bool cancellable;	
	
	void OnTriggerStay2D(Collider2D collider){
		HurtBox hurtBox = collider.gameObject.GetComponentInParent<HurtBox>();
		Animator animator = collider.gameObject.GetComponentInParent<Animator>();
		
		if (hurtBox && !hurtBox.GetHurtBoxCollided()){
			
			hurtBox.SetHurtBoxCollided(true);
			cancellable = true;
			TimeControl.slowDown = true;
			Character hitCharacter = gameObject.GetComponentInParent<Character>();	
			Character hurtCharacter = hurtBox.GetComponentInParent<Character>();
			Animator hitCharAnim = gameObject.GetComponentInParent<Animator>();	
			Rigidbody2D hurtPhysicsbody = hurtCharacter.GetComponent<Rigidbody2D>();
			Rigidbody2D hitPhysicsbody = hitCharacter.GetComponent<Rigidbody2D>();
			
			//hit spark effect
			
			//grab the collider components
			BoxCollider2D hurtCollider = collider.gameObject.GetComponent<BoxCollider2D>();
			BoxCollider2D hitCollider = gameObject.GetComponent<BoxCollider2D>();
			
			//find center
			Vector2 hitLocaterStart = new Vector2(hurtCollider.bounds.center.x, hurtCollider.bounds.center.y);			
			Vector2 hitLocaterEnd = new Vector2(hitCollider.bounds.center.x, hitCollider.bounds.center.y);
			
			//cast a raycast line that locates the spark effect
			RaycastHit2D sparkLocation = Physics2D.Raycast(hitLocaterStart, hitLocaterEnd);
			Vector3 sparkEffect = new Vector3(sparkLocation.transform.position.x, sparkLocation.transform.position.y, 0f);
			
			leftEdgeDistance = Mathf.Abs(CameraControl.leftBGEdge) + hurtCharacter.transform.position.x; 
			rightEdgeDistance = CameraControl.rightBGEdge - hurtCharacter.transform.position.x; 
			distance = hurtCharacter.transform.position.x - hitCharacter.transform.position.x;	 
						
			if (hitCharacter.GetHasntHit() == true){
				if (animator.GetBool("isAirborne") == false){
					if (hurtCharacter.GetBackPressed() == true && animator.GetBool("isAttacking") == false
					    && animator.GetBool("isLiftingOff") == false && animator.GetBool("isInHitStun") == false
					    && animator.GetBool("isKnockedDown") == false){
					    
					    	
						if ((hitCharacter.GetMoveType() == Character.MoveType.low && animator.GetBool("isCrouching") == false) 
						    || (hitCharacter.GetMoveType() == Character.MoveType.mid && animator.GetBool("isStanding") == false)){
						    					    					
							InHitStun(timer, hitCharacter, hurtCharacter, animator, hitCharAnim, sparkEffect, hurtPhysicsbody, hitPhysicsbody);						
				    	}
						else{
							InBlockStun(timer, hitCharacter, animator, hurtPhysicsbody, hitPhysicsbody, sparkEffect);		
						}			
					}
					else{
						InHitStun(timer, hitCharacter, hurtCharacter, animator, hitCharAnim, sparkEffect, hurtPhysicsbody, hitPhysicsbody);				
					}	
				}		
				else{				
					if (hitCharacter.GetHitType() != Character.HitType.normal && hitCharacter.GetHitType() != Character.HitType.hurricaneKick && hitCharacter.GetHitType() != Character.HitType.rekka){
						hurtCharacter.SetDamage(hitCharacter.GetDamage()); 
						OtherHitStunProperties(hitCharacter, hurtCharacter, animator, hitCharAnim, sparkEffect, hurtPhysicsbody);
					}
					else{
						InHitStun(timer, hitCharacter, hurtCharacter, animator, hitCharAnim, sparkEffect, hurtPhysicsbody, hitPhysicsbody);	
					}					
				}	
				hitCharAnim.SetBool("hasntHit", false);
			}	
		}
	}
	
	void OnTriggerExit2D(Collider2D collider){
		cancellable = false;
	}
	
	void InBlockStun(float timerArg, Character attacker, Animator anim, Rigidbody2D receiverRigid, Rigidbody2D attackerRigid, Vector3 sparkPlace){
		timerArg = attacker.GetEnforceBlockStun();
		anim.SetBool("isInBlockStun", true);
		AudioSource.PlayClipAtPoint(blockHit, transform.position);
		if (anim.GetBool("isCrouching") == true){
			anim.Play("CrouchBlockStun",0,0f);
		} 
		else{
			anim.Play("StandBlockStun",0,0f);
		}
		anim.SetFloat ("blockStunTimer", timerArg);
		TimeControl.slowDownTimer = 20;
		PushBack(attacker, receiverRigid, attackerRigid);
		Instantiate(blockSpark, sparkPlace, Quaternion.identity);
	}
	
	void InHitStun(float timerArg, Character attacker, Character receiver, Animator anim, Animator attAnim, Vector3 sparkPlace, Rigidbody2D receiverRigid, Rigidbody2D attackerRigid){
		receiver.SetDamage(attacker.GetDamage()); 
		anim.SetBool("isInHitStun", true);
		if (attacker.GetHitType() != Character.HitType.normal && attacker.GetHitType() != Character.HitType.hurricaneKick && attacker.GetHitType() != Character.HitType.rekka){
			timerArg = attacker.GetEnforceHitStun();
			OtherHitStunProperties(attacker, receiver, anim, attAnim, sparkPlace, receiverRigid);
		}
		else{
			if (anim.GetBool("isAirborne") == true){
				timerArg = 15f;
				AudioSource.PlayClipAtPoint(normalHit, transform.position);
				if (receiver.GetHealth () <= 0){				
					GotKOed (receiver, anim, receiverRigid);
				}
				else{
					anim.Play("MidAirHit",0,0f);
					TimeControl.slowDownTimer = 20;
					if (distance > 0){
						receiver.side = Character.Side.P2;
						receiver.SideSwitch();
						receiverRigid.velocity = new Vector2(attacker.GetEnforcePushBack() * 0.15f, 2f);
					}
					else{
						receiver.side = Character.Side.P1;
						receiver.SideSwitch();
						receiverRigid.velocity = new Vector2(-attacker.GetEnforcePushBack() * 0.15f, 2f);						
					}
					TimeControl.slowDownTimer = 20;
				}
				
				Instantiate(hitSpark, sparkPlace, Quaternion.identity);
			}
			else{
			
				timerArg = attacker.GetEnforceHitStun();
				AudioSource.PlayClipAtPoint(normalHit, transform.position);
				if (receiver.GetHealth () <= 0){				
					GotKOed (receiver, anim, receiverRigid);
				}
				else if (anim.GetBool("isCrouching") == true){
					anim.Play("CrouchHit",0,0f);
					TimeControl.slowDownTimer = 20;
				}
				else{
					if (attacker.GetMoveType() == Character.MoveType.low){
						anim.Play("LowHit",0,0f);
					}
					else{
						anim.Play("HighHit",0,0f);
					}
					TimeControl.slowDownTimer = 20;
				}
				PushBack(attacker, receiverRigid, attackerRigid);
				Instantiate(hitSpark, sparkPlace, Quaternion.identity);
			} 
			anim.SetFloat ("hitStunTimer", timerArg);	
		}
	}
	
	void OtherHitStunProperties(Character attacker, Character receiver, Animator anim, Animator attAnim, Vector3 sparkPlace, Rigidbody2D rigid){
		AudioSource.PlayClipAtPoint(bigHit, transform.position);
		Rigidbody2D attPhysicsbody = attacker.GetComponent<Rigidbody2D>();
		if (attacker.GetHitType() == Character.HitType.sweep){
			if (distance > 0){
				receiver.side = Character.Side.P2;
				receiver.SideSwitch();
			}
			else{
				receiver.side = Character.Side.P1;
				receiver.SideSwitch();
			}
			if (receiver.GetHealth () <= 0){				
				GotKOed (receiver, anim, rigid);
			}
			else{
				TimeControl.slowDownTimer = 30;
				anim.Play("Trip",0,0f);
			}
		}
		else if (attacker.GetHitType() == Character.HitType.rekkaKnockdown){
			if (attacker.GetComponent<FeiLong>() != null){			
				if (receiver.GetHealth () <= 0){	
					TimeControl.slowDownTimer = 100;					
					anim.Play("KOBlendTree",0,0f);
				}
				else{		
					TimeControl.slowDownTimer = 30;
					anim.Play("KnockDownBlendTree",0,0f);
				}
				if (distance > 0){
					receiver.side = Character.Side.P2;
					receiver.SideSwitch();
					
					if (attAnim.GetInteger("rekkaPunchType") == 0){
						GotKnockedDown(anim, rigid, 2f, 4f);					
					}
					else if (attAnim.GetInteger("rekkaPunchType") == 1){
						GotKnockedDown(anim, rigid, 2.5f, 4f);	
					}
					else{
						GotKnockedDown(anim, rigid, 3f, 4f);	
					}
				}
				else{
					receiver.side = Character.Side.P1;
					receiver.SideSwitch();
					if (attAnim.GetInteger("rekkaPunchType") == 0){
						GotKnockedDown(anim, rigid, -2f, 4f);					
					}
					else if (attAnim.GetInteger("rekkaPunchType") == 1){
						GotKnockedDown(anim, rigid, -2.5f, 4f);	
					}
					else{
						GotKnockedDown(anim, rigid, -3f, 4f);	
					}
				}
				attPhysicsbody.velocity *= 0.6f;
			}
		}
		else if (attacker.GetHitType() == Character.HitType.shoryuken){
			if (attacker.GetComponent<Ken>() != null){
				if (receiver.GetHealth () <= 0){	
					TimeControl.slowDownTimer = 100;					
					anim.Play("KOBlendTree",0,0f);
				}
				else{		
					TimeControl.slowDownTimer = 30;
					anim.Play("KnockDownBlendTree",0,0f);
				}
				if (distance > 0){
					receiver.side = Character.Side.P2;
					receiver.SideSwitch();
					if (attAnim.GetInteger("shoryukenPunchType") == 0){
						GotShoryukened(anim, rigid, 1f, 3f, 4f);					
					}
					else if (attAnim.GetInteger("shoryukenPunchType") == 1){
						GotShoryukened(anim, rigid, 1.5f, 3f, 4f);	
					}
					else{
						GotShoryukened(anim, rigid, 2f, 4.5f, 5.5f);	
					}
				}
				else{
					receiver.side = Character.Side.P1;
					receiver.SideSwitch();
					if (attAnim.GetInteger("shoryukenPunchType") == 0){
						GotShoryukened(anim, rigid, -1f, 3f, 4f);	
					}
					else if (attAnim.GetInteger("shoryukenPunchType") == 1){
						GotShoryukened(anim, rigid, -1.5f, 3f, 4f);	
					}
					else{
						GotShoryukened(anim, rigid, -2f, 4.5f, 5.5f);	
					}
				}
			}			
			else if (attacker.GetComponent<FeiLong>() != null){
				if (receiver.GetHealth () <= 0){	
					TimeControl.slowDownTimer = 100;					
					anim.Play("KOBlendTree",0,0f);
				}
				else{		
					TimeControl.slowDownTimer = 30;
					anim.Play("KnockDownBlendTree",0,0f);
				}
				if (distance > 0){
					receiver.side = Character.Side.P2;
					receiver.SideSwitch();
					if (attAnim.GetInteger("shienKyakuKickType") == 0){
						GotShoryukened(anim, rigid, 0.25f, 3.5f, 4f);					
					}
					else if (attAnim.GetInteger("shienKyakuKickType") == 1){
						GotShoryukened(anim, rigid, 0.5f, 4f, 4.5f);	
					}
					else{
						GotShoryukened(anim, rigid, 0.75f, 4.5f, 5f);	
					}
				}
				else{
					receiver.side = Character.Side.P1;
					receiver.SideSwitch();
					if (attAnim.GetInteger("shienKyakuKickType") == 0){
						GotShoryukened(anim, rigid, -0.25f, 3.5f, 4f);	
					}
					else if (attAnim.GetInteger("shienKyakuKickType") == 1){
						GotShoryukened(anim, rigid, -0.5f, 4f, 4.5f);	
					}
					else{
						GotShoryukened(anim, rigid, -0.75f, 4.5f, 5f);	
					}
				}
			}
		}
		Instantiate(shoryukenSpark, sparkPlace, Quaternion.identity);
	}
	
	void PushBack(Character attacker, Rigidbody2D receiverRigid, Rigidbody2D attackerRigid){							
		if (attacker.GetHitType() != Character.HitType.hurricaneKick && attacker.GetHitType() != Character.HitType.rekka && attacker.GetHitType() != Character.HitType.rekkaKnockdown){
			if (attacker.side == Character.Side.P1){
				if (rightEdgeDistance > 0.25f){
					// if close to the wall, pushback enforced on attacking character instead
					receiverRigid.velocity = new Vector2(attacker.GetEnforcePushBack() * 0.15f, 0f);		
				}
				else{
					attackerRigid.velocity = new Vector2(-attacker.GetEnforcePushBack() * 0.15f, 0f);		
				}
			}
			else{
				if (leftEdgeDistance > 0.25f){
					// if close to the wall, pushback enforced on attacking character instead
					receiverRigid.velocity = new Vector2(-attacker.GetEnforcePushBack() * 0.15f, 0f);		
				}
				else{
					attackerRigid.velocity = new Vector2(attacker.GetEnforcePushBack() * 0.15f, 0f);		
				}
			}
			
		}
		else{
			if (attacker.side == Character.Side.P1){
				receiverRigid.velocity = new Vector2(attacker.GetEnforcePushBack() * 0.15f, 0f);		
			}
			else{
				receiverRigid.velocity = new Vector2(-attacker.GetEnforcePushBack() * 0.15f, 0f);		
			}
			if (attacker.GetHitType() == Character.HitType.rekka || attacker.GetHitType() == Character.HitType.rekkaKnockdown){
				attackerRigid.velocity *= 0.6f;
			}
		}
	}
		
	void GotKOed (Character receiver, Animator anim, Rigidbody2D rigid){
		TimeControl.slowDownTimer = 100;	
		AudioSource.PlayClipAtPoint(bigHit, transform.position);
		anim.Play ("KOBlendTree", 0, 0f);
		if (receiver.side == Character.Side.P1) {
			rigid.velocity = new Vector2 (-2f, 4f);
		}
		else {
			rigid.velocity = new Vector2 (2f, 4f);
		}
	}

	void GotShoryukened (Animator anim, Rigidbody2D rigid, float x, float airborneY, float groundedY){
		if (anim.GetBool ("isAirborne") == false) {
			rigid.velocity = new Vector2 (x, groundedY);
		}
		else {
			rigid.velocity = new Vector2 (x, airborneY);
		}
	}
	
	void GotKnockedDown (Animator anim, Rigidbody2D rigid, float x, float y){
		rigid.velocity = new Vector2 (x, y);
	}
	
	public bool GetCancellabe(){
		return cancellable;
	}
}
