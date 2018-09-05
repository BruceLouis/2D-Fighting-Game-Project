using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBison : MonoBehaviour {

    private Character character;
    private Animator animator;
    private Rigidbody2D physicsbody;

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
        else if (animator.GetBool("psychoCrusherActive"))
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
            //using the wind method to make M Bison smoothly jump back and land forward
            if (direction == Vector2.right)
            {
                physicsbody.AddForce(new Vector2(3f, 0f));
            }
            else
            {
                physicsbody.AddForce(new Vector2(-3f, 0f));
            }
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

        CannonBallTrajectory(1f);
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

    void CannonBallTrajectory(float multiplier)
    {
        float angle;
        Vector3 targetPosition = GetComponentInParent<SharedProperties>().GetPositionOfOtherFighter();

        // Distance along the y axis between objects

        // Planar distance between objects
        Vector2 planarTarget = new Vector2(targetPosition.x, 0);
        Vector2 planarPostion = new Vector2(transform.position.x, 0);

        float distance = Vector3.Distance(planarTarget, planarPostion) * multiplier;

        if (Mathf.Abs(distance) < 1f)
        {
            angle = 80f * Mathf.Deg2Rad;
        }
        else if (Mathf.Abs(distance) >= 1f && Mathf.Abs(distance) < 2f)
        {
            angle = 75f * Mathf.Deg2Rad;
        }
        else if (Mathf.Abs(distance) >= 2f && Mathf.Abs(distance) < 3f)
        {
            angle = 65f * Mathf.Deg2Rad;
        }
        else
        {
            angle = 55f * Mathf.Deg2Rad;
        }

        float gravity = Physics2D.gravity.magnitude;


        float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle))); //1.0f is yOffset
        Vector2 targetVelocity = new Vector2(initialVelocity * Mathf.Cos(angle), initialVelocity * Mathf.Sin(angle));

        // Rotate our velocity to match the direction between the two objects
        float angleBetweenObjects = Vector2.Angle(Vector2.right, planarTarget - planarPostion);
        Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector2.up) * targetVelocity;

        // Fire!
        physicsbody.velocity = finalVelocity;
    }

    void MBisonHeadStompHit()
    {
        character.TakeOffVelocity(0f, 0f);
    }

    void MBisonHeadStompReverse()
    {
        character.MoveProperties(25f, 15f, 2f, 40f, 1, 0, 1, 4f);
        character.TakeOffVelocity(-1.5f, 4f);
        direction = character.side == Character.Side.P1 ? Vector2.right : Vector2.left;        
    }
}
