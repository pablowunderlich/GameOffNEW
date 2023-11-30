using ScriptableObjectArchitecture;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// This script follows the singleton pattern, meaning that i create an
// instance of this class that is static and can be 
// accessed anywhere in the project. Is this very safe? Maybe not,
// but there is only one player in the game and for now this is the 
// most straightforward way of doing this. I might break the player up in components later

// Elements for this script are inspired by the following source: https://github.com/DawnosaurDev/platformer-movement/tree/main/Scripts
public class Player : MonoBehaviour
{

    public static Player Instance { get; private set; } // Instance of the player that can be accessed in the entire project
    // if you want to quickly look for a function, just use ctrl F
    #region RUNTIME_VARIABLES
    // These variables can be changed during runtime for testing purposes. They can also be accessed independently from this script
    [Header("Running")]
    [Tooltip("Maximum running speed")]
    public FloatReference runMaxSpeed; // Max move speed for the player

    [Tooltip("Running acceleration Value")]
    public FloatReference runAcceleration; // Running acceleration

    [Tooltip("Running Decceleration Value")]
    public FloatReference runDecceleration; // Running deceleration

    [Tooltip("Determine the lerp value for running (How quick there should be interpolated")]
    public FloatReference runLerpValue; // Determines the lerp value for running

    [Space(5)]

    [Header("Jumping")]
    [Tooltip("Time it takes from pressing jump, to reaching max jump height CANT BE CHANGED DURING RUNTIME")]
    public FloatReference timeToMaxHeight; // Time it takes from pressing jump, to reaching max jump height

    [Tooltip("Maximum jump height CANT BE CHANGED DURING RUNTIME")]
    public FloatReference maxJumpHeight; // Maximum jump height

    [Tooltip("Determine how long the jump can hang in the hair")]
    public FloatReference jumpHangTimeThreshold; //Determine how long the jump can hang in the hair

    [Space(5)]
    [Header("Wall Jumping")]
    [Tooltip("The force applied to the player when walljumping")]
    public Vector2Reference wallJumpForce;

    [Tooltip("Reduces the force of the player movement while wall jumping for a better feel (VALUE BETWEEN 0 AND 1")]
    public FloatReference wallJumpReduceMovement;

    [Tooltip("The time that the movement is reduced after waljumping (BETWEEN 0 AND 1.5)")]
    public FloatReference wallJumpTime;


    [Space(5)]
    [Header("Falling")]
    [Tooltip(" If the player presses down, their speed downwards gets multiplied by this multiplier")]
    public FloatReference fastFallMultiplier; // If the player presses down they can fall faster

    [Tooltip("Maximum fast fall speed")]
    public FloatReference maxFastFallSpeed; // Maximum fast fall speed

    [Tooltip("the speed downwards gets multiplied by this multiplier")]
    public FloatReference fallMultiplier; // If the player presses down they can fall faster

    [Tooltip("Maximum fall speed")]
    public FloatReference maxFallSpeed; // Maximum fall speed

    [Space(5)]
    [Header("Sliding")]
    [Tooltip("Determine how fast the player slides down")]
    public FloatReference slideSpeed;

    [Tooltip("Determine the acceleration for the slide")]
    public FloatReference slideAcceleration;

    [Space(5)]
    [Header("Gravity")]

    [Tooltip("Multiplier that changes gravity scale so that Jump hang time threshold can be achieved")]
    public FloatReference jumpHangGravityMult; // Multiplier that changes gravity scale so that Jump hang time threshold can be achieved

    [Tooltip("Accelleration multiplier for when maxheight is reached")]
    public FloatReference jumpHangAccelerationMult; //Accelleration multiplier for when maxheight is reached

    [Tooltip("MaxSpeed multiplier when max height is reached")]
    public FloatReference jumpHangMaxSpeedMult; //MaxSpeed multiplier when max height is reached

    [Tooltip("Acceleration value in air. Recommend Between 0f, and 1f)\"")]
    public FloatReference accelInAir; // Acceleration value in air

