using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 100f;
    private CharacterController characterController;
    private float xRotation = 0f;

    [Header("C�mara")]
    public Transform playerCamera; // Asigna la c�mara en el Inspector

    [Header("Interacci�n")]
    public GameObject pressEText;
    private GameObject currentInteractable;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        if (pressEText != null) pressEText.SetActive(false);

        // Verificaci�n de seguridad
        if (playerCamera == null)
        {
            Debug.LogError("No se ha asignado la c�mara en el Inspector!");
            playerCamera = Camera.main.transform;
        }
    }

    void Update()
    {
        Movimiento();
        RotacionCamara();
        Interaccion();
    }

    void Movimiento()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Movimiento relativo a la direcci�n a la que mira el jugador
        Vector3 moveDirection = transform.forward * vertical + transform.right * horizontal;
        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
    }

    void RotacionCamara()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotaci�n vertical (arriba/abajo) - solo afecta a la c�mara
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limita para no voltear completamente

        // Aplica rotaci�n vertical a la c�mara
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotaci�n horizontal (izquierda/derecha) - afecta al jugador completo
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

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Entidad"))
        {
            currentInteractable = null;
            if (pressEText != null) pressEText.SetActive(false);
        }
    }
}

