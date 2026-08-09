using UnityEngine;

// Aula Aventura RA - ScoreManager
// Singleton que persiste entre escenas y guarda el puntaje de cada modulo.
// Se destruye solo cuando la aplicacion termina.
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [HideInInspector] public int scoreModulo1 = 0;
    [HideInInspector] public int scoreModulo2 = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetModule1Score(int score)
    {
        scoreModulo1 = score;
        Debug.Log("[ScoreManager] Modulo 1 puntaje guardado: " + score);
    }

    public void SetModule2Score(int score)
    {
        scoreModulo2 = score;
        Debug.Log("[ScoreManager] Modulo 2 puntaje guardado: " + score);
    }

    public int GetTotal()
    {
        return scoreModulo1 + scoreModulo2;
    }

    public void Reset()
    {
        scoreModulo1 = 0;
        scoreModulo2 = 0;
    }
}
