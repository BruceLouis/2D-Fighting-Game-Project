using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBison : MonoBehaviour {

    private Character character;
    private Animator animator;
    private Rigidbody2D physicsbody;

    private float amountTimeTravelledTimer;

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
        else
        {
            physicsbody.isKinematic = false;
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
}
