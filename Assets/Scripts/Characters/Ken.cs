using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ken : MonoBehaviour
{
    [SerializeField] GameObject projectile;
    [SerializeField] AudioClip hadoukenSound, shoryukenSound, shinryukenSound, flameSound;
    [SerializeField] AudioClip hurricaneKickSound, hadoukenCreatedSound, superStartSound;

    private Character character;
    private Animator animator;
    private Rigidbody2D physicsbody;

    private int shoryukenType;
    private bool hurricaneActive, shinryukenActive;

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
        shinryukenActive = animator.GetBool("superActive");
        if (hurricaneActive)
        {
            character.AtTheCorner();
        }
        else if (shinryukenActive)
        {
            physicsbody.isKinematic = true;
        }
        else
        {
            physicsbody.isKinematic = false;
        }
    }

    public void HadoukenRelease()
    {
        Vector3 xOffset = new Vector3(0.75f, 0f, 0f);
        Vector3 yOffset = new Vector3(0f, 0.05f, 0f);
        GameObject hadouken = Instantiate(projectile);
        Projectile hadoukenProjectile = hadouken.GetComponent<Projectile>();
        Rigidbody2D rigidbody = hadouken.GetComponent<Rigidbody2D>();
        SpriteRenderer hadoukenSprite = hadouken.GetComponentInChildren<SpriteRenderer>();
        AudioSource.PlayClipAtPoint(hadoukenCreatedSound, transform.position);
        character.GetSuper += 4.5f;
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
        switch (animator.GetInteger("hadoukenPunchType"))
        {
            case 0:
                SetProjectileVelocityAndSprite(hadoukenProjectile, rigidbody, hadoukenSprite, 2f);
                break;

            case 1:
                SetProjectileVelocityAndSprite(hadoukenProjectile, rigidbody, hadoukenSprite, 2.5f);
                break;

            default:
                SetProjectileVelocityAndSprite(hadoukenProjectile, rigidbody, hadoukenSprite, 3f);
                break;
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

    void KenShoryuken()
    {
        shoryukenType = animator.GetInteger("shoryukenPunchType");
        if (animator.GetBool("isLiftingOff") == false)
        {
            switch (animator.GetInteger("shoryukenPunchType"))
            {
                case 0:
                    character.MoveProperties(30f, 20f, 5f, 80f, 2, 3, 2, 5f);
                    break;
                case 1:
                    character.MoveProperties(40f, 25f, 7.5f, 85f, 2, 3, 2, 5f);
                    break;
                default:
                    character.MoveProperties(60f, 25f, 10f, 90f, 2, 3, 2, 5f);
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
                character.TakeOffVelocity(1.5f, 4f);
                break;

            default:
                character.TakeOffVelocity(2f, 5f);
                break;
        }
        character.PlayNormalAttackSound();
    }

    void KenRoll()
    {
        if (animator.GetBool("isLiftingOff") == false)
        {
            switch (animator.GetInteger("rollKickType"))
            {
                case 0:
                    character.TakeOffVelocity(2f, 0f);
                    break;
                case 1:
                    character.TakeOffVelocity(2.5f, 0f);
                    break;
                default:
                    character.TakeOffVelocity(3f, 0f);
                    break;
            }
            animator.SetBool("isRolling", true);
        }
    }

    void ShinryukenLiftOff()
    {
        animator.SetBool("superActive", true);
        GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Characters";
        character.TakeOffVelocity(0f, 1.25f);
        character.PlayNormalAttackSound();
        AudioSource.PlayClipAtPoint(shinryukenSound, transform.position);
    }

    void ShinryukenProperties(int firstOrFinal)
    {
        if (firstOrFinal == 0)
        {
            character.MoveProperties(60f, 25f, 10f, 30f, 2, 7, 2, 0f);
        }
        else
        {
            character.MoveProperties(60f, 25f, 10f, 75f, 2, 2, 2, 0f);
        }
    }
    void ResetShinryukenActive()
    {
        animator.SetBool("superActive", false);
    }
    
    void KenFinishedRolling()
    {
        animator.SetBool("isRolling", false);
    }

    void KenHurricaneKickLiftOff()
    {
        character.TakeOffVelocity(1f, 1.5f);
        character.MoveProperties(45f, 20f, 10f, 17f, 2, 4, 1);
    }

    void KenHurricaneKickFloat()
    {
        if (!animator.GetBool("isInHitStun") && !animator.GetBool("isKnockedDown"))
        {
            physicsbody.velocity = new Vector2(physicsbody.velocity.x, 0f);
        }
        character.MoveProperties(40f, 20f, 10f, 17f, 2, 4, 1);
    }

    void KenHurricaneLanding()
    {
        animator.SetBool("hurricaneKickActive", false);
    }

    void PlayHadoukenSound()
    {
        AudioSource.PlayClipAtPoint(hadoukenSound, transform.position);
    }

    void PlayShoryukenSound()
    {
        AudioSource.PlayClipAtPoint(shoryukenSound, transform.position);
    }

    void PlayFlamesSound()
    {
        AudioSource.PlayClipAtPoint(flameSound, transform.position);
    }

    void PlayHurricaneKickSound()
    {
        AudioSource.PlayClipAtPoint(hurricaneKickSound, transform.position);
    }

    void PlaySuperStartSound()
    {
        AudioSource.PlayClipAtPoint(superStartSound, transform.position);
    }

    public int GetShoryukenType()
    {
        return shoryukenType;
    }

    public bool GetShinryukenActive()
    {
        return shinryukenActive;
    }
}
