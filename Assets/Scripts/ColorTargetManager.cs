using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Aula Aventura RA - Modulo 1: Atencion selectiva
// Controla el color objetivo actual, el puntaje, la meta de rondas y el fin de juego.
public class ColorTargetManager : MonoBehaviour
{
    public static ColorTargetManager Instance;

    [Header("UI - Juego")]
    public Text instructionText;
    public Text scoreText;
    public Image targetColorImage; // circulo que muestra el color objetivo

    [Header("UI - Fin de juego")]
    public GameObject endPanel;
    public Text endText;

    [Header("Configuracion de colores")]
    public Color[] availableColors;
    public string[] colorNames;

    [Header("Reglas del juego")]
    [Tooltip("Cantidad de aciertos necesarios para completar la ronda")]
    public int roundsToWin = 10;

    private int currentColorIndex;
    private int score = 0;
    private int correctHits = 0;
    private bool gameActive = true;

    void Awake()
    {
        Instance = this;
    }

    [Header("Transicion automatica a Modulo 2")]
    [Tooltip("Segundos de espera antes de pasar automaticamente a Modulo 2 al terminar.")]
    public float delayBeforeModule2 = 2.5f;

    void Start()
    {
        StartGame();
    }

    public Color CurrentTargetColor
    {
        get { return availableColors[currentColorIndex]; }
    }

    public bool IsGameActive
    {
        get { return gameActive; }
    }

    public void StartGame()
    {
        score = 0;
        correctHits = 0;
        gameActive = true;

        if (endPanel != null)
            endPanel.SetActive(false);

        PickNewTargetColor();
        UpdateScoreUI();
    }

    void PickNewTargetColor()
    {
        if (availableColors == null || availableColors.Length == 0) return;

        currentColorIndex = Random.Range(0, availableColors.Length);

        if (instructionText != null)
            instructionText.text = "Toca el animal: " + colorNames[currentColorIndex];

        if (targetColorImage != null)
            targetColorImage.color = availableColors[currentColorIndex];
    }

    public void RegisterHit(bool correct)
    {
        if (!gameActive) return;

        if (correct)
        {
            score++;
            correctHits++;
            UpdateScoreUI();

            if (correctHits >= roundsToWin)
            {
                EndGame();
            }
            else
            {
                PickNewTargetColor();
            }
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Puntaje: " + score + " / " + roundsToWin;
    }

    void EndGame()
    {
        gameActive = false;

        // Pantalla de fin de Modulo 1 desactivada: ya no se necesita para pruebas manuales,
        // ahora la transicion a Modulo 2 es automatica y directa.

        // Guardar puntaje M1 y pasar automaticamente a Modulo 2
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.SetModule1Score(score);

        Invoke(nameof(LoadModule2), delayBeforeModule2);
    }

    void LoadModule2()
    {
        SceneManager.LoadScene("Modulo2ok");
    }

    public void RestartGame()
    {
        CancelInvoke(nameof(LoadModule2));
        StartGame();
    }
}
