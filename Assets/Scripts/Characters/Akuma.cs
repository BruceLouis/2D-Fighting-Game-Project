using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Akuma : MonoBehaviour
{

    [SerializeField] GameObject projectile;
    [SerializeField] AudioClip specialAttackSound1, specialAttackSound2, specialAttackSound3;
    [SerializeField] AudioClip stompSound, hadoukenCreatedSound, akumaDemonTravel;

    private Character character;
    private Animator animator;
    private Rigidbody2D physicsbody;
    private Vector3 startPos, endPos;

    private int shoryukenType;
    private float hadoukenAngle, amountTimeTravelledTimer;
    private bool hurricaneActive, diveKickActive, hyakkishuActive;

    // Use this for initialization
    void Start()
    {
        character = GetComponent<Character>();
        animator = GetComponent<Animator>();
        physicsbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        hurricaneActive = animator.GetBool("hurricaneKickActive");
        diveKickActive = animator.GetBool("diveKickActive");
        hyakkishuActive = animator.GetBool("hyakkishuActive");
        if (hurricaneActive)
        {
            character.AtTheCorner();
        }
        else if (animator.GetBool("isShunGokuSatsuInMotion"))
        {
            character.AtTheCorner();
            amountTimeTravelledTimer--;
            if (amountTimeTravelledTimer <= 0f)
            {
                animator.SetBool("isShunGokuSatsuInMotion", false);
                animator.SetBool("shunGokuSatsuActive", false);
            }
        }
        else if (diveKickActive)
        {
            character.AtTheCorner();
            physicsbody.isKinematic = true;
        }
        else
        {
            physicsbody.isKinematic = false;
        }

        if (hyakkishuActive && animator.GetBool("isAirborne"))
        {
            physicsbody.gravityScale = 2f;
        }
        else
        {
            physicsbody.gravityScale = 1f;
        }
    }

    void HadoukenRelease()
    {
        Vector3 xOffset = new Vector3(0.75f, 0f, 0f);
        GameObject hadouken = Instantiate(projectile);
        Projectile hadoukenProjectile = hadouken.GetComponent<Projectile>();
        Rigidbody2D rigidbody = hadouken.GetComponent<Rigidbody2D>();
        SpriteRenderer hadoukenSprite = hadouken.GetComponentInChildren<SpriteRenderer>();
        AudioSource.PlayClipAtPoint(hadoukenCreatedSound, transform.position);
        HadoukenInitialize(xOffset, Vector3.zero, hadouken);
        character.GetSuper += 4.5f;
        switch (animator.GetInteger("hadoukenPunchType"))
        {
            case 0:
                SetProjectileVelocityAndSprite(hadoukenProjectile, rigidbody, hadoukenSprite, 2.25f);
                break;

            case 1:
                SetProjectileVelocityAndSprite(hadoukenProjectile, rigidbody, hadoukenSprite, 2.75f);
                break;

            default:
                SetProjectileVelocityAndSprite(hadoukenProjectile, rigidbody, hadoukenSprite, 3.25f);
                break;
        }

    }

    void AirHadoukenRelease()
    {
        Vector3 xOffset = new Vector3(0.75f, 0f, 0f);
        Vector3 yOffset = new Vector3(0f, 0.05f, 0f);
        GameObject hadouken = Instantiate(projectile);
        Projectile hadoukenProjectile = hadouken.GetComponent<Projectile>();
        Rigidbody2D rigidbody = hadouken.GetComponent<Rigidbody2D>();
        AudioSource.PlayClipAtPoint(hadoukenCreatedSound, transform.position);
        HadoukenInitialize(xOffset, yOffset, hadouken);
        character.GetSuper += 4.5f;
        switch (animator.GetInteger("hadoukenPunchType"))
        {
            case 0:
                SetAirProjectileVelocityAndSprite(hadoukenProjectile, rigidbody, 0.5f, 1.5f);
                break;

            case 1:
                SetAirProjectileVelocityAndSprite(hadoukenProjectile, rigidbody, 1f, 1.5f);
                break;

            default:
                SetAirProjectileVelocityAndSprite(hadoukenProjectile, rigidbody, 1.5f, 1.5f);
                break;
        }
        //air hadouken angle calculated using tan2 		
        hadoukenAngle = Mathf.Atan2(rigidbody.velocity.y, rigidbody.velocity.x) * Mathf.Rad2Deg;
        hadouken.transform.rotation = Quaternion.AngleAxis(hadoukenAngle, Vector3.forward);
    }

    void HadoukenInitialize(Vector3 xOffset, Vector3 yOffset, GameObject hadouken)
    {
        if (animator.GetInteger("hadoukenOwner") == 1)
        {
            hadouken.gameObject.layer = LayerMask.NameToLayer("ProjectileP1");
            hadouken.gameObject.tag = "Player1";
            hadouken.transform.parent = GameObject.Find("ProjectileP1Parent").transform;
        }
        else
        {
            hadouken.gameObject.layer = LayerMask.NameToLayer("ProjectileP2");
            hadouken.gameObject.tag = "Player2";
            hadouken.transform.parent = GameObject.Find("ProjectileP2Parent").transform;
        }
        if (character.transform.localScale.x == 1)
        {
            hadouken.transform.position = transform.position + xOffset + yOffset;
        }
        else
        {
            hadouken.transform.position = transform.position - xOffset + yOffset;
        }
    }

    void SetProjectileVelocityAndSprite(Projectile projectile, Rigidbody2D rigidbody, SpriteRenderer sprite, float x)
    {
        if (character.transform.localScale.x == 1)
        {
            rigidbody.velocity = new Vector2(x, 0f);
            projectile.XVelocity = x;
        }
        else
        {
            rigidbody.velocity = new Vector2(-x, 0f);
            projectile.XVelocity = -x;
            sprite.flipX = true;
        }
    }

    void SetAirProjectileVelocityAndSprite(Projectile projectile, Rigidbody2D rigidbody, float x, float y)
    {
        if (character.transform.localScale.x == 1)
        {
            rigidbody.velocity = new Vector2(x, -y);
            projectile.XVelocity = x;
        }
        else
        {
            rigidbody.velocity = new Vector2(-x, -y);
            projectile.XVelocity = -x;
        }
        projectile.YVelocity = -y;
    }

    void AkumaShoryuken()
    {
        shoryukenType = animator.GetInteger("shoryukenPunchType");
        if (animator.GetBool("isLiftingOff") == false)
        {
            switch (animator.GetInteger("shoryukenPunchType"))
            {
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

    void ShoryukenLiftOff()
    {
        switch (animator.GetInteger("shoryukenPunchType"))
        {
            case 0:
                character.TakeOffVelocity(1f, 3f);
                break;

            case 1:
                character.TakeOffVelocity(1.5f, 3.75f);
                break;

            default:
                character.TakeOffVelocity(2f, 4.5f);
                break;
        }
        character.PlayNormalAttackSound();
    }

    void AkumaHurricaneKickLiftOff()
    {
        character.TakeOffVelocity(1.25f, 1.25f);
    }

    void AkumaHurricaneKickFloat(float timerHitStun)
    {
        if (!animator.GetBool("isInHitStun") && !animator.GetBool("isKnockedDown"))
        {
            physicsbody.velocity = new Vector2(physicsbody.velocity.x, 0f);
        }
        character.MoveProperties(timerHitStun, 20f, 10f, 30f, 2, 7, 1, 4.5f);
    }

    void AkumaHurricaneLanding()
    {
        animator.SetBool("hurricaneKickActive", false);
    }

    void AkumaHyakkishuLiftOff()
    {
        if (animator.GetBool("isAirborne") == false && animator.GetBool("isInHitStun") == false
            && animator.GetBool("isKnockedDown") == false && animator.GetBool("isInBlockStun") == false
            && animator.GetBool("isThrown") == false && animator.GetBool("isMidAirRecovering") == false
            && animator.GetBool("isLiftingOff") == true)
        {
            switch (animator.GetInteger("hyakkishuKickType"))
            {
                case 0:
                    character.TakeOffVelocity(1f, 6f);
                    break;
                case 1:
                    character.TakeOffVelocity(2f, 6f);
                    break;
                default:
                    character.TakeOffVelocity(3f, 6f);
                    break;
            }
        }
    }

    void AkumaHyakkiGozanProperties()
    {
        character.TakeOffVelocity(1.5f, 0f);
        character.MoveProperties(40f, 20f, 10f, 35f, 0, 1, 1, 4.5f);
    }

    void AkumaHyakkiGoshoProperties()
    {
        character.MoveProperties(40f, 20f, 10f, 40f, 1, 2, 1, 4.5f);
    }

    void AkumaDiveKickProperties()
    {
        character.TakeOffVelocity(0f, 0f);
        character.MoveProperties(40f, 20f, 10f, 20f, 2, 4, 0, 4.5f);
    }

    void AkumaDiveKick()
    {
        character.TakeOffVelocity(1.25f, -3.25f);
    }

    void AkumaShunGokuSatsuInMotion(float amountTimeTravelled)
    {
        amountTimeTravelledTimer = amountTimeTravelled;
        GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Characters";
        AudioSource.PlayClipAtPoint(akumaDemonTravel, transform.position);
        if (character.side == Character.Side.P1)
        {
            physicsbody.velocity = new Vector2(2.5f, 0f);
        }
        else
        {
            physicsbody.velocity = new Vector2(-2.5f, 0f);
        }
        animator.SetBool("isShunGokuSatsuInMotion", true);
    }

    void AkumaShunGokuSatsuReachesTarget()
    {
        animator.SetBool("isShunGokuSatsuInMotion", false);
        physicsbody.velocity *= 0.2f;
    }

    void AkumaShunGokuSatsu(int onOrOff)
    {
        if (onOrOff == 1)
        {
            TimeControl.gettingDemoned = true;
        }
        else
        {
            TimeControl.gettingDemoned = false;
        }
    }

    void PlaySpecialAttackSound1()
    {
        AudioSource.PlayClipAtPoint(specialAttackSound1, transform.position);
    }

    void PlaySpecialAttackSound2()
    {
        AudioSource.PlayClipAtPoint(specialAttackSound2, transform.position);
    }

    void PlaySpecialAttackSound3()
    {
        AudioSource.PlayClipAtPoint(specialAttackSound3, transform.position);
    }

    void PlayStompSound()
    {
        AudioSource.PlayClipAtPoint(stompSound, transform.position);
    }
}
