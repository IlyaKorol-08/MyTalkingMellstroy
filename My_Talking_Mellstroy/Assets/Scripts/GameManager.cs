using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Статы персонажа")]
    public float hunger = 100f;
    public float energy = 100f;
    public float happiness = 100f;

    [Header("Монеты")]
    public int coins = 0;

    [Header("UI")]
    public TextMeshProUGUI coinsText;

    void Awake()
    {
        // Логика синглтона (чтобы GameManager не дублировался)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager создан");

            // ЗАГРУЖАЕМ МОНЕТЫ ПРИ ПЕРВОМ ЗАПУСКЕ
            LoadCoins();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        FindCoinsText();
        UpdateCoinsUI();
    }

    // --- НОВЫЕ МЕТОДЫ ДЛЯ СОХРАНЕНИЯ ---

    void LoadCoins()
    {
        // Пытаемся получить сохраненные монеты. Если их нет, ставим 0.
        coins = PlayerPrefs.GetInt("PlayerCoins", 0);
        Debug.Log("Загружено монет из памяти: " + coins);
    }

    void SaveCoins()
    {
        // Сохраняем текущее количество монет в память устройства
        PlayerPrefs.SetInt("PlayerCoins", coins);
        PlayerPrefs.Save(); // Принудительно записываем на диск
        Debug.Log("Монеты сохранены: " + coins);
    }

    // Метод для сброса прогресса (понадобится для тестов)
    public void ResetProgress()
    {
        coins = 0;
        PlayerPrefs.DeleteKey("PlayerCoins");
        UpdateCoinsUI();
        Debug.Log("Прогресс сброшен!");
    }

    // ------------------------------------

    void FindCoinsText()
    {
        GameObject coinsTextObject = GameObject.Find("CoinsText");

        if (coinsTextObject != null)
        {
            coinsText = coinsTextObject.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogWarning("CoinsText не найден в сцене!");
        }
    }

    void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        FindCoinsText();
        UpdateCoinsUI();
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateCoinsUI();
        SaveCoins(); // СОХРАНЯЕМ СРАЗУ ПОСЛЕ ПОЛУЧЕНИЯ
    }

    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            UpdateCoinsUI();
            SaveCoins(); // СОХРАНЯЕМ СРАЗУ ПОСЛЕ ТРАТЫ
            return true;
        }
        else
        {
            Debug.LogWarning("Недостаточно монет!");
            return false;
        }
    }

    void UpdateCoinsUI()
    {
        if (coinsText != null)
        {
            coinsText.text = $"{coins}";
        }
    }
}