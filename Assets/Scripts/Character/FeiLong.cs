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
			if (animator.GetInteger("shienKyakuKickType") == 0){
				character.MoveProperties(30f, 20f, 5f, 80f, 2, 3);
			}
			else if (animator.GetInteger("shienKyakuKickType") == 1){
				character.MoveProperties(40f, 25f, 7.5f, 85f, 2, 3);
			}
			else{
				character.MoveProperties(60f, 25f, 10f, 90f, 2, 3);
			}
		}
	}	
	public void ShienKyakuLiftOff(){		
		if (animator.GetInteger("shienKyakuKickType") == 0){
			physicsbody.velocity = new Vector2(0f, 3f);
		}
		else if (animator.GetInteger("shienKyakuKickType") == 1){
			physicsbody.velocity = new Vector2(0f, 4f);
		}
		else{
			physicsbody.velocity = new Vector2(0f, 4.5f);
		}
		Vector3 angle = new Vector3(-90f, 0f, 0f); 
		flameEffect = Instantiate(feiLongFlame, transform.position, Quaternion.Euler(angle)) as GameObject;
		flameEffect.transform.parent = gameObject.transform;
		AudioSource.PlayClipAtPoint(character.normalAttackSound,transform.position);
		animator.SetBool("isAirborne", true);
	}
	
	
	public void FeiLongRekkaKun(){
		if (animator.GetBool("isLiftingOff") == false){	
			if (animator.GetInteger("rekkaKunKickType") == 0){
				character.MoveProperties(40f, 20f, 10f, 30f, 2, 4);
			}
			else if (animator.GetInteger("rekkaKunKickType") == 1){
				character.MoveProperties(45f, 25f, 10f, 32f, 2, 4);
			}
			else{
				character.MoveProperties(60f, 25f, 10f, 35f, 2, 4);
			}
		}
	}	
	
	public void RekkaKunLiftOff(){		
		if (animator.GetInteger("rekkaKunKickType") == 0){
			if (character.side == Character.Side.P1){
				physicsbody.velocity = new Vector2(2.5f, 3f);
			}
			else{
				physicsbody.velocity = new Vector2(-2.5f, 3f);
			}
		}
		else if (animator.GetInteger("rekkaKunKickType") == 1){
			if (character.side == Character.Side.P1){
				physicsbody.velocity = new Vector2(3f, 3f);
			}
			else{
				physicsbody.velocity = new Vector2(-3f, 3f);
			}
		}
		else{
			if (character.side == Character.Side.P1){
				physicsbody.velocity = new Vector2(3.5f, 3f);
			}
			else{
				physicsbody.velocity = new Vector2(-3.5f, 3f);
			}
		}
		AudioSource.PlayClipAtPoint(character.normalAttackSound,transform.position);
		animator.SetBool("isAirborne", true);
	}
	
	public void FeiLongRekkaKen(){
		if (animator.GetBool("isLiftingOff") == false){	
			if (animator.GetInteger("rekkaPunchType") == 0){
				character.MoveProperties(40f, 30f, 15f, 20f, 2, 5);
				if (character.side == Character.Side.P1){
					physicsbody.velocity = new Vector2(2f, 0f);
				}
				else{
					physicsbody.velocity = new Vector2(-2f, 0f);
				}
			}
			else if (animator.GetInteger("rekkaPunchType") == 1){
				character.MoveProperties(40f, 30f, 15f, 25f, 2, 5);
				if (character.side == Character.Side.P1){
					physicsbody.velocity = new Vector2(2.5f, 0f);
				}
				else{
					physicsbody.velocity = new Vector2(-2.5f, 0f);
				}
			}
			else{
				character.MoveProperties(40f, 30f, 15f, 30f, 2, 5);
				if (character.side == Character.Side.P1){
					physicsbody.velocity = new Vector2(3f, 0f);
				}
				else{
					physicsbody.velocity = new Vector2(-3f, 0f);
				}
			}
		}
		AudioSource.PlayClipAtPoint(character.normalAttackSound,transform.position);
	}	
	
	public void FeiLongRekkaKenFinal(){
		if (animator.GetBool("isLiftingOff") == false){	
			if (animator.GetInteger("rekkaPunchType") == 0){
				character.MoveProperties(30f, 20f, 15f, 35f, 2, 2);
				if (character.side == Character.Side.P1){
					physicsbody.velocity = new Vector2(2.5f, 0f);
				}
				else{
					physicsbody.velocity = new Vector2(-2.5f, 0f);
				}
			}
			else if (animator.GetInteger("rekkaPunchType") == 1){
				character.MoveProperties(40f, 25f, 20f, 40f, 2, 2);
				if (character.side == Character.Side.P1){
					physicsbody.velocity = new Vector2(3.5f, 0f);
				}
				else{
					physicsbody.velocity = new Vector2(-3.5f, 0f);
				}
			}
			else{
				character.MoveProperties(60f, 25f, 20f, 50f, 2, 2);
				if (character.side == Character.Side.P1){
					physicsbody.velocity = new Vector2(4f, 0f);
				}
				else{
					physicsbody.velocity = new Vector2(-4f, 0f);
				}
			}
		}
		AudioSource.PlayClipAtPoint(character.normalAttackSound,transform.position);
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
		flameEffect.transform.position = new Vector3(transform.position.x + 0.13f, transform.position.y + 0.125f, transform.position.z);
		flameEffect.transform.rotation = Quaternion.Euler (new Vector3(-90f, 0f, 0f)); 
	}
	
	public void FlamePositionBack(){
		flameEffect.transform.position = new Vector3(transform.position.x - 0.13f, transform.position.y + 0.125f, transform.position.z);
		flameEffect.transform.rotation = Quaternion.Euler (new Vector3(-90f, 0f, 0f));
	}
	
	public void FlamePositionRight(){
		flameEffect.transform.position = new Vector3(transform.position.x + 0.235f, transform.position.y + 0.125f, transform.position.z);
		flameEffect.transform.rotation = Quaternion.Euler (new Vector3(-80f, -90f, 90f)); 
	}
	
	public void FlamePositionLeft(){
		flameEffect.transform.position = new Vector3(transform.position.x - 0.235f, transform.position.y + 0.125f, transform.position.z);
		flameEffect.transform.rotation = Quaternion.Euler (new Vector3(-100f, -90f, 90f)); 
	}
		
	public void FlameIsGone(){
		Destroy(flameEffect);
	}
}
