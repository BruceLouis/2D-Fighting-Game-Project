using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sagat : MonoBehaviour {

    [SerializeField] GameObject projectile, superProjectile;
    [SerializeField] AudioClip tigerShotSound, tigerShotCreatedSound, tigerUppercutSound;
    [SerializeField] AudioClip introSound, victorySound;
    [SerializeField] int numTigerCannons;

    private Character character;
    private Animator animator;
    private Rigidbody2D physicsbody;

    private int uppercutType;

    // Use this for initialization
    void Start ()
    {
        character = GetComponent<Character>();
        animator = GetComponent<Animator>();
        physicsbody = GetComponent<Rigidbody2D>();
    }
	
    void TigerShotRelease(int upperOrLower)
    {
        Vector3 height = Vector3.zero;
        int attackType;
        Vector3 offset = new Vector3(0.9f, 0f, 0f);
        GameObject tigerShot = Instantiate(projectile);
		Rigidbody2D rigidbody = tigerShot.GetComponent<Rigidbody2D>();
		SpriteRenderer tigerShotSprite = tigerShot.GetComponentInChildren<SpriteRenderer>();
		AudioSource.PlayClipAtPoint(tigerShotCreatedSound, transform.position);
        if (upperOrLower == 0)
        {
            //upper tiger shot
            height = new Vector3(0f, 0.4f, 0f);
            attackType = animator.GetInteger("upperTigerShotType");
        }
        else
        {
            //lower tiger shot
            attackType = animator.GetInteger("lowerTigerShotType");
            tigerShot.GetComponent<Projectile>().moveType = Projectile.MoveType.low;
        }
        ProjectileInitialize (offset, height, tigerShot); 
		character.GetSuper += 4.5f;
		switch(attackType)
        {
            case 0:
                ProjectileVelocity(rigidbody, tigerShotSprite, 1.75f);
                break;

            case 1:
                ProjectileVelocity(rigidbody, tigerShotSprite, 2.75f);
                break;
			
		    default:
                ProjectileVelocity(rigidbody, tigerShotSprite, 3.75f);
			    break;
		}		
	}

    void TigerCannonRelease()
    {
        Vector3 height = new Vector3(0f, 0.4f, 0f);
        Vector3 offset = new Vector3(0.9f, 0f, 0f);
        GameObject[] tigerCannons = new GameObject[numTigerCannons];
        Rigidbody2D[] rigidbodies = new Rigidbody2D[numTigerCannons];
        SpriteRenderer[] tigerShotSprite = new SpriteRenderer[numTigerCannons];

        StartCoroutine(CreateTigerCannons(height, offset, tigerCannons, rigidbodies, tigerShotSprite));

        GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Characters";
        AudioSource.PlayClipAtPoint(tigerShotCreatedSound, transform.position);
        character.GetSuper = 0f;
    }

    void ProjectileVelocity(Rigidbody2D rigidbody, SpriteRenderer tigerShotSprite, float speed)
    {
        if (character.transform.localScale.x == 1)
        {
            rigidbody.velocity = new Vector2(speed, 0f);
        }
        else
        {
            rigidbody.velocity = new Vector2(-speed, 0f);
        }
    }

    void ProjectileInitialize(Vector3 offset, Vector3 height, GameObject projectile)
    {
        if (animator.GetInteger("tigerShotOwner") == 1)
        {
            projectile.gameObject.layer = LayerMask.NameToLayer("ProjectileP1");
            projectile.gameObject.tag = "Player1";
            projectile.transform.parent = GameObject.Find("ProjectileP1Parent").transform;
        }
        else
        {
            projectile.gameObject.layer = LayerMask.NameToLayer("ProjectileP2");
            projectile.gameObject.tag = "Player2";
            projectile.transform.parent = GameObject.Find("ProjectileP2Parent").transform;
        }
        if (character.transform.localScale.x == 1)
        {
            projectile.transform.position = transform.position + offset + height;
        }
        else
        {
            projectile.transform.position = transform.position - offset + height;
            projectile.transform.localScale = new Vector3(-1f, transform.localScale.y, transform.localScale.z);
        }
    }

    IEnumerator CreateTigerCannons(Vector3 height, Vector3 offset, GameObject[] projectiles, Rigidbody2D[] rigidbodies, SpriteRenderer[] sprites)
    {
        for (int i = 0; i < numTigerCannons; i++)
        {
            projectiles[i] = Instantiate(superProjectile);
            rigidbodies[i] = projectiles[i].GetComponent<Rigidbody2D>();
            sprites[i] = projectiles[i].GetComponentInChildren<SpriteRenderer>();
            ProjectileInitialize(offset, height, projectiles[i]);
            ProjectileVelocity(rigidbodies[i], sprites[i], 3.75f);
            yield return new WaitForSeconds(0.001f);
        }
    }

    void SagatTigerUppercut()
    {
        uppercutType = animator.GetInteger("tigerUppercutPunchType");
        if (animator.GetBool("isLiftingOff") == false)
        {
            switch (animator.GetInteger("tigerUppercutPunchType"))
            {
                case 0:
                    character.MoveProperties(30f, 20f, 5f, 70f, 2, 3, 2, 6f);
                    break;
                case 1:
                    character.MoveProperties(40f, 22.5f, 7.5f, 80f, 2, 3, 2, 6.5f);
                    break;
                default:
                    character.MoveProperties(60f, 25f, 10f, 90f, 2, 3, 2, 7f);
                    break;
            }
        }
    }

    void TigerUppercutLiftOff()
    {
        switch (animator.GetInteger("tigerUppercutPunchType"))
        {
            case 0:
                character.TakeOffVelocity(0.5f, 3f);
                break;

            case 1:
                character.TakeOffVelocity(0.75f, 3.5f);
                break;

            default:
                character.TakeOffVelocity(1f, 4f);
                break;
        }
        character.PlayNormalAttackSound();
    }
    
    void TigerKnee()
    {
        if (animator.GetBool("isLiftingOff") == false)
        {
            switch (animator.GetInteger("tigerKneeKickType"))
            {
                case 0:
                    character.MoveProperties(50f, 20f, 10f, 50f, 2, 2, 1);
                    break;
                case 1:
                    character.MoveProperties(50f, 25f, 10f, 55f, 2, 2, 1);
                    break;
                default:
                    character.MoveProperties(50f, 25f, 10f, 60f, 2, 2, 1);
                    break;
            }
        }
    }

    void TigerKneeLiftOff()
    {
        switch (animator.GetInteger("tigerKneeKickType"))
        {
            case 0:
                character.TakeOffVelocity(1.5f, 2.5f);
                break;
            case 1:
                character.TakeOffVelocity(2f, 2.5f);
                break;
            default:
                character.TakeOffVelocity(2.5f, 2.5f);
                break;
        }
        character.PlayNormalAttackSound();
    }

    void PlayTigerShotSound()
    {
        AudioSource.PlayClipAtPoint(tigerShotSound, transform.position);
    }

    void PlayTigerUppercutSound()
    {
        AudioSource.PlayClipAtPoint(tigerUppercutSound, transform.position);
    }

    void PlayIntroSound()
    {
        AudioSource.PlayClipAtPoint(introSound, transform.position);
    }

    void PlayVictorySound()
    {
        AudioSource.PlayClipAtPoint(victorySound, transform.position);
    }
}
