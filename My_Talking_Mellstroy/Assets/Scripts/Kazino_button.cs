using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MinigameButton : MonoBehaviour
{
    [Header("Настройки")]
    public int minigameSceneIndex = 1;

    [Header("Fade эффект")]
    public GameObject fadeScreen;
    public float fadeDuration = 0.5f;

    private void Start()
    {
        Button button = GetComponent<Button>();
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

        SceneManager.LoadScene(minigameSceneIndex);
    }
}