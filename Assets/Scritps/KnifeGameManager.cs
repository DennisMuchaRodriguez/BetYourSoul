using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class KnifeGameManager : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject handWithKnife; // Modelo mano + cuchillo
    public Text countdownText;
    public Text keyPromptText;
    public Text messageText;
    public Image dangerIndicator; // Alerta de peligro
    public Image bloodOverlay;    // Sangre en pantalla
    public Image distortionFilter; // Efecto de distorsión
    public Text qtePromptText;    // Texto para QTE

    [Header("Configuración")]
    public float knifeSpeed = 1f; // Velocidad base
    public float forceMultiplier = 0.5f; // Fuerza de las flechas
    public float maxOffset = 0.5f; // Límite de movimiento
    public float warningDuration = 3f; // Tiempo de alerta
    public float minBlinkInterval = 0.1f; // Parpadeo rápido al final
    public float maxBlinkInterval = 0.5f; // Parpadeo lento al inicio
    public float bloodOpacityPerCut = 0.2f; // Opacidad de sangre por corte
    public float forceReductionPerCut = 0.8f; // Reducción de fuerza por corte
    public int maxMistakes = 5; // 5 dedos = 5 vidas
    public float shakeIntensity = 0.5f; // Fuerza del temblor
    public float shakeDuration = 0.5f; // Duración del temblor
    public float qteTime = 2f; // Tiempo para QTE

  
    public float safeZoneWidth = 0.15f; // Zona segura entre dedos

    // Variables de estado
    private float currentKnifeSpeed;
    private bool isMovingRight = true;
    private bool isWarningActive;
    private bool isKnifeDropping;
    private int mistakes = 0;
    private float nextDangerTime;
    private float dangerInterval = 5f;
    private Vector3 originalHandPosition;
    private Quaternion originalHandRotation;
    private bool isQTEAvailable = true;
    private KeyCode[] possibleQTEKeys = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.Space };

    void Start()
    {
        originalHandPosition = handWithKnife.transform.position;
        originalHandRotation = handWithKnife.transform.rotation;
        InitializeGame();
    }

    void InitializeGame()
    {
        currentKnifeSpeed = knifeSpeed;
        dangerIndicator.gameObject.SetActive(false);
        bloodOverlay.color = new Color(1, 0, 0, 0);
        distortionFilter.gameObject.SetActive(false);
        qtePromptText.gameObject.SetActive(false);
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        countdownText.text = "3";
        yield return new WaitForSeconds(1);
        countdownText.text = "2";
        yield return new WaitForSeconds(1);
        countdownText.text = "1";
        yield return new WaitForSeconds(1);
        countdownText.text = "¡EMPIEZA!";
        yield return new WaitForSeconds(0.5f);
        countdownText.gameObject.SetActive(false);
        StartGame();
    }

    void StartGame()
    {
        nextDangerTime = Time.time + dangerInterval;
    }

    void Update()
    {
        if (!countdownText.gameObject.activeSelf && !isKnifeDropping)
        {
            UpdateKnifeMovement();

            if (Time.time > nextDangerTime && !isWarningActive && isQTEAvailable)
            {
                StartCoroutine(ShowDangerWarning());
                nextDangerTime = Time.time + dangerInterval;
            }
        }
    }

    void UpdateKnifeMovement()
    {
        if (isKnifeDropping) return; // No mover durante la caída

        // 1. Movimiento automático del cuchillo (eje Z)
        float movement = currentKnifeSpeed * Time.deltaTime;
        Vector3 newPosition = handWithKnife.transform.localPosition +
                             (isMovingRight ? Vector3.forward : Vector3.back) * movement;

        // 2. Aplicar límites en Z
        newPosition.z = Mathf.Clamp(newPosition.z, -maxOffset, maxOffset);
        handWithKnife.transform.localPosition = newPosition;

        // 3. Rebote en los límites (con margen del 95% para mayor fluidez)
        if (Mathf.Abs(newPosition.z) >= maxOffset)
        {
            isMovingRight = !isMovingRight;
        }
    

        // 4. Control del jugador con flechas ← y →
        if (Input.GetKey(KeyCode.RightArrow)) // Mueve hacia +Z
        {
            float newZ = handWithKnife.transform.localPosition.z + (forceMultiplier * Time.deltaTime);
            newZ = Mathf.Clamp(newZ, -maxOffset, maxOffset);
            handWithKnife.transform.localPosition = new Vector3(
                handWithKnife.transform.localPosition.x,
                handWithKnife.transform.localPosition.y,
                newZ
            );
        }
        else if (Input.GetKey(KeyCode.LeftArrow)) // Mueve hacia -Z
        {
            float newZ = handWithKnife.transform.localPosition.z - (forceMultiplier * Time.deltaTime);
            newZ = Mathf.Clamp(newZ, -maxOffset, maxOffset);
            handWithKnife.transform.localPosition = new Vector3(
                handWithKnife.transform.localPosition.x,
                handWithKnife.transform.localPosition.y,
                newZ
            );
        }
    }


    IEnumerator ShowDangerWarning()
    {
        isWarningActive = true;
        float timer = 0f;
        float blinkInterval = maxBlinkInterval;

        dangerIndicator.gameObject.SetActive(true);

        while (timer < warningDuration)
        {
            dangerIndicator.enabled = !dangerIndicator.enabled;
            blinkInterval = Mathf.Lerp(maxBlinkInterval, minBlinkInterval, timer / warningDuration);
            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        dangerIndicator.gameObject.SetActive(false);
        isWarningActive = false;

        // 50% de chance de QTE o caída normal
        if (Random.Range(0, 2) == 0 && isQTEAvailable)
        {
            StartQTE();
        }
        else
        {
            StartCoroutine(DropKnife());
        }
    }

    IEnumerator DropKnife()
    {
        isKnifeDropping = true;

        // Caída en Y (0.15 unidades durante 0.1 segundos)
        Vector3 originalPos = handWithKnife.transform.localPosition;
        Vector3 droppedPos = originalPos + Vector3.down * 0.15f;

        float elapsed = 0f;
        float fallDuration = 0.1f;

        while (elapsed < fallDuration)
        {
            handWithKnife.transform.localPosition = Vector3.Lerp(originalPos, droppedPos, elapsed / fallDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        CheckCut(); // Verificar si cortó

        // Retorno instantáneo
        handWithKnife.transform.localPosition = originalPos;
        isKnifeDropping = false;
    }

    void CheckCut()
    {
        // Ahora usa posición Z para detectar cortes
        float handZPos = handWithKnife.transform.localPosition.z;

        if (Mathf.Abs(handZPos) < safeZoneWidth)
        {
            Debug.Log("¡Perfecto! No cortaste ningún dedo");
        }
        else
        {
            mistakes++;
            Debug.Log("¡Cortaste un dedo! Errores: " + mistakes);
            StartCoroutine(ApplyCutEffects());
            if (mistakes >= maxMistakes) GameOver();
        }
    }

    IEnumerator ApplyCutEffects()
    {

        // Activar sangre
        bloodOverlay.color = new Color(1, 0, 0, bloodOpacityPerCut * mistakes);

        // Mostrar por 0.5 segundos
        yield return new WaitForSeconds(0.5f);

        // Desvanecer (opcional)
        float fadeDuration = 0.3f;
        float elapsed = 0f;
        Color startColor = bloodOverlay.color;
        Color endColor = new Color(1, 0, 0, startColor.a * 0.5f); // Reduce opacidad a la mitad

        while (elapsed < fadeDuration)
        {
            bloodOverlay.color = Color.Lerp(startColor, endColor, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator ApplyDistortion()
    {
        distortionFilter.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        distortionFilter.gameObject.SetActive(false);
    }

    void StartQTE()
    {
        isQTEAvailable = false;
        KeyCode randomKey = possibleQTEKeys[Random.Range(0, possibleQTEKeys.Length)];
        qtePromptText.text = "¡PRESIONA " + randomKey + "!";
        qtePromptText.gameObject.SetActive(true);
        StartCoroutine(QTETimer(randomKey));
    }

    IEnumerator QTETimer(KeyCode requiredKey)
    {
        float timer = 0f;
        bool qteSuccess = false;

        while (timer < qteTime)
        {
            if (Input.GetKeyDown(requiredKey))
            {
                qteSuccess = true;
                break;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        qtePromptText.gameObject.SetActive(false);

        if (qteSuccess)
        {
            // QTE completado, no pasa nada
        }
        else
        {
            // Falló el QTE, corta un dedo
            mistakes++;
            StartCoroutine(ApplyCutEffects());

            if (mistakes >= maxMistakes)
            {
                GameOver();
            }
        }

        isQTEAvailable = true;
    }

    void GameOver()
    {
        messageText.text = "¡PERDISTE!";
        StartCoroutine(ReturnToCasino(3f));
    }

    IEnumerator ReturnToCasino(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Casino");
    }
}



