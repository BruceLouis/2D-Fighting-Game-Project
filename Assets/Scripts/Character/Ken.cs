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
	
	void Update (){
		hurricaneActive = animator.GetBool("hurricaneKickActive");
		if (hurricaneActive){
			physicsbody.isKinematic = true;
			if (character.GetRightEdgeDistance() < 0.5f && character.side == Character.Side.P1){ 			
				physicsbody.velocity = new Vector2(0f, 0f);
			}
			if (character.GetLeftEdgeDistance() < 0.5f && character.side == Character.Side.P2){	
				physicsbody.velocity = new Vector2(0f, 0f);
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
		}
		else{
			hadouken.gameObject.layer = LayerMask.NameToLayer("ProjectileP2");
			hadouken.gameObject.tag = "Player2";
		} 
		if (character.side == Character.Side.P1){
			hadouken.transform.position = transform.position + offset;
		}
		else{
			hadouken.transform.position = transform.position - offset;
		}
		if (animator.GetInteger("hadoukenPunchType") == 0){
			if (character.side == Character.Side.P1){
				rigidbody.velocity = new Vector2(3f, 0f);
			}
			else {
				rigidbody.velocity = new Vector2(-3f, 0f);	
				hadoukenSprite.flipX = true;
			}
		}
		else if (animator.GetInteger("hadoukenPunchType") == 1){
			if (character.side == Character.Side.P1){
				rigidbody.velocity = new Vector2(3.5f, 0f);
			}
			else {
				rigidbody.velocity = new Vector2(-3.5f, 0f);	
				hadoukenSprite.flipX = true;
			}
		}
		else{
			if (character.side == Character.Side.P1){
				rigidbody.velocity = new Vector2(4f, 0f);
			}
			else {
				rigidbody.velocity = new Vector2(-4f, 0f);	
				hadoukenSprite.flipX = true;
			}
		}
		
	}
	
	public void KenShoryuken(){
		shoryukenType = animator.GetInteger("shoryukenPunchType");
		if (animator.GetBool("isLiftingOff") == false){	
			if (animator.GetInteger("shoryukenPunchType") == 0){
				character.MoveProperties(30f, 20f, 5f, 80f, 2, 3);
			}
			else if (animator.GetInteger("shoryukenPunchType") == 1){
				character.MoveProperties(40f, 25f, 7.5f, 85f, 2, 3);
			}
			else{
				character.MoveProperties(60f, 25f, 10f, 90f, 2, 3);
			}
		}
	}	
	public void ShoryukenLiftOff(){		
		if (animator.GetInteger("shoryukenPunchType") == 0){
			if (character.side == Character.Side.P1){
				physicsbody.velocity = new Vector2(1f, 3f);
			}
			else{
				physicsbody.velocity = new Vector2(-1f, 3f);
			}
		}
		else if (animator.GetInteger("shoryukenPunchType") == 1){
			if (character.side == Character.Side.P1){
				physicsbody.velocity = new Vector2(1.5f, 4f);
			}
			else{
				physicsbody.velocity = new Vector2(-1.5f, 4f);
			}
		}
		else{
			if (character.side == Character.Side.P1){
				physicsbody.velocity = new Vector2(2f, 5f);
			}
			else{
				physicsbody.velocity = new Vector2(-2f, 5f);
			}
		}
		AudioSource.PlayClipAtPoint(character.normalAttackSound,transform.position);
		animator.SetBool("isAirborne", true);
	}
	
	public void KenRoll(){
		if (animator.GetBool("isLiftingOff") == false){	
			if (animator.GetInteger("rollKickType") == 0){
				if (character.side == Character.Side.P1){
					physicsbody.velocity = new Vector2(2f, 0f);
				}
				else{
					physicsbody.velocity = new Vector2(-2f, 0f);
				}				
			}
			else if (animator.GetInteger("rollKickType") == 1){
				if (character.side == Character.Side.P1){
					physicsbody.velocity = new Vector2(2.5f, 0f);
				}
				else{
					physicsbody.velocity = new Vector2(-2.5f, 0f);
				}				
			}
			else{
				if (character.side == Character.Side.P1){
					physicsbody.velocity = new Vector2(3f, 0f);
				}
				else{
					physicsbody.velocity = new Vector2(-3f, 0f);
				}				
			}
			animator.SetBool("isRolling", true);
		}
	}
	
	public void KenFinishedRolling(){
		animator.SetBool("isRolling", false);
	}
		
	public void KenHurricaneKick(){		
		if (animator.GetBool("isLiftingOff") == false){	
			transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, 0.5f, transform.position.z), 15f * Time.deltaTime);
			if (character.side == Character.Side.P1){				
				physicsbody.velocity = new Vector2 (1f, 0f);
			}
			else{
				physicsbody.velocity = new Vector2 (-1f, 0f);
			}			
			character.MoveProperties(45f, 20f, 10f, 17f, 2, 4);
			animator.SetBool("isAirborne", true);
		}
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
