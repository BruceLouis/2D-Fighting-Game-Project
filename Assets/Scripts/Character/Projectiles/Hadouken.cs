using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hadouken : MonoBehaviour {

	public float damage, pushBack;
	public float hitStun, blockStun;
	
	public GameObject shoryukenSpark;
	public GameObject blockSpark;
	
	public AudioClip connectedSound;
	public AudioClip blockedSound;
	
	private Animator animator;
	private Rigidbody2D physicsBody;
	
	
	void Start(){
		animator = GetComponent<Animator>();
		physicsBody = GetComponent<Rigidbody2D>();
		animator.SetBool("madeContact", false); 
	}
	
	void StopMovingHadouken(){
		physicsBody.velocity = new Vector2(0f,0f);
	}	
	
	void DestroyHadouken(){
		Destroy(gameObject);
	}
	
	void PushBack(Character receiver, Rigidbody2D rigid){
		if (receiver.side == Character.Side.P2){			
			rigid.velocity = new Vector2(pushBack * 0.2f, 0f);		
		}
		else{
			rigid.velocity = new Vector2(-pushBack * 0.2f, 0f);				
		}
	}
		
	void OnTriggerEnter2D(Collider2D collider){
		HurtBox hurtBox = collider.gameObject.GetComponentInParent<HurtBox>();	
		Animator hurtCharAnimator = collider.gameObject.GetComponentInParent<Animator>();	
		Hadouken otherHadouken = collider.gameObject.GetComponent<Hadouken>();
		float timer; 
		if (hurtBox && hurtBox.gameObject.tag != gameObject.tag){	
			Character hurtCharacter = hurtBox.GetComponentInParent<Character>();
			Rigidbody2D hurtPhysicsbody = hurtCharacter.GetComponent<Rigidbody2D>();
			animator.SetBool("madeContact", true);	
			
			if (hurtCharacter.GetBackPressed() == true && hurtCharAnimator.GetBool("isAttacking") == false
			    && hurtCharAnimator.GetBool("isLiftingOff") == false && hurtCharAnimator.GetBool("isAirborne") == false
			    && hurtCharAnimator.GetBool("isInHitStun") == false){
				
				AudioSource.PlayClipAtPoint(blockedSound, transform.position);
				timer = blockStun;
				hurtCharAnimator.SetBool("isInBlockStun", true);
				if (hurtCharAnimator.GetBool("isStanding") == true){
					hurtCharAnimator.Play("StandBlockStun",0,0f);
					PushBack(hurtCharacter, hurtPhysicsbody);
				}
				else if (hurtCharAnimator.GetBool("isCrouching") == true){
					hurtCharAnimator.Play("CrouchBlockStun",0,0f);
					PushBack(hurtCharacter, hurtPhysicsbody);
				} 
				hurtCharAnimator.SetFloat ("blockStunTimer", timer);	
				Instantiate(blockSpark, transform.position, Quaternion.identity);  
			}
			else{	
				TimeControl.slowDown = true;  
				AudioSource.PlayClipAtPoint(connectedSound, transform.position);
				hurtCharacter.SetDamage(damage);
				if (hurtCharacter.GetHealth () <= 0){	
					TimeControl.slowDownTimer = 100;				
					hurtCharAnimator.Play("KOBlendTree",0);
					if (hurtCharacter.side == Character.Side.P1){
						hurtPhysicsbody.velocity = new Vector2(-2f, 4f);
					}
					else{
						hurtPhysicsbody.velocity = new Vector2(2f, 4f);
					}
				}		    
				else{
					TimeControl.slowDownTimer = 30;
					timer = hitStun;
					if (hurtCharAnimator.GetBool("isAirborne") == true){
						hurtCharAnimator.Play("KnockDownBlendTree",0,0f);
						if (hurtCharacter.side == Character.Side.P2){
							hurtPhysicsbody.velocity = new Vector2(pushBack * 0.15f, 3f);
						}
						else{
							hurtPhysicsbody.velocity = new Vector2(-pushBack * 0.15f, 3f);						
						}
					}
					else if (hurtCharAnimator.GetBool("isCrouching") == true){
						hurtCharAnimator.Play("CrouchHit",0,0f);
						PushBack(hurtCharacter, hurtPhysicsbody);
					}
					else {
						hurtCharAnimator.Play("HighHit",0,0f);
						PushBack(hurtCharacter, hurtPhysicsbody);
					}						
					hurtCharAnimator.SetBool("isInHitStun", true);
					hurtCharAnimator.SetFloat ("hitStunTimer", timer);	
					Instantiate(shoryukenSpark, transform.position, Quaternion.identity);
				}
			}
			    			
		}
		if (otherHadouken){		
			animator.SetBool("madeContact", true);
		}
	}
}
