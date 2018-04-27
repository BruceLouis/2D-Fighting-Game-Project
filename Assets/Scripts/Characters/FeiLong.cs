using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeiLong : MonoBehaviour {
	
	public GameObject feiLongFlame;
	public AudioClip rekkaFirstSound, rekkaSecondSound, rekkaThirdSound;
	public AudioClip flameKickSound, chickenWingSound, victorySound;
	
	private Character character;
	private Animator animator; 
	private Rigidbody2D physicsbody;
	private GameObject flameEffect;
		
	// Use this for initialization
	void Start () {
		character = GetComponent<Character>();
		animator = GetComponent<Animator>();
		physicsbody = GetComponent<Rigidbody2D>();		
	}
	
	void Update () {
		if (!animator.GetBool("shienKyakuActive") && GameObject.Find ("FeiLongFlames(Clone)")){
			FlameIsGone();
		}
	}
	
	public void FeiLongCloseJab(){
		if (animator.GetBool("isLiftingOff") == false){
			AudioSource.PlayClipAtPoint(character.normalAttackSound, transform.position);
			animator.Play ("StandJabClose", 0);
			character.MoveProperties(20f, 20f, 7.5f, 25f, 2, 0);
		}
	}
	public void FeiLongCloseStrong(){
		if (animator.GetBool("isLiftingOff") == false){
			AudioSource.PlayClipAtPoint(character.normalAttackSound, transform.position);
			animator.Play ("StandStrongClose", 0);
			character.MoveProperties(30f, 25f, 10f, 40f, 2, 0);
		}
	}
	public void FeiLongCloseFierce(){
		if (animator.GetBool("isLiftingOff") == false){
			AudioSource.PlayClipAtPoint(character.normalAttackSound, transform.position);
			animator.Play ("StandFierceClose", 0);
			character.MoveProperties(40f, 20f, 10f, 60f, 2, 0);
		}
	}
	
	public void FeiLongShienKyaku(){
		if (animator.GetBool("isLiftingOff") == false){	
			switch(animator.GetInteger("shienKyakuKickType")){
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
	
	public void ShienKyakuLiftOff(){		
		switch(animator.GetInteger("shienKyakuKickType")){
			case 0:
				FeiLongTakeOffVelocity(0f, 3f);
				break;
			case 1:
				FeiLongTakeOffVelocity(0f, 4f);
				break;
			default:
				FeiLongTakeOffVelocity(0f, 4.5f);
				break;
		}
		Vector3 angle = new Vector3(-90f, 0f, 0f); 
		flameEffect = Instantiate(feiLongFlame, transform.position, Quaternion.Euler(angle)) as GameObject;
		flameEffect.transform.parent = gameObject.transform;
		AudioSource.PlayClipAtPoint(character.normalAttackSound,transform.position);
	}
	
	
	public void FeiLongRekkaKun(){
		if (animator.GetBool("isLiftingOff") == false){	
			switch(animator.GetInteger("rekkaKunKickType")){
				case 0:
					character.MoveProperties(40f, 20f, 10f, 30f, 2, 4);
					break;
				case 1:
					character.MoveProperties(40f, 25f, 10f, 32f, 2, 4);
					break;
				default:
					character.MoveProperties(40f, 25f, 10f, 35f, 2, 4);
					break;
			}
		}
	}	
	
	public void RekkaKunLiftOff(){		
		switch(animator.GetInteger("rekkaKunKickType")){
			case 0:
				FeiLongTakeOffVelocity (2.5f, 3f);
				break;
			case 1:
				FeiLongTakeOffVelocity (3f, 3f);
				break;
			default:
				FeiLongTakeOffVelocity (3.5f, 3f);
				break;
		}
		AudioSource.PlayClipAtPoint(character.normalAttackSound,transform.position);
	}
	
	public void FeiLongRekkaKen(){
		if (animator.GetBool("isLiftingOff") == false){	
			switch(animator.GetInteger("rekkaPunchType")){
				case 0:
					character.MoveProperties(40f, 30f, 15f, 20f, 2, 5);
					FeiLongTakeOffVelocity(2f, 0f);
					break;
				case 1:
					character.MoveProperties(40f, 30f, 15f, 25f, 2, 5);
					FeiLongTakeOffVelocity(2.5f, 0f);
					break;
				default:
					character.MoveProperties(40f, 30f, 15f, 30f, 2, 5);
					FeiLongTakeOffVelocity(3f, 0f);
					break;
			}
			AudioSource.PlayClipAtPoint(character.normalAttackSound,transform.position);
		}
	}	
	
	public void FeiLongRekkaKenFinal(){
		if (animator.GetBool("isLiftingOff") == false){	
			switch(animator.GetInteger("rekkaPunchType")){
			 	case 0:
					character.MoveProperties(40f, 20f, 15f, 35f, 2, 2);
					FeiLongTakeOffVelocity(2.5f, 0f);
					break;
				case 1:
					character.MoveProperties(50f, 22.5f, 20f, 40f, 2, 2);
					FeiLongTakeOffVelocity(3.5f, 0f);
					break;
				default:
					character.MoveProperties(60f, 25f, 20f, 50f, 2, 2);
					FeiLongTakeOffVelocity(4f, 0f);
					break;
			}
			AudioSource.PlayClipAtPoint(character.normalAttackSound,transform.position);
		}
	}	

	void FeiLongTakeOffVelocity (float x, float y){
		if (character.transform.localScale.x == 1) {
			physicsbody.velocity = new Vector2 (x, y);
		}
		else {
			physicsbody.velocity = new Vector2 (-x, y);
		}
	}
	
	public void PlayRekkaFirstSound(){
		AudioSource.PlayClipAtPoint(rekkaFirstSound, transform.position);
	}
	
	public void PlayRekkaSecondSound(){
		AudioSource.PlayClipAtPoint(rekkaSecondSound, transform.position);
	}
	
	public void PlayRekkaThirdSound(){
		AudioSource.PlayClipAtPoint(rekkaThirdSound, transform.position);
	}
	
	public void PlayFlameKickSound(){
		AudioSource.PlayClipAtPoint(flameKickSound, transform.position);
	}
	
	public void PlayChickenWingSound(){
		AudioSource.PlayClipAtPoint(chickenWingSound, transform.position);
	}
	
	public void PlayFlamesSound(){
		AudioSource.PlayClipAtPoint(character.flameSound, transform.position);
	}
	
	public void PlayVictorySound(){
		AudioSource.PlayClipAtPoint(victorySound, transform.position);
	}
	
	public void FlamePositionFront(){
		flameEffect.transform.rotation = Quaternion.Euler (new Vector3(-90f, 0f, 0f)); 
		if (character.side == Character.Side.P1){
			flameEffect.transform.position = new Vector3(transform.position.x + 0.13f, transform.position.y + 0.125f, transform.position.z);
		}
		else{
			flameEffect.transform.position = new Vector3(transform.position.x - 0.13f, transform.position.y + 0.125f, transform.position.z);
		}
	}
	
	public void FlamePositionBack(){
		flameEffect.transform.rotation = Quaternion.Euler (new Vector3(-90f, 0f, 0f));
		if (character.side == Character.Side.P2){
			flameEffect.transform.position = new Vector3(transform.position.x + 0.13f, transform.position.y + 0.125f, transform.position.z);
		}
		else{
			flameEffect.transform.position = new Vector3(transform.position.x - 0.13f, transform.position.y + 0.125f, transform.position.z);
		}
	}
	
	public void FlamePositionRight(){
		if (character.side == Character.Side.P1){
			flameEffect.transform.rotation = Quaternion.Euler (new Vector3(-80f, -90f, 90f)); 
			flameEffect.transform.position = new Vector3(transform.position.x + 0.235f, transform.position.y + 0.125f, transform.position.z);
		}
		else{
			flameEffect.transform.rotation = Quaternion.Euler (new Vector3(-100f, -90f, 90f)); 
			flameEffect.transform.position = new Vector3(transform.position.x - 0.235f, transform.position.y + 0.125f, transform.position.z);
		}
	}
	
	public void FlamePositionLeft(){
		if (character.side == Character.Side.P2){
			flameEffect.transform.rotation = Quaternion.Euler (new Vector3(-80f, -90f, 90f)); 
			flameEffect.transform.position = new Vector3(transform.position.x + 0.235f, transform.position.y + 0.125f, transform.position.z);
		}
		else{
			flameEffect.transform.rotation = Quaternion.Euler (new Vector3(-100f, -90f, 90f)); 
			flameEffect.transform.position = new Vector3(transform.position.x - 0.235f, transform.position.y + 0.125f, transform.position.z);
		}
	}
		
	public void FlameIsGone(){
		Destroy(flameEffect);
	}
}
