using UnityEngine;

public class MagicBarrier : MonoBehaviour
{
    [SerializeField] private GameObject barrierVisual;
    [SerializeField] private GameObject secondBarrierVisual; // הויזואליה הנוספת
    [SerializeField] private Collider barrierCollider;
    [SerializeField] private AudioSource unlockSound; // סאונד לפתיחה

    private void OnEnable()
    {
        GameEvents.OnPuzzleOrCombatSolved += RemoveBarrier;
    }

    private void OnDisable()
    {
        GameEvents.OnPuzzleOrCombatSolved -= RemoveBarrier;
    }

    private void RemoveBarrier()
    {
        // מעלימים את כל האובייקטים הויזואליים
        if (barrierVisual != null) barrierVisual.SetActive(false);
        if (secondBarrierVisual != null) secondBarrierVisual.SetActive(false);

        // מבטלים את הקוליידר
        if (barrierCollider != null) barrierCollider.enabled = false;

        // מפעילים את הסאונד
        if (unlockSound != null) unlockSound.Play();

        Debug.Log("ניצחון זוהה - המחסומים הוסרו והופעל סאונד!");
    }
}