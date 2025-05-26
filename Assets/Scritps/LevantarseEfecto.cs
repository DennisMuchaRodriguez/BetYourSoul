using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevantarseEfecto : MonoBehaviour
{
    [Header("Configuración Altura (Cámara)")]
    public float duracionAltura = 2.0f;
    public float duracionRotacion = 1.5f;
    public float alturaInicialCamara = 0.2f;
    public float alturaFinalCamara = 0.8f;

    [Header("Configuración Rotación")]
    public float rotacionInicialX = -80f;
    public float rotacionFinalX = 2.8f;

    [Header("Referencias")]
    public PlayerController playerController;
    public Transform camara;
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;

    private float tiempoInicio;
    private bool efectoCompletado = false;

    void Start()
    {
        tiempoInicio = Time.time;
        rb = playerController.GetComponent<Rigidbody>();
        capsuleCollider = playerController.GetComponent<CapsuleCollider>();

  
        playerController.enabled = false;
        rb.isKinematic = true; 
        capsuleCollider.enabled = false;

      
        if (camara == null) camara = Camera.main.transform;
        camara.localPosition = new Vector3(0, alturaInicialCamara, 0);
        camara.localRotation = Quaternion.Euler(rotacionInicialX, 0, 0);
    }

    void Update()
    {
        if (!efectoCompletado)
        {
            float tiempoTranscurrido = Time.time - tiempoInicio;
            float progresoAltura = Mathf.Clamp01(tiempoTranscurrido / duracionAltura);
            float progresoRotacion = Mathf.Clamp01(tiempoTranscurrido / duracionRotacion);


            float nuevaAltura = Mathf.Lerp(alturaInicialCamara, alturaFinalCamara, progresoAltura);
            camara.localPosition = new Vector3(0, nuevaAltura, 0);

            float nuevaRotacionX = Mathf.Lerp(rotacionInicialX, rotacionFinalX, progresoRotacion);
            camara.localRotation = Quaternion.Euler(nuevaRotacionX, 0, 0);

         
            if (progresoAltura >= 1f && progresoRotacion >= 1f)
            {
                efectoCompletado = true;
                ReactivarControles();
            }
        }
    }

    void ReactivarControles()
    {

        playerController.enabled = true;
        rb.isKinematic = false;
        capsuleCollider.enabled = true;

        Vector3 pos = playerController.transform.position;
        pos.y = 0;
        playerController.transform.position = pos;


        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
