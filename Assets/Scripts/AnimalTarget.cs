using UnityEngine;
using System.Collections;

// Aula Aventura RA - Modulo 1: Atencion selectiva
// Se coloca en cada "animal" (por ahora una figura de color).
// Al tocarlo, verifica si su color coincide con el color objetivo actual.
public class AnimalTarget : MonoBehaviour
{
    public Color animalColor;

    private Renderer rend;
    private Vector3 originalScale;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = animalColor;
        }
        originalScale = transform.localScale;
    }

    public void HandleTouch()
    {
        if (ColorTargetManager.Instance == null) return;

        bool correct = ColorsMatch(animalColor, ColorTargetManager.Instance.CurrentTargetColor);
        ColorTargetManager.Instance.RegisterHit(correct);

        StopAllCoroutines();
        StartCoroutine(FeedbackPulse(correct));
    }

    bool ColorsMatch(Color a, Color b)
    {
        return Mathf.Approximately(a.r, b.r) &&
               Mathf.Approximately(a.g, b.g) &&
               Mathf.Approximately(a.b, b.b);
    }

    IEnumerator FeedbackPulse(bool correct)
    {
        Vector3 target = correct ? originalScale * 1.3f : originalScale * 0.8f;
        float duration = 0.15f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, target, t / duration);
            yield return null;
        }

        t = 0f;
        Vector3 start = transform.localScale;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(start, originalScale, t / duration);
            yield return null;
        }

        transform.localScale = originalScale;
    }
}
