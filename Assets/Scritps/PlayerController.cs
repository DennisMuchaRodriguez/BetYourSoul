using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float mouseSensitivity = 100f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    [Header("Componentes")]
    public Transform playerCamera;
    public GameObject pressEText;

    // Nuevos componentes físicos
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;


    private float xRotation = 0f;
    private float velocityY = 0f;
    private bool isGrounded;
    private GameObject currentInteractable;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        
        rb.freezeRotation = true;
        rb.useGravity = false; 

        Cursor.lockState = CursorLockMode.Locked;
        if (pressEText != null) pressEText.SetActive(false);

        if (playerCamera == null)
        {
            Debug.LogError("¡No se ha asignado la cámara en el Inspector!");
            playerCamera = Camera.main.transform;
        }
    }

    void Update()
    {
     
        RotacionCamara();
        Interaccion();
        CheckGrounded();
    }

    void FixedUpdate()
    {
        // Movimiento basado en física
        Movimiento();
        AplicarGravedad();

        // Nueva línea añadida: Detener movimiento horizontal cuando no hay input
        if (Mathf.Abs(Input.GetAxis("Horizontal")) < 0.1f && Mathf.Abs(Input.GetAxis("Vertical")) < 0.1f)
        {
            DetenerMovimientoHorizontal();
        }
    }

    void Movimiento()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
        {
            Vector3 moveDirection = (transform.forward * vertical + transform.right * horizontal).normalized;
            Vector3 targetVelocity = moveDirection * moveSpeed;
            targetVelocity.y = rb.velocity.y;

            // Aplicar movimiento con suavizado
            rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, 10f * Time.fixedDeltaTime);
        }
    }
    void DetenerMovimientoHorizontal()
    {
        // Detener solo el movimiento horizontal (manteniendo velocidad vertical)
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
    }
    void RotacionCamara()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void AplicarGravedad()
    {
        if (isGrounded && velocityY < 0)
        {
            velocityY = -2f; // Pequeña fuerza hacia abajo para asegurar contacto
        }
        else
        {
            velocityY += gravity * Time.deltaTime;
        }

        // Aplicar gravedad manualmente
        rb.velocity = new Vector3(rb.velocity.x, velocityY, rb.velocity.z);
    }

    void CheckGrounded()
    {
        float rayLength = capsuleCollider.height * 0.5f + 0.1f;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, rayLength);
    }

    void Interaccion()
    {
        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable.GetComponent<Entidad>().MostrarMenuMinijuegos();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Entidad"))
        {
            currentInteractable = other.gameObject;
            if (pressEText != null) pressEText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Entidad"))
        {
            currentInteractable = null;
            if (pressEText != null) pressEText.SetActive(false);
        }
    }
}

