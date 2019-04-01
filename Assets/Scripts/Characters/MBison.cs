using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBison : MonoBehaviour {

    [SerializeField] AudioClip superStartSound, psychoCrusherSound, psychoCrusherEffectSound, psychoSound, reverseSound;
    [SerializeField] AudioClip scissorKicksSound, headStompLaunchSound, headStompHitSound, somerSaultSound, slideSound;
    [SerializeField] float headStompGravity;

    private Character character;
    private Animator animator;
    private Rigidbody2D physicsbody;

    private bool kneePressNightmareActive, headStompActive, devilReverseActive;
    private float amountTimeTravelledTimer;
    private Vector2 direction;

    // Use this for initialization
    void Start ()
    {
        character = GetComponent<Character>();
        animator = GetComponent<Animator>();
        physicsbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        kneePressNightmareActive = animator.GetBool("superActive");
        headStompActive = animator.GetBool("headStompActive");
        devilReverseActive = animator.GetBool("devilReverseActive");

        if (animator.GetBool("isSweeping"))
        {
            character.AtTheCorner();
            amountTimeTravelledTimer--;
            physicsbody.isKinematic = true;
            if (amountTimeTravelledTimer <= 0f)
            {
                animator.SetBool("isSweeping", false);
            }
        }
        else if (animator.GetBool("psychoCrusherActive") || kneePressNightmareActive)
        {
            character.AtTheCorner();
            physicsbody.isKinematic = true;
        }
        else if (animator.GetBool("headStompLanded"))
        {
            physicsbody.isKinematic = true;
        }
        else
        {
            physicsbody.isKinematic = false;
        }
        
        if (animator.GetBool("reverseActive"))
        {
            WindForReverses(3f);
        }

        if (headStompActive || devilReverseActive)
        {
            physicsbody.gravityScale = headStompGravity;
        }
        else
        {
            physicsbody.gravityScale = 1f;
        }
    }
    
    void MBisonSweepStartUp(float amountTimeTravelled)
    {
        amountTimeTravelledTimer = amountTimeTravelled;
        animator.SetBool("isSweeping", true);
        if (!animator.GetBool("isInHitStun") && !animator.GetBool("isKnockedDown"))
        {
            character.TakeOffVelocity(2.5f, 0f);
        }
    }

    void SweepSlowdown()
    {
        if (!animator.GetBool("isInHitStun") && !animator.GetBool("isKnockedDown"))
        {
            physicsbody.velocity = new Vector2(physicsbody.velocity.x * 0.3f, physicsbody.velocity.y);
        }
    }

    void MBisonScissorKicks()
    {
        if (!animator.GetBool("isInHitStun") && !animator.GetBool("isKnockedDown"))
        {
            switch (animator.GetInteger("scissorKicksKickType"))
            {
                case 0:
                    character.TakeOffVelocity(2.5f, 1.5f);
                    break;
                case 1:
                    character.TakeOffVelocity(3f, 1.75f);
                    break;
                default:
                    character.TakeOffVelocity(3.5f, 2f);
                    break;
            }
        }
    }
    
    void MBisonKneePressNightmare()
    {
        if (!animator.GetBool("isInHitStun") && !animator.GetBool("isKnockedDown"))
        {
            character.TakeOffVelocity(3f, 0f);
        }
        GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Characters";
    }

    void MBisonSuperProperties(int type)
    {
        switch (type)
        {
            case 0:
                character.MoveProperties(40f, 30f, 5f, 50f, 2, 4, 1, 0f);
                break;
            case 1:
                character.MoveProperties(40f, 30f, 10f, 60f, 2, 4, 1, 0f);
                break;
            default:
                character.MoveProperties(40f, 20f, 12f, 100f, 0, 8, 1, 0f);
                break;
        }
    }

    void MBisonScissorKicksProperties(int order)
    {
        if (order <= 0)
        {
            switch (animator.GetInteger("scissorKicksKickType"))
            {
                case 0:
                    character.MoveProperties(40f, 20f, 5f, 25f, 2, 0, 1, 3.5f);
                    break;
                case 1:
                    character.MoveProperties(40f, 20f, 5f, 30f, 2, 0, 1, 3.75f);
                    break;
                default:
                    character.MoveProperties(40f, 20f, 5f, 35f, 2, 0, 1, 4f);
                    break;
            }
        }
        else
        {
            switch (animator.GetInteger("scissorKicksKickType"))
            {
                case 0:
                    character.MoveProperties(40f, 20f, 10f, 40f, 2, 2, 1, 5f);
                    break;
                case 1:
                    character.MoveProperties(40f, 20f, 10f, 45f, 2, 2, 1, 5.25f);
                    break;
                default:
                    character.MoveProperties(40f, 20f, 10f, 55f, 2, 2, 1, 5.5f);
                    break;
            }
        }
    }

    void MBisonPsychoCrusher()
    {
        if (!animator.GetBool("isInHitStun") && !animator.GetBool("isKnockedDown"))
        {
            switch (animator.GetInteger("psychoCrusherPunchType"))
            {
                case 0:
                    character.TakeOffVelocity(1f, 0f);
                    break;
                case 1:
                    character.TakeOffVelocity(2f, 0f);
                    break;
                default:
                    character.TakeOffVelocity(3f, 0f);
                    break;
            }
        }
        switch (animator.GetInteger("psychoCrusherPunchType"))
        {
            case 0:
                character.MoveProperties(40f, 20f, 5f, 60f, 2, 9, 1, 3.5f);
                break;
            case 1:
                character.MoveProperties(40f, 25f, 5f, 65f, 2, 9, 1, 3.75f);
                break;
            default:
                character.MoveProperties(40f, 30f, 5f, 70f, 2, 9, 1, 4f);
                break;
        }
    }

    void MBisonHeadStomp()
    {
        /*   we will use the cannonball solution here since this particular attack of M Bison's is very similar to how a cannonball works
         *   where m bison will launch towards the opponent's head (towards the opponent for now) regardless of where M Bison stands at
         *   the 1 in the parameter is multiplier. for the devil's reverse, the multiplier would be 1.5
         */

        if (!animator.GetBool("isInHitStun") && !animator.GetBool("isKnockedDown"))
        {
            CannonBallTrajectory(1f, headStompGravity);
        }
        switch (animator.GetInteger("headStompKickType"))
        {
            case 0:
                character.MoveProperties(20f, 15f, 0f, 30f, 1, 0, 1, 4f);
                break;
            case 1:
                character.MoveProperties(20f, 15f, 0f, 35f, 1, 0, 1, 4f);
                break;
            default:
                character.MoveProperties(20f, 15f, 0f, 40f, 1, 0, 1, 4f);
                break;
        }
    }

    void MBisonDevilReverse()
    {
        /*   we will use the cannonball solution here since this particular attack of M Bison's is very similar to how a cannonball works
         *   where m bison will launch towards the opponent's head (towards the opponent for now) regardless of where M Bison stands at
         *   the 1 in the parameter is multiplier. for the devil's reverse, the multiplier would be 1.5
         */

        if (!animator.GetBool("isInHitStun") && !animator.GetBool("isKnockedDown"))
        {
            CannonBallTrajectory(1.5f, headStompGravity);
            direction = character.side == Character.Side.P1 ? Vector2.right : Vector2.left;
        }
    }

    void CannonBallTrajectory(float multiplier, float scale)
    {
        /*  this could be improved by using time as the known constant instead so that we can calculate the angles
         *  on demand, rather than give magic numbers to the angles based on distance.
         */
        
        float angle, distance, xVelocity, yVelocity;
        float gravity = Physics2D.gravity.magnitude * scale;
        float timeTravelled = 0.85f;

        Vector3 targetPosition = GetComponentInParent<SharedProperties>().GetPositionOfOtherFighter();

        // Distance along the y axis between objects

        // Planar distance between objects
        Vector2 planarTarget = new Vector2(targetPosition.x, 0);
        Vector2 planarPostion = new Vector2(transform.position.x, 0);

        distance = Vector3.Distance(planarTarget, planarPostion) * multiplier;
        
        xVelocity = distance / timeTravelled;
        yVelocity = 0.5f * gravity * Mathf.Pow(timeTravelled, 2f);

        angle = Mathf.Atan(yVelocity / xVelocity);

        float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle)));
        Vector2 targetVelocity = new Vector2(initialVelocity * Mathf.Cos(angle), initialVelocity * Mathf.Sin(angle));
        
        // Rotate our velocity to match the direction between the two objects
        float angleBetweenObjects = Vector2.Angle(Vector2.right, planarTarget - planarPostion);
        Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector2.up) * targetVelocity;

        // Fire!
        physicsbody.velocity = finalVelocity;
    }

    void MBisonHeadStompHit()
    {
        if (!animator.GetBool("isInHitStun") && !animator.GetBool("isKnockedDown"))
        {
            character.TakeOffVelocity(0f, 0f);
        }
    }

    void MBisonReverse(int reverseType)
    {
        character.MoveProperties(25f, 15f, 2f, 40f, 1, 0, 1, 4f);

        if (!animator.GetBool("isInHitStun") && !animator.GetBool("isKnockedDown"))
        {
            if (reverseType <= 0)
            {
                character.TakeOffVelocity(-1.5f, 4f);
            }
            else
            {
                character.TakeOffVelocity(-1.5f, 3f);
            }
        }
    }
    
    void DetermineReverseDirection()
    {
        direction = character.side == Character.Side.P1 ? Vector2.right : Vector2.left;
    }

    void WindForReverses(float speed)
    {
        //using the wind method to make M Bison smoothly jump back and land forward
        if (direction == Vector2.right)
        {
            physicsbody.AddForce(new Vector2(speed, 0f));
        }
        else
        {
            physicsbody.AddForce(new Vector2(-speed, 0f));
        }
    }

    void PlaySuperStartSound()
    {
        AudioSource.PlayClipAtPoint(superStartSound, transform.position);
    }

    void PlaySlideSound()
    {
        AudioSource.PlayClipAtPoint(slideSound, transform.position);
    }

    void PlayScissorKicksSound()
    {
        AudioSource.PlayClipAtPoint(scissorKicksSound, transform.position);
    }

    void PlayPsychoSound()
    {
        AudioSource.PlayClipAtPoint(psychoSound, transform.position);
    }

    void PlayPsychoCrusherSound()
    {
        AudioSource.PlayClipAtPoint(psychoCrusherSound, transform.position);
        AudioSource.PlayClipAtPoint(psychoCrusherEffectSound, transform.position);
    }

    void PlayHeadStompLaunchSound()
    {
        AudioSource.PlayClipAtPoint(headStompLaunchSound, transform.position);
    }

    void PlayHeadStompHitSound()
    {
        AudioSource.PlayClipAtPoint(headStompHitSound, transform.position);
    }

    void PlaySomerSaultSound()
    {
        AudioSource.PlayClipAtPoint(somerSaultSound, transform.position);
    }

    void PlayReverseSound()
    {
        AudioSource.PlayClipAtPoint(reverseSound, transform.position);
    }

    public bool GetKneePressNightmareActive()
    {
        return kneePressNightmareActive;
    }
}
