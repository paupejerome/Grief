using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    Player player;
    Animator animator;
    Rigidbody rb;

    void Start()
    {
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (player.isMoving)
        {
            if (player.isJumping)
            {
                animator.SetInteger("animation", 6); // jump
            }

            if (player.isGrounded)
                animator.SetInteger("animation", 12); // run
            else
                animator.SetInteger("animation", 17); // fly
        }
    }
}
