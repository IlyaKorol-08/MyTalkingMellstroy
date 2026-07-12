using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LocationManager : MonoBehaviour
{
    public static bool IsTransitioning { get; private set; }
    public static string SavedLocationName = "Location_kipr";
    public static bool IsReturningFromMinigame = false;

    [Header("Все локации (4 объекта)")]
    public GameObject[] allLocations;

    [Header("Все кнопки (4 кнопки в том же порядке)")]
    public Button[] allButtons;

    [Header("Персонаж")]
    public PetInteraction petCharacter;

    [Header("Fade экран")]
    public GameObject fadeScreen;
    public float fadeDuration = 0.5f;

    [Header("Кнопка мини-игры")]
    public GameObject minigameButton;

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
                Debug.Log("Загружена локация: " + loc.name);
            }
        }

        for (int i = 0; i < allButtons.Length; i++)
        {
            int index = i;
            allButtons[i].onClick.AddListener(() =>
            {
                Debug.Log("Нажата кнопка: " + allLocations[index].name);
                StartTransition(allLocations[index]);
            });
        }

        UpdateMinigameButton();
        UpdateButtons();

        // Если вернулись из мини-игры, запускаем fade-in
        if (IsReturningFromMinigame)
        {
            IsReturningFromMinigame = false;
            StartCoroutine(FadeInFromMinigame());
        }
    }

    IEnumerator FadeInFromMinigame()
    {
        Image fadeImage = fadeScreen.GetComponent<Image>();
        fadeScreen.SetActive(true);

        Color color = fadeImage.color;
        color.a = 1f;
        fadeImage.color = color;

        IsTransitioning = true;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            color.a = Mathf.Lerp(1f, 0f, t);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;

        fadeScreen.SetActive(false);
        IsTransitioning = false;
    }

    void StartTransition(GameObject newLocation)
    {
        if (IsTransitioning || newLocation == currentLocation)
        {
            Debug.Log("️Переход невозможен");
            return;
        }

        Debug.Log("Начинаем переход в: " + newLocation.name);
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
        Debug.Log("Перешли в: " + newLocation.name);

        UpdateMinigameButton();

        // ← ДОБАВЬТЕ ЭТУ СТРОКУ: обновляем видимость кнопок потребностей
        UpdatePetNeedsButtons();

        yield return new WaitForSeconds(0.1f);

        yield return StartCoroutine(FadeAlpha(fadeImage, 1f, 0f, fadeDuration));

        fadeScreen.SetActive(false);
        UpdateButtons();
        IsTransitioning = false;
    }

    // ← ДОБАВЬТЕ ЭТОТ МЕТОД
    void UpdatePetNeedsButtons()
    {
        // Находим PetNeeds в сцене
        PetNeeds petNeeds = FindObjectOfType<PetNeeds>();
        if (petNeeds != null)
        {
            petNeeds.UpdateButtonVisibility();
            Debug.Log("Обновлена видимость кнопок потребностей");
        }
    }

    void UpdateMinigameButton()
    {
        if (minigameButton != null)
        {
            bool isKipr = (currentLocation != null && currentLocation.name == "Location_kipr");
            minigameButton.SetActive(isKipr);
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