    [Tooltip("Decceleration value in air. Recommend Between 0f, and 1f)\"")]
    public FloatReference deccelInAir; // Decceleration value in air

    [Tooltip("Enable if the player should jump be able to jump cut")]
    public BoolReference canJumpCut;

    [Tooltip("This multiplier increases (or decrease) gravity if the player releases the jump button")]
    public FloatReference jumpCutMultiplier;

    [Tooltip("do we want to conserve momentum??")]
    public BoolReference conserveMomentum; //do we want to conserve momentum??

    [Header("Health")]
    [Tooltip("Maximum Health")]
    public IntReference maxHealth; // Maximum health for the player

    [Tooltip("Current Health")]
    public IntReference currentHealth; // Current player health
    public Image heart1; 
    public Image heart2; 
    public Image heart3;

    [Header("Timing")]
    // This is a short time lapse where the player can jump even if they pressed jump just before the player hits ground.
    // This allows for better responsiveness
    [Tooltip("his is a short time lapse where the player can jump even if they pressed jump just before the player hits ground. Recommend Between 0.01f, and 0.5f)\"")]
    public FloatReference jumpInputBufferTime;

    [Tooltip("This is a short time lapse where the player can jump even if they have already moved off platform. ( Recommend Between 0.01f, and 0.5f)")]
    // This is a short time lapse where the player can jump even if they have already moved off platform. This increases responsiveness
    public FloatReference coyoteTime;

    [Space(5)]
    [Header("Time")]
    public FloatReference InvincibilityTime;

