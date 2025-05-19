using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject panelPress;
    [SerializeField] private GameObject panelMiniGames;
    private void Start()
    {
        panelPress.SetActive(false);
    }
    private void OnEnable()
    {
        Entidad.OnPlayerEnter += ActivePanel;
        Entidad.OnPlayerExit += DesactivePanel;
        PlayerController.OnPlayerInteracted += ShowPanelMiniGames;
    }

    private void OnDisable()
    {
        Entidad.OnPlayerEnter -= ActivePanel;
        Entidad.OnPlayerExit -= DesactivePanel;
        PlayerController.OnPlayerInteracted -= ShowPanelMiniGames;
    }
    private void ActivePanel()
    {
        if (panelPress != null)
        {
            panelPress.SetActive(true);
        }
    }
    private void DesactivePanel()
    {
        if (panelPress != null)
        {
            panelPress.SetActive(false);
        }
    }
    private void ShowPanelMiniGames()
    {
        panelPress.SetActive(false);
        panelMiniGames.SetActive(true);
    }
}
