using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class BackButton : MonoBehaviour
{
    [Header("Настройки")]
    public int mainSceneIndex = 0; // Индекс основной сцены в Build Settings

    [Header("Fade эффект")]
    public GameObject fadeScreen; // Белый экран
    public float fadeDuration = 0.5f;

    private bool isTransitioning = false;

    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(GoBack);
        }

        // Скрываем FadeScreen при старте
        if (fadeScreen != null)
        {
            fadeScreen.SetActive(true);
            Image fadeImage = fadeScreen.GetComponent<Image>();
            fadeImage.color = new Color(1, 1, 1, 0); // Прозрачный
        }
    }

    void GoBack()
    {
        if (isTransitioning) return;

        isTransitioning = true;
        Debug.Log("Возврат в основную сцену...");

        if (fadeScreen != null)
        {
            StartCoroutine(FadeAndGoBack());
        }
        else
        {
            SceneManager.LoadScene(mainSceneIndex);
        }
    }

    IEnumerator FadeAndGoBack()
    {
        Image fadeImage = fadeScreen.GetComponent<Image>();
        Color color = fadeImage.color;

        // Появление белого экрана
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            t = t * t * (3f - 2f * t); // Smoothstep
            color.a = Mathf.Lerp(0f, 1f, t);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;

        // Небольшая пауза
        yield return new WaitForSeconds(0.1f);

        // Загрузка основной сцены
        SceneManager.LoadScene(mainSceneIndex);
    }
}