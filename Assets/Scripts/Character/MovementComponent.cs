using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementComponent : MonoBehaviour
{
    private PlayerInputActions playerActions;

    [SerializeField] private float WalkSpeed = 10.0f;
    [SerializeField] private float RunSpeed = 20.0f;
    [SerializeField] private float JumpForce;

    //Components
    private PlayerController playerController;
    private Animator playerAnimator;
    private Rigidbody playerRigidbody;

    //References
    private Transform playerTransform;

    private Vector2 InputVector = Vector2.zero;
    private Vector3 moveDirection = Vector3.zero;

    //Animator Hashes
    public readonly int MovementXHash = Animator.StringToHash("MovementX");
    public readonly int MovementYHash = Animator.StringToHash("MovementY");
    public readonly int IsJumpingHash = Animator.StringToHash("IsJumping");
    public readonly int IsRunningHash = Animator.StringToHash("IsRunning");

    private void Awake()
    {
        playerTransform = transform;
        playerController = GetComponent<PlayerController>();
        playerAnimator = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
    }

    public void OnMovement(InputValue value)
    {
        InputVector = value.Get<Vector2>();

        playerAnimator.SetFloat(MovementXHash, InputVector.x);
        playerAnimator.SetFloat(MovementYHash, InputVector.y);
    }

    public void OnRun(InputValue value)
    {
        Debug.Log((value.isPressed));
        playerController.IsRunning = value.isPressed;
        playerAnimator.SetBool(IsRunningHash, value.isPressed);
    }

    public void OnJump(InputValue value)
    {
        playerController.IsJumping = value.isPressed;
        playerAnimator.SetBool(IsJumpingHash, value.isPressed);

        playerRigidbody.AddForce((playerTransform.up + moveDirection) * JumpForce, ForceMode.Impulse);
    }

    private void Update()
    {
        if (playerController.IsJumping) return;

        if (!(InputVector.magnitude > 0)) return;

        moveDirection = playerTransform.forward * InputVector.y + playerTransform.right * InputVector.x;

        float currentSpeed = playerController.IsRunning ? RunSpeed : WalkSpeed;

        Vector3 movementDirection = moveDirection * (currentSpeed * Time.deltaTime);

        playerTransform.position += movementDirection;
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground") && !playerController.IsJumping) return;

        playerController.IsJumping = false;
        playerAnimator.SetBool(IsJumpingHash, false);
    }
}
