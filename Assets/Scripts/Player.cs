using ScriptableObjectArchitecture;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script follows the singleton pattern, meaning that i create an
// instance of this class that is static and can be 
// accessed anywhere in the project. Is this very safe? Maybe not,
// but there is only one player in the game and for now this is the 
// most straightforward way of doing this. I might break the player up in components later

// Elements for this script are inspired by the following source: https://github.com/DawnosaurDev/platformer-movement/tree/main/Scripts
public class Player : MonoBehaviour
{

    public static Player Instance { get; private set; } // Instance of the player that can be accessed in the entire project

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
    [Tooltip("Time it takes from pressing jump, to reaching max jump height")]
    public FloatReference timeToMaxHeight; // Time it takes from pressing jump, to reaching max jump height

    [Tooltip("Maximum jump height")]
    public FloatReference maxJumpHeight; // Maximum jump height

    [Tooltip("Determine how long the jump can hang in the hair")]
    public FloatReference jumpHangTimeThreshold; //Determine how long the jump can hang in the hair
   
    [Space(5)]
    [Header("Falling")]
    [Tooltip(" If the player presses down, their speed downwards gets multiplied by this multiplier")]
    public FloatReference fastFallMultiplier; // If the player presses down they can fall faster

    [Tooltip("Maximum fast fall speed")]
    public FloatReference maxFastFallSpeed; // Maximum fast fall speed

    [Tooltip("Maximum fall speed")]
    public FloatReference maxFallSpeed; // Maximum fall speed
    
    [Space(5)]
    [Header("Gravity")]
    [Tooltip("Gravity Scale")]
    public FloatReference gravityScale; // Gravity scale

    [Tooltip("Multiplier that changes gravity scale so that Jump hang time threshold can be achieved")]
    public FloatReference jumpHangGravityMult; // Multiplier that changes gravity scale so that Jump hang time threshold can be achieved

    [Tooltip("Accelleration multiplier for when maxheight is reached")]
    public FloatReference jumpHangAccelerationMult; //Accelleration multiplier for when maxheight is reached

    [Tooltip("MaxSpeed multiplier when max height is reached")]
    public FloatReference jumpHangMaxSpeedMult; //MaxSpeed multiplier when max height is reached

    [Tooltip(" Acceleration value in air. Recommend Between 0f, and 1f)\"")]
    public FloatReference accelInAir; // Acceleration value in air

    [Tooltip(" Decceleration value in air. Recommend Between 0f, and 1f)\"")]
    public FloatReference deccelInAir; // Decceleration value in air

    [Tooltip("do we want to conserve momentum??")]
    public BoolReference conserveMomentum; //do we want to conserve momentum??

    [Header("Health")]
    [Tooltip("Maximum Health")]
    public IntReference maxHealth; // Maximum health for the player

    [Header("Timing")]
    // This is a short time lapse where the player can jump even if they pressed jump just before the player hits ground.
    // This allows for better responsiveness
    [Tooltip("his is a short time lapse where the player can jump even if they pressed jump just before the player hits ground. Recommend Between 0.01f, and 0.5f)\"")]
    public FloatReference jumpInputBufferTime;

    [Tooltip("This is a short time lapse where the player can jump even if they have already moved off platform. ( Recommend Between 0.01f, and 0.5f)")]
    // This is a short time lapse where the player can jump even if they have already moved off platform. This increases responsiveness
    public FloatReference coyoteTime;