    public FloatReference blinkInterval;
    #endregion
    #region INSPECTOR_VARIABLES
    [Header("Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    // the value is based on character size, might make this dynamic later
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);

    [SerializeField] private Transform _frontWallCheckPoint;
    [SerializeField] private Transform _backWallCheckPoint;
    [SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);

    [SerializeField] private Transform _enemyCheckPoint;
    private Vector2 _enemyCheckSize;

    [Header("Layers and Tags")]
    [SerializeField] private LayerMask _enemyLayer; 
    [SerializeField] private LayerMask _groundLayer; //this is for if we want different layers

    #endregion
    #region OTHER_VARIABLES
    // These variables are constant, can be changed but not during runtime
    [SerializeField]
    private Sprite _spriteReference; // reference to the player sprite
    private SpriteRenderer _spriteRenderer; // reference to player sprite renderer to blink on taking damage
    [SerializeField] private Checkpoint _startPoint;
    bool canOpenExitMenu = true;
    bool canMove = true;
    [SerializeField] GameObject exitMenuUI;
    public Rigidbody2D RB { get; private set; } // Player rigidbody

    public bool isFacingRight { get; private set; } // Check the direction the character is facing
    public bool isJumping { get; private set; } // check if character is jumping (Usefull if we wanna do things in the air)
    public bool isWallJumping { get; private set; } // check if character is walljumping
    public bool isSliding { get; private set; } // check if player is sliding of the wall
    public float LastOnGroundTime { get; private set; } //Timer to check when the player has been in the air
    public float LastOnWallTime { get; private set; }
    public float LastOnRightWallTime { get; private set; }
    public float LastOnLeftWallTime { get; private set; }

    private Vector2 _moveInput; // Vector to determine 2D movement direction
    public float LastPressedJumpTime { get; private set; }

    private float _gravityStrength;
    private float _jumpForce;
    private float _runAccelAmount; // Acceleration value for running
    private float _runDeccelAmount; // Decceleration value for running
    private float _gravityScale; // Gravity scale
    private bool _isJumpFalling; // is falling down after jump?
    private bool _isJumpCut; // is cutting the jump by releasing the jump button early
    private Vector2 _spawnLocation; // spawnlocation
    //Wall jump variables
    private float _wallJumpStartTime;
    private int _lastWallJumpDir; // check which direction the player was facing at the last wall jump
    private float _timeSinceLastHit;

    #endregion

    #region Awake&Start
    private void Awake()
    {
        if (Instance == null) // check if there is not already an instance of this class in the scene
        {
            Instance = this; // if there is no instance, create one 
        //    DontDestroyOnLoad(gameObject); // Don't remove this object when changing scene (can be changed if need be)
        }
        //else
        //{
        //    Destroy(gameObject); // remove this gameObject, cause you cant have two instances. Destruction would be imminent
        //}

        RB = GetComponent<Rigidbody2D>();
        _spriteRenderer = transform.Find("PlayerSprite").GetComponent<SpriteRenderer>();
        _spawnLocation = _startPoint.transform.position;
        _gravityStrength = -(2 * maxJumpHeight.Value) / Mathf.Pow(timeToMaxHeight.Value, 2);
        _gravityScale = _gravityStrength / Physics2D.gravity.y;
        _jumpForce = Mathf.Abs(_gravityStrength) * timeToMaxHeight.Value;
        _runAccelAmount = (50 * runAcceleration.Value) / runMaxSpeed.Value;
        _runDeccelAmount = (50 * runDecceleration.Value) / runMaxSpeed.Value;
        runAcceleration.Value = Mathf.Clamp(runAcceleration.Value, 0.01f, runMaxSpeed.Value);
        runDecceleration.Value = Mathf.Clamp(runDecceleration.Value, 0.01f, runMaxSpeed.Value);


        // isJumping = false;
    }
    void Start()
    {
        SetGravityScale(_gravityScale); // Set the gravity scale
        isFacingRight = true; //Start game with player facing right
        _enemyCheckSize = transform.localScale;
        transform.position = _spawnLocation;

    }
    #endregion
    #region Update&FixedUpdate
    // Update is called once per frame
    void Update()
    {
        #region TIMERS
        LastOnGroundTime -= Time.deltaTime;
        LastOnWallTime -= Time.deltaTime;
        LastOnLeftWallTime -= Time.deltaTime;
        LastOnRightWallTime -= Time.deltaTime;
        LastPressedJumpTime -= Time.deltaTime;

        _timeSinceLastHit += Time.deltaTime;

        #endregion

        HandleInput();
        HandleCollision();
        HandleJump();
        HandleGravityScale();
    }

    private void FixedUpdate()
    {
        // Handle running (moving along the x axis), depending on if the player is wall jumping or not
        if (isWallJumping)
        {
            Run(wallJumpReduceMovement.Value);
        }
        else
        {
            Run(runLerpValue.Value);
        }

        if (isSliding)
        {
            Slide();
        }
    }
    #endregion

    #region Handlers
    private void HandleInput()
    {
        if(canMove)
        {
            _moveInput.x = Input.GetAxisRaw("Horizontal");
            _moveInput.y = Input.GetAxisRaw("Vertical");
        }

        if (_moveInput.x != 0) // if there is imput with intention of moving left or right
        {
            CheckDirectionToFace(_moveInput.x > 0); // check what direction to face by checking if x input > 0
            CheckDirectionToFace(_moveInput.x > 0); // check what direction to face by checking if x input > 0
        }

        // checks to see if the jump button is pressed and released (second one could be used for short jumps etc
        if (Input.GetKeyDown(KeyCode.Space) && canMove)
        {
            OnJumpInput();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EscapeMenu();
        }
        if (Input.GetKeyDown(KeyCode.Space) && !canOpenExitMenu)
        {
            LoadMainMenu();
        }
        if (Input.GetKeyUp(KeyCode.Space) && canMove)
        {
            if (canJumpCut.Value)
            {
                OnJumpUpInput();
            }
        }
    }
    private void HandleCollision()
    {
        #region collision with ground
        if (!isJumping) // if player is not jumping
        {
            // Ground check
            if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) && !isJumping)
            {
                LastOnGroundTime = coyoteTime.Value;
            }

            //Right wall check, if the player is touching the wall that he is facing and he is facing right,
            //or if touches the wall he isnt facing when he is facing left and not wall jumping. 
            if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && isFacingRight)
                || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !isFacingRight)) && !isWallJumping)
            {

                LastOnRightWallTime = coyoteTime.Value;
            }
            // same but for left wall
            if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !isFacingRight)
               || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && isFacingRight)) && !isWallJumping)
            {

                LastOnLeftWallTime = coyoteTime.Value;
            }

            //If the player turns, the checkpoints swap sides, so its good to make sure that the longest on a wall time value is storedz`
            LastOnWallTime = Mathf.Max(LastOnLeftWallTime, LastOnRightWallTime);
        }
        #endregion

        #region enemy pass-through collision
        Collider2D[] results;
        results = Physics2D.OverlapBoxAll(_enemyCheckPoint.position, _enemyCheckSize, 0, _enemyLayer);
        foreach (Collider2D col in results)
        {
            // I AM AWARE I COULD JUST MAKE ONE IF STATEMENT, BUT
            // I WANT TO LEAVE IT LIKE THIS TO EASILY ADD FUNCTIONALITY LATER
            if (col.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (col.gameObject.CompareTag("Geary"))
                {
                    LoseLife();


                }
                if (col.gameObject.CompareTag("Acid"))
                {
                    LoseLife();

                }
                if (col.gameObject.CompareTag("Spikes"))
                {
                    LoseLife();

                }
                if(col.gameObject.CompareTag("Bullet"))
                {
                    LoseLife();
                }

            }
        }
        #endregion
    }

    
    private void HandleJump()
    {
        if (isJumping && RB.velocity.y < 0) // if player is falling 
        {
            isJumping = false;
            if (!isWallJumping)
            {
                _isJumpFalling = true;
            }
        }
        if (isWallJumping && Time.time - _wallJumpStartTime > wallJumpTime.Value) // check if the player can still walljump
        {
            isWallJumping = false;
        }

        if (CanJump() && !isWallJumping)
        {
            _isJumpCut = false;
            if (!isJumping)
            {
                _isJumpFalling = false;
            }
        }

        if (CanJump() && LastPressedJumpTime > 0) // check if player can jump, then jump
        {
            isJumping = true;
            isWallJumping = false;
            _isJumpCut = false;
            _isJumpFalling = false;
            Jump();
        }
        else if (CanWallJump() && LastPressedJumpTime > 0)
        {
            isWallJumping = true;
            isJumping = false;
            _isJumpCut = false;
            _isJumpFalling = false;
            _wallJumpStartTime = Time.time;

            // this sets which direction we were facing on the last wall jump.
            // It does so by checking if the last on r
            _lastWallJumpDir = (LastOnRightWallTime > 0) ? -1 : 1;

            WallJump(_lastWallJumpDir);
        }
    }
    private void HandleGravityScale()
    {

        //check if player can slide and if they are moving towards the wall theyre facing
        if (CanSlide() && ((LastOnLeftWallTime > 0 && _moveInput.x < 0) || (LastOnRightWallTime > 0 && _moveInput.x > 0)))
        {
            isSliding = true;
        }
        else
        {
            isSliding = false;
        }

        if (isSliding)
        {
            SetGravityScale(0); //ignore gravity
        }
        else if (RB.velocity.y < 0 && _moveInput.y < 0) //if falling and pressing down
        {
            //increase gravitational pull when holding down
            SetGravityScale(_gravityScale * fastFallMultiplier.Value);
            RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -maxFastFallSpeed.Value));// Ensure the player falls faster but not infinitely
        }
        else if (_isJumpCut)
        {
            SetGravityScale(_gravityScale * jumpCutMultiplier.Value);
            RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -maxFallSpeed.Value));
        }
        else if ((isJumping || isWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < jumpHangTimeThreshold.Value) // check if jumping and the value is smaller than threshold
        {
            SetGravityScale(_gravityScale * jumpHangGravityMult.Value);
        }
        else if (RB.velocity.y < 0)
        {
            //Higher gravity when falling
            SetGravityScale(_gravityScale * fallMultiplier.Value);
            RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -maxFallSpeed.Value));
        }
        else
        {
            SetGravityScale(_gravityScale);
        }

    }
    #endregion
    #region playerActions
    private void Turn()
    {
        //store the scale and flip the character along the x axis
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale; // change direction

        isFacingRight = !isFacingRight; //change bool value
    }

    private void Jump()
    {
        // Make sure we can not jump multiple times from one press
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;

        #region Pablo

        //im adding the jump stop event so it stops playing
        AkSoundEngine.PostEvent("Play_Player_Jump", this.gameObject);

        #endregion Pablo

        float force = _jumpForce;
        if (RB.velocity.y < 0) // if player is going down
        {
            force -= RB.velocity.y;
        }
        RB.AddForce(Vector2.up * force, ForceMode2D.Impulse); // add force as an impulse to the rb

    }
    private void WallJump(int direction)
    {
        // Similar to jump, lets make sure we cant wall jump multiple times from one press
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;
        LastOnRightWallTime = 0;
        LastOnLeftWallTime = 0;


        #region Pablo

        //im adding the jump stop event so it stops playing
        AkSoundEngine.PostEvent("Play_Player_WallJump", this.gameObject);

        #endregion Pablo

        Vector2 force = new Vector2(wallJumpForce.Value.x, wallJumpForce.Value.y);
        force.x *= direction;
        // make sure we apply the force the opposite direction from the wall
        if (Mathf.Sign(RB.velocity.x) != Mathf.Sign(force.x))
        {
            force.x -= RB.velocity.x;
        }
        if (RB.velocity.y < 0) // check if player is falling, if so substract the fall velocity from the jump force 
        {
            force.y -= RB.velocity.y;
        }
        RB.AddForce(force, ForceMode2D.Impulse);
    }

    private void Slide()
    {

        float speedDif = slideSpeed.Value - RB.velocity.y;
        float movement = speedDif * slideAcceleration.Value;

        // fixedupdate can overcorrect the movment, so we clamp the movement value so that the acceleration cant be greater than the number of frames per second
        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

        RB.AddForce(movement * Vector2.up);
    }

    private void Run(float lerpValue)
    {
        // calculate the direction and desired velocity
        float targetSpeed = _moveInput.x * runMaxSpeed.Value;
        targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpValue);

        // calculate acceleration rate
        float accelRate;
        if (LastOnGroundTime > 0)
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _runAccelAmount : _runDeccelAmount;
        }
        else
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _runAccelAmount * accelInAir.Value : _runDeccelAmount * deccelInAir.Value;
        }

        //Increase the acceleration and maxSpeed when at the maxheight of their jump, makes the jump feel a bit more bouncy, responsive and natural
        if ((isJumping || isWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < jumpHangTimeThreshold.Value)
        {
            accelRate *= jumpHangAccelerationMult.Value;
            targetSpeed *= jumpHangMaxSpeedMult.Value;
        }

        //Conserve momentum, lets not slow the player down if they move in their desired direction faster than maxspeed
        if (conserveMomentum.Value && Mathf.Abs(RB.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
            accelRate = 0; //Dont accelrate (conserve momentum)
        }

        float speedDif = targetSpeed - RB.velocity.x; // Figure out the speed we want to reach and how far we are away from that
        float movement = speedDif * accelRate; // Mutiply this by accelration to get to the movement speed

        RB.AddForce(movement * Vector2.right, ForceMode2D.Force); // apply this speed by the right vector as a force vector (DIRECTION AAAND MAGNITUDE)

    }

    #endregion
    #region OnInput
    public void OnJumpInput() // call when jump is pressed
    {
        LastPressedJumpTime = jumpInputBufferTime.Value;
    }
    private void EscapeMenu()
    {
        if(canOpenExitMenu)
        {
            _moveInput.x = 0;
            _moveInput.y = 0;
            canMove = false;
            canOpenExitMenu=false;
            exitMenuUI.SetActive(true);
        }
        else
        {
            canMove = true;
            canOpenExitMenu=true;
            exitMenuUI.SetActive(false);
        }
    }
    private void LoadMainMenu()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
        Debug.Log("Back to main");
    }
    public void OnJumpUpInput()
    {
        if (CanJumpCut() || CanWallJumpCut())
        {
            _isJumpCut = true;
        }
    }

    #endregion
    #region CanDoAction
    private bool CanJump()
    {
        return LastOnGroundTime > 0 && !isJumping;
    }
    private bool CanWallJump()
    {
        // checks if: 
        // Is player allowed to press jump again
        // Is player is touching a wall
        // Is player not on the ground
        // Isnt already wallJumping or facing the walls in the wrong direction

        return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!isWallJumping ||
             (LastOnRightWallTime > 0 && _lastWallJumpDir == 1) || (LastOnLeftWallTime > 0 && _lastWallJumpDir == -1));
    }
    private bool CanJumpCut()
    {
        return isJumping && RB.velocity.y > 0;
    }
    private bool CanWallJumpCut()
    {
        return isWallJumping && RB.velocity.y > 0; // if the player is doing a wall jump and is not yet falling
    }
    
    private bool CanSlide()
    {
        // check if the player is on a wall
        // if they are not jumping
        // if they are not wall jumping
        // if they are not on the ground 
        if (LastOnWallTime > 0 && !isJumping && !isWallJumping && LastOnGroundTime <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    #endregion
    #region taking damage and death
    IEnumerator Blink(float duration, float interval)
    {
        float end = Time.time + duration;

        while (Time.time < end)
        {
            _spriteRenderer.enabled = !_spriteRenderer.enabled;
            yield return new WaitForSeconds(interval);
        }

        _spriteRenderer.enabled = true;
    }
    private void LoseLife()
    {
        if (_timeSinceLastHit > InvincibilityTime.Value) // if the player can get hit
        {
            currentHealth.Value -= 1; // remove a life
            _timeSinceLastHit = 0;
            Debug.Log(currentHealth);
            #region Pablo
            AkSoundEngine.PostEvent("Play_Player_Damaged", this.gameObject);
            #endregion
            if (currentHealth.Value == 2)
            {
                heart3.gameObject.SetActive(false);
            }
            if (currentHealth.Value == 1)
            {
                heart2.gameObject.SetActive(false);
            }
            if (currentHealth.Value <= 0)
            {
                heart3.gameObject.SetActive(true);
                heart2.gameObject.SetActive(true);
                currentHealth.Value = 3; // set lifes back to 3
                Respawn(); // respawn the player at latest checkpoint
            }
            else // do damage animation + sound
            {
                StartCoroutine(Blink(InvincibilityTime.Value, blinkInterval.Value));
                #region Pablo
                AkSoundEngine.PostEvent("Play_Player_Damaged", this.gameObject);
                #endregion 
            }
        }

    }
    #endregion
    #region spawning
    Vector2 GetSpawnLocation() { return _spawnLocation; }
    public void SetSpawnLocation(Vector2 spawnLocation) { _spawnLocation = spawnLocation; }

    public void Respawn()
    {
        transform.position = GetSpawnLocation();
        #region Pablo
        AkSoundEngine.PostEvent("Play_Player_Respawn", this.gameObject);
        #endregion
    }

    #endregion

    #region GIZMOS
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
        Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_enemyCheckPoint.position, _enemyCheckSize);


    }
    #endregion
    public void SetGravityScale(float scale)
    {
        RB.gravityScale = scale;
    }

    public void CheckDirectionToFace(bool IsMovingRight)
    {
        if (IsMovingRight != isFacingRight)
        {
            Turn();
        }
    }
}


