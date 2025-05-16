using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]

public class PlayerControllerInput : MonoBehaviour
{
    //[SerializeField] private ThirdPersonCamera cameraScript;
    [SerializeField] private CinemachineVirtualCamera emoteVCam;
    [SerializeField] private CinemachineFreeLook normalVCam;
    [SerializeField] private CinemachineVirtualCamera firstVCam;

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

    public GameObject bulletPrefab;
    public Transform firePoint;

    private bool aiming = false;

    public int HP = 100;

    void Awake()
    {

        controls = new PlayerControls();

        controls.Gameplay.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Gameplay.Jump.performed += ctx => jumpTriggered = true;
        controls.Gameplay.Fire.performed += ctx => Fire();
        controls.Gameplay.Emote.performed += ctx => Emote();

        emoteVCam.Priority = 0;


    }

    void OnEnable() => controls.Gameplay.Enable();
    void OnDisable() => controls.Gameplay.Disable();

    void Start()
    {
        if (PlayerPrefs.HasKey("PlayerX") && PlayerPrefs.HasKey("PlayerY") && PlayerPrefs.HasKey("PlayerZ"))
            transform.position = new Vector3(PlayerPrefs.GetFloat("PlayerX"), PlayerPrefs.GetFloat("PlayerY"), PlayerPrefs.GetFloat("PlayerZ"));
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

        if (Input.GetMouseButton(1)) firstVCam.Priority = 20;
        else firstVCam.Priority = 0;

        if (HP <=0 ) Destroy(gameObject);
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
            StartCoroutine(jumpAnim());
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        jumpTriggered = false;
    }

    void Fire()
    {
        Debug.Log("Disparo!");
        animator.SetTrigger("Shoot");

        if (bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
        else
        {
            Debug.LogWarning("Falta asignar el bulletPrefab o firePoint.");
        }
    }

    void Emote()
    {
        Debug.Log("¡Baile o emote activado!");
        StartCoroutine(emoteCamera());
        // Reproduce animación de baile
    }

    public IEnumerator emoteCamera()
    {
        emoteVCam.Priority = 20;
        firstVCam.Priority = 0;
        normalVCam.Priority = 0;
        animator.SetBool("Emoting", true);
        animator.SetLayerWeight(2, 1);
        yield return new WaitForSeconds(3f);
        animator.SetLayerWeight(2, 0);
        animator.SetBool("Emoting", false);
        emoteVCam.Priority = 0;
        firstVCam.Priority = 0;
        normalVCam.Priority = 20;
    }

    public IEnumerator jumpAnim()
    {
        animator.SetBool("Jumping", true);
        yield return new WaitForSeconds(1.2f);
        animator.SetBool("Jumping", false);
    }

    public void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("PlayerX", transform.position.x);
        PlayerPrefs.SetFloat("PlayerY", transform.position.y);
        PlayerPrefs.SetFloat("PlayerZ", transform.position.z);
    }
}
