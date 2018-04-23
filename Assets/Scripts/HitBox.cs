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

	void OnTriggerEnter2D(Collider2D collider){
		HurtBox hurtBox = collider.gameObject.GetComponentInChildren<HurtBox>();		
		Animator animator = collider.gameObject.GetComponentInParent<Animator>();	
		
		if (hurtBox){
			
			cancellable = true;
			TimeControl.slowDown = true;
			Character hitCharacter = gameObject.GetComponentInParent<Character>();	
			Character hurtCharacter = hurtBox.GetComponentInParent<Character>();
			Animator hitCharAnim = gameObject.GetComponentInParent<Animator>();	
			Rigidbody2D hurtPhysicsbody = hurtCharacter.GetComponent<Rigidbody2D>();
			Rigidbody2D hitPhysicsbody = hitCharacter.GetComponent<Rigidbody2D>();
			Vector2 hitLocaterStart = new Vector2(hurtBox.transform.position.x, hurtBox.transform.position.y);
			Vector2 hitLocaterEnd = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
			RaycastHit2D sparkLocation = Physics2D.Linecast(hitLocaterStart, hitLocaterEnd);
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
						    					    					
							InHitStun(timer, hitCharacter, hurtCharacter, animator, hitCharAnim, sparkEffect, hurtPhysicsbody);						
				    	}
						else{
							InBlockStun(timer, hitCharacter, animator, sparkEffect);
						}					
	
					}
					else{
						InHitStun(timer, hitCharacter, hurtCharacter, animator, hitCharAnim, sparkEffect, hurtPhysicsbody);				
					}
					if (hurtCharacter.GetHealth () <= 0){				
						animator.Play("KOBlendTree",0);
						if (hurtCharacter.side == Character.Side.P1){
							hurtPhysicsbody.velocity = new Vector2(-2f, 4f);
						}
						else{
							hurtPhysicsbody.velocity = new Vector2(2f, 4f);
						}
					}
					else{
						if (hitCharacter.GetHitType() == Character.HitType.normal){
							if (hitCharacter.side == Character.Side.P1){
								if (rightEdgeDistance > 0.25f){
									// if close to the wall, pushback enforced on attacking character instead
									hurtPhysicsbody.velocity = new Vector2(hitCharacter.GetEnforcePushBack() * 0.15f, 0f);		
								}
								else{
									hitPhysicsbody.velocity = new Vector2(-hitCharacter.GetEnforcePushBack() * 0.15f, 0f);		
								}
							}
							else{
								if (leftEdgeDistance > 0.25f){
									// if close to the wall, pushback enforced on attacking character instead
									hurtPhysicsbody.velocity = new Vector2(-hitCharacter.GetEnforcePushBack() * 0.15f, 0f);		
								}
								else{
									hitPhysicsbody.velocity = new Vector2(hitCharacter.GetEnforcePushBack() * 0.15f, 0f);		
								}
							}
						}
						else if (hitCharacter.GetHitType() == Character.HitType.hurricaneKick){
							if (hitCharacter.side == Character.Side.P1){
								hurtPhysicsbody.velocity = new Vector2(hitCharacter.GetEnforcePushBack() * 0.15f, 0f);		
							}
							else{
								hurtPhysicsbody.velocity = new Vector2(-hitCharacter.GetEnforcePushBack() * 0.15f, 0f);		
							}
						}
					}	
				}		
				else{				
					if (hitCharacter.GetHitType() != Character.HitType.normal && hitCharacter.GetHitType() != Character.HitType.hurricaneKick){
						hurtCharacter.SetDamage(hitCharacter.GetDamage()); 
						OtherHitStunProperties(hitCharacter, hurtCharacter, animator, hitCharAnim, sparkEffect, hurtPhysicsbody);
					}
					else{
						InHitStun(timer, hitCharacter, hurtCharacter, animator, hitCharAnim, sparkEffect, hurtPhysicsbody);	
					}					
				}	
				hitCharAnim.SetBool("hasntHit", false);
			}	
		}
	}
	
	void OnTriggerExit2D(Collider2D collider){
		cancellable = false;
	}
	
	void InBlockStun(float timerArg, Character attacker, Animator anim, Vector3 sparkPlace){
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
		Instantiate(blockSpark, sparkPlace, Quaternion.identity);
	}
	
	void InHitStun(float timerArg, Character attacker, Character receiver, Animator anim, Animator attAnim, Vector3 sparkPlace, Rigidbody2D rigid){
		receiver.SetDamage(attacker.GetDamage()); 
		anim.SetBool("isInHitStun", true);
		if (attacker.GetHitType() != Character.HitType.normal && attacker.GetHitType() != Character.HitType.hurricaneKick){
			timerArg = attacker.GetEnforceHitStun();
			OtherHitStunProperties(attacker, receiver, anim, attAnim, sparkPlace, rigid);
		}
		else if (anim.GetBool("isAirborne") == true && attacker.GetHitType() != Character.HitType.sweep  && attacker.GetHitType() != Character.HitType.shoryuken){
			
			timerArg = 15f;
			AudioSource.PlayClipAtPoint(normalHit, transform.position);
			if (receiver.GetHealth () <= 0){				
				GotKOed (receiver, anim, rigid);
			}
			else{
				anim.Play("MidAirHit",0,0f);
				TimeControl.slowDownTimer = 20;
				if (distance > 0){
					receiver.side = Character.Side.P2;
					receiver.SideSwitch();
					rigid.velocity = new Vector2(attacker.GetEnforcePushBack() * 0.15f, 2f);
				}
				else{
					receiver.side = Character.Side.P1;
					receiver.SideSwitch();
					rigid.velocity = new Vector2(-attacker.GetEnforcePushBack() * 0.15f, 2f);						
				}
				TimeControl.slowDownTimer = 20;
			}
			
			Instantiate(hitSpark, sparkPlace, Quaternion.identity);
		}
		else{
		
			timerArg = attacker.GetEnforceHitStun();
			AudioSource.PlayClipAtPoint(normalHit, transform.position);
			if (receiver.GetHealth () <= 0){				
				GotKOed (receiver, anim, rigid);
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
			Instantiate(hitSpark, sparkPlace, Quaternion.identity);
		} 
		anim.SetFloat ("hitStunTimer", timerArg);	
	}
	
	void OtherHitStunProperties(Character attacker, Character receiver, Animator anim, Animator attAnim, Vector3 sparkPlace, Rigidbody2D rigid){
		AudioSource.PlayClipAtPoint(bigHit, transform.position);
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
		}
		Instantiate(shoryukenSpark, sparkPlace, Quaternion.identity);
	}

	void GotKOed (Character receiver, Animator anim, Rigidbody2D rigid)
	{
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

	void GotShoryukened (Animator anim, Rigidbody2D rigid, float x, float airborneY, float groundedY)
	{
		if (anim.GetBool ("isAirborne") == false) {
			rigid.velocity = new Vector2 (x, groundedY);
		}
		else {
			rigid.velocity = new Vector2 (x, airborneY);
		}
	}
	
	public bool GetCancellabe(){
		return cancellable;
	}
}
