using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class KnifeGameManager : MonoBehaviour
{
    public KeyCode[] posiblesTeclas = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.Space };
    public TextMeshProUGUI teclaTexto;
    public Image temporizadorUI;
    public float tiempoInicial = 1f;
    private float tiempoActual;
    private KeyCode teclaActual;

    void Start()
    {
        GenerarNuevoQTE();
    }

    void GenerarNuevoQTE()
    {
        teclaActual = posiblesTeclas[Random.Range(0, posiblesTeclas.Length)];
        teclaTexto.text = teclaActual.ToString();
        tiempoActual = tiempoInicial;
        StartCoroutine(ContadorTiempo());
    }

    IEnumerator ContadorTiempo()
    {
        while (tiempoActual > 0)
        {
            tiempoActual -= Time.deltaTime;
            temporizadorUI.fillAmount = tiempoActual / tiempoInicial;
            yield return null;
        }
        CortarDedo(); // Si no presionó a tiempo  
    }

    void Update()
    {
        if (Input.GetKeyDown(teclaActual))
        {
            StopAllCoroutines();
            Acierto();
        }
    }

    void Acierto()
    {
        // Animación de clavar + sonido  
        tiempoInicial *= 0.9f; // Aumentar dificultad  
        GenerarNuevoQTE();
    }

    void CortarDedo()
    {
        // Animación de corte + reducir vida  
        GenerarNuevoQTE();
    }
}
