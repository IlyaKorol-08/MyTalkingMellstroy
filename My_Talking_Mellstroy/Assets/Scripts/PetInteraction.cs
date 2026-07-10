using UnityEngine;

public class PetInteraction : MonoBehaviour
{
    [Header("Спрайты")]
    public Sprite normalSprite;      // Обычный спрайт
    public Sprite touchedSprite;     // Спрайт при поглаживании

    [Header("Звук")]
    public AudioClip touchSound;     // Звук при поглаживании

    [Header("Настройки")]
    public float revertTime = 1.5f;  // Через сколько секунд вернуть обычный спрайт
    public float minDragDistance = 0.1f; // Минимальное расстояние "проведения"

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private bool isTouched = false;
    private Vector2 lastTouchPosition;
    private float dragDistance = 0f;
    private Coroutine revertCoroutine;

    void Start()
    {
        // Получаем компоненты
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        // Если AudioSource нет, добавляем
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Устанавливаем начальный спрайт
        if (normalSprite != null)
        {
            spriteRenderer.sprite = normalSprite;
        }
    }

    void Update()
    {
        // Проверяем ввод (работает и на ПК с мышью, и на телефоне с пальцем)
        if (Input.GetMouseButtonDown(0)) // Нажатие/касание началось
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Проверяем, попали ли мы в коллайдер персонажа
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                lastTouchPosition = worldPos;
                dragDistance = 0f;
            }
        }
        else if (Input.GetMouseButton(0)) // Палец/мышь двигается
        {
            if (dragDistance >= 0) // Если начали касание именно по персонажу
            {
                Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                dragDistance += Vector2.Distance(lastTouchPosition, worldPos);
                lastTouchPosition = worldPos;

                // Если провели достаточно далеко — активируем реакцию
                if (dragDistance >= minDragDistance && !isTouched)
                {
                    OnPetted();
                }
            }
        }
        else if (Input.GetMouseButtonUp(0)) // Палец отпущен
        {
            dragDistance = -1f; // Сбрасываем
        }
    }

    void OnPetted()
    {
        isTouched = true;

        // Меняем спрайт
        if (touchedSprite != null)
        {
            spriteRenderer.sprite = touchedSprite;
        }

        // Воспроизводим звук
        if (touchSound != null)
        {
            audioSource.PlayOneShot(touchSound);
        }

        // Запускаем таймер возврата к обычному состоянию
        if (revertCoroutine != null)
        {
            StopCoroutine(revertCoroutine);
        }
        revertCoroutine = StartCoroutine(RevertToNormal());
    }

    System.Collections.IEnumerator RevertToNormal()
    {
        yield return new WaitForSeconds(revertTime);

        // Возвращаем обычный спрайт
        if (normalSprite != null)
        {
            spriteRenderer.sprite = normalSprite;
        }

        isTouched = false;
    }
}