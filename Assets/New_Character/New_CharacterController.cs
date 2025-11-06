using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]

public class New_CharacterController : MonoBehaviour
{
    [Header("Movimiento")]
    public float WalkSpeed = 4f;
    public float SprintSpeed = 6f;
    public float jumpHeight = 2f;
    public float rotationSpeed = 10f;
    public float mouseSensitivity = 1f;

    public float gravity = -20;

    [Header("Referenciación")]
    public Transform cameraTransform;
    public Animator animator;

    private CharacterController characterController;
    private Vector3 Velocity;
    private float currentSpeed;
    private float yaw;

    public bool IsMoving { get; private set; }
    public Vector2 CurrentInput { get; private set; }
    public bool IsGrounded { get; private set; }
    public float CurrentYaw => yaw;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        updateAnimator();
    }

    void HandleMovement()
    {
        IsGrounded = characterController.isGrounded;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;
        IsMoving = inputDirection.magnitude > 0.1f;

        Vector3 moveDirection = Vector3.zero;

        if (IsMoving)
        {
            moveDirection = Quaternion.Euler(0f, cameraTransform.eulerAngles.y, 0f) * inputDirection;
            bool isSprinting = Input.GetKey(KeyCode.LeftShift);
            currentSpeed = isSprinting ? SprintSpeed : WalkSpeed;
        }

        // Detectar si estamos sobre una plataforma móvil (por parenting)
        bool onPlatform = transform.parent != null && transform.parent.CompareTag("MovingPlatform");

        // Si no está en plataforma, usar gravedad normal
        if (!onPlatform)
        {
            if (IsGrounded && Velocity.y < 0f)
            {
                Velocity.y = -2f;
            }

            if (Input.GetButtonDown("Jump") && IsGrounded)
            {
                Velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                animator?.SetBool("isJumping", true);
            }

            Velocity.y += gravity * Time.deltaTime;
        }
        else
        {
            // Si está sobre una plataforma, mantenerlo pegado a ella sin saltitos
            Velocity.y = -2f;
        }

        // Aplicar movimiento
        Vector3 finalMovement = (moveDirection * currentSpeed) * Time.deltaTime;
        finalMovement.y += Velocity.y * Time.deltaTime;

        characterController.Move(finalMovement);

        // Si está en el suelo, terminar animación de salto
        if (IsGrounded && Velocity.y < 0f)
        {
            animator?.SetBool("isJumping", false);
        }
    }

    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        yaw += mouseX;

        if (IsMoving)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.Euler(0f, yaw, 0f),
                rotationSpeed * Time.deltaTime
            );
        }
    }

    void updateAnimator()
    {
        float SpeedPercent = IsMoving ? (currentSpeed == SprintSpeed ? 1f : 0.5f) : 0f;
        animator?.SetFloat("Speed", SpeedPercent, 0.1f, Time.deltaTime);
        animator?.SetBool("IsGrounded", IsGrounded);
        animator?.SetFloat("VerticalSpeed", Velocity.y);
    }
}