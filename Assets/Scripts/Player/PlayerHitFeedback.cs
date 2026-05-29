using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerHitFeedback : MonoBehaviour
{
    [Header("Flash")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;

    [Header("Knockback")]
    [SerializeField] private float knockbackDistance = 0.4f;
    [SerializeField] private float knockbackDuration = 0.08f;

    private Rigidbody rb;
    private Color originalColor;
    private Coroutine knockbackRoutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }
    private void OnEnable()
    {
        // 🤝 חוזרים להקשיב לאירוע הגלובלי של המשחק
        GameEvents.OnPlayerHit += HandlePlayerHit;
    }

    private void OnDisable()
    {
        // 🛑 ביטול הרשמה
        GameEvents.OnPlayerHit -= HandlePlayerHit;
    }

    private void HandlePlayerHit(float damage)
    {
        // האפקט האדום יעבוד בצורה מושלמת בכל פעם שהאירוע נורה!
        Flash();

        // לגבי הקנוקבק: מאחר והאירוע הגלובלי רק אומר "השחקן נפגע", 
        // נרתע פשוט לכיוון האחורי של השחקן (או שנשנה את ה-Event בעתיד שיעביר גם מיקום)
        Vector3 sourcePosition = transform.position + Vector3.back;
        KnockbackFrom(sourcePosition);
    }


    private void Flash()
    {
        if (spriteRenderer == null) return;

        StopCoroutine(nameof(FlashRoutine));
        StartCoroutine(nameof(FlashRoutine));
    }

    private IEnumerator FlashRoutine()
    {
        spriteRenderer.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        spriteRenderer.color = originalColor;
    }

    private void KnockbackFrom(Vector3 sourcePosition)
    {
        Vector3 dir = (transform.position - sourcePosition).normalized;
        dir.y = 0f;

        Vector3 targetPosition =
            rb.position + dir * knockbackDistance;

        if (knockbackRoutine != null)
            StopCoroutine(knockbackRoutine);

        knockbackRoutine =
            StartCoroutine(KnockbackRoutine(targetPosition));
    }

    private IEnumerator KnockbackRoutine(Vector3 targetPosition)
    {
        Vector3 start = rb.position;

        float timer = 0f;

        while (timer < knockbackDuration)
        {
            timer += Time.fixedDeltaTime;

            float t = timer / knockbackDuration;

            rb.MovePosition(Vector3.Lerp(start, targetPosition, t));

            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(targetPosition);
    }
}