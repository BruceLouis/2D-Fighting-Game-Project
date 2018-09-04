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
            physicsbody.velocity = character.side == Character.Side.P1 ? new Vector2(2.5f, 0f) : new Vector2(-2.5f, 0f);
        }
    }

    void SweepSlowdown()
    {
        if (!animator.GetBool("isInHitStun") && !animator.GetBool("isKnockedDown"))
        {
            physicsbody.velocity = new Vector2(physicsbody.velocity.x * 0.3f, physicsbody.velocity.y);
        }
    }
}
