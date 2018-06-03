using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	[SerializeField] float damage, pushBack;
	[SerializeField] float hitStun, blockStun;
    [SerializeField] int numHits;
	
	public GameObject shoryukenSpark;
	public GameObject blockSpark;
	
	public AudioClip connectedSound;
	public AudioClip blockedSound;

    public enum MoveType { low, mid, high};
    [HideInInspector]
    public MoveType moveType = MoveType.high;
	
	private ComboCounter comboCounter;
	private Animator animator;
	private Rigidbody2D physicsBody;
    private float timer;

    void Start(){
		animator = GetComponent<Animator>();
		physicsBody = GetComponent<Rigidbody2D>();
		comboCounter = FindObjectOfType<ComboCounter>();
		animator.SetBool("madeContact", false); 
	}
	
	void StopMovingProjectile(){
		physicsBody.velocity = new Vector2(0f,0f);
	}	
	
	void DestroyProjectile(){
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
	
	void CharacterKOed(Character receiver, Rigidbody2D recRigid, Animator recAnim){
		TimeControl.slowDownTimer = 100f;				
		recAnim.Play("KnockDownBlendTree", 0);
		if (receiver.side == Character.Side.P1){
			recRigid.velocity = new Vector2(-2f, 4f);
		}
		else{
			recRigid.velocity = new Vector2(2f, 4f);
		}
    }

    void OnTriggerStay2D(Collider2D collider){
        HurtBox hurtBox = collider.gameObject.GetComponentInParent<HurtBox>();
        Animator hurtCharAnimator = collider.gameObject.GetComponentInParent<Animator>();
        Projectile otherProjectile = collider.gameObject.GetComponent<Projectile>();
        if (hurtBox && hurtBox.gameObject.tag != gameObject.tag && !hurtBox.GetHurtBoxCollided() && !animator.GetBool("madeContact")){
            CountNumHits();
            hurtBox.SetHurtBoxCollided(true);
            Character hurtCharacter = hurtBox.GetComponentInParent<Character>();
            Rigidbody2D hurtPhysicsbody = hurtCharacter.GetComponent<Rigidbody2D>();

            if (hurtCharacter.GetBackPressed() == true && hurtCharAnimator.GetBool("isAttacking") == false
                && hurtCharAnimator.GetBool("isLiftingOff") == false && hurtCharAnimator.GetBool("isAirborne") == false
                && hurtCharAnimator.GetBool("isInHitStun") == false){
                //if attack is blocked
                if ((moveType == MoveType.low && hurtCharAnimator.GetBool("isCrouching") == false)
                    || (moveType == MoveType.mid && hurtCharAnimator.GetBool("isStanding") == false)){
                    ProjectileLanded(hurtCharAnimator, hurtCharacter, hurtPhysicsbody);
                }
                else{
                    ProjectileBlocked(hurtCharAnimator, hurtCharacter, hurtPhysicsbody);
                }
            }
            else{
                ProjectileLanded(hurtCharAnimator, hurtCharacter, hurtPhysicsbody);
            }

        }
        if (otherProjectile || collider.gameObject.GetComponent<Ground>() && !animator.GetBool("madeContact")){
            CountNumHits();
        }
    }

    void CountNumHits(){
        numHits--;
        if (numHits <= 0){
            animator.SetBool("madeContact", true);
        }
    }

    void ProjectileLanded(Animator hurtCharAnimator, Character hurtCharacter, Rigidbody2D hurtPhysicsbody){
        TimeControl.slowDown = true;
        AudioSource.PlayClipAtPoint(connectedSound, transform.position);
        hurtCharacter.SetDamage(damage);
        if (gameObject.tag == "Player1"){
            if (hurtCharAnimator.GetBool("isInHitStun")){
                comboCounter.GetComboCountP1++;
                comboCounter.GetStartTimer = false;
                comboCounter.ResetComboFinishedTimer();
            }
        }
        else if (gameObject.tag == "Player2"){
            if (hurtCharAnimator.GetBool("isInHitStun")){
                comboCounter.GetComboCountP2++;
                comboCounter.GetStartTimer = false;
                comboCounter.ResetComboFinishedTimer();
            }
        }
        if (hurtCharacter.GetHealth() <= 0f){
            CharacterKOed(hurtCharacter, hurtPhysicsbody, hurtCharAnimator);
        }
        else{
            TimeControl.slowDownTimer = 30f;
            timer = hitStun * 0.2f;
            if (hurtCharAnimator.GetBool("isAirborne") == true){
                hurtCharAnimator.Play("KnockDownBlendTree", 0, 0f);
                if (hurtCharacter.side == Character.Side.P2){
                    hurtPhysicsbody.velocity = new Vector2(pushBack * 0.15f, 3f);
                }
                else{
                    hurtPhysicsbody.velocity = new Vector2(-pushBack * 0.15f, 3f);
                }
            }
            else{
                if (hurtCharAnimator.GetBool("isCrouching") == true){
                    hurtCharAnimator.Play("CrouchHit", 0, 0f);
                }
                else{
                    if (moveType == MoveType.low){
                        hurtCharAnimator.Play("LowHit", 0, 0f);
                    }
                    else{
                        hurtCharAnimator.Play("HighHit", 0, 0f);
                    }
                }
                PushBack(hurtCharacter, hurtPhysicsbody);
            }
            hurtCharAnimator.SetBool("isInHitStun", true);
            hurtCharAnimator.SetFloat("hitStunTimer", timer);
            Instantiate(shoryukenSpark, transform.position, Quaternion.identity);
        }
    }

    void ProjectileBlocked(Animator hurtCharAnimator, Character hurtCharacter, Rigidbody2D hurtPhysicsbody){
        AudioSource.PlayClipAtPoint(blockedSound, transform.position);
        timer = blockStun * 0.2f;
        hurtCharAnimator.SetBool("isInBlockStun", true);
        if (hurtCharAnimator.GetBool("isStanding") == true)
        {
            hurtCharAnimator.Play("StandBlockStun", 0, 0f);
            PushBack(hurtCharacter, hurtPhysicsbody);
        }
        else if (hurtCharAnimator.GetBool("isCrouching") == true)
        {
            hurtCharAnimator.Play("CrouchBlockStun", 0, 0f);
            PushBack(hurtCharacter, hurtPhysicsbody);
        }
        hurtCharAnimator.SetFloat("blockStunTimer", timer);
        Instantiate(blockSpark, transform.position, Quaternion.identity);
    }
}
