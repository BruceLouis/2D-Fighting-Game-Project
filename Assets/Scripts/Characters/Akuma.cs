using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Akuma : MonoBehaviour {

	[SerializeField] GameObject projectile;
	[SerializeField] AudioClip specialAttackSound1, specialAttackSound2, specialAttackSound3;
	[SerializeField] AudioClip stompSound, hadoukenCreatedSound, akumaDemonTravel;
	
	private Character character;
	private Animator animator; 
	private Rigidbody2D physicsbody;
	private Vector3 startPos, endPos;
	
	private int shoryukenType;
	private float hadoukenAngle, amountTimeTravelledTimer;
	private bool hurricaneActive, diveKickActive;	
	
	// Use this for initialization
	void Start () {
		character = GetComponent<Character>();
		animator = GetComponent<Animator>();
		physicsbody = GetComponent<Rigidbody2D>();
	}
	
	void FixedUpdate (){
		hurricaneActive = animator.GetBool("hurricaneKickActive");
		diveKickActive = animator.GetBool("diveKickActive");
		if (hurricaneActive){
			character.AtTheCorner ();
		}
		else if (animator.GetBool("isShunGokuSatsuInMotion")){
			character.AtTheCorner ();
			amountTimeTravelledTimer--;
			if (amountTimeTravelledTimer <= 0f){
				animator.SetBool("isShunGokuSatsuInMotion", false);		
				animator.SetBool("shunGokuSatsuActive", false);		
			}
		}
		else if (diveKickActive){
			physicsbody.isKinematic = true;
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
		HadoukenInitialize (offset, hadouken); 
		character.GetSuper += 4.5f;
		switch(animator.GetInteger("hadoukenPunchType")){
		case 0:
			if (character.transform.localScale.x == 1){
				rigidbody.velocity = new Vector2(2.25f, 0f);
			}
			else {
				rigidbody.velocity = new Vector2(-2.25f, 0f);	
				hadoukenSprite.flipX = true;
			}
			break;
			
		case 1:
			if (character.transform.localScale.x == 1){
				rigidbody.velocity = new Vector2(2.75f, 0f);
			}
			else {
				rigidbody.velocity = new Vector2(-2.75f, 0f);	
				hadoukenSprite.flipX = true;
			}
			break;
			
		default:
			if (character.transform.localScale.x == 1){
				rigidbody.velocity = new Vector2(3.25f, 0f);
			}
			else {
				rigidbody.velocity = new Vector2(-3.25f, 0f);	
				hadoukenSprite.flipX = true;
			}
			break;
		}
		
	}
	
	public void AirHadoukenRelease(){
		Vector3 offset = new Vector3(0.75f, 0f, 0f);
		GameObject hadouken = Instantiate(projectile);
		Rigidbody2D rigidbody = hadouken.GetComponent<Rigidbody2D>();	
		AudioSource.PlayClipAtPoint(hadoukenCreatedSound, transform.position);
		HadoukenInitialize (offset, hadouken); 
		character.GetSuper += 4.5f;
		switch(animator.GetInteger("hadoukenPunchType")){
		case 0:
			if (character.transform.localScale.x == 1){
				rigidbody.velocity = new Vector2(0.5f, -1.5f);
			}
			else {
				rigidbody.velocity = new Vector2(-0.5f, -1.5f);	
			}
			break;
			
		case 1:
			if (character.transform.localScale.x == 1){
				rigidbody.velocity = new Vector2(1f, -1.5f);
			}
			else {
				rigidbody.velocity = new Vector2(-1f, -1.5f);	
			}
			break;
			
		default:
			if (character.transform.localScale.x == 1){
				rigidbody.velocity = new Vector2(1.5f, -1.5f);
			}
			else {
				rigidbody.velocity = new Vector2(-1.5f, -1.5f);	
			}
			break;
		}
		//air hadouken angle calculated using tan2 		
		hadoukenAngle = Mathf.Atan2(rigidbody.velocity.y, rigidbody.velocity.x) * Mathf.Rad2Deg;
		hadouken.transform.rotation = Quaternion.AngleAxis(hadoukenAngle, Vector3.forward);
	}	

	public void AkumaShoryuken(){
		shoryukenType = animator.GetInteger("shoryukenPunchType");
		if (animator.GetBool("isLiftingOff") == false){				
			switch(animator.GetInteger("shoryukenPunchType")){
			case 0:
				character.MoveProperties(30f, 20f, 5f, 75f, 2, 3, 2, 6f);
				break;
			case 1:
				character.MoveProperties(40f, 25f, 7.5f, 80f, 2, 3, 2, 6.5f);
				break;
			default:
				character.MoveProperties(60f, 25f, 10f, 85f, 2, 3, 2, 7f);
				break;
			}
		}
	}	
	
	public void ShoryukenLiftOff(){		
		switch(animator.GetInteger("shoryukenPunchType")){
		case 0:
			AkumaTakeOffVelocity(1f, 3f);
			break;
			
		case 1:			
			AkumaTakeOffVelocity(1.5f, 3.75f);
			break;
			
		default:	
			AkumaTakeOffVelocity(2f, 4.5f);			
			break;	
		}				
		character.PlayNormalAttackSound();
	}
	
	public void AkumaHurricaneKickLiftOff(){		
		AkumaTakeOffVelocity(1.25f, 1.25f);
	}
	
	public void AkumaHurricaneKickFloat(float timerHitStun){		
		physicsbody.velocity = new Vector2 (physicsbody.velocity.x, 0f);
		character.MoveProperties(timerHitStun, 20f, 10f, 30f, 2, 7, 1, 4.5f);
	}	
	
	public void AkumaHurricaneLanding(){
		animator.SetBool("hurricaneKickActive", false);
	}
	
	public void AkumaHyakkishuLiftOff(){
		if (animator.GetBool("isAirborne") == false && animator.GetBool("isInHitStun") == false 
		    && animator.GetBool("isKnockedDown") == false && animator.GetBool("isInBlockStun") == false
		    && animator.GetBool("isThrown") == false && animator.GetBool("isMidAirRecovering") == false
		    && animator.GetBool("isLiftingOff") == true){			
			switch(animator.GetInteger("hyakkishuKickType")){
				case 0:
					AkumaTakeOffVelocity(1f, 4.5f);
					break;
				case 1:
					AkumaTakeOffVelocity(1.5f, 4.5f);
					break;
				default:
					AkumaTakeOffVelocity(2f, 4.5f);
					break;
			}
		}
	}
	
	public void AkumaHyakkiGozanProperties(){
		AkumaTakeOffVelocity(1.5f, 0f);
		character.MoveProperties(40f, 20f, 10f, 35f, 0, 1, 1, 4.5f);
	}
	
	public void AkumaHyakkiGoshoProperties(){
		character.MoveProperties(40f, 20f, 10f, 40f, 1, 2, 1, 4.5f);
	}
	
	public void AkumaDiveKickProperties(){
		AkumaTakeOffVelocity(0f, 0f);
		character.MoveProperties(40f, 20f, 10f, 20f, 2, 4, 0, 4.5f);
	}
	
	public void AkumaDiveKick(){
		AkumaTakeOffVelocity(1.25f, -3.25f);
	}
	
	public void AkumaShunGokuSatsuInMotion(float amountTimeTravelled){
		amountTimeTravelledTimer = amountTimeTravelled;
		GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Characters";
		AudioSource.PlayClipAtPoint(akumaDemonTravel,transform.position);
		if (character.side == Character.Side.P1){
			physicsbody.velocity = new Vector2 (2.5f,0f);
		}
		else{
			physicsbody.velocity = new Vector2 (-2.5f,0f);
		}
		animator.SetBool("isShunGokuSatsuInMotion", true);
	}
	
	public void AkumaShunGokuSatsuReachesTarget(){
		animator.SetBool("isShunGokuSatsuInMotion", false);
		physicsbody.velocity *= 0.2f;			
	}	
	
	public void AkumaShunGokuSatsu(int onOrOff){
		if (onOrOff == 1){
			TimeControl.gettingDemoned = true;		
		}
		else{
			TimeControl.gettingDemoned = false;		
		}
	}	
	
	public void PlaySpecialAttackSound1(){
		AudioSource.PlayClipAtPoint(specialAttackSound1, transform.position);
	}
	
	public void PlaySpecialAttackSound2(){
		AudioSource.PlayClipAtPoint(specialAttackSound2, transform.position);
	}
	
	public void PlaySpecialAttackSound3(){
		AudioSource.PlayClipAtPoint(specialAttackSound3, transform.position);
	}
	
	public void PlayStompSound(){
		AudioSource.PlayClipAtPoint(stompSound, transform.position);
	}
	
	void HadoukenInitialize (Vector3 offset, GameObject hadouken){
		if (animator.GetInteger ("hadoukenOwner") == 1) {
			hadouken.gameObject.layer = LayerMask.NameToLayer ("ProjectileP1");
			hadouken.gameObject.tag = "Player1";
			hadouken.transform.parent = GameObject.Find ("ProjectileP1Parent").transform;
		}
		else {
			hadouken.gameObject.layer = LayerMask.NameToLayer ("ProjectileP2");
			hadouken.gameObject.tag = "Player2";
			hadouken.transform.parent = GameObject.Find ("ProjectileP2Parent").transform;
		}
		if (character.transform.localScale.x == 1) {
			hadouken.transform.position = transform.position + offset;
		}
		else {
			hadouken.transform.position = transform.position - offset;
		}
	}	
	
	void AkumaTakeOffVelocity (float x, float y){
		if (character.transform.localScale.x == 1) {
			physicsbody.velocity = new Vector2 (x, y);
		}
		else {
			physicsbody.velocity = new Vector2 (-x, y);
		}
	}	
	
}
