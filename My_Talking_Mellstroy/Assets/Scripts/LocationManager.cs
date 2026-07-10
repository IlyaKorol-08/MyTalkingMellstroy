using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LocationManager : MonoBehaviour
{
    public static bool IsTransitioning { get; private set; }

    [Header("Персонаж")]
    public PetInteraction petCharacter;

    [Header("Объекты локаций")]
    public GameObject locationHammam;
    public GameObject locationSpalnya;
    public GameObject locationKipr;
    public GameObject locationMurino;

    [Header("Кнопки")]
    public Button buttonHammam;
    public Button buttonSpalnya;
    public Button buttonKipr;
    public Button buttonMurino;

    [Header("Fade экран")]
    public GameObject fadeScreen;
    public float fadeDuration = 0.5f;

    [Header("Персонаж")]
    public GameObject character; // Ссылка на персонажа
    public float resetDelay = 0.1f; // Задержка перед сбросом

    [Header("Цвета кнопок")]
    public Color activeColor = new Color(1f, 0.8f, 0f, 1f);
    public Color inactiveColor = Color.white;

    private GameObject currentLocation;
    private Image fadeImage;
    private bool isTransitioning = false;
    private PetInteraction petInteraction;

    void Start()
    {
        fadeImage = fadeScreen.GetComponent<Image>();
        fadeImage.color = new Color(1, 1, 1, 0);

        // Получаем компонент PetInteraction с персонажа
        if (character != null)
        {
            petInteraction = character.GetComponent<PetInteraction>();
        }

        // Определяем текущую локацию
        if (locationHammam.activeSelf) currentLocation = locationHammam;
        else if (locationSpalnya.activeSelf) currentLocation = locationSpalnya;
        else if (locationKipr.activeSelf) currentLocation = locationKipr;
        else if (locationMurino.activeSelf) currentLocation = locationMurino;

        // Назначаем обработчики
        buttonHammam.onClick.AddListener(() => StartTransition(locationHammam));
        buttonSpalnya.onClick.AddListener(() => StartTransition(locationSpalnya));
        buttonKipr.onClick.AddListener(() => StartTransition(locationKipr));
        buttonMurino.onClick.AddListener(() => StartTransition(locationMurino));

        UpdateButtons();
    }

    void StartTransition(GameObject newLocation)
    {
        if (isTransitioning || newLocation == currentLocation)
        {
            return;
        }

        isTransitioning = true;
        StartCoroutine(FadeTransition(newLocation));
    }

    IEnumerator FadeTransition(GameObject newLocation)
    {
        isTransitioning = true;
        IsTransitioning = true;

        // Принудительно сбрасываем персонажа
        if (petCharacter != null)
        {
            petCharacter.ForceReset();
            Debug.Log("Персонаж сброшен");
        }
        else
        {
            Debug.LogWarning("Pet Character не назначен в Inspector!");
        }

        SetAllButtonsInteractable(false);
        fadeScreen.SetActive(true);

        // Появление белого экрана
        yield return StartCoroutine(FadeAlpha(0f, 1f, fadeDuration));

        // СБРОС состояния персонажа ПЕРЕД переключением
        if (petInteraction != null)
        {
            petInteraction.ForceResetToNormal();
        }

        // Небольшая задержка для сброса
        yield return new WaitForSeconds(resetDelay);

        // Переключение локации
        if (currentLocation != null)
        {
            currentLocation.SetActive(false);
        }
        newLocation.SetActive(true);
        currentLocation = newLocation;

        Debug.Log("Переход в: " + newLocation.name);

        // Исчезновение белого экрана
        yield return StartCoroutine(FadeAlpha(1f, 0f, fadeDuration));

        fadeScreen.SetActive(false);
        SetAllButtonsInteractable(true);
        UpdateButtons();

        IsTransitioning = false;
        isTransitioning = false;
    }

    IEnumerator FadeAlpha(float from, float to, float duration)
    {
        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = t * t * (3f - 2f * t);

            color.a = Mathf.Lerp(from, to, t);
            fadeImage.color = color;

            yield return null;
        }

        color.a = to;
        fadeImage.color = color;
    }

    void SetAllButtonsInteractable(bool state)
    {
        buttonHammam.interactable = state;
        buttonSpalnya.interactable = state;
        buttonKipr.interactable = state;
        buttonMurino.interactable = state;
    }

    void UpdateButtons()
    {
        SetButtonState(buttonHammam, locationHammam == currentLocation);
        SetButtonState(buttonSpalnya, locationSpalnya == currentLocation);
        SetButtonState(buttonKipr, locationKipr == currentLocation);
        SetButtonState(buttonMurino, locationMurino == currentLocation);
    }

    void SetButtonState(Button button, bool isActive)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = isActive ? activeColor : inactiveColor;
        colors.highlightedColor = isActive ? activeColor : inactiveColor;
        colors.pressedColor = isActive ? activeColor : inactiveColor;
        button.colors = colors;

        button.interactable = !isActive;
    }
}