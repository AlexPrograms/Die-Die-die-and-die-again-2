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
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private AudioSource jumpSoundEffect;

    private IPlayerState currentState;

    public enum MovementState
    {
        Idle,
        Running,
        Jumping,
        Falling
    }

    public interface IPlayerState
    {
        void EnterState(PlayerMovement player);
        void UpdateState(PlayerMovement player);
    }

    public class IdleState : IPlayerState
    {
        public void EnterState(PlayerMovement player) => player.SetAnimationState(MovementState.Idle);
        public void UpdateState(PlayerMovement player)
        {
            if (Mathf.Abs(player.dirX) > 0)
                player.SetState(new RunningState());
        }
    }

    public class RunningState : IPlayerState
    {
        public void EnterState(PlayerMovement player)
        {
            player.SetAnimationState(MovementState.Running);
            player.UpdateSpriteDirection();
        }

        public void UpdateState(PlayerMovement player)
        {
            if (Mathf.Abs(player.dirX) == 0)
                player.SetState(new IdleState());
            else
                player.UpdateSpriteDirection();
        }
    }

    public class JumpingState : IPlayerState
    {
        private bool hasFlipped = false;

        public void EnterState(PlayerMovement player)
        {
            player.SetAnimationState(MovementState.Jumping);
            player.UpdateSpriteDirection();
            hasFlipped = false;
        }

        public void UpdateState(PlayerMovement player)
        {
            player.UpdateSpriteDirection();

            if (player.rb.velocity.y < 0.1f)
            {
                player.SetState(new FallingState());
                hasFlipped = false; 
            }


            if (!hasFlipped && Mathf.Abs(player.dirX) > 0)
            {
                player.sprite.flipX = player.dirX < 0; 
                hasFlipped = true; 
            }
        }
    }

    public class FallingState : IPlayerState
    {
        public void EnterState(PlayerMovement player)
        {
            player.UpdateSpriteDirection();
            player.SetAnimationState(MovementState.Falling);
        }

        public void UpdateState(PlayerMovement player)
        {
            player.UpdateSpriteDirection();
            if (player.IsGrounded())
                player.SetState(new IdleState());
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        bColl = GetComponent<BoxCollider2D>();

        SetState(new IdleState());
    }

    private void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(dirX * runSpeed, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpSoundEffect.Play();
            SetState(new JumpingState());
        }

        currentState.UpdateState(this);
    }

    public void SetState(IPlayerState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }

    public void SetAnimationState(MovementState state)
    {
        anim.SetInteger("state", (int)state);
    }

    public void UpdateSpriteDirection()
    {
        if (Mathf.Abs(dirX) > 0)
            sprite.flipX = dirX < 0;
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(bColl.bounds.center, bColl.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }
}
