using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class KnifeGameManager : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject knifeModel;
    public Text countdownText;
    public Text keyPromptText;
    public Text messageText;
    public Image timerBar;
    //public AudioSource soundEffects;
    //public AudioClip successSound;
    //public AudioClip failSound;

    [Header("Posiciones entre dedos")]
    public Transform[] fingerPositions; // Asigna los 4 GameObjects en el Inspector

    [Header("Configuración")]
    public KeyCode[] possibleKeys = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.Space };
    public float initialTime = 1.8f; // Tiempo inicial más generoso
    public float minTime = 0.4f; // Tiempo mínimo que puede alcanzar
    public int requiredSuccesses = 50; // 50 aciertos para ganar
    public int maxMistakes = 5; // 5 errores para perder
    public float knifeMoveSpeed = 5f;

    private KeyCode currentKey;
    private float currentTime;
    private int currentPositionIndex = 0;
    private int successes = 0;
    private int mistakes = 0;
    private bool gameActive = false;
    private bool isMoving = false;
    private Vector3 targetPosition;

    void Start()
    {
        if (fingerPositions.Length != 4)
        {
            Debug.LogError("Debes asignar exactamente 4 posiciones entre dedos");
            return;
        }

        StartCoroutine(CountdownToStart());
    }

    IEnumerator CountdownToStart()
    {
        countdownText.gameObject.SetActive(true);

        countdownText.text = "3";
        yield return new WaitForSeconds(1f);

        countdownText.text = "2";
        yield return new WaitForSeconds(1f);

        countdownText.text = "1";
        yield return new WaitForSeconds(1f);

        countdownText.text = "¡EMPIEZA!";
        yield return new WaitForSeconds(0.5f);

        countdownText.gameObject.SetActive(false);
        StartGame();
    }

    void StartGame()
    {
        gameActive = true;
        knifeModel.transform.position = fingerPositions[0].position; // Posición inicial
        NextRound();
    }

    void NextRound()
    {
        if (successes >= requiredSuccesses)
        {
            EndGame(true);
            return;
        }

        // Selecciona nueva tecla
        currentKey = possibleKeys[Random.Range(0, possibleKeys.Length)];
        keyPromptText.text = currentKey.ToString();

        // Calcula nuevo tiempo (disminuye progresivamente pero con límite mínimo)
        float progress = Mathf.Clamp01((float)successes / requiredSuccesses);
        currentTime = Mathf.Lerp(initialTime, minTime, progress);
        timerBar.fillAmount = 1f;

        // Mueve a siguiente posición (cíclico entre 0-3)
        currentPositionIndex = (currentPositionIndex + 1) % 4;
        MoveKnifeToPosition(currentPositionIndex);
    }

    void MoveKnifeToPosition(int positionIndex)
    {
        if (isMoving) return;

        targetPosition = fingerPositions[positionIndex].position;
        StartCoroutine(MoveKnifeSmoothly());
    }

    IEnumerator MoveKnifeSmoothly()
    {
        isMoving = true;

        while (Vector3.Distance(knifeModel.transform.position, targetPosition) > 0.01f)
        {
            knifeModel.transform.position = Vector3.MoveTowards(
                knifeModel.transform.position,
                targetPosition,
                knifeMoveSpeed * Time.deltaTime
            );
            yield return null;
        }

        knifeModel.transform.position = targetPosition;
        isMoving = false;
    }

    void Update()
    {
        if (!gameActive || isMoving) return;

        // Actualiza temporizador
        currentTime -= Time.deltaTime;
        timerBar.fillAmount = currentTime / Mathf.Lerp(initialTime, minTime, (float)successes / requiredSuccesses);

        // Verifica tiempo agotado
        if (currentTime <= 0)
        {
            Mistake();
            return;
        }

        // Verifica input
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(currentKey))
            {
                Success();
            }
            else
            {
                Mistake();
            }
        }
    }

    void Success()
    {
        //soundEffects.PlayOneShot(successSound);
        successes++;
        NextRound();
    }

    void Mistake()
    {
        //soundEffects.PlayOneShot(failSound);
        mistakes++;

        // Animación de corte
        StartCoroutine(PlayCutAnimation());

        if (mistakes >= maxMistakes)
        {
            EndGame(false);
        }
        else
        {
            NextRound();
        }
    }

    IEnumerator PlayCutAnimation()
    {
        gameActive = false;

        Vector3 originalPos = knifeModel.transform.position;
        Vector3 cutPosition = originalPos + Vector3.down * 0.05f;

        float cutDuration = 0.1f;
        float elapsed = 0f;

        // Movimiento hacia abajo (corte)
        while (elapsed < cutDuration)
        {
            knifeModel.transform.position = Vector3.Lerp(originalPos, cutPosition, elapsed / cutDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Pequeña pausa dramática
        yield return new WaitForSeconds(0.2f);

        // Vuelve a la posición original
        elapsed = 0f;
        while (elapsed < cutDuration)
        {
            knifeModel.transform.position = Vector3.Lerp(cutPosition, originalPos, elapsed / cutDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        knifeModel.transform.position = originalPos;
        gameActive = true;
    }

    void EndGame(bool won)
    {
        gameActive = false;
        keyPromptText.text = " ";
        messageText.text = won ? "¡GANASTE!" : "¡PERDISTE!";
        StartCoroutine(ReturnToCasino(won ? 3f : 5f));
    }

    IEnumerator ReturnToCasino(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("SampleScene");
    }
}