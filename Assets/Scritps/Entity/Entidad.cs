using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Entidad : MonoBehaviour
{
    public static event Action OnPlayerEnter;
    public static event Action OnPlayerExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerEnter?.Invoke();
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null) pc.SetCanInteract(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        OnPlayerExit?.Invoke();
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc != null) pc.SetCanInteract(false);
    }
}
