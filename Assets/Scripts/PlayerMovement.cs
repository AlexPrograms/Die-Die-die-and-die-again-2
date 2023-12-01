using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D bColl;
    private Animator anim;
    private SpriteRenderer sprite;
    private float dirX = 0f;
    [SerializeField]private float jumpForce = 14f;
    [SerializeField]private float runSpeed = 7f;
    [SerializeField] private LayerMask jumpableGround;
    private enum movementState { idle, running, jumping, falling };

    [SerializeField] private AudioSource jumpSoundEffect;
    
    private void Start()
    {
        Debug.Log("Hello world");
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        bColl = GetComponent<BoxCollider2D>();
    }

    
    private void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(dirX * runSpeed, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpSoundEffect.Play();
        }

        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {

        movementState state;

        if (dirX > 0f)
        {
            state = movementState.running;
            sprite.flipX = false;
        }
        else if (dirX < 0f)
        {
            state = movementState.running;
            sprite.flipX = true;
        }
        else
        {
            state = movementState.idle;
        }

        if (rb.velocity.y > .1f) 
        { 
            state = movementState.jumping;
        }
        else if (rb.velocity.y < -.1f)
        {
            state = movementState.falling;
        }

        anim.SetInteger("state", (int)state);
    }

    private bool IsGrounded()
    {
       return Physics2D.BoxCast(bColl.bounds.center, bColl.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }
}
