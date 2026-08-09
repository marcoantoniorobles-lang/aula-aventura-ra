using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Aula Aventura RA - Modulo 2: Ordenar numeros 1 al 5
// Controla el flujo del juego: condicion de victoria, celebracion y resultados.
// Sin limite de tiempo (igual que Modulo 1). Puntaje maximo: 10 (2 pts por bola).
// Adjuntar a un GameObject vacio en la escena Modulo2ok.
public class GameManager_M2 : MonoBehaviour
{
    public static GameManager_M2 Instance;

    [Header("Configuracion")]
    public int totalBalls = 5;
    public int pointsPerBall = 2;              // 5 bolas x 2 pts = 10 pts maximo (igual que Modulo 1)

    [Header("Puntaje total para celebracion")]
    [Tooltip("Si el TOTAL (Modulo1 + Modulo2, maximo 20) supera este valor, se muestra la carita feliz.")]
    public int happyThreshold = 12;

    [Header("UI - Juego")]
    public Text progressText;              // "Puntaje: X / 5"
    public Text instructionText;

    [Header("UI - Panel de Victoria")]
    public GameObject winPanel;
    public Text winMessage;
    public Text scoreM1Text;
    public Text scoreM2Text;
    public Text scoreTotalText;

    [Header("UI - Panel de Carita Feliz")]
    public GameObject happyFacePanel;   // Se muestra en el panel final si el total supera happyThreshold
    public Text happyFaceText;             // Texto tipo ":)" u otro mensaje alegre

    [Header("Celebracion")]
    public ParticleSystem confetti;     // Opcional: sistema de particulas

    // Referencia interna al GameObject de confetti/celebracion (asignado por Setup_M2)
    [HideInInspector] public GameObject happyFaceGO;

    private int ballsPlaced = 0;
    private bool gameActive = true;
    private int finalScore = 0;

    void Awake()
    {
        Instance = this;
        ShuffleBallPositions();
    }

    // Baraja las posiciones de las 5 pelotas numeradas para que no aparezcan en orden 1-2-3-4-5.
    // Cada pelota conserva su propio numero/textura, solo se mezclan las posiciones donde aparecen.
    void ShuffleBallPositions()
    {
        NumberBall[] balls = FindObjectsByType<NumberBall>(FindObjectsSortMode.None);
        if (balls.Length <= 1) return;

        Vector3[] positions = new Vector3[balls.Length];
        for (int i = 0; i < balls.Length; i++)
            positions[i] = balls[i].transform.localPosition;

        // Fisher-Yates shuffle
        for (int i = positions.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Vector3 temp = positions[i];
            positions[i] = positions[j];
            positions[j] = temp;
        }

        for (int i = 0; i < balls.Length; i++)
            balls[i].transform.localPosition = positions[i];
    }

    void Start()
    {
        ballsPlaced = 0;
        gameActive = true;

        if (winPanel != null)    winPanel.SetActive(false);
        if (happyFacePanel != null) happyFacePanel.SetActive(false);

        UpdateUI();
        // El texto instructivo se deja configurado desde el Inspector (no se sobreescribe aqui).
    }

    // Llamado por NumberBall cada vez que una pelota se coloca correctamente
    public void OnBallPlaced()
    {
        if (!gameActive) return;

        ballsPlaced++;
        UpdateUI();

        if (ballsPlaced >= totalBalls)
        {
            EndGame();
        }
    }

    void UpdateUI()
    {
        if (progressText != null)
            progressText.text = "Puntaje: " + ballsPlaced + " / " + totalBalls;
    }

    void EndGame()
    {
        gameActive = false;

        // Puntaje fijo: pelotas colocadas x pointsPerBall (sin bono de tiempo, sin limite de tiempo)
        finalScore = ballsPlaced * pointsPerBall;

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.SetModule2Score(finalScore);

        StartCoroutine(ShowVictory());
    }

    IEnumerator ShowVictory()
    {
        if (confetti != null) confetti.Play();
        yield return new WaitForSeconds(1f);

        ShowResults();
    }

    void ShowResults()
    {
        if (winPanel == null) return;

        winPanel.SetActive(true);

        int m1 = ScoreManager.Instance != null ? ScoreManager.Instance.scoreModulo1 : 0;
        int m2 = finalScore;
        int total = m1 + m2;
        bool isHappy = total > happyThreshold;

        if (winMessage != null)
            winMessage.text = "¡Juego completado!";

        if (scoreM1Text != null)
            scoreM1Text.text = "Modulo 1: " + m1 + " pts";

        if (scoreM2Text != null)
            scoreM2Text.text = "Modulo 2: " + m2 + " pts";

        if (scoreTotalText != null)
            scoreTotalText.text = "TOTAL: " + total + " / 20 pts";

        if (happyFacePanel != null)
            happyFacePanel.SetActive(isHappy);

        if (happyFaceText != null)
            happyFaceText.text = isHappy ? ":)" : "Sigue practicando";

        if (isHappy && confetti != null)
            confetti.Play();
    }

    public void RestartGame()
    {
        // Reinicia todo el flujo desde Modulo 1 (no solo Modulo 2)
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.Reset();

        UnityEngine.SceneManagement.SceneManager.LoadScene("Modulo1ok");
    }

    public void GoToModule1()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Modulo1ok");
    }
}
