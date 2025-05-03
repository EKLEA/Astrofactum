using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    public float fadeDuration = 1f; // Общая длительность эффекта
    [Range(0.0f, 1.0f)]
    public float fadeMax = 1f; // Максимальная прозрачность
    public AnimationCurve fadeCurve = AnimationCurve.Linear(0, 0, 1, 1); // Кривая для контроля плавности

    private Coroutine fadeCoroutine;
    private Image fadeImage;

    private void Awake()
    {
        fadeImage = GetComponent<Image>();
        // Убедимся, что изображение полностью прозрачно при старте
        Color color = fadeImage.color;
        color.a = 0f;
        fadeImage.color = color;
    }

    public void StartFadeIn()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(0f, fadeMax));
    }

    public void StartFadeOut()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(fadeImage.color.a, 0f));
    }

    private IEnumerator FadeRoutine(float startAlpha, float targetAlpha)
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        float currentAlpha = startAlpha;

        while (elapsedTime < fadeDuration)
        {
            // Используем кривую для плавного изменения
            float t = fadeCurve.Evaluate(elapsedTime / fadeDuration);
            currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            
            color.a = currentAlpha;
            fadeImage.color = color;
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Гарантируем, что достигли конечного значения
        color.a = targetAlpha;
        fadeImage.color = color;
    }
}