using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 100f;
    private CharacterController characterController;
    private float xRotation = 0f;

    [Header("Cámara")]
    public Transform playerCamera; 

    [Header("Interacción")]
    public GameObject pressEText;
    private GameObject currentInteractable;
    [Header("Gravedad")]
    public float gravity = -9.81f;
    private float velocityY = 0f;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        if (pressEText != null) pressEText.SetActive(false);

        // Verificación de seguridad
        if (playerCamera == null)
        {
            Debug.LogError("No se ha asignado la cámara en el Inspector!");
            playerCamera = Camera.main.transform;
        }
    }

    void Update()
    {
        Movimiento();
        RotacionCamara();
        Interaccion();
        AplicarGravedad();
    }

    void Movimiento()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

  
        Vector3 moveDirection = transform.forward * vertical + transform.right * horizontal;
        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
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
    void AplicarGravedad()
    {
        if (!characterController.isGrounded)
        {
            velocityY += gravity * Time.deltaTime;
        }
        else
        {
            velocityY = 0;
        }

        characterController.Move(Vector3.up * velocityY * Time.deltaTime);
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

