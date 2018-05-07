using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour {

	delegate bool GetActiveSuper();
	
	[SerializeField] GameObject hitSpark, bigHitSpark, shoryukenSpark, blockSpark;		
	[SerializeField] AudioClip normalHit, bigHit, shoryukenHit, blockHit, superKOSound;	
	[SerializeField] float normalHitSlowDown, bigHitSlowDown, KOSlowDown;
	
	private ComboCounter comboCounter;
	private float distance, leftEdgeDistance, rightEdgeDistance;
	private bool cancellable;	
	
	void Start(){
		comboCounter = FindObjectOfType<ComboCounter>();
	}
	
	void OnTriggerStay2D(Collider2D collider){
		HurtBox hurtBox = collider.gameObject.GetComponentInParent<HurtBox>();
		Animator hurtCharAnim = collider.gameObject.GetComponentInParent<Animator>();
//		Vector2 hitLocaterStart, hitLocaterEnd;
		
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
//			if (hitCharacter.side == Character.Side.P1){
//				hitLocaterStart = new Vector2(hurtCollider.bounds.min.x, hurtCollider.bounds.min.y);			
//				hitLocaterEnd = new Vector2(hitCollider.bounds.max.x, hitCollider.bounds.max.y);
//			}
//			else{
//				hitLocaterStart = new Vector2(hurtCollider.bounds.max.x, hurtCollider.bounds.max.y);			
//				hitLocaterEnd = new Vector2(hitCollider.bounds.min.x, hitCollider.bounds.min.y);
//			}

			Vector2 hitLocaterStart = new Vector2(hurtBox.transform.position.x, hurtBox.transform.position.y);			
			Vector2 hitLocaterEnd = new Vector2(transform.position.x, transform.position.y);
			
			//cast a raycast line that locates the spark effect
			RaycastHit2D sparkLocation = Physics2D.Raycast(hitLocaterStart, hitLocaterEnd);
			Vector3 sparkEffect = new Vector3(sparkLocation.transform.position.x, sparkLocation.transform.position.y, 0f);
			
			leftEdgeDistance = Mathf.Abs(CameraControl.leftBGEdge) + hurtCharacter.transform.position.x; 
			rightEdgeDistance = CameraControl.rightBGEdge - hurtCharacter.transform.position.x; 
			distance = hurtCharacter.transform.position.x - hitCharacter.transform.position.x;	 
						
			if (hitCharacter.GetHasntHit() == true){
				if (hurtCharAnim.GetBool("isAirborne") == false){
					if (hurtCharacter.GetBackPressed() == true && hurtCharAnim.GetBool("isAttacking") == false
					    && hurtCharAnim.GetBool("isLiftingOff") == false && hurtCharAnim.GetBool("isInHitStun") == false
					    && hurtCharAnim.GetBool("isKnockedDown") == false){
					    
					    	
						if ((hitCharacter.GetMoveType() == Character.MoveType.low && hurtCharAnim.GetBool("isCrouching") == false) 
						    || (hitCharacter.GetMoveType() == Character.MoveType.mid && hurtCharAnim.GetBool("isStanding") == false)){
						    					    					
						    //if blocked high, but got hit by a low attack
						    //or if blocked low, but got hit by an overhead(jumping attacks only)
							InHitStun(hitCharacter, hurtCharacter, hurtCharAnim, hitCharAnim, sparkEffect, hurtPhysicsbody, hitPhysicsbody);						
				    	}
						else{
							//successful block
							InBlockStun(hitCharacter, hurtCharAnim, hurtPhysicsbody, hitPhysicsbody, sparkEffect);		
						}			
					}
					else{
						//hit everytime with no block attempt
						InHitStun(hitCharacter, hurtCharacter, hurtCharAnim, hitCharAnim, sparkEffect, hurtPhysicsbody, hitPhysicsbody);				
					}	
				}		
				else{							
					//there's no blocking in this game when you're airborne like in normal street fighter alphas, because i said so
					InHitStun(hitCharacter, hurtCharacter, hurtCharAnim, hitCharAnim, sparkEffect, hurtPhysicsbody, hitPhysicsbody);					
				}	
				hurtCharacter.GetSuper += 1.5f;
				hitCharacter.GetSuper += hitCharacter.GetSuperAccumulated();
				hitCharAnim.SetBool("hasntHit", false);
			}	
		}
	}
	
	void OnTriggerExit2D(Collider2D collider){
		cancellable = false;
	}
	
	void InBlockStun(Character attacker, Animator anim, Rigidbody2D receiverRigid, Rigidbody2D attackerRigid, Vector3 sparkPlace){
		float blockStunTimer = attacker.GetEnforceBlockStun() * 0.2f;
		anim.SetBool("isInBlockStun", true);
		AudioSource.PlayClipAtPoint(blockHit, transform.position);
		if (anim.GetBool("isCrouching") == true){
			anim.Play("CrouchBlockStun",0,0f);
		} 
		else{
			anim.Play("StandBlockStun",0,0f);
		}
		anim.SetFloat ("blockStunTimer", blockStunTimer);
		TimeControl.slowDownTimer = normalHitSlowDown;
		PushBack(attacker, receiverRigid, attackerRigid);
		Instantiate(blockSpark, sparkPlace, Quaternion.identity);
	}
	
	void InHitStun(Character attacker, Character receiver, Animator anim, Animator attAnim, Vector3 sparkPlace, Rigidbody2D receiverRigid, Rigidbody2D attackerRigid){
		if (attacker.gameObject.tag == "Player1"){
			if (anim.GetBool("isInHitStun")){
				comboCounter.GetComboCountP1++;		
				comboCounter.GetStartTimer = false;
				comboCounter.ResetComboFinishedTimer();
			}
		}
		else if (attacker.gameObject.tag == "Player2"){
			if (anim.GetBool("isInHitStun")){
				comboCounter.GetComboCountP2++;		
				comboCounter.GetStartTimer = false;	
				comboCounter.ResetComboFinishedTimer();
			}
		}
		float hitStunTimer = attacker.GetEnforceHitStun() * 0.2f;
		receiver.SetDamage(attacker.GetDamage()); 
		anim.SetBool("isInHitStun", true); 
		if (attacker.GetHitType() != Character.HitType.normal && attacker.GetHitType() != Character.HitType.hurricaneKick && attacker.GetHitType() != Character.HitType.rekka){
			OtherHitStunProperties(attacker, receiver, anim, attAnim, sparkPlace, receiverRigid);
		}
		else{
			if (anim.GetBool("isAirborne") == true){
				if (receiver.GetHealth () <= 0){	
					if (attacker.GetComponent<FeiLong>() != null){				
						SuperKO (attacker.GetComponent<FeiLong>().GetRekkaShinkenActive);
					}
					else if (attacker.GetComponent<Balrog>() != null){
						SuperKO (attacker.GetComponent<Balrog>().GetGigatonPunchActive);
					}			
					GotKOed (receiver, anim, receiverRigid);
				}
				else{
					anim.Play("MidAirHit",0,0f);
					TimeControl.slowDownTimer = normalHitSlowDown;
					hitStunTimer = 3f;
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
					TimeControl.slowDownTimer = normalHitSlowDown;
				}				
			}
			else{
			
				if (receiver.GetHealth () <= 0){	
					if (attacker.GetComponent<FeiLong>() != null){				
						SuperKO (attacker.GetComponent<FeiLong>().GetRekkaShinkenActive);
					}
					else if (attacker.GetComponent<Balrog>() != null){
						SuperKO (attacker.GetComponent<Balrog>().GetGigatonPunchActive);
					}
					GotKOed (receiver, anim, receiverRigid);
				}
				else{
					if (anim.GetBool("isCrouching") == true){
						anim.Play("CrouchHit",0,0f);
						TimeControl.slowDownTimer = normalHitSlowDown;
					}
					else{
						if (attacker.GetMoveType() == Character.MoveType.low){
							anim.Play("LowHit",0,0f);
						}
						else{
							anim.Play("HighHit",0,0f);
						}
						TimeControl.slowDownTimer = normalHitSlowDown;
					}
					PushBack(attacker, receiverRigid, attackerRigid);
				}
			} 	
		}
		switch(attacker.GetSparkType()){
			case Character.SparkType.normal:
				AudioSource.PlayClipAtPoint(normalHit, transform.position);
				Instantiate(hitSpark, sparkPlace, Quaternion.identity);
				break;
			case Character.SparkType.big:
				AudioSource.PlayClipAtPoint(bigHit, transform.position);
				Instantiate(bigHitSpark, sparkPlace, Quaternion.identity);
				break;
			case Character.SparkType.shoryuken:
				AudioSource.PlayClipAtPoint(shoryukenHit, transform.position);
				Instantiate(shoryukenSpark, sparkPlace, Quaternion.identity);
				break;
		}
		anim.SetFloat ("hitStunTimer", hitStunTimer);
	}
	
	void OtherHitStunProperties(Character attacker, Character receiver, Animator recAnim, Animator attAnim, Vector3 sparkPlace, Rigidbody2D receiverRigid){		
		Rigidbody2D attPhysicsbody = attacker.GetComponent<Rigidbody2D>();
		if (attacker.GetHitType() == Character.HitType.sweep || attacker.GetHitType() == Character.HitType.dashLow){
			if (attacker.side == Character.Side.P1){
				receiver.side = Character.Side.P2;
				receiver.SideSwitch();
			}
			else{
				receiver.side = Character.Side.P1;
				receiver.SideSwitch();
			}
			if (receiver.GetHealth () <= 0){				
				GotKOed (receiver, recAnim, receiverRigid);
			}
			else{
				TimeControl.slowDownTimer = bigHitSlowDown;
				recAnim.Play("Trip",0,0f);
			}
		}
		else if (attacker.GetHitType() == Character.HitType.rekkaKnockdown){
			recAnim.Play("KnockDownBlendTree",0,0f);
			if (receiver.GetHealth () <= 0){	
				if (attacker.GetComponent<Ken>() != null){
					SuperKO (attacker.GetComponent<Ken>().GetShinryukenActive);
				}
				else if (attacker.GetComponent<FeiLong>() != null){
					SuperKO (attacker.GetComponent<FeiLong>().GetRekkaShinkenActive);
				}
				else if (attacker.GetComponent<Balrog>() != null){
					SuperKO (attacker.GetComponent<Balrog>().GetGigatonPunchActive);
				}			
				TimeControl.slowDownTimer = KOSlowDown;		
			}
			else{		
				TimeControl.slowDownTimer = bigHitSlowDown;
			}
			if (attacker.GetComponent<FeiLong>() != null){		
				switch(attAnim.GetInteger("rekkaPunchType")){
					case 0:
						GotKnockedUp(attacker, receiver, attAnim, recAnim, receiverRigid, 2f, 4f);
						break;
					case 1:
						GotKnockedUp(attacker, receiver, attAnim, recAnim, receiverRigid, 2.5f, 4f);
						break;		
					default:
						GotKnockedUp(attacker, receiver, attAnim, recAnim, receiverRigid, 3f, 4f);
						break;										
				}		
				attPhysicsbody.velocity *= 0.6f;
			}
			else if (attacker.GetComponent<Balrog>() != null){	
				switch(attAnim.GetInteger("dashRushPunchType")){
					case 0:
						GotKnockedUp(attacker, receiver, attAnim, recAnim, receiverRigid, 2f, 4f);
						break;
					case 1:
						GotKnockedUp(attacker, receiver, attAnim, recAnim, receiverRigid, 2.5f, 4f);
						break;		
					default:
						GotKnockedUp(attacker, receiver, attAnim, recAnim, receiverRigid, 3f, 4f);
						break;										
				}		
			}		
			else if (attacker.GetComponent<Akuma>() != null){
				GotKnockedUp(attacker, receiver, attAnim, recAnim, receiverRigid, 2f, 2f);
			}
			else if (attacker.GetComponent<Ken>() != null){
				GotKnockedUp(attacker, receiver, attAnim, recAnim, receiverRigid, 1f, 3f);
			}
		}
		else if (attacker.GetHitType() == Character.HitType.shoryuken){
			recAnim.Play("KnockDownBlendTree",0,0f);
			if (receiver.GetHealth () <= 0){	
				TimeControl.slowDownTimer = KOSlowDown;		
			}
			else{		
				TimeControl.slowDownTimer = bigHitSlowDown;
			}
			if (attacker.GetComponent<Ken>() != null){
				switch(attAnim.GetInteger("shoryukenPunchType")){
				case 0:
					GotKnockedUp(attacker, receiver, attAnim, recAnim, receiverRigid, 1f, 4f);
					break;
				case 1:
					GotKnockedUp(attacker, receiver, attAnim, recAnim, receiverRigid, 1.5f, 4.5f);
					break;		
				default:
					GotKnockedUp(attacker, receiver, attAnim, recAnim, receiverRigid, 2f, 5.5f);
					break;										
				}		
			}			
			else if (attacker.GetComponent<Akuma>() != null){
				switch(attAnim.GetInteger("shoryukenPunchType")){
				case 0:
					GotKnockedUp(attacker, receiver, attAnim, recAnim, receiverRigid, 1f, 4f);
					break;
				case 1:
					GotKnockedUp(attacker, receiver, attAnim, recAnim, receiverRigid, 1.25f, 4.5f);
					break;		
				default:
					GotKnockedUp(attacker, receiver, attAnim, recAnim, receiverRigid, 1.5f, 5f);
					break;										
				}		
			}			
			else if (attacker.GetComponent<FeiLong>() != null){				
				switch(attAnim.GetInteger("shienKyakuKickType")){
				case 0:
					GotKnockedUp(attacker, receiver, attAnim, recAnim, receiverRigid, 0.5f, 4f);
					break;
				case 1:
					GotKnockedUp(attacker, receiver, attAnim, recAnim, receiverRigid, 0.625f, 4.5f);
					break;		
				default:
					GotKnockedUp(attacker, receiver, attAnim, recAnim, receiverRigid, 0.75f, 5f);
					break;										
				}		
			}
			else if (attacker.GetComponent<Balrog>() != null){
				GotKnockedUp(attacker, receiver, attAnim, recAnim, receiverRigid, 2f, 3f);
			}
		}
		else if (attacker.GetHitType() == Character.HitType.akumaHurricaneKick){
			if (receiver.GetHealth () <= 0){	
				if (attacker.GetComponent<Ken>() != null){
					SuperKO (attacker.GetComponent<Ken>().GetShinryukenActive);
				}
				TimeControl.slowDownTimer = KOSlowDown;					
				recAnim.Play("KnockDownBlendTree",0,0f);
			}
			else{		
				TimeControl.slowDownTimer = bigHitSlowDown;
				recAnim.Play("MidAirHit",0,0f);
			}
			if (attacker.GetComponent<Akuma>() != null){
				switch(attAnim.GetInteger("hurricaneKickType")){
				case 0:
					GotKnockedUp(attacker, receiver, attAnim, recAnim, receiverRigid, 0.75f, 3.75f);
					break;
				case 1:
					GotKnockedUp(attacker, receiver, attAnim, recAnim, receiverRigid, 1.5f, 2.5f);
					break;		
				default:
					GotKnockedUp(attacker, receiver, attAnim, recAnim, receiverRigid, 1.5f, 2.25f);
					break;										
				}		
			}	
			else if (attacker.GetComponent<Ken>() != null){
				GotKnockedUp(attacker, receiver, attAnim, recAnim, receiverRigid, 0f, 2f);
			}	
		}
	}
	
	void PushBack(Character attacker, Rigidbody2D receiverRigid, Rigidbody2D attackerRigid){						
		if (attacker.GetHitType() != Character.HitType.hurricaneKick && attacker.GetHitType() != Character.HitType.rekka
			&& attacker.GetHitType() != Character.HitType.akumaHurricaneKick && attacker.GetHitType() != Character.HitType.rekkaKnockdown 
			&& attacker.GetHitType() != Character.HitType.dashLow){
			if (attacker.side == Character.Side.P1){
				if (rightEdgeDistance > 0.25f){
					// if close to the wall, pushback enforced on attacking character instead
					receiverRigid.velocity = new Vector2(attacker.GetEnforcePushBack() * 0.15f, 0f);		
				}
				else{
					attackerRigid.velocity = new Vector2(-attacker.GetEnforcePushBack() * 0.15f, attackerRigid.velocity.y);		
				}
			}
			else{
				if (leftEdgeDistance > 0.25f){
					// if close to the wall, pushback enforced on attacking character instead
					receiverRigid.velocity = new Vector2(-attacker.GetEnforcePushBack() * 0.15f, 0f);		
				}
				else{
					attackerRigid.velocity = new Vector2(attacker.GetEnforcePushBack() * 0.15f, attackerRigid.velocity.y);		
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
		TimeControl.slowDownTimer = KOSlowDown;	
		AudioSource.PlayClipAtPoint(bigHit, transform.position);
		anim.Play ("KnockDownBlendTree", 0, 0f);
		if (receiver.side == Character.Side.P1) {
			rigid.velocity = new Vector2 (-2f, 4f);
		}
		else {
			rigid.velocity = new Vector2 (2f, 4f);
		}
	}
	
	void GotKnockedUp (Character attacker, Character receiver, Animator attAnim, Animator recAnim, Rigidbody2D hurtRigid, float x, float y){				
		if (attacker.transform.localScale.x == 1){
			//if attacking sprite is facing p1 side
			receiver.side = Character.Side.P2;
			hurtRigid.velocity = new Vector2 (x, y);				
		}
		else{
			receiver.side = Character.Side.P1;
			hurtRigid.velocity = new Vector2 (-x, y);		
		}	
	}

	void SuperKO (GetActiveSuper getActiveSuper){
		if (getActiveSuper()) {
			TimeControl.superKO = true;
			AudioSource.PlayClipAtPoint (superKOSound, transform.position);
		}
	}
	
	public bool GetCancellabe(){
		return cancellable;
	}
}
