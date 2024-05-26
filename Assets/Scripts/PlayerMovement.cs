using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    private Rigidbody2D rb;

    [Header("Movement")]
    public float moveSpeed;
    public float acceleration;
    public float decceleration;
    public float velPower;
    [Space(10)]
    private float moveInput;
    [Space(10)]
    public float frictionAmount;

    [Header("Jump")]
    public float jumpForce;
    [Range(0, 1)]
    public float jumpCutMultiplier;
    [Space(10)]
    public float jumpCoyoteTime;
    private float lastGroundedTime;
    public float jumpBufferTime;
    private float lastJumpTime;
    [Space(10)]
    public float fallGravityMultiplier;
    private float gravityScale;
    [Space(10)]
    private bool isJumping;

    [Header("Checks")]
    public Transform groundCheckPoint;
    public Vector2 groundCheckSize;
    [Space(10)]
    public LayerMask groundLayer;
    [Space(10)]

    [Header("Camera and Facing")]
    [SerializeField] private CameraFollowObject _cameraFollowGO;
    [SerializeField] private bool IsFacingRight;
    [Space(10)]

    [Header("Animation")]
    public Animator animator;
    [Space(10)]

    private bool jumpInputReleased;
    private CameraFollowObject _cameraFollowObject;
    private float _fallSpeedYDampingChangeThreshold;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (transform.rotation.y == 0)
        {
            IsFacingRight = true;
        }
        else if (transform.rotation.y == 180)
        {
            IsFacingRight = false;
        }
    }
    void Start()
    {
        gravityScale = rb.gravityScale;

        _cameraFollowObject = _cameraFollowGO.GetComponent<CameraFollowObject>();

        _fallSpeedYDampingChangeThreshold = CameraManager.instance._fallSpeedYDampingChangeThreshold;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        TurnCheck();

        #region Interpolation based on Vertical velocity
        if (rb.velocity.y < _fallSpeedYDampingChangeThreshold && !CameraManager.instance.IsLerpingYDamping && !CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(true);
        }

        if (rb.velocity.y >= 0f && !CameraManager.instance.IsLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpedFromPlayerFalling = false;
            CameraManager.instance.LerpYDamping(false);
        }
        #endregion

        #region Jump logic
        if (Input.GetKey(KeyCode.Space))
        {
            lastJumpTime = jumpBufferTime;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            OnJumpUp();
        }

        if (Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer))
        {
            lastGroundedTime = jumpCoyoteTime;
        }

        if (Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer))
        {
            isJumping = false;
        }

        if (lastGroundedTime > 0 && lastJumpTime > 0 && !isJumping && jumpInputReleased && Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer))
        {
            Jump();
        }
        #endregion

        #region Changing gravity when you start falling
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = gravityScale * fallGravityMultiplier;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }
        #endregion

        lastGroundedTime -= Time.deltaTime;
        lastJumpTime -= Time.deltaTime;
    }
    private void FixedUpdate()
    {
        Run();
        Friction();
    }
    public void Run()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("HorizontalMove", Mathf.Abs(moveInput));
        float targetSpeed = moveInput * moveSpeed;
        float speedDif = targetSpeed - rb.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
        rb.AddForce(movement * Vector2.right);
    }
    public void Friction()
    {
        if (lastGroundedTime > 0 && rb.velocity.magnitude < 0.01f)
        {
            float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(frictionAmount));
            amount *= Mathf.Sign(rb.velocity.x);
            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }
    }
    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        lastGroundedTime = 0;
        lastJumpTime = 0;
        isJumping = true;
        jumpInputReleased = false;
        Debug.Log("JUMP");
    }
    public void OnJump()
    {
        lastJumpTime = jumpBufferTime;
    }
    public void OnJumpUp()
    {
        if (rb.velocity.y > 0 && isJumping)
        {
            rb.AddForce(Vector2.down * rb.velocity.y * (1 - jumpCutMultiplier), ForceMode2D.Impulse);
        }
        jumpInputReleased = true;
        lastJumpTime = 0;
    }
    private void TurnCheck()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        if (moveInput > 0 && !IsFacingRight)
        {
            Turn();
        }
        else if (moveInput < 0 && IsFacingRight)
        {
            Turn();
        }
    }
    private void Turn()
    {
        if (IsFacingRight)
        {
            Vector3 rotator = new Vector3(0, -180f, 0);
            transform.rotation = Quaternion.Euler(rotator);
            IsFacingRight = !IsFacingRight;

            _cameraFollowObject.CallTurn();
        }
        else
        {
            Vector3 rotator = new Vector3(0, 0f, 0);
            transform.rotation = Quaternion.Euler(rotator);
            IsFacingRight = !IsFacingRight;

            _cameraFollowObject.CallTurn();
        }
    }

}
