using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class BackToMainScene : MonoBehaviour
{
    [Header("Настройки")]
    public int mainSceneIndex = 0;
    public GameObject fadeScreen;
    public float fadeDuration = 0.5f;

    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(GoBack);
        }

        if (fadeScreen != null)
        {
            fadeScreen.SetActive(false);

            Image fadeImage = fadeScreen.GetComponent<Image>();
            if (fadeImage != null)
            {
                fadeImage.raycastTarget = false;
            }
        }
    }

    void GoBack()
    {
        LocationManager.IsReturningFromMinigame = true;

        if (fadeScreen != null)
        {
            StartCoroutine(FadeAndLoadScene());
        }
        else
        {
            SceneManager.LoadScene(mainSceneIndex);
        }
    }

    IEnumerator FadeAndLoadScene()
    {
        Image fadeImage = fadeScreen.GetComponent<Image>();
        fadeScreen.SetActive(true);

        float elapsed = 0f;
        Color color = fadeImage.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            color.a = Mathf.Lerp(0f, 1f, t);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;

        yield return new WaitForSeconds(0.1f);

        SceneManager.LoadScene(mainSceneIndex);
    }
}