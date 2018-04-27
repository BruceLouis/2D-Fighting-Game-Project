using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ken : MonoBehaviour {
	
	public GameObject projectile;
	public AudioClip hadoukenSound, shoryukenSound, flameSound;
	public AudioClip hurricaneKickSound, hadoukenCreatedSound;
	
	private Character character;
	private Animator animator; 
	private Rigidbody2D physicsbody;
	
	private int shoryukenType;
	private bool hurricaneActive;

	// Use this for initialization
	void Start () {
		character = GetComponent<Character>();
		animator = GetComponent<Animator>();
		physicsbody = GetComponent<Rigidbody2D>();
		hurricaneActive = animator.GetBool("hurricaneKickActive");
	}
	
	void FixedUpdate (){
		hurricaneActive = animator.GetBool("hurricaneKickActive");
		if (hurricaneActive){
			physicsbody.isKinematic = true;
			if (character.GetRightEdgeDistance() < 0.5f && character.side == Character.Side.P1){ 			
				physicsbody.velocity = new Vector2(0f, physicsbody.velocity.y);
			}
			if (character.GetLeftEdgeDistance() < 0.5f && character.side == Character.Side.P2){	
				physicsbody.velocity = new Vector2(0f, physicsbody.velocity.y);
			}
		}
		else{
			physicsbody.isKinematic = false;
		}
	}
	
	public void HadoukenRelease(){
		Vector3 offset = new Vector3(0.75f, 0f, 0f);
		GameObject hadouken = Instantiate(projectile);
		Rigidbody2D rigidbody = hadouken.GetComponent<Rigidbody2D>();
		SpriteRenderer hadoukenSprite = hadouken.GetComponentInChildren<SpriteRenderer>();		
		AudioSource.PlayClipAtPoint(hadoukenCreatedSound, transform.position);
		if (animator.GetInteger("hadoukenOwner") == 1){
			hadouken.gameObject.layer = LayerMask.NameToLayer("ProjectileP1");
			hadouken.gameObject.tag = "Player1";
			hadouken.transform.parent = GameObject.Find("ProjectileP1Parent").transform;
		}
		else{
			hadouken.gameObject.layer = LayerMask.NameToLayer("ProjectileP2");
			hadouken.gameObject.tag = "Player2";
			hadouken.transform.parent = GameObject.Find("ProjectileP2Parent").transform;
		} 
		if (character.side == Character.Side.P1){
			hadouken.transform.position = transform.position + offset;
		}
		else{
			hadouken.transform.position = transform.position - offset;
		}
		switch(animator.GetInteger("hadoukenPunchType")){
			case 0:
				if (character.side == Character.Side.P1){
					rigidbody.velocity = new Vector2(2f, 0f);
				}
				else {
					rigidbody.velocity = new Vector2(-2f, 0f);	
					hadoukenSprite.flipX = true;
				}
				break;
		
			case 1:
				if (character.side == Character.Side.P1){
					rigidbody.velocity = new Vector2(2.5f, 0f);
				}
				else {
					rigidbody.velocity = new Vector2(-2.5f, 0f);	
					hadoukenSprite.flipX = true;
				}
				break;
		
			default:
				if (character.side == Character.Side.P1){
					rigidbody.velocity = new Vector2(3f, 0f);
				}
				else {
					rigidbody.velocity = new Vector2(-3f, 0f);	
					hadoukenSprite.flipX = true;
				}
				break;
		}
		
	}
	
	public void KenShoryuken(){
		shoryukenType = animator.GetInteger("shoryukenPunchType");
		if (animator.GetBool("isLiftingOff") == false){				
			switch(animator.GetInteger("shoryukenPunchType")){
				case 0:
					character.MoveProperties(30f, 20f, 5f, 80f, 2, 3);
					break;
				case 1:
					character.MoveProperties(40f, 25f, 7.5f, 85f, 2, 3);
					break;
				default:
					character.MoveProperties(60f, 25f, 10f, 90f, 2, 3);
					break;
			}
		}
	}	
	public void ShoryukenLiftOff(){		
		switch(animator.GetInteger("shoryukenPunchType")){
			case 0:
				KenTakeOffVelocity(1f, 3f);
				break;
				
			case 1:			
				KenTakeOffVelocity(1.5f, 4f);
				break;
				
			default:	
				KenTakeOffVelocity(2f, 5f);			
				break;	
		}				
		AudioSource.PlayClipAtPoint(character.normalAttackSound,transform.position);
	}
	
	public void KenRoll(){
		if (animator.GetBool("isLiftingOff") == false){	
			switch(animator.GetInteger("rollKickType")){
				case 0:
					KenTakeOffVelocity (2f, 0f);				
					break;
				case 1:
					KenTakeOffVelocity (2.5f, 0f);				
					break;
				default: 
					KenTakeOffVelocity (3f, 0f);							
					break;
			}
			animator.SetBool("isRolling", true);
		}
	}

	void KenTakeOffVelocity (float x, float y){
		if (character.transform.localScale.x == 1) {
			physicsbody.velocity = new Vector2 (x, y);
		}
		else {
			physicsbody.velocity = new Vector2 (-x, y);
		}
	}
	
	public void KenFinishedRolling(){
		animator.SetBool("isRolling", false);
	}
		
	public void KenHurricaneKickLiftOff(){		
		KenTakeOffVelocity(1f, 1.5f);
		character.MoveProperties(45f, 20f, 10f, 17f, 2, 4);
	}
	
	public void KenHurricaneKickFloat(){		
		physicsbody.velocity = new Vector2 (physicsbody.velocity.x, 0f);
		character.MoveProperties(40f, 20f, 10f, 17f, 2, 4);
	}	
	
	public void KenHurricaneLanding(){
		animator.SetBool("hurricaneKickActive", false);
	}
	
	public void PlayHadoukenSound(){
		AudioSource.PlayClipAtPoint(hadoukenSound, transform.position);
	}
	
	public void PlayShoryukenSound(){
		AudioSource.PlayClipAtPoint(shoryukenSound, transform.position);
	}
	
	public void PlayFlamesSound(){
		AudioSource.PlayClipAtPoint(character.flameSound, transform.position);
	}
	
	public void PlayHurricaneKickSound(){
		AudioSource.PlayClipAtPoint(hurricaneKickSound, transform.position);
	}
	
	public int GetShoryukenType(){
		return shoryukenType;
	}
}
