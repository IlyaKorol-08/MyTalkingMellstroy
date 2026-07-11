using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MinigameButton : MonoBehaviour
{
    [Header("Настройки")]
    public int minigameSceneIndex = 1; // Индекс сцены мини-игры (из Build Settings)

    [Header("Fade эффект (опционально)")]
    public GameObject fadeScreen;
    public float fadeDuration = 0.5f;

    private void Start()
    {
        // Находим кнопку и добавляем обработчик
        UnityEngine.UI.Button button = GetComponent<UnityEngine.UI.Button>();
        if (button != null)
        {
            button.onClick.AddListener(StartMinigame);
        }
    }

    void StartMinigame()
    {
        Debug.Log("Запуск мини-игры...");

        if (fadeScreen != null)
        {
            StartCoroutine(FadeAndLoadScene());
        }
        else
        {
            SceneManager.LoadScene(minigameSceneIndex);
        }
    }

    System.Collections.IEnumerator FadeAndLoadScene()
    {
        Image fadeImage = fadeScreen.GetComponent<Image>();
        fadeScreen.SetActive(true);

        // Появление белого экрана
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            Color color = fadeImage.color;
            color.a = t;
            fadeImage.color = color;
            yield return null;
        }

        // Загрузка сцены
        SceneManager.LoadScene(minigameSceneIndex);
    }
}