    #endregion
    #region INSPECTOR_VARIABLES
    [Header("Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    // the value is based on character size, might make this dynamic later
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);

    [Header("Layers and Tags")]
    [SerializeField] private LayerMask _groundLayer; //this is for if we want different layers

    #endregion
    #region OTHER_VARIABLES
    // These variables are constant, can be changed but not during runtime
    [SerializeField]
    private Sprite _spriteReference; // reference to the player sprite

    public Rigidbody2D RB { get; private set; } // Player rigidbody


    private float _currentHealth; // Current player health

    public bool isFacingRight { get; private set; } // Check the direction the character is facing
    public bool isJumping { get; private set; } // check if character is jumping (Usefull if we wanna do things in the air)
    public float LastOnGroundTime { get; private set; } //Timer to check when the player has been in the air
    private bool _isJumpFalling;

    private Vector2 _moveInput; // Vector to determine 2D movement direction
    public float LastPressedJumpTime { get; private set; }

    private float _gravityStrength;
    private float _jumpForce;
    private float _runAccelAmount; // Acceleration value for running
    private float _runDeccelAmount; // Decceleration value for running
    #endregion
    private void Awake()
    {
        if (Instance == null) // check if there is not already an instance of this class in the scene
        {
            Instance = this; // if there is no instance, create one 
            DontDestroyOnLoad(gameObject); // Don't remove this object when changing scene (can be changed if need be)
        }
        else
        {
            Destroy(gameObject); // remove this gameObject, cause you cant have two instances. Destruction would be imminent
        }

        RB = GetComponent<Rigidbody2D>();

        _gravityStrength = -(2 * maxJumpHeight.Value) / Mathf.Pow(timeToMaxHeight.Value, 2);
        _jumpForce = Mathf.Abs(RB.velocity.x);
        runAcceleration.Value = Mathf.Clamp(runAcceleration.Value, 0.01f, runMaxSpeed.Value);
        runDecceleration.Value = Mathf.Clamp(runDecceleration.Value, 0.01f, runMaxSpeed.Value);

        _runAccelAmount = (50 * runAcceleration.Value) / runMaxSpeed.Value;
        _runDeccelAmount = (50 * runDecceleration.Value) / runMaxSpeed.Value;
        conserveMomentum.Value = true;
    }
    void Start()
    {
        SetGravityScale(gravityScale.Value); // Set the gravity scale
        isFacingRight = true; //Start game with player facing right
    }

    // Update is called once per frame
    void Update()
    {
        #region TIMERS
        LastOnGroundTime -= Time.deltaTime;
        LastPressedJumpTime -= Time.deltaTime;
        #endregion

        HandleInput();
        HandleCollision();
        HandleJump();
        HandleGravityScale();
    }

    private void FixedUpdate()
    {
        // Handle running
        Run(runLerpValue.Value);
    }

    private void HandleInput()
    {
        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _moveInput.y = Input.GetAxisRaw("Vertical");

        if (_moveInput.x != 0) // if there is imput with intention of moving left or right
        {
            CheckDirectionToFace(_moveInput.x > 0); // check what direction to face by checking if x input > 0
        }

        // checks to see if the jump button is pressed and released (second one could be used for short jumps etc
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnJumpInput();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            OnJumpInput();
        }
    }
    private void HandleCollision()
    {
        if (!isJumping) // if player is not jumping
        {
            // Ground check
            if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) && !isJumping)
            {
                LastOnGroundTime = coyoteTime.Value;
            }
        }
    }
    private void HandleJump()
    {
        if (isJumping && RB.velocity.y < 0) // if player is not jumping 
        {
            isJumping = false;
        }

        if (CanJump())
        {
            if (!isJumping)
            {
                _isJumpFalling = false;
            }
        }

        if (CanJump() && LastPressedJumpTime > 0)
        {
            isJumping = true;
            _isJumpFalling = false;
            Jump();
        }
    }
    private void HandleGravityScale()
    {
        if (RB.velocity.y < 0 && _moveInput.y < 0)
        {
            //increase gravitational pull when holding down
            SetGravityScale(gravityScale.Value * fastFallMultiplier.Value);
            RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -maxFastFallSpeed.Value));// Ensure the player falls faster but not infinitely
        }
        else if ((isJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < jumpHangTimeThreshold.Value) // check if jumping and the value is smaller than threshold
        {
            SetGravityScale(gravityScale.Value * jumpHangGravityMult.Value);
        }
        else if (RB.velocity.y < 0)
        {
            //Higher gravity when falling
            SetGravityScale(gravityScale.Value * jumpHangGravityMult.Value);
        }
        else
        {
            SetGravityScale(gravityScale.Value);
        }

    }
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

        float force = _jumpForce;
        if (RB.velocity.y < 0) // if player is going down
        {
            force -= RB.velocity.y;
        }
        RB.AddForce(Vector2.up * force, ForceMode2D.Impulse); // add force as an impulse to the rb
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
        if ((isJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < jumpHangTimeThreshold.Value)
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
    public void OnJumpInput() // call when jump is pressed
    {
        LastPressedJumpTime = jumpInputBufferTime.Value;
    }

    private bool CanJump()
    {
        return LastOnGroundTime > 0 && !isJumping;
    }
}

//    #region GIZMOS
//    private void OnDrawGizmosSelected()
//    {
//        Gizmos.color = Color.green;
//        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
//    }
//    #endregion
//}


