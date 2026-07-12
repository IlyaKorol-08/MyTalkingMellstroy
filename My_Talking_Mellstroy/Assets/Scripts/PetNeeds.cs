using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PetNeeds : MonoBehaviour
{
    [Header("Показатели потребностей (0-100)")]
    [Range(0, 100)] public float hunger = 100f;
    [Range(0, 100)] public float playfulness = 100f;
    [Range(0, 100)] public float cleanliness = 100f;
    [Range(0, 100)] public float toilet = 100f;
    [Range(0, 100)] public float energy = 100f;

    [Header("Скорость уменьшения (в секунду)")]
    public float hungerDecay = 0.5f;
    public float playfulnessDecay = 0.8f;
    public float cleanlinessDecay = 0.3f;
    public float toiletDecay = 0.4f;
    public float energyDecay = 0.6f;

    [Header("UI - Полоски прогресса")]
    public Image hungerBar;
    public Image playfulnessBar;
    public Image cleanlinessBar;
    public Image toiletBar;
    public Image energyBar;

    [Header("UI - Текст значений")]
    public TextMeshProUGUI hungerText;
    public TextMeshProUGUI playfulnessText;
    public TextMeshProUGUI cleanlinessText;
    public TextMeshProUGUI toiletText;
    public TextMeshProUGUI energyText;

    [Header("Кнопки действий")]
    public GameObject feedButton;      // Кнопка "Покормить"
    public GameObject playButton;      // Кнопка "Играть"
    public GameObject washButton;      // Кнопка "Помыть"
    public GameObject toiletButton;    // Кнопка "Туалет"
    public GameObject sleepButton;     // Кнопка "Спать"

    [Header("Цвета полосок")]
    public Color highColor = Color.green;
    public Color mediumColor = Color.yellow;
    public Color lowColor = Color.red;

    [Header("Критический уровень")]
    public float criticalLevel = 20f;

    [Header("Уведомления")]
    public GameObject warningPanel;
    public TextMeshProUGUI warningText;
    public float warningDuration = 3f;

    private float lastWarningTime = 0f;
    private string lastWarning = "";

    void Start()
    {
        LoadNeeds();
        UpdateAllUI();
        UpdateButtonVisibility(); // ← ДОБАВЬТЕ ЭТУ СТРОКУ
    }

    void Update()
    {
        DecreaseNeeds();
        UpdateAllUI();
        CheckCriticalLevels();
    }

    void DecreaseNeeds()
    {
        hunger = Mathf.Clamp(hunger - hungerDecay * Time.deltaTime, 0, 100);
        playfulness = Mathf.Clamp(playfulness - playfulnessDecay * Time.deltaTime, 0, 100);
        cleanliness = Mathf.Clamp(cleanliness - cleanlinessDecay * Time.deltaTime, 0, 100);
        toilet = Mathf.Clamp(toilet - toiletDecay * Time.deltaTime, 0, 100);
        energy = Mathf.Clamp(energy - energyDecay * Time.deltaTime, 0, 100);
    }

    void UpdateAllUI()
    {
        UpdateBar(hungerBar, hungerText, hunger, "Сытость");
        UpdateBar(playfulnessBar, playfulnessText, playfulness, "Игривость");
        UpdateBar(cleanlinessBar, cleanlinessText, cleanliness, "Чистота");
        UpdateBar(toiletBar, toiletText, toilet, "Туалет");
        UpdateBar(energyBar, energyText, energy, "Энергия");
    }

    void UpdateBar(Image bar, TextMeshProUGUI text, float value, string name)
    {
        if (bar != null)
        {
            bar.fillAmount = value / 100f;
            bar.color = GetColorByValue(value);
        }

        if (text != null)
        {
            text.text = $"{Mathf.RoundToInt(value)}%";
        }
    }

    Color GetColorByValue(float value)
    {
        if (value > 60f) return highColor;
        if (value > criticalLevel) return mediumColor;
        return lowColor;
    }

    void CheckCriticalLevels()
    {
        string warning = "";

        if (hunger < criticalLevel)
            warning = "Питомец голоден!";
        else if (playfulness < criticalLevel)
            warning = "Питомец хочет играть!";
        else if (cleanliness < criticalLevel)
            warning = "Питомец грязный!";
        else if (toilet < criticalLevel)
            warning = "Питомец хочет в туалет!";
        else if (energy < criticalLevel)
            warning = "Питомец устал!";

        if (warning != "" && warning != lastWarning && Time.time - lastWarningTime > warningDuration)
        {
            ShowWarning(warning);
            lastWarning = warning;
            lastWarningTime = Time.time;
        }
    }

    void ShowWarning(string message)
    {
        if (warningPanel != null)
        {
            warningPanel.SetActive(true);
            if (warningText != null)
            {
                warningText.text = message;
            }

            Invoke("HideWarning", warningDuration);
        }
        else
        {
            Debug.LogWarning(message);
        }
    }

    void HideWarning()
    {
        if (warningPanel != null)
        {
            warningPanel.SetActive(false);
        }
    }

    // === НОВЫЙ МЕТОД ДЛЯ ПОПОЛНЕНИЯ ИГРИВОСТИ ===
    public void AddPlayfulness(float amount)
    {
        playfulness = Mathf.Clamp(playfulness + amount, 0, 100);
        Debug.Log($"Игривость пополнена на {amount}%. Текущая: {Mathf.RoundToInt(playfulness)}%");
        UpdateAllUI();
        SaveNeeds();
    }

    // === МЕТОДЫ ДЕЙСТВИЙ ===

    public void Feed()
    {
        hunger = Mathf.Clamp(hunger + 30f, 0, 100);
        Debug.Log("Питомец покормлен! Сытость: " + Mathf.RoundToInt(hunger) + "%");
        SaveNeeds();
    }

    public void Play()
    {
        if (energy < 10f)
        {
            Debug.LogWarning("Питомец слишком устал для игры!");
            return;
        }

        playfulness = Mathf.Clamp(playfulness + 40f, 0, 100);
        energy = Mathf.Clamp(energy - 10f, 0, 100);
        Debug.Log("Питомец поиграл! Игривость: " + Mathf.RoundToInt(playfulness) + "%");
        SaveNeeds();
    }

    public void Wash()
    {
        cleanliness = Mathf.Clamp(cleanliness + 50f, 0, 100);
        Debug.Log("Питомец помыт! Чистота: " + Mathf.RoundToInt(cleanliness) + "%");
        SaveNeeds();
    }

    public void UseToilet()
    {
        toilet = Mathf.Clamp(toilet + 60f, 0, 100);
        Debug.Log("Питомец сходил в туалет! Нужда: " + Mathf.RoundToInt(toilet) + "%");
        SaveNeeds();
    }

    public void Sleep()
    {
        energy = Mathf.Clamp(energy + 50f, 0, 100);
        Debug.Log("Питомец поспал! Энергия: " + Mathf.RoundToInt(energy) + "%");
        SaveNeeds();
    }

    // === УПРАВЛЕНИЕ ВИДИМОСТЬЮ КНОПОК ===

    public void UpdateButtonVisibility()
    {
        string currentLocation = LocationManager.SavedLocationName;

        Debug.Log($"Текущая локация: {currentLocation}");

        // Кнопка "Покормить" видна только на локации Murino
        if (feedButton != null)
        {
            feedButton.SetActive(currentLocation == "Location_murino");
        }

        // Кнопка "Играть" СКРЫТА ВЕЗДЕ (игривость пополняется через слот-машину)
        if (playButton != null)
        {
            playButton.SetActive(false); // Всегда скрыта
        }

        // Кнопки "Помыть" и "Туалет" видны только на локации Hammam
        if (washButton != null)
        {
            washButton.SetActive(currentLocation == "Location_hammam");
        }

        if (toiletButton != null)
        {
            toiletButton.SetActive(currentLocation == "Location_hammam");
        }

        // Кнопка "Спать" видна только на локации Spalnya
        if (sleepButton != null)
        {
            sleepButton.SetActive(currentLocation == "Location_spalnya");
        }
    }

    // === СОХРАНЕНИЕ И ЗАГРУЗКА ===

    void SaveNeeds()
    {
        PlayerPrefs.SetFloat("PetHunger", hunger);
        PlayerPrefs.SetFloat("PetPlayfulness", playfulness);
        PlayerPrefs.SetFloat("PetCleanliness", cleanliness);
        PlayerPrefs.SetFloat("PetToilet", toilet);
        PlayerPrefs.SetFloat("PetEnergy", energy);
        PlayerPrefs.Save();
    }

    void LoadNeeds()
    {
        hunger = PlayerPrefs.GetFloat("PetHunger", 100f);
        playfulness = PlayerPrefs.GetFloat("PetPlayfulness", 100f);
        cleanliness = PlayerPrefs.GetFloat("PetCleanliness", 100f);
        toilet = PlayerPrefs.GetFloat("PetToilet", 100f);
        energy = PlayerPrefs.GetFloat("PetEnergy", 100f);

        Debug.Log("Загружены потребности питомца");
    }

    public void ResetNeeds()
    {
        hunger = 100f;
        playfulness = 100f;
        cleanliness = 100f;
        toilet = 100f;
        energy = 100f;

        PlayerPrefs.DeleteKey("PetHunger");
        PlayerPrefs.DeleteKey("PetPlayfulness");
        PlayerPrefs.DeleteKey("PetCleanliness");
        PlayerPrefs.DeleteKey("PetToilet");
        PlayerPrefs.DeleteKey("PetEnergy");

        UpdateAllUI();
        Debug.Log("Потребности сброшены!");
    }
}