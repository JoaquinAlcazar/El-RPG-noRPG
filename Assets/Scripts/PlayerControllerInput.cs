using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]

public class PlayerControllerInput : MonoBehaviour
{
    [SerializeField] private ThirdPersonCamera cameraScript;

    public float moveSpeed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public Transform cameraTransform;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 velocity;
    private bool isGrounded;
    private bool jumpTriggered;

    private PlayerControls controls;

    private Animator animator;
    private CharacterController characterController;

    void Awake()
    {
        controls = new PlayerControls();

        controls.Gameplay.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Gameplay.Jump.performed += ctx => jumpTriggered = true;
        controls.Gameplay.Fire.performed += ctx => Fire();
        controls.Gameplay.Emote.performed += ctx => Emote();

        

        
    }

    void OnEnable() => controls.Gameplay.Enable();
    void OnDisable() => controls.Gameplay.Disable();

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

    }

    void Update()
    {
        HandleMovement();
        HandleJump();

        if (controls.Gameplay.Move.ReadValue<Vector2>() != Vector2.zero)
            animator.SetBool("Walking", true);
        else
            animator.SetBool("Walking", false);
    }

    void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        Vector3 move = cameraTransform.right * moveInput.x + cameraTransform.forward * moveInput.y;
        move.y = 0;

        if (move.magnitude >= 0.1f)
        {
            // ROTAR al personaje hacia la dirección de movimiento
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

            controller.Move(move.normalized * moveSpeed * Time.deltaTime);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }



    void HandleJump()
    {
        if (jumpTriggered && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        jumpTriggered = false;
    }

    void Fire()
    {
        Debug.Log("Disparo!");
        // Instancia un proyectil o animación
        animator.SetTrigger("Shoot");
    }

    void Emote()
    {
        Debug.Log("¡Baile o emote activado!");
        // Reproduce animación de baile
    }
}
