using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed;
    public float mouseSensitivity = 20f;

    [Header("Referencias")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private InputReader inputReader;

    [Header("Components")]
    private Rigidbody myRBD;
    private Vector2 movement;
    private Vector2 lookInput;
    private float xRotation;
    public bool canMove = true;
    private bool canInteract = false;


    public static event Action OnPlayerInteracted;


    private void OnEnable()
    {
        inputReader.OnMovementInput += OnMovement;
        inputReader.OnLookInput += OnLook;
        inputReader.OnInteract += OnInteract;
    }
    private void OnDisable()
    {
        inputReader.OnMovementInput -= OnMovement;
        inputReader.OnLookInput -= OnLook;
        inputReader.OnInteract -= OnInteract;
    }

    private void Awake()
    {
        myRBD = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void FixedUpdate()
    {
        ApplyPhysics();
    }
    private void Update()
    {
        HandleLookPlayer();
    }
    private void OnMovement(Vector2 movementInput)
    {
        if (canMove)
        {
            movement = movementInput;
        }
    }
    private void OnLook(Vector2 inputLook)
    {
        lookInput = inputLook;
    }
    private void HandleLookPlayer()
    {
        transform.Rotate(Vector3.up * lookInput.x * mouseSensitivity * Time.deltaTime);

        xRotation -= lookInput.y * mouseSensitivity * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
    private void ApplyPhysics()
    {
        Vector3 moveDirection = (transform.right * movement.x + transform.forward * movement.y).normalized;
        Vector3 velocity = moveDirection * walkSpeed;
        velocity.y = myRBD.velocity.y;
        myRBD.velocity = velocity;
    }
    private void OnInteract()
    {
        if (canInteract)
        {
            OnPlayerInteracted?.Invoke();
            inputReader.canHandleInput = false;
        }
    }
    public void SetCanInteract(bool value)
    {
        canInteract = value;
    }
}