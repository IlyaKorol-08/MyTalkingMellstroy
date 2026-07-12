using UnityEngine;
using System.Collections;

public class PetInteraction : MonoBehaviour
{
    [Header("Спрайты")]
    public Sprite normalSprite;
    public Sprite touchedSprite;

    [Header("Звук")]
    public AudioClip touchSound;

    [Header("Настройки")]
    public float revertTime = 1.5f;
    public float minDragDistance = 0.15f;

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private bool isTouched = false;
    private Vector2 lastTouchPosition;
    private float dragDistance = 0f;
    private Coroutine revertCoroutine;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        if (normalSprite != null) spriteRenderer.sprite = normalSprite;
    }

    void Update()
    {
        // ИГНОРИРУЕМ ВВОД ВО ВРЕМЯ ПЕРЕХОДА МЕЖДУ ЛОКАЦИЯМИ
        if (LocationManager.IsTransitioning) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                lastTouchPosition = worldPos;
                dragDistance = 0f;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (dragDistance >= 0)
            {
                Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                dragDistance += Vector2.Distance(lastTouchPosition, worldPos);
                lastTouchPosition = worldPos;

                if (dragDistance >= minDragDistance && !isTouched)
                {
                    OnPetted();
                }
            }
        }
        else
        {
            dragDistance = -1f;
        }
    }

    void OnPetted()
    {
        isTouched = true;
        if (touchedSprite != null) spriteRenderer.sprite = touchedSprite;
        if (touchSound != null) audioSource.PlayOneShot(touchSound);

        if (revertCoroutine != null) StopCoroutine(revertCoroutine);
        revertCoroutine = StartCoroutine(RevertToNormal());
    }

    IEnumerator RevertToNormal()
    {
        yield return new WaitForSeconds(revertTime);
        if (normalSprite != null) spriteRenderer.sprite = normalSprite;
        isTouched = false;
    }

    public void ForceReset()
    {
        isTouched = false;
        dragDistance = -1f;

        if (revertCoroutine != null)
        {
            StopCoroutine(revertCoroutine);
            revertCoroutine = null;
        }

        if (normalSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = normalSprite;
        }
    }
}