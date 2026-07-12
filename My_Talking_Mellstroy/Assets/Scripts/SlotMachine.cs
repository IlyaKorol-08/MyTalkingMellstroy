using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SlotMachine : MonoBehaviour
{
    [Header("Барабаны")]
    public Image[] reels;

    [Header("Символы")]
    public Sprite[] symbols;

    [Header("Ручка")]
    public Button handleButton;
    public float handleRotationAngle = 45f;

    [Header("Настройки игры")]
    public float spinDuration = 2f;
    public int prizeAmount = 100;

    [Header("UI")]
    public TextMeshProUGUI prizeText;
    public GameObject winAnimation;

    [Header("Звуки")]
    public AudioClip spinSound;
    public AudioClip winSound;

    private AudioSource audioSource;
    private bool isSpinning = false;
    private int[] currentSymbols = new int[3];

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        for (int i = 0; i < reels.Length; i++)
        {
            currentSymbols[i] = Random.Range(0, symbols.Length);
            reels[i].sprite = symbols[currentSymbols[i]];
        }

        if (handleButton != null)
        {
            handleButton.onClick.AddListener(PullHandle);
        }

        if (prizeText != null)
        {
            prizeText.gameObject.SetActive(false);
        }

        if (winAnimation != null)
        {
            winAnimation.SetActive(false);
        }
    }

    void PullHandle()
    {
        if (isSpinning) return;

        isSpinning = true;
        StartCoroutine(AnimateHandle());

        if (spinSound != null) audioSource.PlayOneShot(spinSound);
        StartCoroutine(SpinReels());
    }

    IEnumerator AnimateHandle()
    {
        Transform handleTransform = handleButton.transform;
        Quaternion startRotation = handleTransform.rotation;
        Quaternion pulledRotation = startRotation * Quaternion.Euler(0, 0, handleRotationAngle);

        float elapsed = 0f;
        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime;
            handleTransform.rotation = Quaternion.Lerp(startRotation, pulledRotation, elapsed / 0.3f);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime;
            handleTransform.rotation = Quaternion.Lerp(pulledRotation, startRotation, elapsed / 0.3f);
            yield return null;
        }
    }

    IEnumerator SpinReels()
    {
        float elapsed = 0f;
        float symbolChangeInterval = 0.1f;
        float lastChangeTime = 0f;

        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;

            if (elapsed - lastChangeTime >= symbolChangeInterval)
            {
                for (int i = 0; i < reels.Length; i++)
                {
                    reels[i].sprite = symbols[Random.Range(0, symbols.Length)];
                }
                lastChangeTime = elapsed;
            }

            yield return null;
        }

        for (int i = 0; i < reels.Length; i++)
        {
            currentSymbols[i] = Random.Range(0, symbols.Length);
            reels[i].sprite = symbols[currentSymbols[i]];
            yield return new WaitForSeconds(0.3f);
        }

        CheckWin();
        isSpinning = false;
    }

    void CheckWin()
    {
        bool isWin = false;

        if (currentSymbols[0] == currentSymbols[1] && currentSymbols[1] == currentSymbols[2])
        {
            isWin = true;
            ShowWin(prizeAmount * 3);
        }
        else if (currentSymbols[0] == currentSymbols[1] ||
                 currentSymbols[1] == currentSymbols[2] ||
                 currentSymbols[0] == currentSymbols[2])
        {
            isWin = true;
            ShowWin(prizeAmount);
        }

        if (!isWin)
        {
            Debug.Log("Нет выигрыша. Попробуйте ещё раз!");
        }
    }

    void ShowWin(int amount)
    {
        Debug.Log($"Выигрыш: {amount} монет!");

        if (winSound != null) audioSource.PlayOneShot(winSound);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddCoins(amount);
        }

        if (prizeText != null)
        {
            prizeText.text = $"+{amount}";
            prizeText.gameObject.SetActive(true);
            prizeText.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            StartCoroutine(AnimatePrizeText());
        }

        if (winAnimation != null)
        {
            winAnimation.SetActive(true);
            StartCoroutine(AnimateWinPopup());
        }
    }

    IEnumerator AnimatePrizeText()
    {
        float elapsed = 0f;
        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(0.1f, 1.3f, elapsed / 0.3f);
            prizeText.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }

        prizeText.transform.localScale = Vector3.one;
        yield return new WaitForSeconds(2f);
        prizeText.gameObject.SetActive(false);
    }

    IEnumerator AnimateWinPopup()
    {
        RectTransform rectTransform = winAnimation.GetComponent<RectTransform>();

        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float scale = Mathf.Lerp(0.1f, 1.2f, t);
            float rotation = Mathf.Lerp(-30f, 0f, t);

            rectTransform.localScale = new Vector3(scale, scale, 1f);
            rectTransform.rotation = Quaternion.Euler(0, 0, rotation);

            yield return null;
        }

        elapsed = 0f;
        duration = 0.2f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float scale = Mathf.Lerp(1.2f, 1.0f, t);
            rectTransform.localScale = new Vector3(scale, scale, 1f);

            yield return null;
        }

        yield return new WaitForSeconds(2f);

        elapsed = 0f;
        duration = 0.3f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float scale = Mathf.Lerp(1.0f, 0.1f, t);
            rectTransform.localScale = new Vector3(scale, scale, 1f);

            yield return null;
        }

        winAnimation.SetActive(false);
    }
}