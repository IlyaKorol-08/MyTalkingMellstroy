using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LocationManager : MonoBehaviour
{
    public static bool IsTransitioning { get; private set; }
    public static string SavedLocationName = "Location_kipr";

    [Header("Все локации")]
    public GameObject[] allLocations;

    [Header("Кнопки")]
    public Button[] allButtons;

    [Header("Персонаж")]
    public PetInteraction petCharacter;

    [Header("Fade экран")]
    public GameObject fadeScreen;
    public float fadeDuration = 0.5f;

    [Header("Кнопка мини-игры")]
    public GameObject minigameButton; // ? Добавьте это поле

    [Header("Цвета кнопок")]
    public Color activeColor = new Color(1f, 0.8f, 0f, 1f);
    public Color inactiveColor = Color.white;

    private GameObject currentLocation;

    void Start()
    {
        IsTransitioning = false;

        foreach (GameObject loc in allLocations)
        {
            loc.SetActive(false);
        }

        foreach (GameObject loc in allLocations)
        {
            if (loc.name == SavedLocationName)
            {
                loc.SetActive(true);
                currentLocation = loc;
            }
        }

        for (int i = 0; i < allButtons.Length; i++)
        {
            int index = i;
            allButtons[i].onClick.AddListener(() => StartTransition(allLocations[index]));
        }

        // Показываем/скрываем кнопку мини-игры
        UpdateMinigameButton();

        UpdateButtons();
    }

    void StartTransition(GameObject newLocation)
    {
        if (IsTransitioning || newLocation == currentLocation) return;

        IsTransitioning = true;
        StartCoroutine(FadeTransition(newLocation));
    }

    IEnumerator FadeTransition(GameObject newLocation)
    {
        if (petCharacter != null) petCharacter.ForceReset();

        Image fadeImage = fadeScreen.GetComponent<Image>();
        fadeScreen.SetActive(true);

        yield return StartCoroutine(FadeAlpha(fadeImage, 0f, 1f, fadeDuration));

        if (currentLocation != null) currentLocation.SetActive(false);
        newLocation.SetActive(true);
        currentLocation = newLocation;

        SavedLocationName = newLocation.name;

        // Обновляем видимость кнопки мини-игры
        UpdateMinigameButton();

        yield return new WaitForSeconds(0.1f);

        yield return StartCoroutine(FadeAlpha(fadeImage, 1f, 0f, fadeDuration));

        fadeScreen.SetActive(false);
        UpdateButtons();
        IsTransitioning = false;
    }

    // Новый метод для управления кнопкой мини-игры
    void UpdateMinigameButton()
    {
        if (minigameButton != null)
        {
            // Показываем кнопку только если текущая локация - Кипр
            bool isKipr = (currentLocation != null && currentLocation.name == "Location_kipr");
            minigameButton.SetActive(isKipr);

            Debug.Log("Кнопка мини-игры: " + (isKipr ? "видна" : "скрыта"));
        }
    }

    IEnumerator FadeAlpha(Image img, float from, float to, float duration)
    {
        float elapsed = 0f;
        Color color = img.color;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            color.a = Mathf.Lerp(from, to, t);
            img.color = color;
            yield return null;
        }
        color.a = to;
        img.color = color;
    }

    void UpdateButtons()
    {
        for (int i = 0; i < allButtons.Length; i++)
        {
            bool isActive = (allLocations[i] == currentLocation);
            ColorBlock colors = allButtons[i].colors;
            colors.normalColor = isActive ? activeColor : inactiveColor;
            colors.highlightedColor = isActive ? activeColor : inactiveColor;
            colors.pressedColor = isActive ? activeColor : inactiveColor;
            allButtons[i].colors = colors;
            allButtons[i].interactable = !isActive;
        }
    }
}