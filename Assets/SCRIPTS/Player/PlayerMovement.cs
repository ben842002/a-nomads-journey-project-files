using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{   
    Rigidbody2D rb;
    Animator animator;
    PlayerStats stats;
    PlayerCombat combat;

    // used in PlayerCombat script
    public bool facingRight;

    [Header("Water")]
    [SerializeField] private LayerMask waterMask;
    public bool isInWater;
    public float swimPlaybackSpeed = 1.5f;
    public float swimSpeed;

    [Header("Move/Run Speed")]
    public float moveSpeed;
    float moveInput;
    public float attackMovementDecrease = 5f;
    public float gravityDecrease;

    [Header("Ground Check")]
    [SerializeField] Transform feetPos;
    [SerializeField] float checkRadius;
    public bool isGrounded;
    [SerializeField] LayerMask groundMask;

    [Header("Jump Timer")]
    // jump 1
    [SerializeField] float jumpTimerCountdown;
    public bool isJumping;
    float jumpTimer;

    bool initialJump = false;
    bool countdownJump = false;
    // ---------------------
    // jump 2 (double jump)
    bool canDoubleJump;
    bool initialDoubleJump;
    public bool isDoubleJumping; // referenced in Falling.cs
    
    [Header("Coyote Time")]
    [SerializeField] float coyoteCountdown;
    float coyoteTimer;

    [Header("Dash")]
    public float dashTimerCountdown;
    float dashTimer;

    // used in combat script to prevent attacking while dashing
    public bool isDashing;

    // dash time interval (designed to limit spamming ground dashes)
    public float intervalDashCountdown;
    float intervalDashTimer;
    bool inInterval = false;

    int direction;
    bool initialDash; // targeted towards consistent air dashing

    [Header("Knockback")]
    public float knockBackForce;
    public bool knockedFromRight;
    [Tooltip("Dont change this! It's public because we need to reference in player knockback script!")]
    public float knockBackTimer;
    public float knockBackTimerCountdown;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        combat = GetComponent<PlayerCombat>();
        stats = PlayerStats.instance;

        // initialize variables
        moveSpeed = stats.moveSpeed;
        dashTimer = dashTimerCountdown;
        intervalDashTimer = intervalDashCountdown;
    }

    /// <summary>
    /// TRY TO KEEP ALL PHYSICS MOVEMENT OVER TIME (not code for a single frame) (RB) RELATED STUFF IN F.U 
    /// You want to avoid framerate dependent movement
    /// </summary>
    private void FixedUpdate()
    {   
        if (PauseMenu.GameIsPaused == false)
        {   
            // only do stuff if player ISNT knocked back
            if (knockBackTimer <= 0)
            {
                // move left or right
                float horizontal = moveInput * moveSpeed;
                rb.velocity = new Vector2(horizontal, rb.velocity.y);

                // ----------------------------
                // JUMPING

                // first jump
                if (initialJump == true)
                {
                    rb.velocity = new Vector2(rb.velocity.x, stats.jumpSpeed); 
                    initialJump = false;
                }

                // note: rb.velocity = ... code is in the CountDown function
                // if player holds jump button, countdown timer
                if (countdownJump == true)              
                    CountdownProlongedJump(horizontal);          

                // second jump
                if (initialDoubleJump == true)
                {
                    rb.velocity = new Vector2(rb.velocity.x, stats.jumpSpeed * 2f);
                    initialDoubleJump = false;
                }

                // ---------------------------
                // DASHING
                if (animator.GetBool("isDashing") == true)
                {
                    // cancel space bar jump when dashing
                    if (isJumping == true)
                    {
                        isJumping = false;
                        countdownJump = false;
                    }

                    dashTimer -= Time.fixedDeltaTime;
                    if (direction == 1)
                        rb.velocity = Vector2.right * stats.dashSpeed;
                    else if (direction == 2)
                        rb.velocity = Vector2.left * stats.dashSpeed;
                }
                // --------------------------
            }
            else
            {   
                // cancel dash if player is knocked back DURING dash
                if (isDashing == true)
                {
                    animator.SetBool("isDashing", false);
                    direction = 0;
                    isDashing = false;
                    dashTimer = dashTimerCountdown;
                }

                /* if player gets knocked back while jumping and the knockback finishes, the player will go up
                 * which is not intended. This happens because the jumpTimer is still > 0. This means that when the kb is 
                 * finished, the if statement resumes for the countdown jump. (It doesn't matter that isJumping == false because
                 * the force is applied when the timer > 0; Uou make countdownjump == false to avoid the 
                 * jumptimer problem totally;
                 */
                if (countdownJump == true)
                {
                    countdownJump = false;
                    isJumping = false;
                }

                KnockBack();
            }

            // check for ground
            CheckIsGrounded();

            // check for water
            CheckIsInWater();
        }
    }

    // TRY TO KEEP INPUTS IN UPDATE 
    private void Update()
    {   
        if (PauseMenu.GameIsPaused == false)
        {
            // player is free to move
            if (knockBackTimer <= 0)
            {
                // get left (-1) or right (1) direction input
                moveInput = Input.GetAxisRaw("Horizontal");
                animator.SetFloat("Speed", Mathf.Abs(moveInput));

                // dont flip when player is attacking
                if (combat.isAttacking == false)
                {
                    // flip the player.localScale when moving left or right 
                    if (moveInput > 0 && !facingRight)
                        Flip();
                    else if (moveInput < 0 && facingRight)
                        Flip();
                }

                // ------------------------------------------------------
                // JUMP

                CheckForJumpInput();

                // -----------------------------------------------------
                // DASH

                // countdown timer to prevent repetitive dashing
                if (inInterval == true)
                    CountdownInterval();

                // don't allow dashing in water
                if (isInWater == false)
                    CheckForDashInput();
                // -----------------------------------------------------

                HandleAnimationLayers();

                if (isGrounded == true)
                {
                    // can dash once grounded
                    initialDash = true;

                    // reset coyote timer since grounded
                    // note: isgrounded includes water and tiles
                    coyoteTimer = coyoteCountdown;

                    // reset doublejump bool
                    canDoubleJump = true;

                    isDoubleJumping = false;
                }
                else
                {
                    // countdown timer because player is in air
                    coyoteTimer -= Time.deltaTime;
                }
            }
        }
    }

    // -----------------------------------
    // JUMP CODE

    /// <summary>
    /// If you need a in-depth explanation of script, check link below
    /// LINK: https://www.youtube.com/watch?v=j111eKN8sJw
    /// </summary>
    void CheckForJumpInput()
    {   
        // Initial Jump phase
        if (coyoteTimer > 0 && Input.GetButtonDown("Jump"))
        {
            // player is now jumping
            isJumping = true;
            animator.SetBool("isJumping", true);
            AudioManager.instance.Play("PlayerJump");

            // set timer so player can jump higher if they hold jump button longer
            jumpTimer = jumpTimerCountdown;
            coyoteTimer = 0;

            // initial jump force is called in FixedUpdate
            initialJump = true;
        }

        // if player holds down jump button after initial jump
        HoldJumpButton();
        
        // ---------------------------------------------------
        // Second Jump Phase

        if (Input.GetButtonDown("Jump") && stats.doubleJump == true && canDoubleJump == true && initialJump == false)
        {
            AudioManager.instance.Play("PlayerJump2");
            animator.Play("player_jump", 1, 1f);    // reset jump anim
            rb.velocity = Vector2.zero;
            canDoubleJump = false;

            initialDoubleJump = true;           // inital 2nd jump force is called in fixedupdate
            isDoubleJumping = true;
        }
    }

    void HoldJumpButton()
    {
        // jump higher if player holds on jump button while jumping (spacebar in this case)
        // note: rb.velocity is applied constantly in F.U only UNTIL isJumping == false
        if (Input.GetButton("Jump") && isJumping == true)
            countdownJump = true;

        // if player releases jump button, make them go to the ground
        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
            countdownJump = false;
        }
    }

    /// <summary>
    /// Countdown timer when player is holding jump button.
    /// (A Thing to Note: the bool countdownJump doesn't technically affect the jump logic because force is only applied
    /// when isJumping == true. However, for the sake of sychronizing the bools, it is added)
    /// </summary>
    void CountdownProlongedJump(float horizontal)
    {
        // when timer hits 0, the max time holding space bar is met
        if (jumpTimer <= 0)
        {
            // do not allow player to jump in mid air
            isJumping = false;
            countdownJump = false;
        }
        else
        {
            // count down timer until player starts falls
            jumpTimer -= Time.fixedDeltaTime;
            rb.velocity = new Vector2(horizontal, stats.jumpSpeed);
        }
    }

    // ---------------------------------
    // DASH CODE
        
    void CheckForDashInput()
    {   
        if (stats.dashAbility == true)
        {   
            // direction is 0 when player is not dashing.
            if (direction == 0)
            {   
                if (Input.GetKeyDown(KeyCode.E) && initialDash == true && inInterval == false)
                {
                    isDashing = true;
                    AudioManager.instance.Play("PlayerDash");

                    // check if player is facing right/left to determine dash direction
                    if (facingRight == true)
                        direction = 1;
                    else
                        direction = 2;

                    // cancel regular attack combo
                    combat.combo = false;
                    animator.SetBool("RegularCombo", false);
                }
            }
            else
            {
                // player is not dashing if timer <= 0. They are if > 0
                if (dashTimer <= 0)
                {   
                    // reset values
                    dashTimer = dashTimerCountdown;
                    direction = 0;
                    rb.velocity = Vector2.zero;
                    isDashing = false;

                    // start interval timer to prevent constant dashing
                    inInterval = true;

                    // dont let player repeatedly dash in the middle of the air
                    if (isGrounded == false)
                        initialDash = false;

                    animator.SetBool("isDashing", false);
                }
                else
                    animator.SetBool("isDashing", true);
            }
        }
    }

    void CountdownInterval()
    {
        if (intervalDashTimer <= 0)
        {
            inInterval = false;
            intervalDashTimer = intervalDashCountdown;
        }
        else
            intervalDashTimer -= Time.deltaTime;
    }

    // ---------------------------------

    void CheckIsGrounded()
    {
        // check if player is grounded
        if (Physics2D.OverlapCircle(feetPos.position, checkRadius, groundMask) == true)
        {
            isGrounded = true;
            animator.SetBool("isGrounded", true);
        }
        else
        {
            isGrounded = false;
            animator.SetBool("isGrounded", false);
        }
    }

    void CheckIsInWater()
    {
        // check for water
        if (Physics2D.OverlapCircle(feetPos.position, checkRadius, waterMask) == true)       
            SetWaterParameters(true);    
        else
            SetWaterParameters(false);

        // disable all combat abilities
        if (isInWater == true)
        {   
            if (combat.isAttacking == true)
                combat.isAttacking = false;

            if (combat.combo == true)
                combat.combo = false;

            if (combat.airCombo == true)
                combat.airCombo = false;

            if (animator.GetCurrentAnimatorStateInfo(2).IsName("Default State") == false)
                animator.Play("Default State", 2);
        }

        // slow down when the player is idle in water, play faster when they swim
        if (isInWater == true && Mathf.Abs(moveInput) > 0)
        {
            moveSpeed = swimSpeed;
            animator.speed = swimPlaybackSpeed;
        }
        else
        {
            if (combat.isAttacking == false)
                moveSpeed = stats.moveSpeed;

            animator.speed = 1f;
        }
    }

    void SetWaterParameters(bool trueOrFalse)
    {
        isInWater = trueOrFalse;
        animator.SetBool("Swim", trueOrFalse);
    }

    private void KnockBack()
    {
        knockBackTimer -= Time.fixedDeltaTime;

        if (knockedFromRight)
            rb.velocity = new Vector2(-knockBackForce, 0f);
        else
            rb.velocity = new Vector2(knockBackForce, 0f);
    }

    // this function is called on anim frame for running 
    void PlayFootstepSound()
    {   
        if (isGrounded == true && combat.isAttacking == false)
        {
            int num = Random.Range(0, 2);
            if (num == 0)
                AudioManager.instance.Play("PlayerFootstep");
            else
                AudioManager.instance.Play("PlayerFootstep2");
        }
    }

    void HandleAnimationLayers()
    {
        if (isGrounded == false)
        {
            // layers have index assignment like lists (starting at 0)
            animator.SetLayerWeight(1, 1);
        }
        else if (isGrounded == true && animator.GetBool("Land") == false)
        {
            animator.SetLayerWeight(1, 0);
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;                 // face opposite direction
        Vector3 theScale = transform.localScale;    // get the localScale
        theScale.x *= -1;                           // flip the X axis
        transform.localScale = theScale;            // apply adjusted scale to the localScale
    }

    private void OnDrawGizmosSelected()
    {
        if (feetPos == null)
            return;

        Gizmos.DrawWireSphere(feetPos.position, checkRadius);
    }
}
