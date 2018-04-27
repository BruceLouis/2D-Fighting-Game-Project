using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balrog : MonoBehaviour {

	public AudioClip one, two, three, four, five, six, final;
	public AudioClip balrogRushPunchSound;

	private Character character;	
	private Animator animator; 
	private Player player;
	private Opponent opponent;
	private ChargeSystem chargeSystem;
	private Rigidbody2D physicsbody;
	private Vector3 startPos, endPos;
	private float distanceFromOtherGuy;
		
	// Use this for initialization
	void Start () {
		character = GetComponent<Character>();
		animator = GetComponent<Animator>();
		physicsbody = GetComponent<Rigidbody2D>();					
		chargeSystem = GetComponentInParent<ChargeSystem>();
		if (gameObject.tag == "Player1"){
			player = GetComponentInParent<Player>();
		}
		else if (gameObject.tag == "Player2"){
			opponent = GetComponentInParent<Opponent>();
		}
	}
	
	void Update () {
		if (player != null){
			distanceFromOtherGuy = player.GetDistanceFromOpponent();
		}
		else if (opponent != null){
			distanceFromOtherGuy = opponent.GetDistanceFromPlayer();
		}
		if (animator.GetBool("isDashRushTravelling")){
			physicsbody.isKinematic = true;		
			if (Vector3.Distance(transform.position, endPos) <= 0.25f || distanceFromOtherGuy <= 0.75f){
				animator.SetBool("didRushPunchGetThere", true);		
			}
		}
		else{
			physicsbody.isKinematic = false;
		}
	}
	
	public void TurnPunch(){
		switch(animator.GetInteger("turnPunchStrength")){
			case 1:
				character.MoveProperties(30f, 25f, 12f, 50f, 2, 2);
				AudioSource.PlayClipAtPoint(one, transform.position);
				break;
			case 2:				
				character.MoveProperties(30f, 25f, 12f, 60f, 2, 2);
				AudioSource.PlayClipAtPoint(two, transform.position);
				break;
			case 3:
				character.MoveProperties(30f, 25f, 12f, 80f, 2, 2);
				AudioSource.PlayClipAtPoint(three, transform.position);
				break;
			case 4:
				character.MoveProperties(30f, 25f, 12f, 120f, 2, 2);
				AudioSource.PlayClipAtPoint(four, transform.position);
				break;
			case 5:				
				character.MoveProperties(30f, 25f, 12f, 250f, 2, 2);
				AudioSource.PlayClipAtPoint(five, transform.position);
				break;
			case 6:
				character.MoveProperties(30f, 25f, 12f, 450f, 2, 2);
				AudioSource.PlayClipAtPoint(six, transform.position);
				break;
			case 7:
				character.MoveProperties(30f, 25f, 12f, 600f, 2, 2);
				AudioSource.PlayClipAtPoint(final, transform.position);
				break;
		}
	}
		
	void DashRushStartUp(float distanceTravel){
		Vector3 travelDistance;
		if (character.side == Character.Side.P1){
			travelDistance = new Vector3 (distanceTravel, 0f, 0f);
			physicsbody.velocity = new Vector2 (4f,0f);
		}
		else{
			travelDistance = new Vector3 (-distanceTravel, 0f, 0f);
			physicsbody.velocity = new Vector2 (-4f,0f);
		}
		startPos = transform.position;
		endPos = transform.position + travelDistance;
		animator.SetBool("isDashRushTravelling", true);
	}	
	
	void DashRushGotThere(){
		physicsbody.velocity *= 0.5f;
	}		
	
	void HeadButtSlowDown(){
		physicsbody.velocity *= 0.25f;
	}		
	
	void TurnPunchRushStart(){
		if (character.transform.localScale.x == 1){
			physicsbody.velocity = new Vector2(3f, 0f);	
		}
		else{
			physicsbody.velocity = new Vector2(-3f, 0f);	
		}
	}
	
	void DashStraightProperties(){
		switch(animator.GetInteger("dashRushPunchType")){
			case 0:
				character.MoveProperties(30f, 25f, 7.5f, 50f, 2, 4);
				break;
			case 1:
				character.MoveProperties(35f, 25f, 7.5f, 55f, 2, 4);
				break;
			default:
				character.MoveProperties(40f, 25f, 7.5f, 60f, 2, 4);
				break;
		}
	}
	
	void DashLowProperties(){
		switch(animator.GetInteger("dashRushPunchType")){
			case 0:
				character.MoveProperties(40f, 30f, 12f, 65f, 0, 6);
				break;
			case 1:
				character.MoveProperties(40f, 30f, 12f, 70f, 0, 6);
				break;
			default:
				character.MoveProperties(40f, 30f, 12f, 75f, 0, 6);
				break;
		}
	}
	
	void KickRushProperties(){
		switch(animator.GetInteger("dashRushPunchType")){
			case 0:
				character.MoveProperties(30f, 25f, 12f, 50f, 2, 2);
				break;
			case 1:
				character.MoveProperties(35f, 25f, 12f, 55f, 2, 2);
				break;
			default:
				character.MoveProperties(40f, 25f, 12f, 60f, 2, 2);
				break;
		}
	}
	
	void HeadButtProperties(){
		switch(animator.GetInteger("dashRushPunchType")){
			case 0:
				character.MoveProperties(30f, 25f, 12f, 75f, 2, 3);
				break;
			case 1:
				character.MoveProperties(35f, 25f, 12f, 80f, 2, 3);
				break;
			default:
				character.MoveProperties(40f, 25f, 12f, 85f, 2, 3);
				break;
		}
	}
	
	void HeadButtLiftOff(float x){
		if (character.transform.localScale.x == 1){
			physicsbody.velocity = new Vector2(x, 4f);
		}
		else{
			physicsbody.velocity = new Vector2(-x, 4f);
		}
	}
	
	public void PlayBalrogRushPunchSound(){
		AudioSource.PlayClipAtPoint(balrogRushPunchSound, transform.position);
	}
	
}
